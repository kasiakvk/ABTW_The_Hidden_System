# ============================================================
# ABTW: The Hidden System — PowerShell Script 03
# PURPOSE: Generate all JSON and YAML config files
# RUN: After 01_Setup_Project_Structure.ps1
# ============================================================

param(
    [string]$RootPath = "C:\ABTW_The_Hidden_System\ABTW_The_Hidden_System\Assets\ABTW",
    [switch]$DryRun
)

function Write-ConfigFile($relativePath, $content) {
    $fullPath = Join-Path $RootPath $relativePath
    $dir = Split-Path $fullPath -Parent
    if ($DryRun) {
        Write-Host "  [DRY RUN] $relativePath" -ForegroundColor Gray
        return
    }
    if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
    if (-not (Test-Path $fullPath)) {
        Set-Content -Path $fullPath -Value $content -Encoding UTF8
        Write-Host "  ✅ $relativePath" -ForegroundColor Green
    } else {
        Write-Host "  ⏭  Exists: $relativePath" -ForegroundColor DarkGray
    }
}

Write-Host "`n🌌 ABTW Config File Generator" -ForegroundColor Cyan

# ── academy_config.yaml ──────────────────────────────────────
Write-ConfigFile "Data\World\academy_config.yaml" @"
# ABTW: The Hidden System — Academy World Config
# Canon Lock v1.0

world:
  name: "Academy Beyond This World"
  version: "1.0"
  canon_lock: true

perception:
  awareness_threshold: 0.3
  clarity_threshold: 0.5
  connection_threshold: 0.7
  trio_sync_required: true

time_states:
  - id: morning
    anomaly_multiplier: 0.5
    emotional_tone: "quiet_awareness"
  - id: afternoon
    anomaly_multiplier: 1.0
    emotional_tone: "warm_social"
  - id: sunset
    anomaly_multiplier: 1.5
    emotional_tone: "transition"
  - id: night
    anomaly_multiplier: 2.0
    emotional_tone: "perception_depth"
  - id: rain
    anomaly_multiplier: 1.8
    emotional_tone: "instability"

seasons:
  - id: autumn
    memory_trace_visible: true
    stability_modifier: 0.0
  - id: winter
    memory_trace_visible: false
    stability_modifier: 0.1
  - id: spring
    memory_trace_visible: false
    stability_modifier: 0.05
  - id: summer
    memory_trace_visible: false
    stability_modifier: 0.0

golden_rule: "The Academy does not reveal itself. It responds to perception."
"@

# ── character_traits.json ────────────────────────────────────
Write-ConfigFile "Data\Stats\character_traits.json" @"
{
  "version": "1.0",
  "canon_lock": true,
  "characters": [
    {
      "id": "ASTRA",
      "role": "Perceiver",
      "player_controlled": true,
      "stats": {
        "awareness": 1,
        "clarity": 0,
        "connection": 0,
        "timing": 0,
        "stability": 1
      },
      "visual": {
        "hair": "dark indigo-purple, slightly wavy, medium length",
        "accessory": "small star-shaped hair clip (left side)",
        "bracelet": "glowing softly, reacts to anomaly proximity",
        "outfit": "oversized cosmic hoodie (deep navy/indigo), comfortable trousers, worn trainers",
        "palette": ["#1A2942", "#2D3A6B", "#5B4A8A", "#C9A84C"]
      }
    },
    {
      "id": "LUMI",
      "role": "Analyst",
      "player_controlled": false,
      "stats": {
        "awareness": 0,
        "clarity": 2,
        "connection": 0,
        "timing": 0,
        "stability": 1
      },
      "visual": {
        "hair": "light blonde, neat, often tucked behind ears",
        "accessory": "notebook — always present",
        "outfit": "crisp white shirt, soft mint cardigan, navy trousers",
        "palette": ["#7EC8C8", "#FFFFFF", "#1A2942", "#CCFFEE"]
      }
    },
    {
      "id": "NELA",
      "role": "Aligner",
      "player_controlled": false,
      "stats": {
        "awareness": 0,
        "clarity": 0,
        "connection": 1,
        "timing": 2,
        "stability": 0
      },
      "visual": {
        "hair": "warm auburn, curly, often escaping",
        "accessory": "mismatched earrings — one star, one moon",
        "outfit": "layered warm tones — orange-gold jacket, patterned shirt",
        "palette": ["#C9A84C", "#E8A020", "#8B4A00", "#FFD080"]
      }
    }
  ]
}
"@

# ── player_stats_default.json ────────────────────────────────
Write-ConfigFile "Data\Stats\player_stats_default.json" @"
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

# ── world_states.json ────────────────────────────────────────
Write-ConfigFile "Data\World\world_states.json" @"
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

# ── notebook_entries.json ────────────────────────────────────
Write-ConfigFile "Data\Notebook\notebook_entries.json" @"
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

# ── spawn_table.json ─────────────────────────────────────────
Write-ConfigFile "Data\Patterns\spawn_table.json" @"
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

# ── pipeline_config.yaml ─────────────────────────────────────
Write-ConfigFile "Data\World\pipeline_config.yaml" @"
# ABTW Pipeline Configuration
# Connects Unity project with Python/Graph App pipeline

pipeline:
  version: "1.0"
  project_root: "C:/ABTW_The_Hidden_System/ABTW_The_Hidden_System"
  data_output: "Assets/ABTW/Data"
  python_connector: "pipeline/abtw_pipeline_connector.py"
  graph_app_endpoint: "http://localhost:5000/api/abtw"

export:
  formats: ["json", "yaml"]
  auto_export_on_save: true
  export_path: "pipeline/exports"

logging:
  level: "INFO"
  log_file: "pipeline/logs/pipeline.log"
"@

Write-Host "`n✅ All config files generated!" -ForegroundColor Green