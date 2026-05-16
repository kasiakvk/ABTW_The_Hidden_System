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
