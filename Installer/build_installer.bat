VerSync -bin="..\ShooterDownloader\bin\Release\ShooterDownloader.exe" -in=installer.nsi.tmpl -out=installer.nsi
"%NSIS_PATH%\makensis" installer.nsi
VerSync -bin="..\ShooterDownloader\bin\Release\ShooterDownloader.exe" -in=build_7z.bat.tmpl -out=build_7z.bat
build_7z.bat