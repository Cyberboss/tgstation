$destination = "TGS3.zip"

 If(Test-path $destination) {Remove-item $destination}

Add-Type -assembly "system.io.compression.filesystem"

[io.compression.zipfile]::CreateFromDirectory("($Env:APPVEYOR_BUILD_FOLDER)tools/tgstation-server-3/TGServiceInstaller/bin/Release", $destination) 