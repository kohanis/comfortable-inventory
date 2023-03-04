using namespace System.IO.Compression

param([string]$projectPath) 

if ($projectPath -eq '') {
    $modDir = Get-ChildItem -Filter 'modinfo.json' -Recurse -Force | % Directory
    if ($modDir -is [array]){
        Write-Output "Multiple mod directories finded:"
        Write-Output $modDir
        exit 1
    }
    $projectPath = $modDir
}

if (!(Test-Path $projectPath) -or !(Test-Path (Join-Path $projectPath 'modinfo.json'))) {
    Write-Output "Path doesn't exist or isn't a mod folder"
    exit 1
}

$projectPath = Resolve-Path -Path $projectPath.TrimEnd('\')

$files = @{}

foreach ($file in Get-ChildItem -Path $projectPath -Recurse -Attributes !Directory | % fullname | ? { $_ -notmatch '(obj|bin)|.+\.(cs(proj)|ps1|zip)' })
{
    $files.Add($file.Replace("${projectPath}\", ""), $file)
}

if (Test-Path "LICENSE.txt") { 
    $files.Add("LICENSE.txt", (Get-Item "LICENSE.txt").fullname) 
}

$zipPath = "${projectPath}.rmod"
if (Test-Path $zipPath)
{
    Remove-Item $zipPath
}

Add-Type -Assembly System.IO.Compression
Add-Type -Assembly System.IO.Compression.FileSystem

$zip = [ZipFile]::Open($zipPath, [ZipArchiveMode]::Create)
foreach ($file in $files.GetEnumerator())
{
    $zipEntry = $zip.CreateEntry($file.Key)
    $zipEntryWriter = New-Object -TypeName System.IO.BinaryWriter $zipEntry.Open()
    $zipEntryWriter.Write([System.IO.File]::ReadAllBytes($file.Value))
    $zipEntryWriter.Flush()
    $zipEntryWriter.Close()
    $zipEntryWriter.Dispose()
}
$zip.Dispose()

Write-Output "Created archive at '${zipPath}'"