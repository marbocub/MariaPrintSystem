# MariaPrintPort

What is this?
=============

This is a printer port monitor for Windows which redirects print data stream to a temporary file.

Requirements and Limitations
============================

This program worked on Windows 2000 and later. I have developed on Windows 10 version 1709.

Implemented
-----------

    Data redirection

Under the developing
--------------------

    ConfigurePort UI such as choosing directory of temporary files.
    AsyncNotification to communicate between the program and UI applications.

How to use it
=============

After the build dll files, you can install the port monitor by commands below in the command prompt run as Administrator.
You can able to select port named MARIAPRINT: in a property of printer.

    net stop spooler
    copy mariaprintmon.dll %SystemRoot%\System32\
    reg add HKLM\SYSTEM\CurrentControlSet\Control\Print\Monitors\MariaPrintMon /v Driver /t REG_SZ /d mariaprintmon.dll /f
    net start spooler

The port monitor can change output directory.
Configuring directory path by the registry value below.

    reg add HKLM\SYSTEM\CurrentControlSet\Control\Print\Monitors\MariaPrintMon\Settings /v OutputDirectory /t REG_SZ /d "C:\directoryname" /f

The port monitor can notify the end of output by creating the file with extension .fin.
Configuring this feature by the registry value below.

    reg add HKLM\SYSTEM\CurrentControlSet\Control\Print\Monitors\MariaPrintMon\Settings /v TouchFinFile /t REG_DWORD /d 1 /f

License
=======

Selection in progress.