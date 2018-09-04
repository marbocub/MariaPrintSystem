Option Explicit
Dim wsh
Dim RetVal

Set wsh = CreateObject("WScript.Shell")
RetVal = wsh.Run("cscript c:\windows\system32\printing_admin_scripts\ja-jp\prndrvr.vbs -a -m ""Maria Print System (A4/A3/B5/B4) margin 5mm"" -v 3 -i ""%ProgramFiles%\MariaPrintSystem\Inf\mariaprt.inf""",1,True)
RetVal = wsh.Run("cscript c:\windows\system32\printing_admin_scripts\ja-jp\prnmngr.vbs -a -p ""Maria Print System (A4/A3/B5/B4) margin 5mm"" -m ""Maria Print System (A4/A3/B5/B4)"" -r ""MARIAPRINT:""",1,True)
RetVal = wsh.Run("cscript c:\windows\system32\printing_admin_scripts\ja-jp\prndrvr.vbs -a -m ""Maria Print System (A4) margin 5mm"" -v 3 -i ""%ProgramFiles%\MariaPrintSystem\Inf\mariaprt.inf""",1,True)
RetVal = wsh.Run("cscript c:\windows\system32\printing_admin_scripts\ja-jp\prnmngr.vbs -a -p ""Maria Print System (A4) margin 5mm"" -m ""Maria Print System (A4)"" -r ""MARIAPRINT:""",1,True)
RetVal = wsh.Run("cscript c:\windows\system32\printing_admin_scripts\ja-jp\prnmngr.vbs -t -p ""Maria Print System (A4/A3/B5/B4) margin 5mm""",1,True)
