# ============================================================
# ABTW: The Hidden System — PowerShell Script 04
# PURPOSE: Generate Python pipeline connector scripts
# RUN: After 01_Setup_Project_Structure.ps1
# ============================================================

param(
    [string]$ProjectRoot = "C:\ABTW_The_Hidden_System\ABTW_The_Hidden_System",
    [switch]$DryRun
)

function Write-PipelineFile($relativePath, $content) {
    $fullPath = Join-Path $ProjectRoot $relativePath
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

Write-Host "`n🌌 ABTW Python Pipeline Generator" -ForegroundColor Cyan
Write-Host "Target: $ProjectRoot" -ForegroundColor Yellow

# ── abtw_pipeline_connector.py ───────────────────────────────
Write-PipelineFile "pipeline\abtw_pipeline_connector.py" @"
"""
ABTW: The Hidden System — Pipeline Connector
Connects Unity project data with Graph App via REST API
Version: 1.0
"""

import json
import yaml
import os
import logging
import requests
from pathlib import Path
from datetime import datetime

# ── Configuration ─────────────────────────────────────────────
PROJECT_ROOT = Path(__file__).parent.parent
DATA_PATH    = PROJECT_ROOT / "Assets" / "ABTW" / "Data"
EXPORT_PATH  = PROJECT_ROOT / "pipeline" / "exports"
LOG_PATH     = PROJECT_ROOT / "pipeline" / "logs"
CONFIG_FILE  = DATA_PATH / "World" / "pipeline_config.yaml"

GRAPH_APP_ENDPOINT = "http://localhost:5000/api/abtw"

# ── Logging Setup ─────────────────────────────────────────────
LOG_PATH.mkdir(parents=True, exist_ok=True)
logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(message)s",
    handlers=[
        logging.FileHandler(LOG_PATH / "pipeline.log"),
        logging.StreamHandler()
    ]
)
log = logging.getLogger("ABTW_Pipeline")


# ── Data Loaders ──────────────────────────────────────────────
def load_json(path: Path) -> dict:
    with open(path, "r", encoding="utf-8") as f:
        return json.load(f)

def load_yaml(path: Path) -> dict:
    with open(path, "r", encoding="utf-8") as f:
        return yaml.safe_load(f)

def save_json(path: Path, data: dict):
    path.parent.mkdir(parents=True, exist_ok=True)
    with open(path, "w", encoding="utf-8") as f:
        json.dump(data, f, indent=2, ensure_ascii=False)
    log.info(f"Saved: {path}")


# ── World State Exporter ──────────────────────────────────────
def export_world_state() -> dict:
    world_state  = load_json(DATA_PATH / "World"    / "world_states.json")
    player_stats = load_json(DATA_PATH / "Stats"    / "player_stats_default.json")
    notebook     = load_json(DATA_PATH / "Notebook" / "notebook_entries.json")
    symbol_map   = load_json(DATA_PATH / "symbol_map.json") if (DATA_PATH / "symbol_map.json").exists() else {}

    payload = {
        "timestamp":    datetime.utcnow().isoformat(),
        "world_state":  world_state,
        "player_stats": player_stats,
        "notebook":     notebook,
        "symbol_map":   symbol_map,
    }

    export_file = EXPORT_PATH / f"world_export_{datetime.utcnow().strftime('%Y%m%d_%H%M%S')}.json"
    save_json(export_file, payload)
    log.info("World state exported.")
    return payload


# ── Graph App Sender ──────────────────────────────────────────
def send_to_graph_app(payload: dict) -> bool:
    try:
        response = requests.post(
            GRAPH_APP_ENDPOINT,
            json=payload,
            timeout=10,
            headers={"Content-Type": "application/json"}
        )
        if response.status_code == 200:
            log.info(f"Graph App accepted payload. Response: {response.json()}")
            return True
        else:
            log.warning(f"Graph App returned {response.status_code}: {response.text}")
            return False
    except requests.exceptions.ConnectionError:
        log.warning("Graph App not reachable. Export saved locally.")
        return False
    except Exception as e:
        log.error(f"Unexpected error sending to Graph App: {e}")
        return False


# ── Anomaly State Sync ────────────────────────────────────────
def sync_anomaly_states():
    spawn_table = load_json(DATA_PATH / "Patterns" / "spawn_table.json")
    world_state = load_json(DATA_PATH / "World"    / "world_states.json")

    active_ids   = {a["id"] for a in spawn_table["anomalies"]}
    recorded_ids = set(world_state.get("anomalies_recorded", []))
    echo_ids     = set(world_state.get("anomalies_echo", []))

    report = {
        "total_anomalies": len(active_ids),
        "recorded":        list(recorded_ids),
        "echo":            list(echo_ids),
        "unresolved":      list(active_ids - recorded_ids - echo_ids),
    }

    save_json(EXPORT_PATH / "anomaly_sync_report.json", report)
    log.info(f"Anomaly sync: {report['total_anomalies']} total, "
             f"{len(recorded_ids)} recorded, {len(echo_ids)} echo.")
    return report


# ── Main Pipeline Run ─────────────────────────────────────────
def run_pipeline():
    log.info("=== ABTW Pipeline Starting ===")
    EXPORT_PATH.mkdir(parents=True, exist_ok=True)

    payload = export_world_state()
    sync_anomaly_states()
    send_to_graph_app(payload)

    log.info("=== ABTW Pipeline Complete ===")


if __name__ == "__main__":
    run_pipeline()
"@

# ── abtw_asset_validator.py ───────────────────────────────────
Write-PipelineFile "pipeline\abtw_asset_validator.py" @"
"""
ABTW: The Hidden System — Asset Validator
Validates that all required C# scripts, JSON configs, and YAML files exist.
Version: 1.0
"""

import os
import json
from pathlib import Path

PROJECT_ROOT = Path(__file__).parent.parent
ASSETS_ROOT  = PROJECT_ROOT / "Assets" / "ABTW"

REQUIRED_SCRIPTS = [
    "Core/GameManager.cs",
    "Core/PlayerMovement.cs",
    "Core/PlayerStats.cs",
    "Core/CameraController.cs",
    "Core/InputHandler.cs",
    "Core/SceneSetup.cs",
    "Core/ChapterOneDirector.cs",
    "Systems/Perception/PerceptionSystem.cs",
    "Systems/Notebook/NotebookSystem.cs",
    "Systems/Glitch/GlitchSystem.cs",
    "Systems/Echo/EchoSystem.cs",
    "Systems/Memory/MemoryWalkSystem.cs",
    "Systems/Trio/TrioSystem.cs",
    "Systems/World/WorldManager.cs",
    "Systems/Spawner/AnomalySpawner.cs",
    "Systems/Save/SaveSystem.cs",
    "Anomalies/Base/AnomalyBase.cs",
    "Anomalies/Shadow/ShadowDelayAnomaly.cs",
    "Anomalies/Light/LightInstabilityAnomaly.cs",
    "Anomalies/Symbol/SymbolFragmentAnomaly.cs",
    "Anomalies/Reflection/ReflectionMismatchAnomaly.cs",
    "Anomalies/Sound/SoundEchoAnomaly.cs",
    "Companions/Lumi/LumiCompanion.cs",
    "Companions/Nela/NelaCompanion.cs",
    "UI/Core/UIManager.cs",
    "UI/Notebook/NotebookUI.cs",
    "UI/HUD/HUDController.cs",
    "Editor/ABTWEditorTools.cs",
]

REQUIRED_DATA = [
    "Data/symbol_map.json",
    "Data/Stats/character_traits.json",
    "Data/Stats/player_stats_default.json",
    "Data/World/world_states.json",
    "Data/World/academy_config.yaml",
    "Data/World/pipeline_config.yaml",
    "Data/Notebook/notebook_entries.json",
    "Data/Patterns/spawn_table.json",
]

def validate():
    missing = []
    found   = []

    all_required = REQUIRED_SCRIPTS + REQUIRED_DATA
    for rel_path in all_required:
        full = ASSETS_ROOT / rel_path
        if full.exists():
            found.append(rel_path)
        else:
            missing.append(rel_path)

    print(f"\n🌌 ABTW Asset Validator")
    print(f"   Root: {ASSETS_ROOT}")
    print(f"   Found:   {len(found)}/{len(all_required)}")
    print(f"   Missing: {len(missing)}")

    if missing:
        print("\n❌ Missing files:")
        for m in missing:
            print(f"   - {m}")
    else:
        print("\n✅ All required assets present!")

    report = {
        "total":   len(all_required),
        "found":   len(found),
        "missing": missing,
        "status":  "PASS" if not missing else "FAIL"
    }

    report_path = PROJECT_ROOT / "pipeline" / "exports" / "asset_validation_report.json"
    report_path.parent.mkdir(parents=True, exist_ok=True)
    with open(report_path, "w", encoding="utf-8") as f:
        json.dump(report, f, indent=2)

    print(f"\n📄 Report saved: {report_path}")
    return report

if __name__ == "__main__":
    validate()
"@

# ── requirements.txt ─────────────────────────────────────────
Write-PipelineFile "pipeline\requirements.txt" @"
requests>=2.31.0
pyyaml>=6.0
pathlib
"@

# ── README_pipeline.md ────────────────────────────────────────
Write-PipelineFile "pipeline\README_pipeline.md" @"
# ABTW Pipeline — Quick Start

## Setup
```bash
pip install -r pipeline/requirements.txt
```

## Run Pipeline (export world state + sync to Graph App)
```bash
python pipeline/abtw_pipeline_connector.py
```

## Validate All Assets
```bash
python pipeline/abtw_asset_validator.py
```

## Exports
All exports are saved to: `pipeline/exports/`

## Logs
Pipeline logs: `pipeline/logs/pipeline.log`
"@

Write-Host "`n✅ Python pipeline files generated!" -ForegroundColor Green
Write-Host "📁 Location: $ProjectRoot\pipeline\" -ForegroundColor Cyan