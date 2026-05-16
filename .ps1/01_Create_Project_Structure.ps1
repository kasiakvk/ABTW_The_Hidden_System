# ============================================================
# ABTW: THE HIDDEN SYSTEM — Project Structure Generator
# PowerShell Script 01 — Creates full Unity project folder tree
# Run from: C:\ABTW_The_Hidden_System\
# ============================================================

param(
    [string]$RootPath = "C:\ABTW_The_Hidden_System\ABTW_The_Hidden_System",
    [switch]$DryRun
)

function New-ABTWFolder {
    param([string]$Path)
    if ($DryRun) {
        Write-Host "[DRY RUN] Would create: $Path" -ForegroundColor Cyan
    } else {
        if (-not (Test-Path $Path)) {
            New-Item -ItemType Directory -Path $Path -Force | Out-Null
            Write-Host "[CREATED] $Path" -ForegroundColor Green
        } else {
            Write-Host "[EXISTS]  $Path" -ForegroundColor Yellow
        }
    }
}

Write-Host "`n🌌 ABTW PROJECT STRUCTURE GENERATOR" -ForegroundColor Magenta
Write-Host "Root: $RootPath" -ForegroundColor Cyan
Write-Host "Mode: $(if ($DryRun) { 'DRY RUN' } else { 'LIVE' })" -ForegroundColor Cyan
Write-Host "─────────────────────────────────────────`n"

$folders = @(
    # Unity Root
    "$RootPath\Assets",
    "$RootPath\Assets\ABTW",
    "$RootPath\Packages",
    "$RootPath\ProjectSettings",
    "$RootPath\Builds",
    "$RootPath\Builds\ABTW_Windows",

    # Core
    "$RootPath\Assets\ABTW\Core",

    # Systems
    "$RootPath\Assets\ABTW\Systems",
    "$RootPath\Assets\ABTW\Systems\Perception",
    "$RootPath\Assets\ABTW\Systems\Notebook",
    "$RootPath\Assets\ABTW\Systems\Glitch",
    "$RootPath\Assets\ABTW\Systems\Echo",
    "$RootPath\Assets\ABTW\Systems\Memory",
    "$RootPath\Assets\ABTW\Systems\Trio",
    "$RootPath\Assets\ABTW\Systems\World",
    "$RootPath\Assets\ABTW\Systems\Spawner",
    "$RootPath\Assets\ABTW\Systems\Save",

    # Anomalies
    "$RootPath\Assets\ABTW\Anomalies",
    "$RootPath\Assets\ABTW\Anomalies\Base",
    "$RootPath\Assets\ABTW\Anomalies\Shadow",
    "$RootPath\Assets\ABTW\Anomalies\Light",
    "$RootPath\Assets\ABTW\Anomalies\Symbol",
    "$RootPath\Assets\ABTW\Anomalies\Reflection",
    "$RootPath\Assets\ABTW\Anomalies\Sound",

    # Companions
    "$RootPath\Assets\ABTW\Companions",
    "$RootPath\Assets\ABTW\Companions\Lumi",
    "$RootPath\Assets\ABTW\Companions\Nela",

    # UI
    "$RootPath\Assets\ABTW\UI",
    "$RootPath\Assets\ABTW\UI\Core",
    "$RootPath\Assets\ABTW\UI\Notebook",
    "$RootPath\Assets\ABTW\UI\HUD",
    "$RootPath\Assets\ABTW\UI\Fonts",
    "$RootPath\Assets\ABTW\UI\Prefabs",

    # Levels
    "$RootPath\Assets\ABTW\Levels",
    "$RootPath\Assets\ABTW\Levels\Chapter01",
    "$RootPath\Assets\ABTW\Levels\Test",

    # Visual
    "$RootPath\Assets\ABTW\Visual",
    "$RootPath\Assets\ABTW\Visual\Materials",
    "$RootPath\Assets\ABTW\Visual\Shaders",
    "$RootPath\Assets\ABTW\Visual\Lighting",
    "$RootPath\Assets\ABTW\Visual\FX",

    # Audio
    "$RootPath\Assets\ABTW\Audio",
    "$RootPath\Assets\ABTW\Audio\Ambient",
    "$RootPath\Assets\ABTW\Audio\Perception",
    "$RootPath\Assets\ABTW\Audio\UI",
    "$RootPath\Assets\ABTW\Audio\Music",

    # Data
    "$RootPath\Assets\ABTW\Data",
    "$RootPath\Assets\ABTW\Data\Patterns",
    "$RootPath\Assets\ABTW\Data\Stats",
    "$RootPath\Assets\ABTW\Data\Notebook",
    "$RootPath\Assets\ABTW\Data\World",

    # Prefabs
    "$RootPath\Assets\ABTW\Prefabs",
    "$RootPath\Assets\ABTW\Prefabs\Player",
    "$RootPath\Assets\ABTW\Prefabs\Anomalies",
    "$RootPath\Assets\ABTW\Prefabs\UI",

    # ScriptableObjects
    "$RootPath\Assets\ABTW\ScriptableObjects",

    # Resources
    "$RootPath\Assets\ABTW\Resources",

    # Editor (excluded from build)
    "$RootPath\Assets\ABTW\Editor",

    # Graphics (PNG Assets)
    "$RootPath\Assets\ABTW\Graphics",
    "$RootPath\Assets\ABTW\Graphics\Characters",
    "$RootPath\Assets\ABTW\Graphics\Characters\Astra",
    "$RootPath\Assets\ABTW\Graphics\Characters\Lumi",
    "$RootPath\Assets\ABTW\Graphics\Characters\Nela",
    "$RootPath\Assets\ABTW\Graphics\Characters\Supporting",
    "$RootPath\Assets\ABTW\Graphics\Environments",
    "$RootPath\Assets\ABTW\Graphics\Environments\EastCorridor",
    "$RootPath\Assets\ABTW\Graphics\Environments\Observatory",
    "$RootPath\Assets\ABTW\Graphics\Environments\Archive",
    "$RootPath\Assets\ABTW\Graphics\Environments\Dormitories",
    "$RootPath\Assets\ABTW\Graphics\Environments\Gardens",
    "$RootPath\Assets\ABTW\Graphics\Symbols",
    "$RootPath\Assets\ABTW\Graphics\UI",
    "$RootPath\Assets\ABTW\Graphics\FX",
    "$RootPath\Assets\ABTW\Graphics\Covers"
)

foreach ($folder in $folders) {
    New-ABTWFolder -Path $folder
}

Write-Host "`n✅ Project structure complete! ($($folders.Count) folders)" -ForegroundColor Green
Write-Host "Next: Run 02_Generate_CS_Scripts.ps1`n" -ForegroundColor Cyan
