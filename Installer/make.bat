@ECHO OFF

COPY ..\RebusData6\RebusData6\bin\debug\RebusData6.dll /b/y
COPY ..\RebusData6\RebusData6\obj\debug\Interop.ADOX.dll /b/y
COPY ..\RebusSQL6\RebusSQL6\bin\debug\RebusSQL6.exe /b/y
COPY ..\RebusSQL6\RebusSQL6\Resources\RebusSQL32.ico /y
COPY ..\RebusSQL6\RebusSQL6\bin\debug\RebusDatabaseProviders.txt /y
rem COPY "C:\Users\Lee\Documents\Git\Cetani\automation\WiX\Install Source\msi\MsiCustomActions\MsiCustomActions\bin\debug\MsiCustomActions.CA.dll" /b/y

candle -ext WixNetFxExtension -ext WixUtilExtension -ext WixSqlExtension RebusSQL6.wxs
light -ext WixNetFxExtension -ext WixUIExtension -ext WixUtilExtension -ext WixSqlExtension RebusSQL6.wixobj

del *.wixobj
del *.wixpdb

dir /od
