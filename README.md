# MariaPrintSystem

This is a print management system for Windows based on V3 printer model.

Subsystems
----------

* MariaPrintPort is a port monitor redirects print data stream to a file named automatically generated. It using with a postscript virtual V3 printer can save postscript file to a directory. Here we call that directory the virtual spool.

* MariaPrintTray is a tasktray icon/program monitor new files in the virtual spool and kickstart a program with arguments.

* MariaPrintManager is a preview and print program prints a postscript file in the virtual spool to a real printer. It supports authenticate a user and accounting by page count.

Requirements and Limitations
============================

This program is worked on Windows with .NET 3.0 and later. 
I have developed on Windows 10 version 1709.

How to use it
=============

Port monitor
------------

After the build DLL files, you can install the port monitor by commands below in the command prompt run as Administrator.
Then you can able to select port named MARIAPRINT: in property page of a printer.

    net stop spooler
    copy mariaprintmon.dll %SystemRoot%\System32\
    reg add HKLM\SYSTEM\CurrentControlSet\Control\Print\Monitors\MariaPrintMon /v Driver /t REG_SZ /d mariaprintmon.dll /f
    net start spooler

Relationship between the port monitor and the tasktray program
--------------------------------------------------------------

The virtual spool directory of the port monitor is able to customize and is stored in the registry value. The tasktray program read the registry value same as the port monitor and monitor that directory.

    reg add "HKLM\SYSTEM\CurrentControlSet\Control\Print\Monitors\MariaPrintMon\Settings" /v "OutputDirectory" /t REG_SZ /d "C:\directoryname" /f

Relationship between the tasktray program and the preview application
---------------------------------------------------------------------

The tasktray program kickstart a program saved in the registry value. The argument is a name of file created new in the virtual spool.

    reg add "HKLM\SOFTWARE\Marbocub\MariaPrintSystem" /v "PsShell" /t REG_SZ /d "MariaPrintManager.exe" /f

The preview program print postscript file to a printer saved in the registry value.

    reg add "HKLM\SOFTWARE\Marbocub\MariaPrintSystem" /v "Printer" /t REG_SZ /d "Your Real Printer" /f

License
=======

Copyright (c) 2018 @marbocub <marbocub@gmail.com>, All rights reserved.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 

This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 3 as published by the Free Software Foundation, see LICENSE.

The program Icon
================

The Icon made by [Freepik][2] from [www.flaticon.com][3] is licensed by [CC 3.0 BY][4].

[1]: https://github.com/marbocub/MariaPrintPort
[2]: http://www.freepik.com/
[3]: https://www.flaticon.com/
[4]: http://creativecommons.org/licenses/by/3.0/
