using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using ABTW.Systems;
using ABTW.UI;

namespace ABTW.Core
{
    /// <summary>
    /// Central game manager — owns all system references and game state.
    /// Single source of truth for the current chapter, rank, and world state.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // ── Singleton ────────────────────────────────────────────────────────
        public static GameManager Instance { get; private set; }

        // ── Inspector References ─────────────────────────────────────────────
        [Header("— PLAYER DATA —")]
        [SerializeField] public PlayerStats playerStats;

        [Header("— SYSTEMS —")]
        [SerializeField] public PerceptionSystem  perceptionSystem;
        [SerializeField] public NotebookSystem    notebookSystem;
        [SerializeField] public EchoSystem        echoSystem;
        [SerializeField] public MemoryWalkSystem  memoryWalkSystem;
        [SerializeField] public TrioSystem        trioSystem;

        [Header("— UI —")]
        [SerializeField] public UIManager         uiManager;

        [Header("— WORLD STATE —")]
        [SerializeField] private int   currentChapter = 1;
        [SerializeField] private float worldStability = 1f; // 0=chaotic, 1=stable

        // ── Runtime State ────────────────────────────────────────────────────
        public bool IsMemoryWalkActive  { get; private set; }
        public bool IsPerceptionActive  { get; private set; }
        public bool IsTrioActive        { get; private set; }
        public int  AnomaliesFound      { get; private set; }
        public int  PatternsRecorded    { get; private set; }

        // ── Unity ────────────────────────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            ValidateSystems();
        }

        private void Start()
        {
            SubscribeToEvents();
            uiManager?.ShowHUD();
            Debug.Log($"[ABTW] Chapter {currentChapter} — {playerStats.CurrentRankName}");
        }

        // ── Public API ───────────────────────────────────────────────────────

        /// <summary>Called when player finds an anomaly.</summary>
        public void OnAnomalyFound(AnomalyType type, int expReward)
        {
            AnomaliesFound++;
            playerStats.AddEXP(expReward);
            playerStats.RaiseStat(StatType.Awareness, 2f);

            uiManager?.ShowNotification($"Anomaly detected: {type}", NotificationType.Perception);
            uiManager?.UpdateHUD();

            // First anomaly — unlock Notebook
            if (AnomaliesFound == 1)
                notebookSystem?.UnlockNotebook();

            Debug.Log($"[ABTW] Anomaly found: {type} | EXP +{expReward}");
        }

        /// <summary>Called when player records a pattern.</summary>
        public void OnPatternRecorded(PatternData pattern)
        {
            PatternsRecorded++;
            playerStats.AddEXP(pattern.expOnRecord);
            playerStats.RaiseStat(StatType.Clarity, 3f);

            worldStability = Mathf.Clamp01(worldStability + 0.05f);
            uiManager?.ShowNotification("Pattern recorded.", NotificationType.Record);
            uiManager?.UpdateHUD();

            // Schedule echo if recorded early
            if (pattern.recordedBeforeTrio)
                echoSystem?.ScheduleEcho(pattern, delay: 30f);
        }

        /// <summary>Called when player chooses to Wait.</summary>
        public void OnPatternWaited(PatternData pattern)
        {
            playerStats.RaiseStat(StatType.Timing, 2f);
            worldStability = Mathf.Clamp01(worldStability - 0.03f);
            uiManager?.ShowNotification("The pattern continues…", NotificationType.Wait);
        }

        /// <summary>Called when Trio synchronises.</summary>
        public void OnTrioActivated()
        {
            IsTrioActive = true;
            playerStats.AddEXP(80);
            playerStats.RaiseStat(StatType.Connection, 5f);
            playerStats.RaiseStat(StatType.Awareness,  2f);

            uiManager?.ShowNotification("TRIO SYNCHRONIZATION", NotificationType.Trio);
            StartCoroutine(ResetTrioFlag(5f));
        }

        /// <summary>Enter / exit Memory Walk mode.</summary>
        public void SetMemoryWalk(bool active)
        {
            IsMemoryWalkActive = active;
            memoryWalkSystem?.SetActive(active);

            if (!active)
            {
                playerStats.RaiseStat(StatType.Stability, 3f);
                uiManager?.ShowNotification("Memory Walk complete.", NotificationType.Memory);
            }
        }

        /// <summary>Toggle Perception Mode.</summary>
        public void SetPerceptionMode(bool active)
        {
            IsPerceptionActive = active;
            perceptionSystem?.SetActive(active);
            uiManager?.SetPerceptionOverlay(active);
        }

        // ── Private ──────────────────────────────────────────────────────────

        private void SubscribeToEvents()
        {
            if (playerStats != null)
            {
                playerStats.OnRankUp      += rank => uiManager?.ShowRankUp(rank);
                playerStats.OnStatChanged += _ => uiManager?.UpdateHUD();
            }
        }

        private void ValidateSystems()
        {
            if (playerStats      == null) Debug.LogError("[ABTW] PlayerStats not assigned!");
            if (perceptionSystem == null) Debug.LogWarning("[ABTW] PerceptionSystem missing.");
            if (notebookSystem   == null) Debug.LogWarning("[ABTW] NotebookSystem missing.");
            if (echoSystem       == null) Debug.LogWarning("[ABTW] EchoSystem missing.");
            if (memoryWalkSystem == null) Debug.LogWarning("[ABTW] MemoryWalkSystem missing.");
            if (trioSystem       == null) Debug.LogWarning("[ABTW] TrioSystem missing.");
            if (uiManager        == null) Debug.LogWarning("[ABTW] UIManager missing.");
        }

        private IEnumerator ResetTrioFlag(float delay)
        {
            yield return new WaitForSeconds(delay);
            IsTrioActive = false;
        }
    }
}