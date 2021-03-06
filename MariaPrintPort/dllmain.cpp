/*
* Copyright (c) @marbocub <marbocub@gmail.com>
* All rights reserved.
*
* MariaPrintMon : A part of Maria Print System.
*
*/
#include "stdafx.h"
#include <stdio.h>

HINSTANCE hInst = NULL;
CRITICAL_SECTION CriticalSection;

extern VOID EnterCritical(VOID)
{
	EnterCriticalSection(&CriticalSection);
}

extern VOID LeaveCritical(VOID)
{
	LeaveCriticalSection(&CriticalSection);
}


BOOL APIENTRY DllMain(
	HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved)
{
	static BOOL bInit = FALSE;

	UNREFERENCED_PARAMETER(lpReserved);

	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		hInst = hModule;
		bInit = InitializeCriticalSectionAndSpinCount(&CriticalSection, 0x80000000);
		DisableThreadLibraryCalls(hModule);
		break;

	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
		break;

	case DLL_PROCESS_DETACH:
		if (bInit)
		{
			DeleteCriticalSection(&CriticalSection);
			bInit = FALSE;
		}
		break;
	}
	return TRUE;
}

