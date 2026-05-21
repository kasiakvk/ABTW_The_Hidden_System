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
