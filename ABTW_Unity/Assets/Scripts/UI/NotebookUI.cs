using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using ABTW.Systems;

namespace ABTW.UI
{
    /// <summary>
    /// NotebookUI — Production-ready Notebook interface.
    ///
    /// Layout (per GDD):
    /// ┌────────────────────────────────────────────┐
    /// │ 📓 ASTRA'S NOTEBOOK                        │
    /// ├────────────────────────────────────────────┤
    /// │ [PATTERNS] [SYMBOLS] [CONNECTIONS] [MEMORY]│
    /// ├────────────────────────────────────────────┤
    /// │ PATTERN: SHADOW DELAY                      │
    /// │ Status: ● Unrecorded                       │
    /// │ Observed: — movement mismatch (×3)         │
    /// │ Lumi: "It's repeating."                    │
    /// │ Nela: "Don't write it yet."                │
    /// ├────────────────────────────────────────────┤
    /// │ [ RECORD ]              [ WAIT ]           │
    /// └────────────────────────────────────────────┘
    /// </summary>
    public class NotebookUI : MonoBehaviour
    {
        // ── Tab Enum ─────────────────────────────────────────────────────────

        private enum NotebookTab { Patterns, Symbols, Connections, Memory }

        // ── Inspector — Panels ───────────────────────────────────────────────

        [Header("— ROOT —")]
        [SerializeField] private GameObject notebookRoot;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("— HEADER —")]
        [SerializeField] private TextMeshProUGUI titleText;

        [Header("— TABS —")]
        [SerializeField] private Button tabPatterns;
        [SerializeField] private Button tabSymbols;
        [SerializeField] private Button tabConnections;
        [SerializeField] private Button tabMemory;
        [SerializeField] private Color  tabActiveColor   = new Color(0.95f, 0.88f, 0.65f);
        [SerializeField] private Color  tabInactiveColor = new Color(0.55f, 0.52f, 0.45f);

        [Header("— PATTERN CONTENT —")]
        [SerializeField] private TextMeshProUGUI patternNameText;
        [SerializeField] private TextMeshProUGUI patternStatusText;
        [SerializeField] private TextMeshProUGUI observationsText;
        [SerializeField] private TextMeshProUGUI lumiNoteText;
        [SerializeField] private TextMeshProUGUI nelaNoteText;
        [SerializeField] private Image           statusDot;
        [SerializeField] private Color           statusUnrecorded = new Color(0.85f, 0.75f, 0.30f);
        [SerializeField] private Color           statusRecorded   = new Color(0.40f, 0.85f, 0.55f);
        [SerializeField] private Color           statusEcho       = new Color(0.60f, 0.40f, 1.00f);

        [Header("— ACTION BUTTONS —")]
        [SerializeField] private Button recordButton;
        [SerializeField] private Button waitButton;
        [SerializeField] private Button deepRecordButton;
        [SerializeField] private Button revisitButton;
        [SerializeField] private Button closeButton;

        [Header("— PATTERN LIST (Patterns Tab) —")]
        [SerializeField] private Transform       patternListParent;
        [SerializeField] private GameObject      patternEntryPrefab;

        [Header("— ANIMATION —")]
        [SerializeField] private float openDuration  = 0.35f;
        [SerializeField] private float closeDuration = 0.25f;

        // ── State ────────────────────────────────────────────────────────────

        private NotebookTab      _activeTab = NotebookTab.Patterns;
        private PatternData      _currentPattern;
        private bool             _isOpen;
        private bool             _trioActive;
        private Coroutine        _fadeRoutine;

        // ── Unity ────────────────────────────────────────────────────────────

        private void Awake()
        {
            // Wire buttons
            recordButton?.onClick.AddListener(OnRecordClicked);
            waitButton?.onClick.AddListener(OnWaitClicked);
            deepRecordButton?.onClick.AddListener(OnDeepRecordClicked);
            revisitButton?.onClick.AddListener(OnRevisitClicked);
            closeButton?.onClick.AddListener(CloseNotebook);

            tabPatterns?.onClick.AddListener(() => SwitchTab(NotebookTab.Patterns));
            tabSymbols?.onClick.AddListener(() => SwitchTab(NotebookTab.Symbols));
            tabConnections?.onClick.AddListener(() => SwitchTab(NotebookTab.Connections));
            tabMemory?.onClick.AddListener(() => SwitchTab(NotebookTab.Memory));

            // Start hidden
            if (notebookRoot != null) notebookRoot.SetActive(false);
            if (canvasGroup  != null) canvasGroup.alpha = 0f;
        }

        private void Start()
        {
            // Subscribe to game events
            var nb = GameManager.Instance?.notebookSystem;
            if (nb != null)
            {
                nb.OnNotebookOpened  += OpenNotebook;
                nb.OnNotebookClosed  += CloseNotebook;
                nb.OnPatternAdded    += ShowPattern;
                nb.OnPatternRecorded += OnPatternRecorded;
                nb.OnPatternWaited   += OnPatternWaited;
            }

            var trio = GameManager.Instance?.trioSystem;
            if (trio != null)
                trio.OnTrioActivated += OnTrioActivated;
        }

        // ── Public ───────────────────────────────────────────────────────────

        public void OpenNotebook()
        {
            if (_isOpen) return;
            _isOpen = true;

            if (notebookRoot != null) notebookRoot.SetActive(true);
            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            _fadeRoutine = StartCoroutine(FadeCanvas(0f, 1f, openDuration));

            RefreshPatternList();
            SwitchTab(_activeTab);
        }

        public void CloseNotebook()
        {
            if (!_isOpen) return;
            _isOpen = false;

            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            _fadeRoutine = StartCoroutine(FadeCanvas(1f, 0f, closeDuration,
                onComplete: () => notebookRoot?.SetActive(false)));
        }

        public void ShowPattern(PatternData pattern)
        {
            _currentPattern = pattern;
            OpenNotebook();
            SwitchTab(NotebookTab.Patterns);
            PopulatePatternView(pattern);
        }

        // ── Private — Tabs ───────────────────────────────────────────────────

        private void SwitchTab(NotebookTab tab)
        {
            _activeTab = tab;
            UpdateTabColors();

            switch (tab)
            {
                case NotebookTab.Patterns:    ShowPatternsTab();    break;
                case NotebookTab.Symbols:     ShowSymbolsTab();     break;
                case NotebookTab.Connections: ShowConnectionsTab(); break;
                case NotebookTab.Memory:      ShowMemoryTab();      break;
            }
        }

        private void UpdateTabColors()
        {
            SetTabColor(tabPatterns,    _activeTab == NotebookTab.Patterns);
            SetTabColor(tabSymbols,     _activeTab == NotebookTab.Symbols);
            SetTabColor(tabConnections, _activeTab == NotebookTab.Connections);
            SetTabColor(tabMemory,      _activeTab == NotebookTab.Memory);
        }

        private void SetTabColor(Button btn, bool active)
        {
            if (btn == null) return;
            var colors = btn.colors;
            colors.normalColor = active ? tabActiveColor : tabInactiveColor;
            btn.colors = colors;
        }

        // ── Private — Tab Content ────────────────────────────────────────────

        private void ShowPatternsTab()
        {
            if (_currentPattern != null)
                PopulatePatternView(_currentPattern);
            else
                ShowEmptyPatternView();

            RefreshPatternList();
            SetActionButtons(showRecord: true, showWait: true,
                             showDeep: _trioActive, showRevisit: false);
        }

        private void ShowSymbolsTab()
        {
            SetPatternText("SYMBOLS", "Observed symbols appear here.", "", "", "");
            SetActionButtons(false, false, false, false);
        }

        private void ShowConnectionsTab()
        {
            SetPatternText("CONNECTIONS", "Linked patterns appear here.", "", "", "");
            SetActionButtons(false, false, false, false);
        }

        private void ShowMemoryTab()
        {
            SetPatternText("MEMORY", "Past recorded events.", "", "", "");
            SetActionButtons(false, false, false, showRevisit: true);
        }

        // ── Private — Pattern View ───────────────────────────────────────────

        private void PopulatePatternView(PatternData pattern)
        {
            if (patternNameText   != null) patternNameText.text   = pattern.anomalyType.ToString().ToUpper();
            if (observationsText  != null) observationsText.text  = pattern.notes;

            // Status
            bool recorded = pattern.status == PatternStatus.Recorded;
            bool isEcho   = pattern.status == PatternStatus.Echo;

            if (patternStatusText != null)
                patternStatusText.text = recorded ? "● Recorded" : isEcho ? "● Echo" : "● Unrecorded";

            if (statusDot != null)
                statusDot.color = recorded ? statusRecorded : isEcho ? statusEcho : statusUnrecorded;

            // Companion notes
            SetCompanionNotes(pattern.anomalyType);

            // Buttons
            SetActionButtons(
                showRecord:  !recorded,
                showWait:    !recorded,
                showDeep:    _trioActive && !recorded,
                showRevisit: recorded);
        }

        private void ShowEmptyPatternView()
        {
            SetPatternText("NO PATTERN SELECTED",
                "Explore the Academy to discover patterns.",
                "", "", "");
            SetActionButtons(false, false, false, false);
        }

        private void SetPatternText(string name, string observations,
            string status, string lumi, string nela)
        {
            if (patternNameText   != null) patternNameText.text   = name;
            if (observationsText  != null) observationsText.text  = observations;
            if (patternStatusText != null) patternStatusText.text = status;
            if (lumiNoteText      != null) lumiNoteText.text      = lumi;
            if (nelaNoteText      != null) nelaNoteText.text      = nela;
        }

        private void SetCompanionNotes(AnomalyType type)
        {
            string lumi = "", nela = "";

            switch (type)
            {
                case AnomalyType.ShadowDelay:
                    lumi = "Lumi: \"It's repeating.\"";
                    nela = "Nela: \"Don't write it yet.\"";
                    break;
                case AnomalyType.LightInstability:
                    lumi = "Lumi: \"The frequency is irregular.\"";
                    nela = "Nela: \"Something is trying to show itself.\"";
                    break;
                case AnomalyType.SymbolFragment:
                    lumi = "Lumi: \"Incomplete. There's more.\"";
                    nela = "Nela: \"Wait for it to finish.\"";
                    break;
                case AnomalyType.ReflectionMismatch:
                    lumi = "Lumi: \"The angles don't match.\"";
                    nela = "Nela: \"Look longer.\"";
                    break;
                case AnomalyType.SoundEcho:
                    lumi = "Lumi: \"Delayed by 0.4 seconds.\"";
                    nela = "Nela: \"It's calling back.\"";
                    break;
                default:
                    lumi = "Lumi: \"Analysing…\"";
                    nela = "Nela: \"…\"";
                    break;
            }

            if (lumiNoteText != null) lumiNoteText.text = lumi;
            if (nelaNoteText != null) nelaNoteText.text = nela;
        }

        private void SetActionButtons(bool showRecord, bool showWait,
            bool showDeep, bool showRevisit)
        {
            if (recordButton     != null) recordButton.gameObject.SetActive(showRecord);
            if (waitButton       != null) waitButton.gameObject.SetActive(showWait);
            if (deepRecordButton != null) deepRecordButton.gameObject.SetActive(showDeep);
            if (revisitButton    != null) revisitButton.gameObject.SetActive(showRevisit);
        }

        // ── Private — Pattern List ───────────────────────────────────────────

        private void RefreshPatternList()
        {
            if (patternListParent == null || patternEntryPrefab == null) return;

            // Clear existing
            foreach (Transform child in patternListParent)
                Destroy(child.gameObject);

            // Populate from NotebookSystem
            var nb = GameManager.Instance?.notebookSystem;
            if (nb == null) return;

            foreach (var pattern in nb.GetAllPatterns())
            {
                var entry = Instantiate(patternEntryPrefab, patternListParent);
                var label = entry.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                    label.text = $"{pattern.anomalyType}  [{pattern.status}]";

                var btn = entry.GetComponent<Button>();
                if (btn != null)
                {
                    var p = pattern; // capture
                    btn.onClick.AddListener(() => PopulatePatternView(p));
                }
            }
        }

        // ── Private — Button Handlers ────────────────────────────────────────

        private void OnRecordClicked()
        {
            if (_currentPattern == null) return;
            GameManager.Instance?.notebookSystem?.RecordPattern(_currentPattern.anomalyType);
            CloseNotebook();
        }

        private void OnWaitClicked()
        {
            if (_currentPattern == null) return;
            GameManager.Instance?.notebookSystem?.WaitPattern(_currentPattern.anomalyType);
            CloseNotebook();
        }

        private void OnDeepRecordClicked()
        {
            if (_currentPattern == null || !_trioActive) return;
            GameManager.Instance?.notebookSystem?.DeepRecordPattern(_currentPattern.anomalyType);
            CloseNotebook();
        }

        private void OnRevisitClicked()
        {
            if (_currentPattern == null) return;
            GameManager.Instance?.memoryWalkSystem?.EnterMemoryWalk(_currentPattern.location);
            CloseNotebook();
        }

        // ── Private — Event Handlers ─────────────────────────────────────────

        private void OnPatternRecorded(PatternData pattern)
        {
            if (_currentPattern?.anomalyType == pattern.anomalyType)
                PopulatePatternView(pattern);
            RefreshPatternList();
        }

        private void OnPatternWaited(PatternData pattern)
        {
            RefreshPatternList();
        }

        private void OnTrioActivated()
        {
            _trioActive = true;
            if (_isOpen && _activeTab == NotebookTab.Patterns)
                SetActionButtons(true, true, true, false);
        }

        // ── Private — Animation ──────────────────────────────────────────────

        private IEnumerator FadeCanvas(float from, float to, float duration,
            Action onComplete = null)
        {
            if (canvasGroup == null) yield break;

            float elapsed = 0f;
            canvasGroup.alpha = from;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
                yield return null;
            }

            canvasGroup.alpha = to;
            onComplete?.Invoke();
        }
    }
}