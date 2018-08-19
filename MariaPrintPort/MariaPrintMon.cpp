/*
 * Copyright (c) @marbocub <marbocub@gmail.com>
 * All rights reserved.
 *
 * MariaPrintMon : A part of Maria Print System.
 *
 */

#include "stdafx.h"
#include <winspool.h>
#include <winsplp.h>
//#include <stdio.h>
#include <stdlib.h>

WCHAR szPortName[] = L"MARIAPRINT:";
WCHAR szMonitorName[] = L"Maria Print Port";
WCHAR szDescription[] = L"Maria Redirecter";

// registry key
WCHAR szRegSettingKey[] = L"Settings";
WCHAR szRegDirectory[] = L"OutputDirectory";
WCHAR szRegTouchFinFile[] = L"TouchFinFile";
WCHAR szRegFileExtension[] = L"FileExtension";
#define MAX_EXTENSION_LENGTH 10

// handle
typedef struct _MARIAMONINI MARIAMONINI, *PMARIAMONINI;
typedef struct _PORTINI PORTINI, *PPORTINI;
struct _PORTINI {
	DWORD        signature;		// unused
	PMARIAMONINI pMariaMonIni;
	HANDLE       hFile;
	HANDLE       hPrinter;
	DWORD        JobId;
	TCHAR        filePath[MAX_PATH + 1] = { 0 };
};
struct _MARIAMONINI {
	DWORD        signature;		// unused
	PMONITORINIT pMonitorInit;
	PPORTINI     pPortIni;
};

VOID EnterCritical(VOID);
VOID LeaveCritical(VOID);

BOOL WINAPI MariaEnumPorts(
	_In_     HANDLE  hMonitor,
	_In_opt_ LPWSTR  pName,
	_In_     DWORD   Level,
	_Out_    LPBYTE  pPorts,
	_In_     DWORD   cbBuf,
	_Out_    LPDWORD pcbNeeded,
	_Out_    LPDWORD pcReturned)
{
	size_t needed;
	size_t cbuf;
	DWORD count;
	PORT_INFO_1* pi1;
	PORT_INFO_2* pi2;
	LPTSTR pstr;

	UNREFERENCED_PARAMETER(pName);

	if (!pcbNeeded || !pcReturned || (!pPorts && (cbBuf > 0)))
	{
		SetLastError(ERROR_INVALID_PARAMETER);
		return FALSE;
	}
	else if ((1 != Level) && (2 != Level))
	{
		SetLastError(ERROR_INVALID_LEVEL);
		return FALSE;
	}

	needed = 0;
	if (1 == Level)
	{
		needed += sizeof(PORT_INFO_1);
		needed += (wcslen(szPortName) + 1) * sizeof(WCHAR);
	}
	else if (2 == Level)
	{
		needed += sizeof(PORT_INFO_2);
		needed += (wcslen(szPortName) + 1) * sizeof(WCHAR);
		needed += (wcslen(szMonitorName) + 1) * sizeof(WCHAR);
		needed += (wcslen(szDescription) + 1) * sizeof(WCHAR);
	}
	if (!(pPorts && (needed <= cbBuf)))
	{
		SetLastError(ERROR_INSUFFICIENT_BUFFER);
		return FALSE;
	}

	count = 0;
	pstr = (LPTSTR)(pPorts + cbBuf);
	pi1 = (PORT_INFO_1*)pPorts;
	pi2 = (PORT_INFO_2*)pPorts;
	if (1 == Level)
	{
		cbuf = (wcslen(szPortName) + 1);
		pstr -= cbuf;
		wcscpy_s(pstr, cbuf, szPortName);
		pi1->pName = pstr;

		count++;
		pi1++;
	}
	else if (2 == Level)
	{
		cbuf = (wcslen(szPortName) + 1);
		pstr -= cbuf;
		wcscpy_s(pstr, cbuf, szPortName);
		pi2->pPortName = pstr;

		cbuf = (wcslen(szMonitorName) + 1);
		pstr -= cbuf;
		wcscpy_s(pstr, cbuf, szMonitorName);
		pi2->pMonitorName = pstr;

		cbuf = (wcslen(szDescription) + 1);
		pstr -= cbuf;
		wcscpy_s(pstr, cbuf, szDescription);
		pi2->pDescription = pstr;

		pi2->fPortType = PORT_TYPE_READ | PORT_TYPE_WRITE;
		pi2->Reserved = 0;

		count++;
		pi2++;
	}

	*pcbNeeded = (DWORD)needed;
	*pcReturned = count;

	return TRUE;
}

BOOL WINAPI MariaOpenPort(
	_In_     HANDLE hMonitor,
	_In_opt_ LPWSTR pName,
	_Out_    PHANDLE pHandle)
{
	PMARIAMONINI pMariaMonIni = (PMARIAMONINI)hMonitor;

	UNREFERENCED_PARAMETER(pName);

	if (!pName || !pHandle)
	{
		SetLastError(ERROR_INVALID_PARAMETER);
		return FALSE;
	}

	*pHandle = NULL;

	EnterCritical();

	PPORTINI pPortIni = NULL;
	if (pMariaMonIni->pPortIni)
	{
		pPortIni = pMariaMonIni->pPortIni;
	}
	else
	{
		pPortIni = (PPORTINI)GlobalAlloc(GPTR, sizeof(PORTINI));
		pPortIni->hFile = INVALID_HANDLE_VALUE;
		pPortIni->hPrinter = INVALID_HANDLE_VALUE;
		pPortIni->pMariaMonIni = pMariaMonIni;
		pMariaMonIni->pPortIni = pPortIni;
	}

	LeaveCritical();

	*pHandle = (HANDLE)pPortIni;

	return TRUE;
}

BOOL WINAPI MariaStartDocPort(
	_In_ HANDLE hPort,
	_In_ LPWSTR pPrinterName,
	_In_ DWORD  JobId,
	_In_ DWORD  Level,
	_In_ LPBYTE pDocInfo)
{
	PPORTINI pPortIni = (PPORTINI)hPort;
	PDOC_INFO_1 pDocInfo1 = (PDOC_INFO_1)pDocInfo;

	if (1 != Level)
	{
		SetLastError(ERROR_INVALID_LEVEL);
		return FALSE;
	}

	if (pPortIni->hFile != INVALID_HANDLE_VALUE)
	{
		return TRUE;
	}


	WCHAR tmpPath[MAX_PATH - 12 + 1] = { 0 };
	WCHAR filePath[MAX_PATH + 1] = { 0 };
	HKEY  hKey = NULL;
	DWORD cbRegValue = 0;
	DWORD dwRegType = 0;
	if ((ERROR_SUCCESS == pPortIni->pMariaMonIni->pMonitorInit->pMonitorReg->fpCreateKey(
			pPortIni->pMariaMonIni->pMonitorInit->hckRegistryRoot,
			szRegSettingKey,
			REG_OPTION_NON_VOLATILE,
			KEY_QUERY_VALUE,
			NULL,
			&hKey,
			NULL,
			pPortIni->pMariaMonIni->pMonitorInit->hSpooler))
		&& hKey)
	{
		cbRegValue = (_countof(filePath) - 12) * sizeof(WCHAR);
		if (ERROR_SUCCESS == pPortIni->pMariaMonIni->pMonitorInit->pMonitorReg->fpQueryValue(
				hKey,
				szRegDirectory,
				&dwRegType,
				(BYTE*)filePath,
				&cbRegValue,
				pPortIni->pMariaMonIni->pMonitorInit->hSpooler))
		{
			if (REG_SZ == dwRegType)
			{
				wcscpy_s(tmpPath, _countof(tmpPath), filePath);
			}
		}
		pPortIni->pMariaMonIni->pMonitorInit->pMonitorReg->fpCloseKey(hKey, pPortIni->pMariaMonIni->pMonitorInit->hSpooler);
	}
	if (wcslen(tmpPath) == 0)
	{
		if (0 == GetTempPath(_countof(tmpPath), tmpPath))
		{
			return FALSE;
		}
	}
	if (0 == GetTempFileName(tmpPath, L"MR_", 0, filePath))
	{
		return FALSE;
	}

	EnterCritical();
	if (OpenPrinter(pPrinterName, &(pPortIni->hPrinter), NULL))
	{
		wcscpy_s(pPortIni->filePath, MAX_PATH + 1, filePath);
		pPortIni->hFile = CreateFile(pPortIni->filePath,
			GENERIC_WRITE,
			FILE_SHARE_READ | FILE_SHARE_WRITE,
			NULL,
			OPEN_ALWAYS,
			FILE_ATTRIBUTE_NORMAL | FILE_FLAG_SEQUENTIAL_SCAN,
			NULL);
		pPortIni->JobId = JobId;
	}
	LeaveCritical();

	if (INVALID_HANDLE_VALUE == pPortIni->hFile)
	{
		return FALSE;
	}


	JOB_INFO_2 *info2 = NULL;
	DWORD needed = 0, used = 0;
	BOOL success = false;
	EnterCritical();
	GetJob(pPortIni->hPrinter, JobId, 2, NULL, 0, (LPDWORD)&needed);
	if (needed >= 0 && (info2 = (JOB_INFO_2*)GlobalAlloc(GPTR, needed)) != NULL) {
		ZeroMemory(info2, needed);
		success = GetJob(pPortIni->hPrinter, JobId, 2, (LPBYTE)info2, needed, (LPDWORD)&used);
		if (success == 0) {
			GlobalFree(info2);
			info2 = NULL;
		}
	}
	LeaveCritical();

	if (info2 != NULL) {
		WCHAR filePath2[MAX_PATH + 1] = { 0 };
		wcscpy_s(filePath2, _countof(filePath2), filePath);
		wcscat_s(filePath2, _countof(filePath2), L".ini");
		WCHAR str[1024+2] = { 0 };
		DEVMODE *devmode = info2->pDevMode;

		/* JOB_INFO_2 */
		wsprintf(str, L"%ld", info2->JobId); WritePrivateProfileStringW(L"JOBINFO", L"JobId", str, filePath2);
		WritePrivateProfileStringW(L"JOBINFO", L"PrinterName", info2->pPrinterName, filePath2);
		WritePrivateProfileStringW(L"JOBINFO", L"MachineName", info2->pMachineName, filePath2);
		WritePrivateProfileStringW(L"JOBINFO", L"UserName", info2->pUserName, filePath2);
		WritePrivateProfileStringW(L"JOBINFO", L"Document", info2->pDocument, filePath2);
		WritePrivateProfileStringW(L"JOBINFO", L"NotifyName", info2->pNotifyName, filePath2);
		WritePrivateProfileStringW(L"JOBINFO", L"Datatype", info2->pDatatype, filePath2);
		WritePrivateProfileStringW(L"JOBINFO", L"PrintProcessor", info2->pPrintProcessor, filePath2);
		WritePrivateProfileStringW(L"JOBINFO", L"Parameters", info2->pParameters, filePath2);
		WritePrivateProfileStringW(L"JOBINFO", L"DriverName", info2->pDriverName, filePath2);
		WritePrivateProfileStringW(L"JOBINFO", L"Status", info2->pStatus, filePath2);

		wsprintf(str, L"%ld", info2->Status); WritePrivateProfileStringW(L"JOBINFO", L"StatusD", str, filePath2);
		wsprintf(str, L"%ld", info2->Priority); WritePrivateProfileStringW(L"JOBINFO", L"Priority", str, filePath2);
		wsprintf(str, L"%ld", info2->Position); WritePrivateProfileStringW(L"JOBINFO", L"Position", str, filePath2);
		wsprintf(str, L"%ld", info2->StartTime); WritePrivateProfileStringW(L"JOBINFO", L"StartTime", str, filePath2);
		wsprintf(str, L"%ld", info2->UntilTime); WritePrivateProfileStringW(L"JOBINFO", L"UntilTime", str, filePath2);
		wsprintf(str, L"%ld", info2->TotalPages); WritePrivateProfileStringW(L"JOBINFO", L"TotalPages", str, filePath2);
		wsprintf(str, L"%ld", info2->Size); WritePrivateProfileStringW(L"JOBINFO", L"Size", str, filePath2);
		wsprintf(str, L"%ld", info2->Time); WritePrivateProfileStringW(L"JOBINFO", L"Time", str, filePath2);
		wsprintf(str, L"%ld", info2->PagesPrinted); WritePrivateProfileStringW(L"JOBINFO", L"PagesPrinted", str, filePath2);

		/* flush buffer */
		WritePrivateProfileStringW(NULL, NULL, NULL, filePath2);

		/* DEVMODE */
		wsprintf(str, L"%d", devmode->dmSpecVersion); WritePrivateProfileStringW(L"DEVMODE", L"SpecVersion", str, filePath2);
		wsprintf(str, L"%d", devmode->dmDriverVersion); WritePrivateProfileStringW(L"DEVMODE", L"DriverVersion", str, filePath2);
		wsprintf(str, L"%d", devmode->dmSize); WritePrivateProfileStringW(L"DEVMODE", L"Size", str, filePath2);
		wsprintf(str, L"%d", devmode->dmDriverExtra); WritePrivateProfileStringW(L"DEVMODE", L"DriverExtra", str, filePath2);
		wsprintf(str, L"%ld", devmode->dmFields); WritePrivateProfileStringW(L"DEVMODE", L"Fields", str, filePath2);
		wsprintf(str, L"%d", devmode->dmOrientation); WritePrivateProfileStringW(L"DEVMODE", L"Orientation", str, filePath2);
		wsprintf(str, L"%d", devmode->dmPaperSize); WritePrivateProfileStringW(L"DEVMODE", L"PaperSize", str, filePath2);
		wsprintf(str, L"%d", devmode->dmPaperLength); WritePrivateProfileStringW(L"DEVMODE", L"PaperLength", str, filePath2);
		wsprintf(str, L"%d", devmode->dmPaperWidth); WritePrivateProfileStringW(L"DEVMODE", L"PaperWidth", str, filePath2);
		wsprintf(str, L"%d", devmode->dmScale); WritePrivateProfileStringW(L"DEVMODE", L"Scale", str, filePath2);
		wsprintf(str, L"%d", devmode->dmCopies); WritePrivateProfileStringW(L"DEVMODE", L"Copies", str, filePath2);
		wsprintf(str, L"%d", devmode->dmDefaultSource); WritePrivateProfileStringW(L"DEVMODE", L"DefaultSource", str, filePath2);
		wsprintf(str, L"%d", devmode->dmPrintQuality); WritePrivateProfileStringW(L"DEVMODE", L"PrintQuality", str, filePath2);
		wsprintf(str, L"%d", devmode->dmColor); WritePrivateProfileStringW(L"DEVMODE", L"Color", str, filePath2);
		wsprintf(str, L"%d", devmode->dmDuplex); WritePrivateProfileStringW(L"DEVMODE", L"Duplex", str, filePath2);
		wsprintf(str, L"%d", devmode->dmYResolution); WritePrivateProfileStringW(L"DEVMODE", L"YResolution", str, filePath2);
		wsprintf(str, L"%d", devmode->dmTTOption); WritePrivateProfileStringW(L"DEVMODE", L"TTOption", str, filePath2);
		wsprintf(str, L"%d", devmode->dmCollate); WritePrivateProfileStringW(L"DEVMODE", L"Collate", str, filePath2);
		wsprintf(str, L"%d", devmode->dmLogPixels); WritePrivateProfileStringW(L"DEVMODE", L"LogPixels", str, filePath2);
		wsprintf(str, L"%u", devmode->dmBitsPerPel); WritePrivateProfileStringW(L"DEVMODE", L"BitsPerPel", str, filePath2);
		wsprintf(str, L"%d", devmode->dmPelsWidth); WritePrivateProfileStringW(L"DEVMODE", L"PelsWidth", str, filePath2);
		wsprintf(str, L"%d", devmode->dmPelsHeight); WritePrivateProfileStringW(L"DEVMODE", L"PelsHeight", str, filePath2);
		wsprintf(str, L"%d", devmode->dmDisplayFlags); WritePrivateProfileStringW(L"DEVMODE", L"DisplayFlags", str, filePath2);
		wsprintf(str, L"%d", devmode->dmDisplayFrequency); WritePrivateProfileStringW(L"DEVMODE", L"DisplayFrequency", str, filePath2);

		/* flush buffer */
		WritePrivateProfileStringW(NULL, NULL, NULL, filePath2);

		GlobalFree(info2);
	}

	return TRUE;
}

BOOL WINAPI MariaWritePort(
	_In_  HANDLE  hPort,
	_In_  LPBYTE  pBuffer,
	      DWORD   cbBuf,
	_Out_ LPDWORD pcbWritten)
{
	PPORTINI pPortIni = (PPORTINI)hPort;
	BOOL result;

	if (!pcbWritten)
	{
		SetLastError(ERROR_INVALID_PARAMETER);
		return FALSE;
	}

	*pcbWritten = 0;
	result = WriteFile(pPortIni->hFile, pBuffer, cbBuf, pcbWritten, NULL);

	if (result && *pcbWritten == 0)
	{
		SetLastError(ERROR_TIMEOUT);
		result = FALSE;
	}

	return result;
}

BOOL WINAPI MariaReadPort(
	_In_  HANDLE  hPort,
	_Out_ LPBYTE  pBuffer,
	      DWORD   cbBuffer,
	_Out_ LPDWORD pcbRead)
{
	*pcbRead = 0;

	return TRUE;
}

BOOL WINAPI MariaEndDocPort(
	_In_ HANDLE hPort)
{
	PPORTINI pPortIni = (PPORTINI)hPort;

	if (pPortIni->hFile != INVALID_HANDLE_VALUE)
	{
		FlushFileBuffers(pPortIni->hFile);

		EnterCritical();
		CloseHandle(pPortIni->hFile);
		pPortIni->hFile = INVALID_HANDLE_VALUE;
		LeaveCritical();

		HKEY  hKey = NULL;
		DWORD dwTouchFinFile = 0;
		DWORD cbTouchFinFile = sizeof(DWORD);
		DWORD dwRegType = 0;
		WCHAR szFileExtension[MAX_EXTENSION_LENGTH + 1] = { 0 };
		DWORD cbFileExtension = (_countof(szFileExtension)) * sizeof(WCHAR);
		BOOL bFileExtension = false;
		if ((ERROR_SUCCESS == pPortIni->pMariaMonIni->pMonitorInit->pMonitorReg->fpCreateKey(
			pPortIni->pMariaMonIni->pMonitorInit->hckRegistryRoot,
			szRegSettingKey,
			REG_OPTION_NON_VOLATILE,
			KEY_QUERY_VALUE,
			NULL,
			&hKey,
			NULL,
			pPortIni->pMariaMonIni->pMonitorInit->hSpooler))
			&& hKey)
		{
			if (ERROR_SUCCESS == pPortIni->pMariaMonIni->pMonitorInit->pMonitorReg->fpQueryValue(
				hKey,
				szRegTouchFinFile,
				&dwRegType,
				NULL,
				NULL,
				pPortIni->pMariaMonIni->pMonitorInit->hSpooler))
			{
				if (REG_DWORD == dwRegType)
				{
					pPortIni->pMariaMonIni->pMonitorInit->pMonitorReg->fpQueryValue(
						hKey,
						szRegTouchFinFile,
						&dwRegType,
						(BYTE*)&dwTouchFinFile,
						&cbTouchFinFile,
						pPortIni->pMariaMonIni->pMonitorInit->hSpooler);
				}
			}
			if (ERROR_SUCCESS == pPortIni->pMariaMonIni->pMonitorInit->pMonitorReg->fpQueryValue(
				hKey,
				szRegFileExtension,
				&dwRegType,
				NULL,
				NULL,
				pPortIni->pMariaMonIni->pMonitorInit->hSpooler))
			{
				if (REG_SZ == dwRegType)
				{
					pPortIni->pMariaMonIni->pMonitorInit->pMonitorReg->fpQueryValue(
						hKey,
						szRegFileExtension,
						&dwRegType,
						(BYTE*)szFileExtension,
						&cbFileExtension,
						pPortIni->pMariaMonIni->pMonitorInit->hSpooler);
					bFileExtension = true;
				}
			}
			pPortIni->pMariaMonIni->pMonitorInit->pMonitorReg->fpCloseKey(hKey, pPortIni->pMariaMonIni->pMonitorInit->hSpooler);
		}
		if (dwTouchFinFile != 0)
		{
			HANDLE hFile;
			WCHAR filePath[MAX_PATH + 1] = { 0 };
			wcscpy_s(filePath, _countof(filePath), pPortIni->filePath);
			wcscat_s(filePath, _countof(filePath), L".fin");
			hFile = CreateFile(filePath,
				GENERIC_WRITE,
				0,
				NULL,
				CREATE_NEW,
				FILE_ATTRIBUTE_NORMAL,
				NULL);
			if (hFile != INVALID_HANDLE_VALUE)
			{
				CloseHandle(hFile);
			}
		}
		if (bFileExtension && wcsnlen_s(szFileExtension, _countof(szFileExtension) - 1) > 0)
		{
			WCHAR filePath[MAX_PATH + 1] = { 0 };
			wcscpy_s(filePath, _countof(filePath), pPortIni->filePath);
			if (wcsncmp(szFileExtension, L".", 1) != 0)
			{
				wcscat_s(filePath, _countof(filePath), L".");
			}
			wcscat_s(filePath, _countof(filePath), szFileExtension);
			MoveFile(pPortIni->filePath, filePath);
		}
	}

	if (pPortIni->hPrinter != INVALID_HANDLE_VALUE)
	{
		SetJob(pPortIni->hPrinter, pPortIni->JobId, 0, NULL, JOB_CONTROL_SENT_TO_PRINTER);

		EnterCritical();
		ClosePrinter(pPortIni->hPrinter);
		pPortIni->hPrinter = INVALID_HANDLE_VALUE;
		LeaveCritical();
	}

	return TRUE;
}

BOOL WINAPI MariaClosePort(
	_In_ HANDLE hPort)
{
	PPORTINI pPortIni = (PPORTINI)hPort;

	EnterCritical();

	PMARIAMONINI pMariaMonIni = pPortIni->pMariaMonIni;
	if (pPortIni->hFile != INVALID_HANDLE_VALUE)
	{
		CloseHandle(pPortIni->hFile);
		pPortIni->hFile = INVALID_HANDLE_VALUE;
	}
	if (pPortIni->hPrinter != INVALID_HANDLE_VALUE)
	{
		ClosePrinter(pPortIni->hPrinter);
		pPortIni->hPrinter = INVALID_HANDLE_VALUE;
	}
	GlobalFree(pPortIni);
	pMariaMonIni->pPortIni = NULL;

	LeaveCritical();

	return TRUE;
}

/*
BOOL WINAPI MariaAddPort(
	HANDLE hMonitor,
	LPWSTR pName,
	HWND hWnd,
	LPWSTR pMonitorName)
{
	return TRUE;
}

BOOL WINAPI MariaAddPortEx(
	HANDLE hMonitor,
	LPWSTR pName,
	DWORD Level,
	LPBYTE lpBuffer,
	LPWSTR pMonitorName)
{
	return TRUE;
}

BOOL WINAPI MariaConfigurePort(
	HANDLE hMonitor,
	LPWSTR pName,
	HWND hWnd,
	LPWSTR pPortName)
{
	return TRUE;
}

BOOL WINAPI MariaDeletePort(
	HANDLE hMonitor,
	LPWSTR pName,
	HWND hWnd,
	LPWSTR pPortName)
{
	return TRUE;
}

BOOL WINAPI MariaGetPrinterDataFromPort(
	HANDLE hPort,
	DWORD ControlID,
	LPWSTR pValueName,
	LPWSTR lpInBuffer,
	DWORD cbInBuffer,
	LPWSTR lpOutBuffer,
	DWORD cbOutBuffer,
	LPDWORD lpcbReturned)
{
	return TRUE;
}

BOOL WINAPI MariaSetPortTimeOuts(
	HANDLE hPort,
	LPCOMMTIMEOUTS lpCTO,
	DWORD reserved)
{
	return TRUE;
}

BOOL WINAPI MariaXcvOpenPort(
	HANDLE hMonitor,
	LPCWSTR pszObject,
	ACCESS_MASK GrantedAccess,
	PHANDLE phXcv)
{
	return TRUE;
}

DWORD WINAPI MariaXcvDataPort(
	HANDLE hXcv,
	LPCWSTR pszDataName,
	PBYTE pInputData,
	DWORD cbInputData,
	PBYTE pOutputData,
	DWORD cbOutputData,
	PDWORD pcbOutputNeeded)
{
	return 0;
}

BOOL WINAPI MariaXcvClosePort(
	HANDLE hXcv)
{
	return TRUE;
}
*/

VOID WINAPI MariaShutdown(
	_In_ HANDLE hMonitor)
{
	PMARIAMONINI pMariaMonIni = (PMARIAMONINI)hMonitor;

	if (pMariaMonIni->pPortIni)
	{
		if (INVALID_HANDLE_VALUE != pMariaMonIni->pPortIni->hFile)
		{
			CloseHandle(pMariaMonIni->pPortIni->hFile);
			pMariaMonIni->pPortIni->hFile = INVALID_HANDLE_VALUE;
		}
		if (pMariaMonIni->pPortIni->hPrinter != INVALID_HANDLE_VALUE)
		{
			ClosePrinter(pMariaMonIni->pPortIni->hPrinter);
			pMariaMonIni->pPortIni->hPrinter = INVALID_HANDLE_VALUE;
		}
		GlobalFree(pMariaMonIni->pPortIni);
		pMariaMonIni->pPortIni = NULL;
	}
	GlobalFree(pMariaMonIni);
}

/*
DWORD WINAPI MariaSendRecvBidiDataFromPort(
	HANDLE hPort,
	DWORD dwAccessBit,
	LPCWSTR pAction,
	PBIDI_REQUEST_CONTAINER pReqData,
	PBIDI_RESPONSE_CONTAINER* ppResData)
{
//	DebugLog(__FUNCTION__ L"() is called.");
	return 0;
}
*/


MONITOR2 mon2 = {
	sizeof(MONITOR2),
	MariaEnumPorts,		//EnumPorts
	MariaOpenPort,		//OpenPort
	NULL,				//OpenPortEx
	MariaStartDocPort,	//StartDocPort
	MariaWritePort,		//WritePort
	MariaReadPort,		//ReadPort
	MariaEndDocPort,	//EndDocPort
	MariaClosePort,		//ClosePort
	NULL,				//AddPort
	NULL,				//AddPortEx
	NULL,				//ConfigurePort
	NULL,				//DeletePort
	NULL,				//GetPrinterDataFromPort
	NULL,				//SetPortTimeOuts
	NULL,				//XcvOpenPort
	NULL,				//XcvDataPort
	NULL,				//XcvClosePort
	MariaShutdown,		//Shutdown
	NULL				//SendRecvBidiDataFromPort
};

LPMONITOR2 WINAPI InitializePrintMonitor2(_In_ PMONITORINIT pMonitorInit, _Out_ PHANDLE phMonitor)
{
	PMARIAMONINI pMariaMonIni = (PMARIAMONINI)GlobalAlloc(GPTR, sizeof(MARIAMONINI));
	pMariaMonIni->pMonitorInit = pMonitorInit;
	pMariaMonIni->pPortIni = NULL;
	*phMonitor = (HANDLE)pMariaMonIni;

	return &mon2;
}