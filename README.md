# MariaPrintPort

What is this?
=============

This is a printer port monitor for Windows which redirects print data stream to a file named automatically generated.
This program works with [MariaPrintSystem][1].

Requirements and Limitations
============================

This program worked on Windows 2000 and later. I have developed on Windows 10 version 1709.

Implemented
-----------

    Data redirection

Under the developing
--------------------

    ConfigurePort UI for choosing output directory.
    Notificate such as end of output from the port monitor to UI applications by AsyncNotification.

How to use it
=============

After the build DLL files, you can install the port monitor by commands below in the command prompt run as Administrator.
Then you can able to select port named MARIAPRINT: in a property of printer.

    net stop spooler
    copy mariaprintmon.dll %SystemRoot%\System32\
    reg add HKLM\SYSTEM\CurrentControlSet\Control\Print\Monitors\MariaPrintMon /v Driver /t REG_SZ /d mariaprintmon.dll /f
    net start spooler

Configuring output directory by the registry value below.

    reg add HKLM\SYSTEM\CurrentControlSet\Control\Print\Monitors\MariaPrintMon\Settings /v OutputDirectory /t REG_SZ /d "C:\directoryname" /f

The port monitor can notify the end of output by creating the file with extension ".fin".
Configuring this feature by the registry value below.

    reg add HKLM\SYSTEM\CurrentControlSet\Control\Print\Monitors\MariaPrintMon\Settings /v TouchFinFile /t REG_DWORD /d 1 /f

License
=======

Copyright (c) 2018 @marbocub <marbocub@gmail.com>, All rights reserved.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 

This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 3 as published by the Free Software Foundation, see https://www.gnu.org/licenses/.

[1]: https://github.com/marbocub/MariaPrintSystem