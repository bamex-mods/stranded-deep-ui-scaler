$ErrorActionPreference = "Stop"

$GameRoot = "F:\SteamLibrary\steamapps\common\Stranded Deep"
$Managed = Join-Path $GameRoot "Stranded_Deep_Data\Managed"
$BepInExCore = Join-Path $GameRoot "BepInEx\core"
$Compiler = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"

$ScalerSource = Join-Path $PSScriptRoot "StrandedDeepUIScaler.cs"
$ClientSource = Join-Path $PSScriptRoot "SDK\ModSettingsClient.cs"
$OutDir = Join-Path $PSScriptRoot "out"
$Output = Join-Path $OutDir "StrandedDeepUIScaler.dll"

foreach ($Required in @($Compiler, $ScalerSource, $ClientSource, (Join-Path $BepInExCore "BepInEx.dll"))) {
    if (-not (Test-Path -LiteralPath $Required)) { throw "Required file not found: $Required" }
}

New-Item -ItemType Directory -Force -Path $OutDir | Out-Null
Remove-Item -LiteralPath $Output -Force -ErrorAction SilentlyContinue
Remove-Item -LiteralPath ([IO.Path]::ChangeExtension($Output, ".pdb")) -Force -ErrorAction SilentlyContinue

$Refs = New-Object System.Collections.Generic.List[string]
$Refs.Add((Join-Path $BepInExCore "BepInEx.dll"))
Get-ChildItem -LiteralPath $Managed -Filter "UnityEngine*.dll" -File |
    Sort-Object Name |
    ForEach-Object { $Refs.Add($_.FullName) }

$Args = New-Object System.Collections.Generic.List[string]
$Args.Add("/nologo")
$Args.Add("/target:library")
$Args.Add("/optimize+")
$Args.Add("/debug:pdbonly")
$Args.Add("/langversion:5")
$Args.Add("/out:$Output")
foreach ($Ref in $Refs) { $Args.Add("/reference:$Ref") }
$Args.Add($ScalerSource)
$Args.Add($ClientSource)

Write-Host "=== BUILD StrandedDeepUIScaler v0.3.0 ==="
Write-Host "Workspace: $PSScriptRoot"
Write-Host "IMPORTANT: no compile reference to StrandedDeepModSettings.dll"
Write-Host "References: $($Refs.Count)"
& $Compiler $Args.ToArray()
if ($LASTEXITCODE -ne 0) { throw "Compilation failed: exit $LASTEXITCODE" }
if (-not (Test-Path -LiteralPath $Output)) { throw "Output missing: $Output" }
$Hash = (Get-FileHash -LiteralPath $Output -Algorithm SHA256).Hash
Write-Host "=== BUILD OK ==="
Write-Host "DLL: $Output"
Write-Host "SHA256: $Hash"
