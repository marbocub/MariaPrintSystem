Maria Print System
==================

This is a print accounting and management system for Windows based on V3 printer model. Under the developing.

Subsystems
----------

* MariaPrintInf is a printer INF file and PPD file for postscript virtual printers. The paper size is now A4 only.

* MariaPrintMon is a port monitor that redirects the print data stream to a file with automatically generated name. When it use with a PostScript virtual printer, a PostScript file can be save in a directory. Here we call that directory the virtual spool.

* MariaPrintTray is a tasktray icon / program that monitors new files in the virtual spool and kickstart the preview program with the name of the created file as an argument.

* MariaPrintManager is a preview and print program for a PostScript file. It prints a PS file in the virtual spool to a real printer. It supports user interface for authenticate a user and accounting by page count.

* MariaPrintProcessor is a print processor to allow or deny print based on the document name of the print spool.

* MariaPrintaSetup is a installer.

* MariaPrintConfigurator is a helper program for the installer. It configuring which print processor will used for a printer. 

Requirements and Limitations
============================

This program is worked on Windows with .NET 3.0 and later. 
We have developed on Windows 10 version 1709.

Installer
=========

After the build all projects, you can find a installer named as MariaPrintSetup.msi under the directory MariaPrintSetup.

Manual installation procedure
=============================

The manual installation procedures are below but we recommend using the installer.

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

All subsystem except MariaPrintProcessor
----------------------------------------

Copyright (c) 2018 @marbocub <marbocub@gmail.com>, All rights reserved.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 

This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 3 as published by the Free Software Foundation, see LICENSE.

Only MariaPrintProcessor subsystem
----------------------------------

Copyright (c) 1990-2003  Microsoft Corporation All Rights Reserved.

Modified by @marbocub <marbocub@gmail.com>, All Rights Reserved.

The MICROSOFT LIMITED PUBLIC LICENSE, see MariaPrintProcessor/license.rtf.

The program Icon
================

The Icon made by [Freepik][2] from [www.flaticon.com][3] is licensed by [CC 3.0 BY][4].

[1]: https://github.com/marbocub/MariaPrintPort
[2]: http://www.freepik.com/
[3]: https://www.flaticon.com/
[4]: http://creativecommons.org/licenses/by/3.0/
