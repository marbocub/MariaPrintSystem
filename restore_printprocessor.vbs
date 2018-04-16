Option Explicit
Dim wsh
Dim PrinterSet
Dim Printer
Dim RetVal

Set wsh = CreateObject("WScript.Shell")
Set PrinterSet = GetObject("winmgmts:").ExecQuery("SELECT * FROM Win32_Printer Where PrintProcessor='mariaprint'")

for each Printer in PrinterSet
	RetVal = wsh.Run("rundll32 printui.dll,PrintUIEntry /Xs /n " & Chr(34) & Printer.Name & Chr(34) & " PrintProcessor winprint", 1, True)
Next

Set PrinterSet = Nothing
Set wsh = Nothing
