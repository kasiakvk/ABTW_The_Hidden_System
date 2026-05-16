
# ============================================================
# ABTW: THE HIDDEN SYSTEM — PowerShell Project Generator
# Generates full Unity project folder structure + starter files
# Target: C:\ABTW_The_Hidden_System\ABTW_The_Hidden_System
# ============================================================

param(
    [string]$RootPath = "C:\ABTW_The_Hidden_System\ABTW_The_Hidden_System",
    [switch]$DryRun,
    [switch]$Verbose
)

function Write-ABTW { param($msg, $color = "Cyan") Write-Host "🌌 ABTW: $msg" -ForegroundColor $color }
function Write-OK   { param($msg) Write-Host "  ✅ $msg" -ForegroundColor Green }
function Write-Skip { param($msg) Write-Host "  ⏭  $msg (exists)" -ForegroundColor Yellow }

Write-ABTW "Academy Beyond This World — Project Structure Generator" "Magenta"
Write-ABTW "Target: $RootPath" "Cyan"
if ($DryRun) { Write-ABTW "DRY RUN — no files will be created" "Yellow" }

# ── Folder list ──────────────────────────────────────────────
$folders = @(
    # Unity root
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
    "Builds\ABTW_Windows",
    # Pipeline / docs
    "Pipeline",
    "Docs\Bibles",
    "Docs\Checklists",
    "Docs\Schedules"
)

# ── C# script stubs ──────────────────────────────────────────
$scripts = @{
    "Assets\ABTW\Core\GameManager.cs" = @"
using UnityEngine;

namespace ABTW.Core
{
    /// <summary>
    /// Central hub — connects all ABTW systems.
    /// Attach to a persistent GameObject in every scene.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Systems")]
        public ABTW.Systems.PerceptionSystem  perception;
        public ABTW.Systems.NotebookSystem    notebook;
        public ABTW.Systems.GlitchSystem      glitch;
        public ABTW.Systems.EchoSystem        echo;
        public ABTW.Systems.MemoryWalkSystem  memory;
        public ABTW.Systems.TrioSystem        trio;
        public ABTW.Systems.WorldManager      world;
        public ABTW.Systems.AnomalySpawner    spawner;
        public ABTW.Systems.SaveSystem        save;

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            world?.Init();
            spawner?.Init();
            Debug.Log("[GameManager] All systems initialised.");
        }
    }
}
"@

    "Assets\ABTW\Core\PlayerMovement.cs" = @"
using UnityEngine;

namespace ABTW.Core
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed   = 4f;
        public float turnSpeed   = 360f;

        [Header("Perception")]
        public float stillThreshold = 2f;   // seconds before Perception Mode triggers
        private float _stillTimer;
        private CharacterController _cc;

        void Awake() => _cc = GetComponent<CharacterController>();

        void Update()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Vector3 dir = new Vector3(h, 0, v).normalized;

            if (dir.magnitude > 0.1f)
            {
                _stillTimer = 0f;
                Quaternion target = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turnSpeed * Time.deltaTime);
                _cc.SimpleMove(dir * moveSpeed);
            }
            else
            {
                _stillTimer += Time.deltaTime;
                if (_stillTimer >= stillThreshold)
                    GameManager.Instance?.perception?.ActivatePerceptionMode();
            }
        }
    }
}
"@

    "Assets\ABTW\Core\PlayerStats.cs" = @"
using UnityEngine;

namespace ABTW.Core
{
    [CreateAssetMenu(menuName = "ABTW/PlayerStats")]
    public class PlayerStats : ScriptableObject
    {
        [Range(0,100)] public float Awareness  = 0f;
        [Range(0,100)] public float Clarity    = 0f;
        [Range(0,100)] public float Connection = 0f;
        [Range(0,100)] public float Timing     = 0f;
        [Range(0,100)] public float Stability  = 50f;

        public int AcademyRank = 1;
        public int EXP         = 0;

        public void AddEXP(int amount)
        {
            EXP += amount;
            CheckRankUp();
        }

        void CheckRankUp()
        {
            int newRank = Mathf.Clamp(1 + EXP / 100, 1, 10);
            if (newRank > AcademyRank)
            {
                AcademyRank = newRank;
                Debug.Log($"[PlayerStats] Rank up → {AcademyRank}");
            }
        }
    }
}
"@

    "Assets\ABTW\Systems\Perception\PerceptionSystem.cs" = @"
using UnityEngine;

namespace ABTW.Systems
{
    public class PerceptionSystem : MonoBehaviour
    {
        public bool IsActive { get; private set; }
        public float slowMotionScale = 0.7f;

        public void ActivatePerceptionMode()
        {
            if (IsActive) return;
            IsActive = true;
            Time.timeScale = slowMotionScale;
            Debug.Log("[Perception] Perception Mode ACTIVE");
            GameManager.Instance?.glitch?.OnPerceptionActive();
        }

        public void DeactivatePerceptionMode()
        {
            IsActive = false;
            Time.timeScale = 1f;
            Debug.Log("[Perception] Perception Mode OFF");
        }
    }
}
"@

    "Assets\ABTW\Systems\Notebook\NotebookSystem.cs" = @"
using UnityEngine;

namespace ABTW.Systems
{
    public enum PatternState { Unrecorded, Recorded, Echo, DeepRecord }

    public class NotebookSystem : MonoBehaviour
    {
        public PatternState CurrentState { get; private set; } = PatternState.Unrecorded;

        public void Record()
        {
            CurrentState = PatternState.Recorded;
            GameManager.Instance?.glitch?.RemoveActiveGlitch();
            GameManager.Instance?.echo?.FlagForEcho();
            Core.PlayerStats stats = Resources.Load<Core.PlayerStats>("PlayerStats");
            stats?.AddEXP(50);
            Debug.Log("[Notebook] Pattern RECORDED");
        }

        public void Wait()
        {
            GameManager.Instance?.glitch?.EvolveGlitch();
            GameManager.Instance?.trio?.CheckActivation();
            Debug.Log("[Notebook] Player WAITED — pattern evolving");
        }

        public void DeepRecord()
        {
            CurrentState = PatternState.DeepRecord;
            GameManager.Instance?.world?.UnlockHiddenLayer();
            Debug.Log("[Notebook] DEEP RECORD — hidden layer unlocked");
        }
    }
}
"@

    "Assets\ABTW\Systems\Glitch\GlitchSystem.cs" = @"
using UnityEngine;

namespace ABTW.Systems
{
    public class GlitchSystem : MonoBehaviour
    {
        [SerializeField] private GameObject activeGlitch;
        public bool GlitchActive { get; private set; }

        public void TriggerGlitch(GameObject glitchPrefab, Vector3 position)
        {
            activeGlitch = Instantiate(glitchPrefab, position, Quaternion.identity);
            GlitchActive = true;
            GameManager.Instance?.notebook?.gameObject.SetActive(true);
            Debug.Log("[Glitch] Anomaly spawned");
        }

        public void RemoveActiveGlitch()
        {
            if (activeGlitch) Destroy(activeGlitch);
            GlitchActive = false;
        }

        public void EvolveGlitch()
        {
            Debug.Log("[Glitch] Glitch evolving — intensity increasing");
            // Increase shader intensity via material property block
        }

        public void OnPerceptionActive()
        {
            Debug.Log("[Glitch] Perception active — glitch now detectable");
        }
    }
}
"@

    "Assets\ABTW\Systems\Echo\EchoSystem.cs" = @"
using UnityEngine;
using System.Collections;

namespace ABTW.Systems
{
    public class EchoSystem : MonoBehaviour
    {
        [SerializeField] private GameObject echoPrefab;
        public float echoDelay = 30f;
        private bool _echoFlagged;

        public void FlagForEcho()
        {
            if (!_echoFlagged)
            {
                _echoFlagged = true;
                StartCoroutine(SpawnEchoAfterDelay());
            }
        }

        IEnumerator SpawnEchoAfterDelay()
        {
            yield return new WaitForSeconds(echoDelay);
            if (echoPrefab)
                Instantiate(echoPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("[Echo] Echo anomaly spawned — incomplete fragment");
        }
    }
}
"@

    "Assets\ABTW\Systems\Trio\TrioSystem.cs" = @"
using UnityEngine;

namespace ABTW.Systems
{
    public class TrioSystem : MonoBehaviour
    {
        public float requiredWaitTime = 3f;
        private float _waitTimer;
        public bool TrioActive { get; private set; }

        void Update()
        {
            if (GameManager.Instance?.glitch?.GlitchActive == true &&
                !Input.anyKey)
            {
                _waitTimer += Time.deltaTime;
                if (_waitTimer >= requiredWaitTime && !TrioActive)
                    ActivateTrio();
            }
            else
            {
                _waitTimer = 0f;
            }
        }

        public void CheckActivation() => Debug.Log("[Trio] Checking activation conditions...");

        void ActivateTrio()
        {
            TrioActive = true;
            Debug.Log("[Trio] TRIO SYNCHRONIZATION — hidden layer accessible");
            GameManager.Instance?.notebook?.DeepRecord();
        }
    }
}
"@

    "Assets\ABTW\Systems\World\WorldManager.cs" = @"
using UnityEngine;

namespace ABTW.Systems
{
    public enum TimeOfDay  { Morning, Afternoon, Sunset, Night, Rain }
    public enum Season     { Autumn, Winter, Spring, Summer }

    public class WorldManager : MonoBehaviour
    {
        public TimeOfDay CurrentTime   = TimeOfDay.Afternoon;
        public Season    CurrentSeason = Season.Autumn;
        public bool      HiddenLayerUnlocked { get; private set; }

        public void Init()
        {
            Debug.Log($"[World] Time={CurrentTime}  Season={CurrentSeason}");
        }

        public void UnlockHiddenLayer()
        {
            HiddenLayerUnlocked = true;
            Debug.Log("[World] Hidden layer UNLOCKED");
        }
    }
}
"@

    "Assets\ABTW\Systems\Save\SaveSystem.cs" = @"
using UnityEngine;
using System.IO;

namespace ABTW.Systems
{
    [System.Serializable]
    public class SaveData
    {
        public int   academyRank;
        public int   exp;
        public float awareness;
        public float clarity;
        public bool  hiddenLayerUnlocked;
        public string patternState;
    }

    public class SaveSystem : MonoBehaviour
    {
        private string SavePath => Path.Combine(Application.persistentDataPath, "abtw_save.json");

        public void Save()
        {
            Core.PlayerStats stats = Resources.Load<Core.PlayerStats>("PlayerStats");
            SaveData data = new SaveData
            {
                academyRank          = stats?.AcademyRank ?? 1,
                exp                  = stats?.EXP ?? 0,
                awareness            = stats?.Awareness ?? 0,
                clarity              = stats?.Clarity ?? 0,
                hiddenLayerUnlocked  = GameManager.Instance?.world?.HiddenLayerUnlocked ?? false,
                patternState         = GameManager.Instance?.notebook?.CurrentState.ToString() ?? "Unrecorded"
            };
            File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
            Debug.Log($"[Save] Game saved → {SavePath}");
        }

        public SaveData Load()
        {
            if (!File.Exists(SavePath)) return null;
            return JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
        }
    }
}
"@

    "Assets\ABTW\Anomalies\Base\AnomalyBase.cs" = @"
using UnityEngine;

namespace ABTW.Anomalies
{
    public abstract class AnomalyBase : MonoBehaviour
    {
        public string AnomalyID;
        public float  Intensity = 1f;
        public bool   IsActive  { get; protected set; }

        public virtual void Activate()   { IsActive = true;  Debug.Log($"[Anomaly] {AnomalyID} activated"); }
        public virtual void Deactivate() { IsActive = false; Destroy(gameObject); }
        public virtual void Evolve()     { Intensity = Mathf.Min(Intensity + 0.2f, 3f); }
    }
}
"@

    "Assets\ABTW\Anomalies\Shadow\ShadowDelayAnomaly.cs" = @"
using UnityEngine;

namespace ABTW.Anomalies
{
    /// <summary>
    /// Shadow moves 0.3–0.5 seconds after its source.
    /// Core tutorial anomaly — East Corridor, Chapter 1.
    /// </summary>
    public class ShadowDelayAnomaly : AnomalyBase
    {
        [Header("Shadow Delay")]
        public Transform shadowTransform;
        public float     delaySeconds = 0.4f;

        private Vector3 _bufferedPosition;
        private float   _timer;

        void Awake()
        {
            AnomalyID = "SYM_001_ShadowDelay";
            Activate();
        }

        void Update()
        {
            if (!IsActive) return;
            _timer += Time.deltaTime;
            if (_timer >= delaySeconds)
            {
                _timer = 0f;
                if (shadowTransform)
                    shadowTransform.position = _bufferedPosition;
                _bufferedPosition = transform.position;
            }
        }
    }
}
"@

    "Assets\ABTW\UI\Notebook\NotebookUI.cs" = @"
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ABTW.UI
{
    public class NotebookUI : MonoBehaviour
    {
        [Header("Panels")]
        public GameObject notebookPanel;

        [Header("Text Fields")]
        public TMP_Text patternNameText;
        public TMP_Text statusText;
        public TMP_Text observationsText;
        public TMP_Text lumiNoteText;
        public TMP_Text nelaNoteText;

        [Header("Buttons")]
        public Button recordButton;
        public Button waitButton;

        void Start()
        {
            recordButton?.onClick.AddListener(OnRecord);
            waitButton?.onClick.AddListener(OnWait);
            notebookPanel?.SetActive(false);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
                ToggleNotebook();
        }

        public void ToggleNotebook()
        {
            bool show = !notebookPanel.activeSelf;
            notebookPanel?.SetActive(show);
        }

        public void PopulateEntry(string name, string status, string observations, string lumi, string nela)
        {
            if (patternNameText)   patternNameText.text   = name;
            if (statusText)        statusText.text        = $"Status: {status}";
            if (observationsText)  observationsText.text  = observations;
            if (lumiNoteText)      lumiNoteText.text      = $"Lumi: \"{lumi}\"";
            if (nelaNoteText)      nelaNoteText.text      = $"Nela: \"{nela}\"";
        }

        void OnRecord() => GameManager.Instance?.notebook?.Record();
        void OnWait()   => GameManager.Instance?.notebook?.Wait();
    }
}
"@
}

# ── JSON / YAML data stubs ────────────────────────────────────
$dataFiles = @{
    "Assets\ABTW\Data\World\academy_config.yaml" = @"
# ABTW Academy Configuration
version: "1.0"
canon_lock: true

world:
  name: "Academy Beyond This World"
  golden_rule: "The Academy does not reveal itself. It responds to perception."
  anomaly_ratio: 0.30   # 30% of environment shows anomalies at any time

time_states:
  - id: morning
    anomaly_multiplier: 0.5
    emotional_tone: "Quiet awareness"
  - id: afternoon
    anomaly_multiplier: 1.0
    emotional_tone: "Warm social rhythm"
  - id: sunset
    anomaly_multiplier: 1.5
    emotional_tone: "Transition and emotional shift"
  - id: night
    anomaly_multiplier: 2.0
    emotional_tone: "Perception depth. Silence."
  - id: rain
    anomaly_multiplier: 1.8
    emotional_tone: "Reflection. Instability."

seasons:
  - id: autumn
    memory_trace_visible: true
    stability_modifier: 0
  - id: winter
    stability_modifier: +10
    anomaly_multiplier_override: 0.8
  - id: spring
    connection_modifier: +5
  - id: summer
    clarity_modifier: +5
"@

    "Assets\ABTW\Data\World\symbol_map.json" = @"
{
  "version": "1.0",
  "canon_lock": true,
  "golden_rule": "Symbols never fully explain themselves. They respond to perception.",
  "symbol_states": ["Hidden","Glitch","Forming","Activation","Responding","Deepening","Consequence"],
  "symbols": [
    {
      "id": "SYM_001",
      "name": "The Delayed Shadow",
      "category": "Layer1_Anomaly",
      "location": "East Corridor",
      "appears_in_book": 1,
      "trio_required": false,
      "hidden_layer": false,
      "color_glitch": "#6666CC",
      "color_activation": "#FFD700",
      "lumi_note": "It's repeating. The interval is consistent.",
      "nela_note": "Don't write it yet. Something else is coming."
    },
    {
      "id": "SYM_002",
      "name": "The Incomplete Circle",
      "category": "Layer2_Symbol",
      "location": "Observatory",
      "appears_in_book": 1,
      "trio_required": true,
      "hidden_layer": true,
      "color_glitch": "#4488FF",
      "color_activation": "#FFFACD",
      "lumi_note": "The gap rotates by 15 degrees each observation.",
      "nela_note": "It's waiting for the right moment to close."
    },
    {
      "id": "SYM_003",
      "name": "The Reflected Word",
      "category": "Layer1_Anomaly",
      "location": "Library Archive",
      "appears_in_book": 2,
      "trio_required": false,
      "hidden_layer": false,
      "color_glitch": "#8866AA",
      "color_activation": "#E8D5FF",
      "lumi_note": "The reflection shows a different language.",
      "nela_note": "Read it when you're not trying to."
    }
  ]
}
"@

    "Assets\ABTW\Data\Stats\character_traits.json" = @"
{
  "version": "1.0",
  "characters": [
    {
      "id": "ASTRA",
      "role": "Perceiver",
      "stats_affinity": ["Awareness", "Timing"],
      "color_palette": ["#1A2942","#2D3A6B","#5B4A8A","#C9A84C"],
      "signature": "star_hair_clip",
      "arc_start": "Something is wrong",
      "arc_end": "Reality responds to perception"
    },
    {
      "id": "LUMI",
      "role": "Analyst",
      "stats_affinity": ["Clarity", "Stability"],
      "color_palette": ["#7EC8C8","#FFFFFF","#1A2942","#CCFFEE"],
      "signature": "notebook",
      "arc_start": "Everything can be understood",
      "arc_end": "Understanding also requires emotional openness"
    },
    {
      "id": "NELA",
      "role": "Aligner",
      "stats_affinity": ["Connection", "Timing"],
      "color_palette": ["#C9A84C","#E8A050","#8B4A2A","#FFD080"],
      "signature": "mismatched_earrings",
      "arc_start": "Follow the feeling",
      "arc_end": "Connection itself changes reality"
    }
  ]
}
"@
}

# ── Create folders ────────────────────────────────────────────
Write-ABTW "Creating folder structure..." "Cyan"
foreach ($folder in $folders)
{
    $full = Join-Path $RootPath $folder
    if (Test-Path $full) { if ($Verbose) { Write-Skip $folder } }
    else
    {
        if (!$DryRun) { New-Item -ItemType Directory -Path $full -Force | Out-Null }
        Write-OK $folder
    }
}

# ── Create C# scripts ─────────────────────────────────────────
Write-ABTW "Creating C# scripts..." "Cyan"
foreach ($kv in $scripts.GetEnumerator())
{
    $full = Join-Path $RootPath $kv.Key
    if (Test-Path $full) { Write-Skip $kv.Key }
    else
    {
        if (!$DryRun)
        {
            $dir = Split-Path $full
            if (!(Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
            Set-Content -Path $full -Value $kv.Value -Encoding UTF8
        }
        Write-OK $kv.Key
    }
}

# ── Create data files ─────────────────────────────────────────
Write-ABTW "Creating data files..." "Cyan"
foreach ($kv in $dataFiles.GetEnumerator())
{
    $full = Join-Path $RootPath $kv.Key
    if (Test-Path $full) { Write-Skip $kv.Key }
    else
    {
        if (!$DryRun)
        {
            $dir = Split-Path $full
            if (!(Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
            Set-Content -Path $full -Value $kv.Value -Encoding UTF8
        }
        Write-OK $kv.Key
    }
}

Write-ABTW "Done! Project structure ready at: $RootPath" "Green"
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Magenta
Write-Host "  1. Open Unity Hub → Add project from disk → select $RootPath" -ForegroundColor Yellow
Write-Host "  2. Open EastCorridor.unity scene" -ForegroundColor Yellow
Write-Host "  3. Attach GameManager.cs to a persistent empty GameObject" -ForegroundColor Yellow
Write-Host "  4. Press Play and test the first glitch trigger" -ForegroundColor Yellow
