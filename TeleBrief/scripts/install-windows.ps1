# Install the 'brief' command on Windows
$ErrorActionPreference = 'Stop'
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$repoRoot = Resolve-Path "$scriptDir\.." | Select-Object -ExpandProperty Path
$targetDir = Join-Path $env:USERPROFILE 'bin'
if (-not (Test-Path $targetDir)) {
    New-Item -ItemType Directory -Path $targetDir | Out-Null
}
$wrapper = "@echo off`r`n" + "dotnet run --project `"$repoRoot`" -- %*"
$wrapperPath = Join-Path $targetDir 'brief.cmd'
Set-Content -Path $wrapperPath -Value $wrapper -Encoding ASCII
if (-not ($env:Path -split ';' | Where-Object { $_ -eq $targetDir })) {
    [Environment]::SetEnvironmentVariable('Path', "$($env:Path);$targetDir", [EnvironmentVariableTarget]::User)
}
Write-Host "Installed 'brief' command. Restart the shell to use 'brief news' or 'brief beat'."
