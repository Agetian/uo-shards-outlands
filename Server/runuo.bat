@echo off
set FRMDIR="%WINDIR%\Microsoft.NET\Framework\v4.0.30319"
set REFS=/r:Ultima.dll /r:System.Drawing.dll /d:Framework_4_0 /optimize+
set OUTFILE=RunUO.exe
set ICON=RunUO.ico
set CSCARGS=/out:%OUTFILE% /unsafe /win32icon:%ICON% /recurse:*.cs /define:NewAsyncSockets;Framework_4_0
%FRMDIR%\csc.exe %REFS% %CSCARGS%
copy RunUO.exe ..\RunUO.exe
pause