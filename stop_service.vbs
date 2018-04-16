Option Explicit
Dim ServiceSet
Dim Service
Dim RetVal

Set ServiceSet = GetObject("winmgmts:").ExecQuery("Select * From Win32_Service Where Name='spooler'")

for each Service in ServiceSet
	RetVal = Service.StopService()
Next

Set ServiceSet = Nothing
