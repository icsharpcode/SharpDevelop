# https://github.com/chocolatey/chocolatey/wiki/CreatePackagesQuickStart
# https://github.com/chocolatey/chocolatey/wiki/HelpersInstallChocolateyPackage

$packageName = 'sharpdevelop'
$installerType = 'msi' 
$url = 'http://downloads.sourceforge.net/sharpdevelop/SharpDevelop_4.4.1.9729_Setup.msi?download'
$silentArgs = '/q'
$validExitCodes = @(0)

Install-ChocolateyPackage "$packageName" "$installerType" "$silentArgs" "$url" -validExitCodes $validExitCodes

