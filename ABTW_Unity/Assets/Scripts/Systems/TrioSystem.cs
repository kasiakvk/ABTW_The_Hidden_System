using UnityEngine;
using System.Collections;
using ABTW.Core;
using ABTW.Companions;

namespace ABTW.Systems
{
    /// <summary>
    /// Trio System — no one completes the system alone.
    /// Astra (see) + Lumi (understand) + Nela (align) = Hidden Layer.
    ///
    /// Activation requires patience: player must NOT act for 3 seconds
    /// while an anomaly is active and both companions are ready.
    /// </summary>
    public class TrioSystem : MonoBehaviour
    {
        [Header("— COMPANIONS —")]
        [SerializeField] private LumiCompanion lumiCompanion;
        [SerializeField] private NelaCompanion nelaCompanion;

        [Header("— TIMING —")]
        [SerializeField] private float waitWindowSeconds  = 3f;   // player must wait this long
        [SerializeField] private float syncWindowSeconds  = 8f;   // window after Nela says "now"
        [SerializeField] private float cooldownSeconds    = 20f;  // between activations

        [Header("— HIDDEN LAYER —")]
        [SerializeField] private GameObject hiddenLayerRoot;      // assign in Inspector
        [SerializeField] private float      hiddenLayerDuration = 60f;

        // Static flag so NotebookSystem can check it
        public static bool WasTrioActiveRecently { get; private set; }

        // State
        private bool  _anomalyActive;
        private bool  _lumiReady;
        private bool  _nelaReady;
        private bool  _syncWindowOpen;
        private bool  _onCooldown;
        private float _waitTimer;
        private float _syncTimer;

        private GlitchSystem  _glitch;
        private NotebookSystem _notebook;

        // ── Unity ────────────────────────────────────────────────────────────

        private void Awake()
        {
            _glitch   = FindObjectOfType<GlitchSystem>();
            _notebook = FindObjectOfType<NotebookSystem>();

            if (hiddenLayerRoot != null)
                hiddenLayerRoot.SetActive(false);
        }

        private void Update()
        {
            if (_onCooldown || !_anomalyActive) return;

            TickWaitTimer();
            TickSyncWindow();
        }

        // ── Public API ───────────────────────────────────────────────────────

        /// <summary>Called by AnomalyBase when an anomaly becomes active.</summary>
        public void NotifyAnomalyActive(AnomalyBase anomaly)
        {
            if (_onCooldown) return;
            _anomalyActive = true;
            _waitTimer     = 0f;

            // Companions begin their analysis
            lumiCompanion?.BeginAnalysis(anomaly);
            nelaCompanion?.BeginAlignment(anomaly);

            Debug.Log("[Trio] Anomaly active — waiting for synchronisation.");
        }

        /// <summary>Called by AnomalyBase when anomaly is resolved (recorded).</summary>
        public void NotifyAnomalyResolved()
        {
            _anomalyActive  = false;
            _lumiReady      = false;
            _nelaReady      = false;
            _syncWindowOpen = false;
            _waitTimer      = 0f;
        }

        /// <summary>Called by LumiCompanion when analysis is complete.</summary>
        public void OnLumiReady()
        {
            _lumiReady = true;
            Debug.Log("[Trio] Lumi ready.");
            CheckSyncConditions();
        }

        /// <summary>Called by NelaCompanion when timing is aligned.</summary>
        public void OnNelaReady()
        {
            _nelaReady = true;
            Debug.Log("[Trio] Nela ready — sync window open.");
            OpenSyncWindow();
        }

        /// <summary>Player explicitly triggers Trio (optional button).</summary>
        public void PlayerTriggerTrio()
        {
            if (_syncWindowOpen) Activate();
        }

        // ── Private ──────────────────────────────────────────────────────────

        private void TickWaitTimer()
        {
            // Player must not press any action key
            bool playerActing = Input.GetMouseButtonDown(0) ||
                                Input.GetKeyDown(KeyCode.E) ||
                                Input.GetKeyDown(KeyCode.Space);

            if (playerActing)
            {
                _waitTimer = 0f; // reset if player acts
                return;
            }

            _waitTimer += Time.deltaTime;

            // After waitWindowSeconds of patience, companions become ready
            if (_waitTimer >= waitWindowSeconds && !_lumiReady)
            {
                lumiCompanion?.CompleteAnalysis();
                // Nela triggers slightly after Lumi
                Invoke(nameof(TriggerNelaReady), 0.8f);
            }
        }

        private void TriggerNelaReady()
        {
            nelaCompanion?.AlignTiming();
        }

        private void TickSyncWindow()
        {
            if (!_syncWindowOpen) return;

            _syncTimer += Time.deltaTime;
            if (_syncTimer >= syncWindowSeconds)
            {
                // Window expired — missed Trio
                _syncWindowOpen = false;
                _syncTimer      = 0f;
                Debug.Log("[Trio] Sync window expired.");
                GameManager.Instance?.uiManager?.ShowNotification(
                    "The moment passed.", NotificationType.Wait);
            }
        }

        private void CheckSyncConditions()
        {
            if (_lumiReady && _nelaReady)
                OpenSyncWindow();
        }

        private void OpenSyncWindow()
        {
            if (!_lumiReady || !_nelaReady) return;
            _syncWindowOpen = true;
            _syncTimer      = 0f;

            // Nela's cue to player
            nelaCompanion?.SayNow();

            // Auto-activate after brief pause if player doesn't act
            // (in single player, Trio activates automatically when conditions met)
            StartCoroutine(AutoActivate());
        }

        private IEnumerator AutoActivate()
        {
            yield return new WaitForSeconds(1.5f);
            if (_syncWindowOpen) Activate();
        }

        private void Activate()
        {
            if (!_syncWindowOpen || _onCooldown) return;

            _syncWindowOpen = false;
            _anomalyActive  = false;
            _lumiReady      = false;
            _nelaReady      = false;

            WasTrioActiveRecently = true;
            StartCoroutine(ResetTrioFlag());
            StartCoroutine(CooldownRoutine());

            // Visual effect
            if (_glitch != null && hiddenLayerRoot != null)
                StartCoroutine(_glitch.TrioActivationEffect(hiddenLayerRoot));

            // Open Hidden Layer
            StartCoroutine(OpenHiddenLayer());

            // Enable Deep Record in notebook
            _notebook?.AddPattern(new PatternData
            {
                name        = "Trio Pattern",
                description = "Observed through full synchronisation.",
                status      = PatternStatus.Evolving,
                lumiNote    = "The data is complete.",
                nelaNote    = "This was the right moment.",
                expOnRecord = 40,
                expOnDeep   = 80
            });

            Debug.Log("[Trio] ACTIVATED — Hidden Layer open.");
        }

        private IEnumerator OpenHiddenLayer()
        {
            if (hiddenLayerRoot == null) yield break;

            hiddenLayerRoot.SetActive(true);
            yield return new WaitForSeconds(hiddenLayerDuration);
            hiddenLayerRoot.SetActive(false);
            Debug.Log("[Trio] Hidden Layer closed.");
        }

        private IEnumerator ResetTrioFlag()
        {
            yield return new WaitForSeconds(10f);
            WasTrioActiveRecently = false;
        }

        private IEnumerator CooldownRoutine()
        {
            _onCooldown = true;
            yield return new WaitForSeconds(cooldownSeconds);
            _onCooldown = false;
        }
    }
}