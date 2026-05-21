
# ============================================================
# ABTW: The Hidden System — PowerShell Script 06
# PURPOSE: Sync canonical JSON/YAML data files from
#          ABTW_The_Hidden_System into ABTW_Unity/Assets/Data/
# RUN:     From any location — paths are resolved automatically
# ============================================================

param(
  [string]$UnityRoot    = "C:\ABTW_The_Hidden_System\ABTW_The_Hidden_System\ABTW_Unity",
  [string]$CanonRoot    = "C:\ABTW_The_Hidden_System\ABTW_The_Hidden_System",
  [switch]$DryRun
)

# ── Colour helpers ────────────────────────────────────────────────────────────
function Write-OK    { param($m) Write-Host "  ✅ $m" -ForegroundColor Green   }
function Write-Skip  { param($m) Write-Host "  ⏭  $m" -ForegroundColor DarkGray }
function Write-Warn  { param($m) Write-Host "  ⚠️  $m" -ForegroundColor Yellow  }
function Write-Fail  { param($m) Write-Host "  ❌ $m" -ForegroundColor Red     }
function Write-Info  { param($m) Write-Host "  ℹ️  $m" -ForegroundColor Cyan    }

Write-Host "`n🌌 ABTW Data Sync — Canon → Unity" -ForegroundColor Magenta
Write-Host "   Canon  : $CanonRoot"  -ForegroundColor Yellow
Write-Host "   Unity  : $UnityRoot"  -ForegroundColor Yellow
if ($DryRun) { Write-Host "   MODE   : DRY RUN (no files written)" -ForegroundColor Cyan }

# ── Verify roots exist ────────────────────────────────────────────────────────
if (-not (Test-Path $CanonRoot)) {
    Write-Fail "Canon root not found: $CanonRoot"
    exit 1
}
if (-not (Test-Path $UnityRoot)) {
    Write-Fail "Unity root not found: $UnityRoot"
    exit 1
}

$UnityData = Join-Path $UnityRoot "Assets\Data"

# ── Mapping: source (relative to CanonRoot\Assets\ABTW\Data) → dest subfolder ─
# Format: @{ SourceRelative = "DestSubfolder" }
$syncMap = @{
    # World configs
    "Assets\ABTW\Data\World\academy_config.yaml"        = "World"
    "Assets\ABTW\Data\World\world_states.json"          = "World"
    "Assets\ABTW\Data\World\pipeline_config.yaml"       = "World"

    # Stats
    "Assets\ABTW\Data\Stats\character_traits.json"      = "Stats"
    "Assets\ABTW\Data\Stats\player_stats_default.json"  = "Stats"

    # Notebook
    "Assets\ABTW\Data\Notebook\notebook_entries.json"   = "Notebook"

    # Patterns / spawn
    "Assets\ABTW\Data\Patterns\spawn_table.json"        = "Patterns"

    # Symbol map (root-level in canon Data folder)
    "Assets\ABTW\Data\symbol_map.json"                  = ""
}

$copied  = 0
$skipped = 0
$missing = 0

Write-Host "`n📂 Syncing files..." -ForegroundColor Cyan

foreach ($entry in $syncMap.GetEnumerator()) {
    $srcPath  = Join-Path $CanonRoot $entry.Key
    $destDir  = if ($entry.Value -ne "") {
                    Join-Path $UnityData $entry.Value
                } else {
                    $UnityData
                }
    $fileName = Split-Path $srcPath -Leaf
    $destPath = Join-Path $destDir $fileName

    if (-not (Test-Path $srcPath)) {
        Write-Warn "Source missing — skipping: $($entry.Key)"
        $missing++
        continue
    }

    if ($DryRun) {
        Write-Info "[DRY RUN] Would copy: $fileName → $destDir"
        $copied++
        continue
    }

    # Ensure destination directory exists
    if (-not (Test-Path $destDir)) {
        New-Item -ItemType Directory -Path $destDir -Force | Out-Null
    }

    # Copy (overwrite if newer)
    $srcItem  = Get-Item $srcPath
    $needCopy = $true
    if (Test-Path $destPath) {
        $destItem = Get-Item $destPath
        if ($destItem.LastWriteTime -ge $srcItem.LastWriteTime) {
            Write-Skip "Up-to-date : $fileName"
            $skipped++
            $needCopy = $false
        }
    }

    if ($needCopy) {
        Copy-Item -Path $srcPath -Destination $destPath -Force
        Write-OK "Copied     : $fileName → Assets\Data\$($entry.Value)"
        $copied++
    }
}

# ── Also create any missing stub files that don't exist in canon yet ──────────
$stubs = @{
    "World\world_states.json" = @"
{
  "version": "1.0",
  "current_chapter": 1,
  "current_time": "afternoon",
  "current_season": "autumn",
  "current_weather": "clear",
  "anomalies_active": [],
  "anomalies_recorded": [],
  "anomalies_echo": [],
  "symbols_discovered": [],
  "symbols_activated": [],
  "trio_activations": 0,
  "world_stability": 1.0,
  "hidden_layer_exposed": false
}
"@
    "Stats\player_stats_default.json" = @"
{
  "version": "1.0",
  "player_id": "ASTRA",
  "rank": 1,
  "rank_title": "Observer",
  "exp": 0,
  "exp_to_next": 100,
  "stats": {
    "awareness": 1,
    "clarity": 0,
    "connection": 0,
    "timing": 0,
    "stability": 1
  },
  "layers_visible": [0],
  "trio_activation_available": false,
  "memory_walk_available": false
}
"@
    "Notebook\notebook_entries.json" = @"
{
  "version": "1.0",
  "entries": [
    {
      "id": "ENTRY_001",
      "pattern_id": "SYM_001",
      "title": "PATTERN: SHADOW DELAY",
      "status": "Unrecorded",
      "tab": "PATTERNS",
      "observations": [
        "movement mismatch (x3)",
        "delay ~0.4s",
        "location: East Corridor"
      ],
      "lumi_note": "It's repeating. The interval is consistent.",
      "nela_note": "Don't write it yet. Something else is coming.",
      "echo_note": "This is not the original pattern. A trace of something recorded too early.",
      "recorded_at": null,
      "is_echo": false
    }
  ]
}
"@
    "Patterns\spawn_table.json" = @"
{
  "version": "1.0",
  "chapter": 1,
  "anomalies": [
    {
      "id": "ANOM_001",
      "type": "ShadowDelay",
      "location": "ROOM_CORRIDOR_EAST",
      "trigger_awareness": 0.3,
      "intensity_start": 0.2,
      "intensity_max": 0.8,
      "trio_required": false,
      "book": 1
    },
    {
      "id": "ANOM_002",
      "type": "ReflectionMismatch",
      "location": "ROOM_LIBRARY_MAIN",
      "trigger_awareness": 0.3,
      "intensity_start": 0.1,
      "intensity_max": 0.5,
      "trio_required": false,
      "book": 2
    },
    {
      "id": "ANOM_003",
      "type": "SymbolFragment",
      "location": "ROOM_OBSERVATORY_MAIN",
      "trigger_awareness": 0.5,
      "intensity_start": 0.3,
      "intensity_max": 1.0,
      "trio_required": true,
      "book": 1
    }
  ]
}
"@
}

Write-Host "`n📝 Checking stub files..." -ForegroundColor Cyan

foreach ($stub in $stubs.GetEnumerator()) {
    $destPath = Join-Path $UnityData $stub.Key
    $destDir  = Split-Path $destPath -Parent

    if (Test-Path $destPath) {
        Write-Skip "Already exists: $($stub.Key)"
        continue
    }

    if ($DryRun) {
        Write-Info "[DRY RUN] Would create stub: $($stub.Key)"
        $copied++
        continue
    }

    if (-not (Test-Path $destDir)) {
        New-Item -ItemType Directory -Path $destDir -Force | Out-Null
    }

    Set-Content -Path $destPath -Value $stub.Value -Encoding UTF8
    Write-OK "Created stub : $($stub.Key)"
    $copied++
}

# ── Summary ───────────────────────────────────────────────────────────────────
Write-Host "`n📊 Summary" -ForegroundColor Magenta
Write-Host "   Copied / Created : $copied" -ForegroundColor Green
Write-Host "   Up-to-date       : $skipped" -ForegroundColor DarkGray
Write-Host "   Source missing   : $missing" -ForegroundColor $(if ($missing -gt 0) { "Yellow" } else { "DarkGray" })
Write-Host "`n✅ Data sync complete!`n" -ForegroundColor Green