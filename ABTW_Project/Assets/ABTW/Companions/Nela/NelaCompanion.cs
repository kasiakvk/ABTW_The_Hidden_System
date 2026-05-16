using UnityEngine;
using System.Collections;
using ABTW.Core;
using ABTW.Systems;
using ABTW.Anomalies;

namespace ABTW.Companions
{
    /// <summary>
    /// Nela — approaches the world with creativity and instinct.
    /// She feels the right moment. Her "…now." is the Trio trigger.
    /// She moves differently from Lumi — more fluid, less structured.
    /// </summary>
    public class NelaCompanion : MonoBehaviour
    {
        [Header("— MOVEMENT —")]
        [SerializeField] private float followDistance = 2.0f;
        [SerializeField] private float followSpeed    = 2.4f;
        [SerializeField] private float wanderRadius   = 1.2f;   // Nela wanders slightly

        [Header("— TIMING —")]
        [SerializeField] private float alignmentDelay  = 3.5f;  // after Lumi is ready
        [SerializeField] private float nowLinePause    = 0.6f;  // dramatic pause before "now"

        [Header("— DIALOGUE LINES —")]
        [SerializeField] private string[] instinctLines = {
            "Something feels off.",
            "Don't write it yet.",
            "Wait…",
            "There's more here."
        };
        [SerializeField] private string[] alignmentLines = {
            "I can feel it aligning.",
            "Almost…",
            "The moment is close.",
            "Don't move yet."
        };
        [SerializeField] private string nowLine = "…now.";

        [SerializeField] private string[] trioLines = {
            "We all saw it.",
            "That's what it was.",
            "The timing was right."
        };

        // State
        private bool        _isAligning;
        private bool        _alignmentComplete;
        private Transform   _playerTransform;
        private TrioSystem  _trioSystem;
        private Vector3     _wanderOffset;
        private float       _wanderTimer;

        // Animator
        private Animator    _animator;
        private static readonly int AnimAlign = Animator.StringToHash("Align");
        private static readonly int AnimWalk  = Animator.StringToHash("Walk");

        // ── Unity ────────────────────────────────────────────────────────────

        private void Awake()
        {
            _animator        = GetComponentInChildren<Animator>();
            _trioSystem      = FindObjectOfType<TrioSystem>();
            _playerTransform = FindObjectOfType<PlayerMovement>()?.transform;
            _wanderOffset    = Random.insideUnitSphere * wanderRadius;
            _wanderOffset.y  = 0f;
        }

        private void Update()
        {
            if (!_isAligning)
                FollowPlayerWithWander();
        }

        // ── Public API ───────────────────────────────────────────────────────

        /// <summary>Called by TrioSystem when anomaly becomes active.</summary>
        public void BeginAlignment(AnomalyBase anomaly)
        {
            _isAligning        = false;
            _alignmentComplete = false;

            // Nela immediately senses something
            float delay = Random.Range(0.5f, 1.2f);
            Invoke(nameof(SayInstinct), delay);
        }

        /// <summary>Called by TrioSystem when player has waited — Nela aligns.</summary>
        public void AlignTiming()
        {
            if (_alignmentComplete) return;
            StartCoroutine(AlignmentRoutine());
        }

        /// <summary>Called by TrioSystem — Nela says "…now."</summary>
        public void SayNow()
        {
            StartCoroutine(SayNowRoutine());
        }

        /// <summary>Called during Trio activation.</summary>
        public void OnTrioActivation()
        {
            Say(trioLines[Random.Range(0, trioLines.Length)]);
        }

        // ── Private ──────────────────────────────────────────────────────────

        private void FollowPlayerWithWander()
        {
            if (_playerTransform == null) return;

            // Update wander offset periodically
            _wanderTimer += Time.deltaTime;
            if (_wanderTimer > 3f)
            {
                _wanderOffset   = Random.insideUnitSphere * wanderRadius;
                _wanderOffset.y = 0f;
                _wanderTimer    = 0f;
            }

            // Follow to the right of player, with wander
            Vector3 targetPos = _playerTransform.position
                - _playerTransform.forward * followDistance
                + _playerTransform.right * 0.9f
                + _wanderOffset;

            float dist = Vector3.Distance(transform.position, targetPos);
            _animator?.SetFloat(AnimWalk, dist > 0.3f ? 1f : 0f);

            transform.position = Vector3.MoveTowards(
                transform.position, targetPos, followSpeed * Time.deltaTime);

            // Look toward player
            Vector3 lookDir = _playerTransform.position - transform.position;
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(lookDir),
                    5f * Time.deltaTime);
        }

        private IEnumerator AlignmentRoutine()
        {
            _isAligning = true;
            _animator?.SetTrigger(AnimAlign);

            // Nela stops and focuses
            yield return new WaitForSeconds(alignmentDelay * 0.4f);
            Say(alignmentLines[Random.Range(0, alignmentLines.Length)]);

            yield return new WaitForSeconds(alignmentDelay * 0.6f);

            _alignmentComplete = true;
            _isAligning        = false;
            _trioSystem?.OnNelaReady();
        }

        private IEnumerator SayNowRoutine()
        {
            yield return new WaitForSeconds(nowLinePause);
            Say(nowLine);
            Debug.Log("[Nela] …now. — Sync window open.");
        }

        private void SayInstinct()
        {
            Say(instinctLines[Random.Range(0, instinctLines.Length)]);
        }

        private void Say(string line)
        {
            GameManager.Instance?.uiManager?.ShowCompanionDialogue("Nela", line);
            Debug.Log($"[Nela] {line}");
        }
    }
}