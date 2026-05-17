# ============================================================
# ABTW: The Hidden System — PowerShell Script 05
# PURPOSE: Generate all .md documentation files
# RUN: After 01_Setup_Project_Structure.ps1
# ============================================================

param(
    [string]$ProjectRoot = "C:\ABTW_The_Hidden_System\ABTW_The_Hidden_System",
    [switch]$DryRun
)

function Write-DocFile($relativePath, $content) {
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

Write-Host "`n🌌 ABTW Documentation Generator" -ForegroundColor Cyan

# ── ABTW_Programming_Bible.md ─────────────────────────────────
Write-DocFile "docs\ABTW_Programming_Bible.md" @"
# ABTW: The Hidden System — Programming Bible
## Version 1.0 — Full Canon Lock

> *"Reality is not fixed. It responds to how it is seen."*

---

## 1. CORE ARCHITECTURE

### 1.1 Master Flow
```
GameManager
  → SceneSetup
  → Player (Core)
  → AnomalySpawner
  → PerceptionSystem
  → GlitchSystem
    → NotebookSystem
        → RECORD → EchoSystem
        → WAIT   → TrioSystem
  → WorldManager
  → SaveSystem
```

### 1.2 Namespace
All scripts use namespace `ABTW`.

### 1.3 Folder Structure
```
Assets/ABTW/
├── Core/
├── Systems/
│   ├── Perception/
│   ├── Notebook/
│   ├── Glitch/
│   ├── Echo/
│   ├── Memory/
│   ├── Trio/
│   ├── World/
│   ├── Spawner/
│   └── Save/
├── Anomalies/
│   ├── Base/
│   ├── Shadow/
│   ├── Light/
│   ├── Symbol/
│   ├── Reflection/
│   └── Sound/
├── Companions/
│   ├── Lumi/
│   └── Nela/
├── UI/
│   ├── Core/
│   ├── Notebook/
│   └── HUD/
├── Data/
│   ├── Stats/
│   ├── World/
│   ├── Notebook/
│   └── Patterns/
└── Editor/
```

---

## 2. PLAYER STATS SYSTEM

| Stat | Symbol | Function |
|------|--------|----------|
| Awareness | 👁 | Detect anomalies |
| Clarity | 🧩 | Understand patterns |
| Connection | 🔗 | Link elements |
| Timing | ⏳ | Correct moment |
| Stability | 🌀 | Resist distortion |

### 2.1 Academy Rank Progression

| Rank | Title | Unlock |
|------|-------|--------|
| 1 | Observer | Layer 0 visible |
| 2 | Noticer | Layer 1 visible |
| 3 | Pattern Seeker | Layer 2 begins |
| 4 | Interpreter | Layer 2 full |
| 5 | Aligner | Trio Activation |
| 6 | System Aware | Seasonal anomalies |
| 7 | Influence Initiate | World state impact |
| 8 | Reality Reader | Memory Walk |
| 9 | System Shifter | Hidden Layer full |
| 10 | Beyond Observer | Full perception |

---

## 3. ANOMALY SYSTEM

### 3.1 Anomaly States
`Hidden → Glitch → Forming → Activation → Responding → Deepening → Consequence`

### 3.2 Activation Conditions
- **Glitch trigger**: Awareness >= 0.3
- **Forming trigger**: Clarity >= 0.5
- **Activation trigger**: Connection >= 0.7 AND Trio synchronized
- **Echo trigger**: Recorded before Activation state

### 3.3 Chapter 1 Anomalies

| ID | Type | Location | Trio Required |
|----|------|----------|---------------|
| ANOM_001 | ShadowDelay | East Corridor | No |
| ANOM_002 | ReflectionMismatch | Library | No |
| ANOM_003 | SymbolFragment | Observatory | Yes |

---

## 4. NOTEBOOK SYSTEM

### 4.1 Decision Logic
```
RECORD → +Clarity, +EXP, world stabilises, Echo flagged
WAIT   → pattern evolves, Trio timer starts
DEEP RECORD (Trio) → max reward, hidden layer unlocked
```

### 4.2 Notebook Tabs
- `PATTERNS` — anomaly patterns
- `SYMBOLS` — symbol catalogue
- `CONNECTIONS` — linked elements
- `MEMORY` — memory walk entries

---

## 5. TRIO SYSTEM

### 5.1 Activation Conditions
1. Astra observing active anomaly (Awareness check)
2. Lumi has analysed pattern (Clarity check)
3. Nela senses correct moment (player must NOT click for 3 seconds)

### 5.2 Result
- Hidden layer appears
- New symbols form
- +Connection, +Awareness
- Special notebook entry created

---

## 6. ECHO SYSTEM

- Echo anomalies appear at ~40% original intensity
- Echo notebook entries are visually faded
- Echo symbols are incomplete
- Echo entry text: *"This is not the original pattern. A trace of something recorded too early."*

---

## 7. MEMORY WALK SYSTEM

- Triggered via Notebook `[REVISIT]` button
- Time scale set to 0.5
- Visual: desaturated, slight blur
- Player CANNOT change past decisions
- Player CAN observe and gain Stability

---

## 8. WORLD MANAGER

### 8.1 Time States
`morning | afternoon | sunset | night | rain`

### 8.2 Seasons
`autumn | winter | spring | summer`

### 8.3 Anomaly Multipliers
| Time | Multiplier |
|------|-----------|
| morning | 0.5x |
| afternoon | 1.0x |
| sunset | 1.5x |
| night | 2.0x |
| rain | 1.8x |

---

## 9. SAVE SYSTEM

Save data includes:
- Player stats and rank
- World state (time, season, weather)
- Anomaly states (active, recorded, echo)
- Notebook entries
- Symbol discovery state
- Trio activation count

---

## 10. CODING STANDARDS

- All classes inherit `MonoBehaviour` (except `ABTWEditorTools`)
- Use `Awake()` for initialization, `Start()` for setup, `Update()` for per-frame logic
- All public references assigned via Inspector
- Use `Debug.Log()` with system prefix: `[PerceptionSystem]`, `[NotebookSystem]`, etc.
- No magic numbers — use ScriptableObject configs
- All anomaly types inherit from `AnomalyBase`

---

## 11. GOLDEN RULES (PROGRAMMING)

1. **No combat mechanics** — ever
2. **Perception > Power** — stats reflect awareness, not strength
3. **Consequence without punishment** — RECORD and WAIT are both valid
4. **Echo, not reset** — closed paths leave traces
5. **The world responds** — every player action must change something

---

*ABTW Programming Bible v1.0 — Full Canon Lock*
"@

# ── ABTW_Graphic_Bible.md ─────────────────────────────────────
Write-DocFile "docs\ABTW_Graphic_Bible.md" @"
# ABTW: The Hidden System — Graphic Production Bible
## Version 1.0 — Full Canon Lock

> *"ABTW succeeds when the player feels: Reality itself is listening."*

---

## 1. CORE VISUAL PHILOSOPHY

- **Painterly softness** — Studio Ghibli meets Journey
- **70% normal, 30% strange** — anomalies are subtle, never spectacular
- **Cinematic restraint** — no explosions, no combat FX, no spectacle
- **Emotional atmosphere first** — every image must feel correct before it looks correct

---

## 2. WORLD COLOUR DNA

| Colour | Hex | Meaning | Usage |
|--------|-----|---------|-------|
| Deep Navy | #1A2942 | Mystery, depth | Primary environment, night, Lumi |
| Indigo | #2D3A6B | Cognitive depth | Secondary env, symbols, Astra |
| Violet | #5B4A8A | Instability | Anomaly states, glitch FX |
| Warm Gold | #C9A84C | Reality anchor | Trio activation, notebook |
| Muted Magenta | #8B4A6B | Awareness shift | Perception Mode UI |
| Slate Blue | #4A5568 | Grounded reality | Normal state environments |
| Ivory/Parchment | #F9F2E7 | Knowledge, safety | Notebook pages, dormitories |
| Soft Cyan | #7EC8C8 | Analysis, clarity | Lumi's ink, Clarity indicators |

---

## 3. CHARACTER VISUAL DNA (CANONICAL — DO NOT DEVIATE)

### ASTRA
- Hair: dark indigo-purple, slightly wavy, medium length
- Accessory: small star-shaped hair clip (left side)
- Bracelet: glowing softly, reacts to anomaly proximity
- Outfit: oversized cosmic hoodie (deep navy/indigo), comfortable trousers, worn trainers
- Palette: #1A2942, #2D3A6B, #5B4A8A, #C9A84C

### LUMI
- Hair: light blonde, neat, often tucked behind ears
- Accessory: notebook — always present, always slightly too full
- Outfit: crisp white shirt, soft mint cardigan, navy trousers
- Palette: #7EC8C8, #FFFFFF, #1A2942, #CCFFEE

### NELA
- Hair: warm auburn, curly, often escaping
- Accessory: mismatched earrings — one star, one moon
- Outfit: layered warm tones — orange-gold jacket, patterned shirt
- Palette: #C9A84C, #E8A020, #8B4A00, #FFD080

### Character Rules
- Characters NEVER change visual design across books/chapters
- Expressions: emotionally readable, never exaggerated
- Silhouettes must be distinguishable in shadow alone
- FORBIDDEN: combat poses, power stances, glowing eyes (except Nimbus)

---

## 4. ENVIRONMENT EMOTIONAL DNA

| Location | Emotional Function | Anomaly Frequency |
|----------|--------------------|-------------------|
| East Corridor | Observation — world is slightly wrong | High |
| Observatory | Wonder — cosmic scale | Medium |
| Archive/Library | Hidden truth — incomplete knowledge | Medium |
| Classrooms | Social warmth — belonging | Low |
| Dormitories | Vulnerability — safety and memory | Very Low |
| Gardens | Reflection — nature as mirror | Low |
| Rooftops | Perspective — seeing the whole | High |
| Hidden Halls | Deep mystery — system beneath system | Very High |

---

## 5. LIGHTING PHILOSOPHY

| Light Type | Temperature | Meaning |
|------------|-------------|---------|
| Warm Gold | 3200K | Reality anchor — safe, grounded |
| Cool Navy | 5500K+ | Cognitive depth — analytical, mysterious |
| Violet Undertone | — | Instability — something is wrong |
| Mixed Gold+Navy | — | Transition — anomalies most likely here |

### Lighting Rules
- Never use harsh directional light — all sources soft and diffused
- Shadows always slightly softer than realistic
- Anomaly lighting changes: max 10% colour temperature shift
- Trio Activation gold pulse: feels like a breath, not an explosion

---

## 6. RENDERING STYLE

| Element | Specification |
|---------|---------------|
| Overall | Painterly softness — anime-inspired but not anime |
| Characters | Rounded forms, soft outlines, no harsh edges |
| Environments | Slightly stylised — proportions grander than reality |
| Shaders | Soft light — no specular highlights, no sharp shadows |
| Textures | Hand-painted feel |
| Post-processing | Subtle colour grading, light bloom on gold only |
| Anomaly FX | Shader-based — distortion, delay, colour shift only |

---

## 7. ANOMALY VISUAL RULES

- 70% of environment must look completely normal at all times
- Anomalies are NEVER flashy
- The strongest anomaly should feel like something is *slightly wrong*
- Anomalies intensify gradually — never appear at full strength immediately

### Chapter 1 Anomaly Catalogue

| Anomaly | Visual | Intensity | Location |
|---------|--------|-----------|----------|
| Shadow Delay | Shadow moves 0.3–0.5s after source | Low→Medium | East Corridor |
| Reflected Word | Text in reflections shows different words | Low | Library |
| Incomplete Circle | Circle missing one segment, gap rotates | Medium | Observatory |
| Echo Corridor | Footsteps echo with different rhythm | Low | West Corridor |
| Memory Trace | Faint outline of figure, night only | Medium | Dormitory Hall |
| Garden Spiral | Mathematically precise plant spiral | Low | Gardens |

---

## 8. PAGE ARCHITECTURE (Book-Game Hybrid)

| Page Type | Description | Text/Illustration |
|-----------|-------------|-------------------|
| Classic Split | Text left, illustration right | 50% / 50% |
| Soft Hybrid | Illustration bleeds into text | 40% / 60% |
| Cinematic Full | Full-page illustration, caption only | 5% / 95% |
| Notebook/Observation | Styled as Lumi's notebook | 60% / 40% |
| Archive/Lore | Aged parchment, dense text | 70% / 30% |
| Reflection/Breathing | Minimal text, atmospheric illustration | 20% / 80% |

---

## 9. QA CHECKLIST — PER ILLUSTRATION

- [ ] Emotional atmosphere correct for context
- [ ] All colours within approved World Colour DNA
- [ ] Characters match canonical visual DNA exactly
- [ ] Anomalies subtle enough (70/30 rule)
- [ ] Environments feel larger than characters
- [ ] Lighting emotionally appropriate for space and time
- [ ] No forbidden elements (combat, horror, spectacle FX)
- [ ] Readable without text explanation

---

## 10. FORBIDDEN VISUAL DIRECTIONS

- Explosive or dramatic magical effects
- Combat-style visual language (impact frames, speed lines)
- Grimdark or horror aesthetics
- Cyberpunk or sci-fi drift
- Spectacle fantasy (dragons, floating castles)
- Empty architecture showcases
- Game-level feeling
- Fantasy theme park aesthetics

---

## 11. TECHNICAL SPECIFICATIONS

| Spec | Value |
|------|-------|
| Resolution | 300 DPI minimum (print) |
| Colour mode | CMYK (print), sRGB (digital/game) |
| Source format | PSD or AI with layers |
| Delivery format | PNG (transparency), TIFF (print), WebP (game) |
| Bleed | 3mm on all print materials |
| Safe zone | 0.375" minimum all sides (KDP) |

---

*ABTW Graphic Production Bible v1.0 — Full Canon Lock*
"@

# ── ABTW_Asset_Checklist.md ───────────────────────────────────
Write-DocFile "docs\ABTW_Asset_Checklist.md" @"
# ABTW: The Hidden System — Asset Checklist
## Production Status Tracker

---

## C# SCRIPTS

### Core
- [ ] GameManager.cs
- [ ] PlayerMovement.cs
- [ ] PlayerStats.cs
- [ ] CameraController.cs
- [ ] InputHandler.cs
- [ ] SceneSetup.cs
- [ ] ChapterOneDirector.cs
- [ ] MainMenuController.cs
- [ ] PauseMenu.cs

### Systems
- [ ] PerceptionSystem.cs
- [ ] NotebookSystem.cs
- [ ] GlitchSystem.cs
- [ ] EchoSystem.cs
- [ ] MemoryWalkSystem.cs
- [ ] TrioSystem.cs
- [ ] WorldManager.cs
- [ ] AnomalySpawner.cs
- [ ] SaveSystem.cs

### Anomalies
- [ ] AnomalyBase.cs
- [ ] ShadowDelayAnomaly.cs
- [ ] LightInstabilityAnomaly.cs
- [ ] SymbolFragmentAnomaly.cs
- [ ] ReflectionMismatchAnomaly.cs
- [ ] SoundEchoAnomaly.cs

### Companions
- [ ] LumiCompanion.cs
- [ ] NelaCompanion.cs

### UI
- [ ] UIManager.cs
- [ ] NotebookUI.cs
- [ ] HUDController.cs

### Editor
- [ ] ABTWEditorTools.cs

---

## DATA FILES (JSON / YAML)

- [x] symbol_map.json
- [x] character_traits.json
- [x] player_stats_default.json
- [x] world_states.json
- [x] academy_config.yaml
- [x] pipeline_config.yaml
- [x] notebook_entries.json
- [x] spawn_table.json

---

## DOCUMENTATION (.md)

- [x] ABTW_Programming_Bible.md
- [x] ABTW_Graphic_Bible.md
- [x] ABTW_Asset_Checklist.md
- [ ] ABTW_World_Bible.md
- [ ] ABTW_Character_Bible.md
- [ ] ABTW_Symbol_Atlas.md
- [ ] ABTW_Environment_Atlas.md

---

## PNG GRAPHIC ASSETS

### Characters
- [ ] Astra — full body reference sheet
- [ ] Astra — expression sheet (6 expressions)
- [ ] Lumi — full body reference sheet
- [ ] Lumi — expression sheet
- [ ] Nela — full body reference sheet
- [ ] Nela — expression sheet
- [ ] Nimbus — reference sheet (colour states)
- [ ] Prof. Lyra Quill — reference sheet
- [ ] Archivist Noctra — reference sheet
- [ ] Orion Vex — reference sheet

### Environments
- [ ] East Corridor — Normal state
- [ ] East Corridor — Glitch state
- [ ] East Corridor — Activation state
- [ ] Observatory — Day
- [ ] Observatory — Night
- [ ] Observatory — Activation
- [ ] Archive/Library — Level 1
- [ ] Archive/Library — Level 2 (deeper, cooler light)
- [ ] Dormitory — Astra's room
- [ ] Dormitory — Common area
- [ ] Gardens — Autumn
- [ ] Gardens — Winter
- [ ] Gardens — Spring
- [ ] Gardens — Summer
- [ ] Rooftop — Sunset
- [ ] Rooftop — Night

### Anomalies / FX
- [ ] Shadow Delay — sequence (3 frames)
- [ ] Reflected Word — before/after
- [ ] Incomplete Circle — all gap positions (8)
- [ ] Echo Corridor — footstep echo visual
- [ ] Memory Trace — night version
- [ ] Garden Spiral — seasonal variants
- [ ] Trio Activation — gold pulse sequence
- [ ] Symbol Formation — 7 states per symbol

### UI Assets
- [ ] Notebook cover
- [ ] Notebook pages (blank, ivory texture)
- [ ] Notebook tabs — PATTERNS, SYMBOLS, CONNECTIONS, MEMORY
- [ ] RECORD button (normal, hover, pressed)
- [ ] WAIT button (normal, hover, pressed)
- [ ] Perception Ring — HUD element
- [ ] Awareness indicator
- [ ] Clarity indicator
- [ ] Connection indicator
- [ ] Timing indicator
- [ ] Stability indicator
- [ ] Echo entry visual (faded style)
- [ ] Memory Walk overlay

### Symbols (per symbol — 7 states each)
- [ ] SYM_001 — The Delayed Shadow
- [ ] SYM_002 — The Incomplete Circle
- [ ] SYM_003 — The Reflected Word
- [ ] SYM_004 — The Convergence Point
- [ ] SYM_005 — The Memory Trace
- [ ] SYM_006 — The Garden Spiral
- [ ] SYM_007 — The Observatory Star Map
- [ ] SYM_008 — The Echo Corridor

### Covers / Marketing
- [ ] Book 1 cover — The First Glitch
- [ ] Book 2 cover — The Notebook That Writes
- [ ] Game key art — main
- [ ] Game key art — horizontal (Steam banner)
- [ ] Academy logo (transparent PNG)

---

## SYSTEM SHEETS (Already Produced ✅)

- [x] ACADEMY_GARDENS_SYSTEM.png
- [x] ACADEMY_GROUNDS_SYSTEM.png
- [x] ACADEMY_INTERIOR_LIFE.png
- [x] ACADEMY_NARRATIVE_VISUAL_DNA.png
- [x] DORMITORY_DNA_SYSTEM.png
- [x] EMOTIONAL_SPACES_SYSTEM.png
- [x] ENVIRONMENT_DNA_SHEETS.png
- [x] LIVING_WORLD_SYSTEM.png
- [x] OUTER_FOREST_SYSTEM.png
- [x] OUTER_WORLD.png
- [x] SEASONAL_SOCIAL_SYSTEM.png
- [x] SYMBOL_CHAMBER.png
- [x] TRIO_SYSTEM.png
- [x] RENDERING_CALIBRATION.png
- [x] FAMILY_1_ARCHITECTURE_SYSTEMS.png
- [x] FAMILY 2 — SYMBOLIC SYSTEMS.png
- [x] FAMILY 3 — WORLD SYSTEMS.png
- [x] FAMILY 4 — CHARACTER SYSTEMS.png
- [x] FAMILY 5 — NOTEBOOK SYSTEMS.png
- [x] CORRIDOR_PROPORTION_SYSTEM.png
- [x] ARCHES_SYSTEM.png
- [x] BRIDGE_RESONANCE_SYSTEM.png
- [x] DOME_LANGUAGE.png
- [x] FLOOR_MOTIF_SYSTEM.png
- [x] THE_ACADEMY_SYSTEMS.png
- [x] THE_TRIO_SYSTEM.png
- [x] LUMIS_NOTEBOOK.png
- [x] MASTER_WORLD_BIBLE.png
- [x] ACADEMY_INTERIOR_LOCK.png

---

## PIPELINE FILES

- [x] pipeline/abtw_pipeline_connector.py
- [x] pipeline/abtw_asset_validator.py
- [x] pipeline/requirements.txt
- [x] pipeline/README_pipeline.md

---

## POWERSHELL SCRIPTS

- [x] PowerShell/01_Setup_Project_Structure.ps1
- [x] PowerShell/02_Generate_CSharp_Stubs.ps1
- [x] PowerShell/03_Generate_JSON_YAML_Configs.ps1
- [x] PowerShell/04_Generate_Python_Pipeline.ps1
- [x] PowerShell/05_Generate_MD_Documentation.ps1

---

*Last updated: 2026-05-16*
"@

Write-Host "`n✅ All documentation files generated!" -ForegroundColor Green
Write-Host "📁 Location: $ProjectRoot\docs\" -ForegroundColor Cyan