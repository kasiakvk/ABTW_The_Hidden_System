# ============================================================
# ABTW: The Hidden System — PowerShell Script 01
# PURPOSE: Create full Unity project folder structure
# RUN: From C:\ABTW_The_Hidden_System\
# ============================================================

param(
    [string]$RootPath = "C:\ABTW_The_Hidden_System\ABTW_The_Hidden_System",
    [switch]$DryRun
)

$folders = @(
    "Assets\ABTW\Core",
    "Assets\ABTW\Systems\Perception",
    "Assets\ABTW\Systems\Notebook",
    "Assets\ABTW\Systems\Glitch",
    "Assets\ABTW\Systems\Echo",
    "Assets\ABTW\Systems\Memory",
    "Assets\ABTW\Systems\Trio",
    "Assets\ABTW\Systems\World",
    "Assets\ABTW\Systems\Spawner",
    "Assets\ABTW\Systems\Save",
    "Assets\ABTW\Anomalies\Base",
    "Assets\ABTW\Anomalies\Shadow",
    "Assets\ABTW\Anomalies\Light",
    "Assets\ABTW\Anomalies\Symbol",
    "Assets\ABTW\Anomalies\Reflection",
    "Assets\ABTW\Anomalies\Sound",
    "Assets\ABTW\Companions\Lumi",
    "Assets\ABTW\Companions\Nela",
    "Assets\ABTW\UI\Core",
    "Assets\ABTW\UI\Notebook",
    "Assets\ABTW\UI\HUD",
    "Assets\ABTW\UI\Fonts",
    "Assets\ABTW\Levels\Chapter01",
    "Assets\ABTW\Levels\Test",
    "Assets\ABTW\Visual\Materials",
    "Assets\ABTW\Visual\Shaders",
    "Assets\ABTW\Visual\Lighting",
    "Assets\ABTW\Visual\FX",
    "Assets\ABTW\Audio\Ambient",
    "Assets\ABTW\Audio\Perception",
    "Assets\ABTW\Audio\UI",
    "Assets\ABTW\Audio\Music",
    "Assets\ABTW\Data\Patterns",
    "Assets\ABTW\Data\Stats",
    "Assets\ABTW\Data\Notebook",
    "Assets\ABTW\Data\World",
    "Assets\ABTW\Prefabs\Player",
    "Assets\ABTW\Prefabs\Anomalies",
    "Assets\ABTW\Prefabs\UI",
    "Assets\ABTW\Resources",
    "Assets\ABTW\Editor",
    "Assets\ABTW\ScriptableObjects",
    "Packages",
    "ProjectSettings",
    "Builds\ABTW_Windows"
)

Write-Host "`n🌌 ABTW Project Structure Setup" -ForegroundColor Cyan
Write-Host "Root: $RootPath" -ForegroundColor Yellow

foreach ($folder in $folders) {
    $fullPath = Join-Path $RootPath $folder
    if ($DryRun) {
        Write-Host "  [DRY RUN] Would create: $folder" -ForegroundColor Gray
    } else {
        if (-not (Test-Path $fullPath)) {
            New-Item -ItemType Directory -Path $fullPath -Force | Out-Null
            Write-Host "  ✅ Created: $folder" -ForegroundColor Green
        } else {
            Write-Host "  ⏭  Exists:  $folder" -ForegroundColor DarkGray
        }
    }
}

Write-Host "`n✅ Project structure ready!" -ForegroundColor Green