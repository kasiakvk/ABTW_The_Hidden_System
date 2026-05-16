using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using ABTW.Core;

namespace ABTW.UI
{
    /// <summary>
    /// Central UI Manager — controls all on-screen elements.
    /// Philosophy: minimal, atmospheric, never intrusive.
    /// No health bars, no damage numbers, no minimap markers.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        // ── HUD ──────────────────────────────────────────────────────────────
        [Header("— HUD —")]
        [SerializeField] private GameObject    hudRoot;
        [SerializeField] private TextMeshProUGUI rankLabel;
        [SerializeField] private TextMeshProUGUI chapterLabel;

        [Header("Stat Bars")]
        [SerializeField] private Slider awarenessBar;
        [SerializeField] private Slider clarityBar;
        [SerializeField] private Slider connectionBar;
        [SerializeField] private Slider timingBar;
        [SerializeField] private Slider stabilityBar;

        [Header("Perception Mode")]
        [SerializeField] private GameObject perceptionOverlay;   // subtle desaturation panel
        [SerializeField] private Image      perceptionRing;      // ring around screen edge
        [SerializeField] private float      perceptionFadeSpeed = 2f;

        // ── NOTEBOOK UI ───────────────────────────────────────────────────────
        [Header("— NOTEBOOK —")]
        [SerializeField] private GameObject      notebookPanel;
        [SerializeField] private TextMeshProUGUI notebookPatternName;
        [SerializeField] private TextMeshProUGUI notebookStatus;
        [SerializeField] private TextMeshProUGUI notebookObservations;
        [SerializeField] private TextMeshProUGUI notebookLumiNote;
        [SerializeField] private TextMeshProUGUI notebookNelaNote;
        [SerializeField] private Button          recordButton;
        [SerializeField] private Button          waitButton;
        [SerializeField] private Button          deepRecordButton;
        [SerializeField] private Button          revisitButton;
        [SerializeField] private Button          closeNotebookButton;

        // ── NOTIFICATIONS ─────────────────────────────────────────────────────
        [Header("— NOTIFICATIONS —")]
        [SerializeField] private GameObject      notificationPanel;
        [SerializeField] private TextMeshProUGUI notificationText;
        [SerializeField] private Image           notificationIcon;
        [SerializeField] private float           notificationDuration = 3.5f;

        [Header("Notification Colors")]
        [SerializeField] private Color colorPerception = new Color(0.79f, 0.66f, 0.30f); // gold
        [SerializeField] private Color colorRecord     = new Color(0.40f, 0.80f, 0.55f); // green
        [SerializeField] private Color colorWait       = new Color(0.48f, 0.31f, 1.00f); // violet
        [SerializeField] private Color colorTrio       = new Color(0.20f, 0.70f, 1.00f); // cyan
        [SerializeField] private Color colorMemory     = new Color(0.70f, 0.70f, 0.85f); // grey-blue
        [SerializeField] private Color colorEcho       = new Color(0.60f, 0.40f, 0.80f); // muted violet

        // ── COMPANION DIALOGUE ────────────────────────────────────────────────
        [Header("— COMPANION DIALOGUE —")]
        [SerializeField] private GameObject      companionDialoguePanel;
        [SerializeField] private TextMeshProUGUI companionNameLabel;
        [SerializeField] private TextMeshProUGUI companionLineLabel;
        [SerializeField] private float           companionDialogueDuration = 4f;

        // ── RANK UP ───────────────────────────────────────────────────────────
        [Header("— RANK UP —")]
        [SerializeField] private GameObject      rankUpPanel;
        [SerializeField] private TextMeshProUGUI rankUpLabel;
        [SerializeField] private TextMeshProUGUI rankUpName;

        // ── Runtime ───────────────────────────────────────────────────────────
        private Coroutine _notificationRoutine;
        private Coroutine _companionRoutine;
        private bool      _notebookOpen;

        // ── Unity ────────────────────────────────────────────────────────────

        private void Awake()
        {
            // Wire notebook buttons
            recordButton?.onClick.AddListener(OnRecordClicked);
            waitButton?.onClick.AddListener(OnWaitClicked);
            deepRecordButton?.onClick.AddListener(OnDeepRecordClicked);
            revisitButton?.onClick.AddListener(OnRevisitClicked);
            closeNotebookButton?.onClick.AddListener(CloseNotebook);

            // Start hidden
            notebookPanel?.SetActive(false);
            notificationPanel?.SetActive(false);
            companionDialoguePanel?.SetActive(false);
            rankUpPanel?.SetActive(false);
            perceptionOverlay?.SetActive(false);
            deepRecordButton?.gameObject.SetActive(false);
        }

        private void Update()
        {
            // N key toggles notebook
            if (Input.GetKeyDown(KeyCode.N))
                ToggleNotebook();
        }

        // ── Public API — HUD ─────────────────────────────────────────────────

        public void ShowHUD()
        {
            hudRoot?.SetActive(true);
            UpdateHUD();
        }

        public void UpdateHUD()
        {
            var stats = GameManager.Instance?.playerStats;
            if (stats == null) return;

            if (rankLabel)   rankLabel.text   = stats.CurrentRankName;
            if (chapterLabel) chapterLabel.text = $"Chapter {1}";

            SetSlider(awarenessBar,   stats.awareness);
            SetSlider(clarityBar,     stats.clarity);
            SetSlider(connectionBar,  stats.connection);
            SetSlider(timingBar,      stats.timing);
            SetSlider(stabilityBar,   stats.stability);
        }

        public void SetPerceptionOverlay(bool active)
        {
            perceptionOverlay?.SetActive(active);
            if (perceptionRing != null)
                StartCoroutine(FadeImage(perceptionRing, active ? 0.35f : 0f,
                    perceptionFadeSpeed));
        }

        // ── Public API — Notebook ─────────────────────────────────────────────

        public void OpenNotebook(PatternData pattern)
        {
            if (pattern == null) return;
            _notebookOpen = true;
            notebookPanel?.SetActive(true);

            // Populate fields
            if (notebookPatternName)   notebookPatternName.text   = pattern.name;
            if (notebookStatus)        notebookStatus.text        = $"Status: {pattern.status}";
            if (notebookLumiNote)      notebookLumiNote.text      = $"Lumi: \"{pattern.lumiNote}\"";
            if (notebookNelaNote)      notebookNelaNote.text      = $"Nela: \"{pattern.nelaNote}\"";

            // Observations list
            if (notebookObservations)
            {
                notebookObservations.text = "";
                foreach (var obs in pattern.observations)
                    notebookObservations.text += $"— {obs}\n";
            }

            // Show Deep Record only if Trio was recently active
            bool trioReady = TrioSystem.WasTrioActiveRecently;
            deepRecordButton?.gameObject.SetActive(trioReady);

            // Pause game time slightly (not full pause — world still breathes)
            Time.timeScale = 0.3f;
        }

        public void CloseNotebook()
        {
            _notebookOpen = false;
            notebookPanel?.SetActive(false);
            Time.timeScale = 1f;
        }

        public void ToggleNotebook()
        {
            var notebook = GameManager.Instance?.notebookSystem;
            if (notebook == null || !notebook.IsUnlocked) return;

            if (_notebookOpen) CloseNotebook();
            else if (notebook.PendingPattern != null)
                OpenNotebook(notebook.PendingPattern);
        }

        // ── Public API — Notifications ────────────────────────────────────────

        public void ShowNotification(string message, NotificationType type)
        {
            if (_notificationRoutine != null)
                StopCoroutine(_notificationRoutine);
            _notificationRoutine = StartCoroutine(
                NotificationRoutine(message, type));
        }

        // ── Public API — Companion Dialogue ──────────────────────────────────

        public void ShowCompanionDialogue(string companionName, string line)
        {
            if (_companionRoutine != null)
                StopCoroutine(_companionRoutine);
            _companionRoutine = StartCoroutine(
                CompanionDialogueRoutine(companionName, line));
        }

        // ── Public API — Rank Up ──────────────────────────────────────────────

        public void ShowRankUp(int newRank)
        {
            StartCoroutine(RankUpRoutine(newRank));
        }

        // ── Button Handlers ───────────────────────────────────────────────────

        private void OnRecordClicked()
        {
            GameManager.Instance?.notebookSystem?.Record();
            CloseNotebook();
        }

        private void OnWaitClicked()
        {
            GameManager.Instance?.notebookSystem?.Wait();
            CloseNotebook();
        }

        private void OnDeepRecordClicked()
        {
            GameManager.Instance?.notebookSystem?.DeepRecord();
            CloseNotebook();
        }

        private void OnRevisitClicked()
        {
            CloseNotebook();
            GameManager.Instance?.SetMemoryWalk(true);
        }

        // ── Coroutines ────────────────────────────────────────────────────────

        private IEnumerator NotificationRoutine(string message, NotificationType type)
        {
            if (notificationPanel == null) yield break;

            notificationPanel.SetActive(true);
            if (notificationText) notificationText.text = message;

            // Set colour by type
            Color col = type switch
            {
                NotificationType.Perception => colorPerception,
                NotificationType.Record     => colorRecord,
                NotificationType.Wait       => colorWait,
                NotificationType.Trio       => colorTrio,
                NotificationType.Memory     => colorMemory,
                NotificationType.Echo       => colorEcho,
                _                           => Color.white
            };
            if (notificationText) notificationText.color = col;

            // Fade in
            var canvasGroup = notificationPanel.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                float t = 0f;
                while (t < 0.4f) { t += Time.unscaledDeltaTime;
                    canvasGroup.alpha = t / 0.4f; yield return null; }
            }

            yield return new WaitForSecondsRealtime(notificationDuration);

            // Fade out
            if (canvasGroup != null)
            {
                float t = 0f;
                while (t < 0.6f) { t += Time.unscaledDeltaTime;
                    canvasGroup.alpha = 1f - t / 0.6f; yield return null; }
            }

            notificationPanel.SetActive(false);
        }

        private IEnumerator CompanionDialogueRoutine(string name, string line)
        {
            if (companionDialoguePanel == null) yield break;

            companionDialoguePanel.SetActive(true);
            if (companionNameLabel) companionNameLabel.text = name;
            if (companionLineLabel) companionLineLabel.text = "";

            // Typewriter effect
            foreach (char c in line)
            {
                if (companionLineLabel) companionLineLabel.text += c;
                yield return new WaitForSecondsRealtime(0.04f);
            }

            yield return new WaitForSecondsRealtime(companionDialogueDuration);
            companionDialoguePanel.SetActive(false);
        }

        private IEnumerator RankUpRoutine(int rank)
        {
            if (rankUpPanel == null) yield break;

            string rankName = rank <= PlayerStats.RankNames.Length
                ? PlayerStats.RankNames[rank - 1] : "Beyond Observer";

            rankUpPanel.SetActive(true);
            if (rankUpLabel) rankUpLabel.text = $"Rank {rank}";
            if (rankUpName)  rankUpName.text  = rankName;

            yield return new WaitForSecondsRealtime(4f);
            rankUpPanel.SetActive(false);
            UpdateHUD();
        }

        private IEnumerator FadeImage(Image img, float targetAlpha, float speed)
        {
            if (img == null) yield break;
            Color c = img.color;
            while (!Mathf.Approximately(c.a, targetAlpha))
            {
                c.a       = Mathf.MoveTowards(c.a, targetAlpha, speed * Time.unscaledDeltaTime);
                img.color = c;
                yield return null;
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static void SetSlider(Slider slider, float value)
        {
            if (slider != null) slider.value = value / 100f;
        }
    }

    // ── Enums ─────────────────────────────────────────────────────────────────

    public enum NotificationType
    {
        Perception, Record, Wait, Trio, Memory, Echo, General
    }
}