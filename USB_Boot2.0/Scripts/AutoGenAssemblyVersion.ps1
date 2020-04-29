#requires -version 3

<#
.SYNOPSIS
Automatically Generate Assembly Version
Change version information in Visual Studio project file AssemblyInfo.cs. Values AssemblyFileVersion and AssemblyVersion.

.DESCRIPTION
Automatically Generate Assembly Version
(c) 2017-2018 Michal Zobec, ZOBEC Consulting. All Rights Reserved.
web: www.michalzobec.cz, mail: michal@zobec.net
License: Creative Commons Attribution-ShareAlike 4.0 International (CC BY-SA 4.0) https://creativecommons.org/licenses/by-sa/4.0/

.PARAMETER $(ProjectDir)
Full path to project directory. By default will called as prebuild command:
"%windir%\System32\WindowsPowerShell\v1.0\powershell.exe" -executionpolicy bypass -file <full path to script file AutoGenAssemblyVersion.ps1><full path to project directory>

Example with Visual Studio values:
"%windir%\System32\WindowsPowerShell\v1.0\powershell.exe" -executionpolicy bypass -file $(ProjectDir)\CommonTools\AutoGenAssemblyVersion.ps1 $(ProjectDir)

.OUTPUTS
Output is forwarded to Visual Studio build output console.

.EXAMPLE
C:\> "%windir%\System32\WindowsPowerShell\v1.0\powershell.exe" -executionpolicy bypass -file $(ProjectDir)\CommonTools\AutoGenAssemblyVersion.ps1 C:\DEV\PROJECT1\

.LINK
http://www.michalzobec.cz/

#>
Set-StrictMode -Version Latest

param(
	[string]$projectDir
)

$filePath = "$projectDir" + "Properties\AssemblyInfo.cs"
$xmlPath = "$projectDir" + "Scripts\config.xml"
if(![System.IO.File]::Exists($filePath))
{
    Write-Error "AssemblyInfo.cs file does not exist on filepath $filePath"
    exit(-1)
}
[xml]$ConfigFile = Get-Content -path $xmlPath
$newProductName = $ConfigFile.zobecautogenassemblyinfo.productname
$fileContent = [System.IO.File]::ReadAllText($filePath)

# check for 'using System.Reflection;' statement, add to the beginning of the file if it does not exist
$usingStatementRegex = "^(?!\/\/).*using[ ]*System.Reflection;.*$"
if (![Regex]::IsMatch($fileContent, $usingStatementRegex, [System.Text.RegularExpressions.RegexOptions]::Multiline))
{
    $fileContent = "using System.Reflection;" + $([System.Environment]::NewLine) + $fileContent
}

# regex to match AssemblyFileInfo attribute
$assemblyFileVersionRegex = '^(?!\/\/).*(\[[ ]*assembly[ ]*:[ ]*AssemblyFileVersion[ ]*\([ ]*")(.*)("[ ]*\)[ ]*\].*)$'
$assemblyVersionRegex = '^(?!\/\/).*(\[[ ]*assembly[ ]*:[ ]*AssemblyVersion[ ]*\([ ]*")(.*)("[ ]*\)[ ]*\].*)$'
$assemblyProductRegex = '^(?!\/\/).*(\[[ ]*assembly[ ]*:[ ]*AssemblyProduct[ ]*\([ ]*")(.*)("[ ]*\)[ ]*\].*)$'
$assemblyTitleRegex = '^(?!\/\/).*(\[[ ]*assembly[ ]*:[ ]*AssemblyTitle[ ]*\([ ]*")(.*)("[ ]*\)[ ]*\].*)$'

$newVersion = (Get-Date).ToString("yy.MM.dd.HHmm")

[string]$initialVersion = $null

[System.Text.RegularExpressions.MatchCollection]$matches = [Regex]::Matches($fileContent, $assemblyFileVersionRegex, [System.Text.RegularExpressions.RegexOptions]::Multiline)
if ($matches.Count -gt 1)
{
    Write-Error "Error: multiple occurences of AssemblyFileVersion in the file. File has not been changed."
    exit(-2)
}
elseif ($matches.Count -eq 1)
{
    $initialVersion = $matches[0].Groups[2].Value

}
if ($matches.Count -eq 1)
{
    # match found in file - replace version
    $fileContent = [Regex]::Replace($fileContent, $assemblyFileVersionRegex, '${1}' + "$newVersion" + '$3', [System.Text.RegularExpressions.RegexOptions]::Multiline)
    $fileContent = [Regex]::Replace($fileContent, $assemblyVersionRegex, '${1}' + "$newVersion" + '$3', [System.Text.RegularExpressions.RegexOptions]::Multiline)
    $fileContent = [Regex]::Replace($fileContent, $assemblyProductRegex, '${1}' + "$newProductName" + '$3', [System.Text.RegularExpressions.RegexOptions]::Multiline)
    $fileContent = [Regex]::Replace($fileContent, $assemblyTitleRegex, '${1}' + "$newProductName" + '$3', [System.Text.RegularExpressions.RegexOptions]::Multiline)
}
else
{
    # match not found in file - add a complete element
    $fileContent += '[assembly: AssemblyFileVersion("' + "$newVersion" + '")]'
}

[System.IO.File]::WriteAllText($filePath, $fileContent)
Write-Host $(if (!$initialVersion) { "no initial file version, new file version $newVersion"} else { "initial file version $initialVersion, new file version $newVersion" })