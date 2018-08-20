Option Explicit
Dim wsh
Dim fso
Dim dir
Dim path
Dim RetVal

On Error Resume Next
Set wsh = CreateObject("WScript.Shell")
Set fso = CreateObject("Scripting.FileSystemObject")

dir = Property("CustomActionData")
path = fso.BuildPath(dir, "MariaPrintTray.exe")

RetVal = wsh.Run("""" & path & """")
On Error Goto 0
