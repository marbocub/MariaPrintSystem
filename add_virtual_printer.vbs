Option Explicit
Dim wsh
Dim RetVal

Set wsh = CreateObject("WScript.Shell")

RetVal = wsh.Run("cscript c:\windows\system32\printing_admin_scripts\ja-jp\prndrvr.vbs -a -m ""Maria Printer"" -v 3 -i %ProgramFiles%\MariaPrintSystem\Inf\mariaprt.inf",1,True)
RetVal = wsh.Run("cscript c:\windows\system32\printing_admin_scripts\ja-jp\prnmngr.vbs -a -p ""Maria Print System Printer"" -m ""Maria Printer"" -r ""MARIAPRINT:""",1,True)
RetVal = wsh.Run("cscript c:\windows\system32\printing_admin_scripts\ja-jp\prnmngr.vbs -t -p ""Maria Print System Printer""",1,True)
