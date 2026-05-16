
# ============================================================
# ABTW: THE HIDDEN SYSTEM — PROJECT SETUP SCRIPT
# Generates complete folder structure + placeholder files
# Run from: C:\ABTW_The_Hidden_System\
# Usage: .\Setup-ABTW-Project.ps1
# ============================================================

param(
    [string]$RootPath = "C:\ABTW_The_Hidden_System\ABTW_The_Hidden_System",
    [switch]$DryRun,
    [switch]$Verbose
)

# ── Helpers ──────────────────────────────────────────────────
function Write-Step  { param($msg) Write-Host "  ► $msg" -ForegroundColor Cyan }
function Write-OK    { param($msg) Write-Host "  ✓ $msg" -ForegroundColor Green }
function Write-Skip  { param($msg) Write-Host "  ~ $msg" -ForegroundColor Yellow }
function Write-Err   { param($msg) Write-Host "  ✗ $msg" -ForegroundColor Red }

function New-Dir {
    param([string]$Path)
    if (-not (Test-Path $Path)) {
        if (-not $DryRun) { New-Item -ItemType Directory -Path $Path -Force | Out-Null }
        Write-OK "Created dir: $Path"
    } else {
        Write-Skip "Exists:      $Path"
    }
}

function New-PlaceholderFile {
    param([string]$Path, [string]$Content = "// ABTW placeholder — replace with production content")
    if (-not (Test-Path $Path)) {
        if (-not $DryRun) { Set-Content -Path $Path -Value $Content -Encoding UTF8 }
        Write-OK "Created file: $(Split-Path $Path -Leaf)"
    } else {
        Write-Skip "Exists:       $(Split-Path $Path -Leaf)"
    }
}

# ── Banner ────────────────────────────────────────────────────
Write-Host ""
Write-Host "╔══════════════════════════════════════════════════════╗" -ForegroundColor Magenta
Write-Host "║   ACADEMY BEYOND THIS WORLD — PROJECT SETUP v1.0    ║" -ForegroundColor Magenta
Write-Host "║   The Hidden System — Unity Build Structure          ║" -ForegroundColor Magenta
Write-Host "╚══════════════════════════════════════════════════════╝" -ForegroundColor Magenta
Write-Host ""
if ($DryRun) { Write-Host "  [DRY RUN MODE — no files will be created]" -ForegroundColor Yellow; Write-Host "" }

$Base = "$RootPath\Assets\ABTW"

# ════════════════════════════════════════════════════════════
# 1. ROOT UNITY PROJECT STRUCTURE
# ════════════════════════════════════════════════════════════
Write-Host "[ 1/8 ] Root Unity Project" -ForegroundColor White
New-Dir "$RootPath\Assets"
New-Dir "$RootPath\Packages"
New-Dir "$RootPath\ProjectSettings"
New-Dir "$RootPath\Builds\ABTW_Windows"
New-Dir "$RootPath\Builds\ABTW_Windows\ABTW_Data"

# ════════════════════════════════════════════════════════════
# 2. CORE — Game Manager & Player
# ════════════════════════════════════════════════════════════
Write-Host ""
Write-Host "[ 2/8 ] Core Systems" -ForegroundColor White
$core = "$Base\Core"
New-Dir $core

$coreFiles = @(
    "GameManager.cs",
    "PlayerMovement.cs",
    "PlayerStats.cs",
    "CameraController.cs",
    "InputHandler.cs",
    "SceneSetup.cs",
    "ChapterOneDirector.cs",
    "MainMenuController.cs",
    "PauseMenu.cs"
)
foreach ($f in $coreFiles) { New-PlaceholderFile "$core\$f" "// ABTW Core — $f`n// See: docs/ABTW_Programming_Bible.md" }

# ════════════════════════════════════════════════════════════
# 3. SYSTEMS — Perception, Notebook, Glitch, Echo, Trio, etc.
# ════════════════════════════════════════════════════════════
Write-Host ""
Write-Host "[ 3/8 ] Game Systems" -ForegroundColor White

$systems = @{
    "Perception" = @("PerceptionSystem.cs", "PerceptionConfig.asset")
    "Notebook"   = @("NotebookSystem.cs", "NotebookData.asset", "NotebookEntry.asset")
    "Glitch"     = @("GlitchSystem.cs", "ShadowDelayConfig.asset", "LightInstabilityConfig.asset")
    "Echo"       = @("EchoSystem.cs", "EchoConfig.asset")
    "Memory"     = @("MemoryWalkSystem.cs", "MemoryConfig.asset")
    "Trio"       = @("TrioSystem.cs", "TrioConfig.asset")
    "World"      = @("WorldManager.cs", "TimeSystem.asset", "WeatherSystem.asset", "SeasonSystem.asset")
    "Spawner"    = @("AnomalySpawner.cs", "SpawnTable.asset")
    "Save"       = @("SaveSystem.cs", "SaveData.json")
}

foreach ($sys in $systems.Keys) {
    $dir = "$Base\Systems\$sys"
    New-Dir $dir
    foreach ($f in $systems[$sys]) {
        $ext = [System.IO.Path]::GetExtension($f)
        $content = if ($ext -eq ".cs") { "// ABTW System — $f`n// See: docs/ABTW_Programming_Bible.md" }
                   elseif ($ext -eq ".json") { '{"version":"1.0","data":{}}' }
                   else { "# ABTW Asset placeholder: $f" }
        New-PlaceholderFile "$dir\$f" $content
    }
}

# ════════════════════════════════════════════════════════════
# 4. ANOMALIES
# ════════════════════════════════════════════════════════════
Write-Host ""
Write-Host "[ 4/8 ] Anomaly System" -ForegroundColor White

$anomalies = @{
    "Base"       = @("AnomalyBase.cs")
    "Shadow"     = @("ShadowDelayAnomaly.cs")
    "Light"      = @("LightInstabilityAnomaly.cs")
    "Symbol"     = @("SymbolFragmentAnomaly.cs")
    "Reflection" = @("ReflectionMismatchAnomaly.cs")
    "Sound"      = @("SoundEchoAnomaly.cs")
}

foreach ($anom in $anomalies.Keys) {
    $dir = "$Base\Anomalies\$anom"
    New-Dir $dir
    foreach ($f in $anomalies[$anom]) {
        New-PlaceholderFile "$dir\$f" "// ABTW Anomaly — $f`n// See: docs/ABTW_Programming_Bible.md"
    }
}

# ════════════════════════════════════════════════════════════
# 5. COMPANIONS, UI, LEVELS
# ════════════════════════════════════════════════════════════
Write-Host ""
Write-Host "[ 5/8 ] Companions + UI + Levels" -ForegroundColor White

# Companions
New-Dir "$Base\Companions\Lumi"
New-Dir "$Base\Companions\Nela"
New-PlaceholderFile "$Base\Companions\Lumi\LumiCompanion.cs"
New-PlaceholderFile "$Base\Companions\Nela\NelaCompanion.cs"

# UI
$uiDirs = @("Core","Notebook","HUD","Fonts")
foreach ($d in $uiDirs) { New-Dir "$Base\UI\$d" }
New-PlaceholderFile "$Base\UI\Core\UIManager.cs"
New-PlaceholderFile "$Base\UI\Notebook\NotebookUI.cs"
New-PlaceholderFile "$Base\UI\HUD\HUDController.cs"

# Levels
New-Dir "$Base\Levels\Chapter01"
New-Dir "$Base\Levels\Test"
$levelFiles = @("EastCorridor.unity","Classroom.unity","Archive.unity","Observatory.unity")
foreach ($f in $levelFiles) { New-PlaceholderFile "$Base\Levels\Chapter01\$f" "# Unity Scene placeholder: $f" }
New-PlaceholderFile "$Base\Levels\Test\Prototype.unity" "# Unity Scene placeholder: Prototype.unity"

# ════════════════════════════════════════════════════════════
# 6. VISUAL, AUDIO, DATA, PREFABS, RESOURCES, EDITOR
# ════════════════════════════════════════════════════════════
Write-Host ""
Write-Host "[ 6/8 ] Visual / Audio / Data / Prefabs" -ForegroundColor White

$simpleDirs = @(
    "$Base\Visual\Materials",
    "$Base\Visual\Shaders",
    "$Base\Visual\Lighting",
    "$Base\Visual\FX",
    "$Base\Audio\Ambient",
    "$Base\Audio\Perception",
    "$Base\Audio\UI",
    "$Base\Audio\Music",
    "$Base\Data\Patterns",
    "$Base\Data\Stats",
    "$Base\Data\Notebook",
    "$Base\Data\World",
    "$Base\Prefabs\Player",
    "$Base\Prefabs\Anomalies",
    "$Base\Prefabs\UI",
    "$Base\Resources",
    "$Base\Editor",
    "$Base\ScriptableObjects"
)
foreach ($d in $simpleDirs) { New-Dir $d }

# Shaders
New-PlaceholderFile "$Base\Visual\Shaders\SoftShader.shader"       "// ABTW Soft Light Shader — painterly, no harsh shadows"
New-PlaceholderFile "$Base\Visual\Shaders\PerceptionShader.shader" "// ABTW Perception Shader — subtle distortion + glow"

# Data JSON
New-PlaceholderFile "$Base\Data\Patterns\PatternsDatabase.json"    '{"version":"1.0","patterns":[]}'
New-PlaceholderFile "$Base\Data\Stats\PlayerStats.json"            '{"awareness":0,"clarity":0,"connection":0,"timing":0,"stability":0}'
New-PlaceholderFile "$Base\Data\World\WorldStates.json"            '{"season":"Autumn","timeOfDay":"Afternoon","weather":"Clear","anomalyLevel":0}'

# Editor
New-PlaceholderFile "$Base\Editor\ABTWEditorTools.cs" "// ABTW Editor Tools — NOT included in build"

# ════════════════════════════════════════════════════════════
# 7. DOCUMENTATION FOLDER
# ════════════════════════════════════════════════════════════
Write-Host ""
Write-Host "[ 7/8 ] Documentation" -ForegroundColor White

$docsDir = "$RootPath\docs"
New-Dir $docsDir

$docFiles = @(
    "ABTW_Programming_Bible.md",
    "ABTW_Graphic_Bible.md",
    "ABTW_Game_Design_Document.md",
    "ABTW_Production_Schedule.md",
    "ABTW_Symbol_Atlas.md",
    "ABTW_PNG_Assets_Checklist.md"
)
foreach ($f in $docFiles) {
    New-PlaceholderFile "$docsDir\$f" "# $f`n> See full content in ABTW Production Package"
}

# ════════════════════════════════════════════════════════════
# 8. PIPELINE (JSON / YAML / Python)
# ════════════════════════════════════════════════════════════
Write-Host ""
Write-Host "[ 8/8 ] Pipeline Files" -ForegroundColor White

$pipelineDir = "$RootPath\pipeline"
New-Dir $pipelineDir
New-PlaceholderFile "$pipelineDir\academy_config.yaml"   "# ABTW Graph App Config — see pipeline/academy_config.yaml"
New-PlaceholderFile "$pipelineDir\symbol_map.json"       '{"version":"1.0","symbols":[]}'
New-PlaceholderFile "$pipelineDir\character_traits.json" '{"version":"1.0","characters":[]}'
New-PlaceholderFile "$pipelineDir\abtw_pipeline.py"      "# ABTW Graph App Pipeline — see pipeline/abtw_pipeline.py"

# ════════════════════════════════════════════════════════════
# SUMMARY
# ════════════════════════════════════════════════════════════
Write-Host ""
Write-Host "╔══════════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║   ABTW PROJECT STRUCTURE CREATED SUCCESSFULLY ✓     ║" -ForegroundColor Green
Write-Host "╚══════════════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""
Write-Host "  Root:      $RootPath" -ForegroundColor White
Write-Host "  Assets:    $Base" -ForegroundColor White
Write-Host "  Docs:      $RootPath\docs" -ForegroundColor White
Write-Host "  Pipeline:  $RootPath\pipeline" -ForegroundColor White
Write-Host ""
Write-Host "  Next steps:" -ForegroundColor Cyan
Write-Host "  1. Open Unity Hub → Add Project → select $RootPath" -ForegroundColor Yellow
Write-Host "  2. Open docs\ABTW_Programming_Bible.md for script specs" -ForegroundColor Yellow
Write-Host "  3. Run pipeline\abtw_pipeline.py to sync with Graph App" -ForegroundColor Yellow
Write-Host ""