using UnityEngine;
using System.Collections;
using ABTW.Core;
using ABTW.Systems;
using ABTW.Anomalies;

namespace ABTW.Companions
{
    /// <summary>
    /// Lumi — seeks answers through logic, data and structure.
    /// She analyses anomalies automatically when player is patient.
    /// Her notebook starts drawing itself during Trio activation.
    /// </summary>
    public class LumiCompanion : MonoBehaviour
    {
        [Header("— MOVEMENT —")]
        [SerializeField] private float followDistance  = 1.8f;
        [SerializeField] private float followSpeed     = 2.2f;
        [SerializeField] private float analysisStopDist = 0.5f; // stops to analyse

        [Header("— ANALYSIS TIMING —")]
        [SerializeField] private float analysisDelay   = 2.0f;  // time before Lumi speaks
        [SerializeField] private float analysisMinTime = 1.5f;  // minimum analysis duration

        [Header("— DIALOGUE LINES —")]
        [SerializeField] private string[] noticeLines = {
            "…there's a pattern here.",
            "The data doesn't match.",
            "I'm recording this.",
            "Something is repeating."
        };
        [SerializeField] private string[] analysisLines = {
            "It's repeating.",
            "The interval is consistent — 0.35 seconds.",
            "This isn't random.",
            "Three occurrences. Same offset."
        };
        [SerializeField] private string[] trioLines = {
            "The notebook is responding.",
            "I see the structure now.",
            "The data is complete."
        };

        // State
        private bool            _isAnalysing;
        private AnomalyBase     _currentAnomaly;
        private Transform       _playerTransform;
        private TrioSystem      _trioSystem;
        private Coroutine       _analysisRoutine;

        // Animator
        private Animator        _animator;
        private static readonly int AnimAnalyse = Animator.StringToHash("Analyse");
        private static readonly int AnimWalk    = Animator.StringToHash("Walk");

        // ── Unity ────────────────────────────────────────────────────────────

        private void Awake()
        {
            _animator       = GetComponentInChildren<Animator>();
            _trioSystem     = FindObjectOfType<TrioSystem>();
            _playerTransform = FindObjectOfType<PlayerMovement>()?.transform;
        }

        private void Update()
        {
            if (!_isAnalysing)
                FollowPlayer();
        }

        // ── Public API ───────────────────────────────────────────────────────

        /// <summary>Called by TrioSystem when an anomaly becomes active.</summary>
        public void BeginAnalysis(AnomalyBase anomaly)
        {
            _currentAnomaly = anomaly;
            _analysisRoutine = StartCoroutine(AnalysisRoutine());
        }

        /// <summary>Called by TrioSystem when player has waited long enough.</summary>
        public void CompleteAnalysis()
        {
            if (!_isAnalysing) return;
            Say(analysisLines[Random.Range(0, analysisLines.Length)]);
            _trioSystem?.OnLumiReady();
            _animator?.SetTrigger(AnimAnalyse);
        }

        /// <summary>Called during Trio activation — notebook draws itself.</summary>
        public void OnTrioActivation()
        {
            Say(trioLines[Random.Range(0, trioLines.Length)]);
            StartCoroutine(NotebookDrawEffect());
        }

        // ── Private ──────────────────────────────────────────────────────────

        private void FollowPlayer()
        {
            if (_playerTransform == null) return;

            // Follow slightly behind and to the left of player
            Vector3 targetPos = _playerTransform.position
                - _playerTransform.forward * followDistance
                + _playerTransform.right * (-0.8f);

            float dist = Vector3.Distance(transform.position, targetPos);
            _animator?.SetFloat(AnimWalk, dist > 0.3f ? 1f : 0f);

            if (dist > analysisStopDist)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, targetPos, followSpeed * Time.deltaTime);
                transform.LookAt(new Vector3(
                    _playerTransform.position.x, transform.position.y,
                    _playerTransform.position.z));
            }
        }

        private IEnumerator AnalysisRoutine()
        {
            _isAnalysing = true;

            // Brief pause before Lumi notices
            yield return new WaitForSeconds(analysisDelay);

            // Say notice line
            Say(noticeLines[Random.Range(0, noticeLines.Length)]);

            // Continue analysing until player waits long enough
            float elapsed = 0f;
            while (_isAnalysing && elapsed < 30f)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            _isAnalysing = false;
        }

        private IEnumerator NotebookDrawEffect()
        {
            // Visual: Lumi's notebook appears to draw symbols automatically
            // In full production: particle system + shader effect on notebook prop
            Debug.Log("[Lumi] Notebook drawing itself — Trio activation visual");
            yield return new WaitForSeconds(2f);
        }

        private void Say(string line)
        {
            GameManager.Instance?.uiManager?.ShowCompanionDialogue("Lumi", line);
            Debug.Log($"[Lumi] {line}");
        }
    }
}