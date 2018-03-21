# MariaPrintSystem

What is this?
=============

This is a print management system for Windows based on V3 printer model.
This program works with [MariaPrintPort][1].

Requirements and Limitations
============================

This program is worked on Windows with .NET 3.0 and later. I have developed by using Visual Studio 2017.

Implemented
-----------

    Tasktray icon/program for virtual spool watcher and the preview application kicker.
    Print Preview/Management UI for preview virtual spools and print to a real printer.

Under the developing
--------------------

    Many

Settings
--------

    reg add "HKLM\SYSTEM\CurrentControlSet\Control\Print\Monitors\MariaPrintMon\Settings" /v "OutputDirectory" /t REG_SZ /d "C:\directoryname" /f
    reg add "HKLM\SOFTWARE\Marbocub\MariaPrintSystem" /v "PsShell" /t REG_SZ /d "MariaPrintManager.exe" /f
    reg add "HKLM\SOFTWARE\Marbocub\MariaPrintSystem" /v "Printer" /t REG_SZ /d "Your Real Printer" /f

License
=======

Copyright (c) 2018 @marbocub <marbocub@gmail.com>, All rights reserved.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 

This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 3 as published by the Free Software Foundation, see LISENCE.

The program Icon
================

The Icon made by [Freepik][2] from [www.flaticon.com][3] is licensed by [CC 3.0 BY][4].

[1]: https://github.com/marbocub/MariaPrintPort
[2]: http://www.freepik.com/
[3]: https://www.flaticon.com/
[4]: http://creativecommons.org/licenses/by/3.0/
