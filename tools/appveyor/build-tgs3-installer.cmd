set DEVENV="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\devenv.exe"
%DEVENV% tools/tgstation-server-3/TGStationServer3.sln /Build Release /Project TGServiceInstaller /Out errorFile.txt
