using UnityEngine;
using System.Collections;
using ABTW.Core;

namespace ABTW.Companions
{
    /// <summary>
    /// Nela — AI companion.
    /// Role: ACTION MAKER — senses the right moment, aligns timing.
    /// She feels when the world is ready to respond.
    /// </summary>
    public class NelaActionMaker : MonoBehaviour
    {
        [Header("Timing Sensitivity")]
        public float timingSensitivity = 1.2f;   // how quickly she detects alignment
        public float alignmentWindow = 2.5f;      // seconds the alignment window stays open
        public float momentReadThreshold = 0.65f; // alignment score needed to "feel the moment"

        [Header("Action Settings")]
        public float actionCooldown = 5f;
        public float instinctRadius = 7f;

        [Header("Dialogue")]
        [TextArea(2, 3)] public string[] momentLines = {
            "Now.",
            "Wait… now.",
            "This is the moment.",
            "I can feel it — act now.",
            "The world is ready."
        };
        [TextArea(2, 3)] public string[] waitingLines = {
            "Not yet.",
            "Hold on…",
            "Something's still forming.",
            "Wait for it.",
            "Almost…"
        };
        [TextArea(2, 3)] public string[] missedLines = {
            "We missed it.",
            "It passed.",
            "There will be another moment.",
            "The echo will come."
        };

        [Header("References")]
        public ActivationLogic activationLogic;
        public Transform followTarget;
        public Animator animator;

        [Header("State")]
        [SerializeField] private bool isSensingMoment = false;
        [SerializeField] private bool momentIsOpen = false;
        [SerializeField] private float currentAlignmentScore = 0f;
        [SerializeField] private int momentsDetected = 0;
        [SerializeField] private int momentsActedOn = 0;

        private float actionCooldownTimer = 0f;
        private bool onCooldown = false;
        private Coroutine momentCoroutine;
        private Coroutine alignmentCoroutine;

        // Animator hashes
        private static readonly int HashIsSensing = Animator.StringToHash("IsSensing");
        private static readonly int HashAct = Animator.StringToHash("Act");
        private static readonly int HashReact = Animator.StringToHash("React");

        // Events
        public System.Action OnMomentDetected;
        public System.Action OnMomentActed;
        public System.Action OnMomentMissed;
        public System.Action<string> OnDialogueLine;
        public System.Action<float> OnAlignmentChanged;

        private void Start()
        {
            if (followTarget == null)
            {
                var astra = FindObjectOfType<AstraController>();
                if (astra != null) followTarget = astra.transform;
            }
        }

        private void Update()
        {
            FollowAstra();
            SenseMoment();
            UpdateCooldown();
        }

        // ─── Follow Behavior ─────────────────────────────────────────────────

        private void FollowAstra()
        {
            if (followTarget == null) return;

            // Nela stays slightly behind and to the right of Astra
            Vector3 targetOffset = followTarget.position
                - followTarget.forward * 1.0f
                + followTarget.right * 0.9f;

            transform.position = Vector3.Lerp(transform.position, targetOffset,
                timingSensitivity * 0.4f * Time.deltaTime);

            transform.rotation = Quaternion.Slerp(transform.rotation,
                followTarget.rotation, 3f * Time.deltaTime);
        }

        // ─── Moment Sensing ──────────────────────────────────────────────────

        private void SenseMoment()
        {
            if (onCooldown || activationLogic == null) return;

            // Nela reads the world's alignment state
            float worldAlignment = CalculateWorldAlignment();
            currentAlignmentScore = Mathf.Lerp(currentAlignmentScore, worldAlignment,
                timingSensitivity * Time.deltaTime);

            OnAlignmentChanged?.Invoke(currentAlignmentScore);

            // Feed alignment into ActivationLogic
            activationLogic.SetAlignment(currentAlignmentScore);

            // Detect moment opening
            if (currentAlignmentScore >= momentReadThreshold && !momentIsOpen)
            {
                OpenMomentWindow();
            }
        }

        private float CalculateWorldAlignment()
        {
            if (activationLogic == null) return 0f;

            // Nela's alignment = combination of attention + understanding already present
            float baseAlignment = (activationLogic.AttentionLevel * 0.4f)
                                + (activationLogic.UnderstandingLevel * 0.6f);

            // Add instinct bonus — Nela senses things slightly ahead of time
            float instinctBonus = Mathf.Sin(Time.time * 0.3f) * 0.1f; // subtle pulse

            return Mathf.Clamp01(baseAlignment + instinctBonus);
        }

        // ─── Moment Window ───────────────────────────────────────────────────

        private void OpenMomentWindow()
        {
            momentIsOpen = true;
            momentsDetected++;
            isSensingMoment = true;

            if (animator != null) animator.SetBool(HashIsSensing, true);

            SayLine(momentLines[Random.Range(0, momentLines.Length)]);
            OnMomentDetected?.Invoke();

            if (momentCoroutine != null) StopCoroutine(momentCoroutine);
            momentCoroutine = StartCoroutine(MomentWindowRoutine());

            Debug.Log($"[NelaActionMaker] Moment window OPEN (score: {currentAlignmentScore:F2})");
        }

        private IEnumerator MomentWindowRoutine()
        {
            float elapsed = 0f;

            while (elapsed < alignmentWindow)
            {
                elapsed += Time.deltaTime;

                // Check if player acted (notebook decision made)
                // This is checked externally via ActOnMoment()

                yield return null;
            }

            // Window closed without action
            if (momentIsOpen)
            {
                CloseMomentWindow(acted: false);
            }
        }

        public void ActOnMoment()
        {
            if (!momentIsOpen) return;

            momentsActedOn++;
            CloseMomentWindow(acted: true);

            if (animator != null) animator.SetTrigger(HashAct);
            OnMomentActed?.Invoke();

            // Boost alignment contribution
            if (activationLogic != null)
                activationLogic.SetAlignment(1f);

            Debug.Log($"[NelaActionMaker] Moment ACTED ON (total: {momentsActedOn}/{momentsDetected})");
        }

        private void CloseMomentWindow(bool acted)
        {
            momentIsOpen = false;
            isSensingMoment = false;

            if (animator != null) animator.SetBool(HashIsSensing, false);

            if (!acted)
            {
                SayLine(missedLines[Random.Range(0, missedLines.Length)]);
                OnMomentMissed?.Invoke();
                StartCooldown();
                Debug.Log("[NelaActionMaker] Moment window CLOSED — missed");
            }
            else
            {
                StartCooldown();
            }
        }

        // ─── Waiting Hint ────────────────────────────────────────────────────

        /// <summary>
        /// Called when player is about to record too early.
        /// Nela warns them to wait.
        /// </summary>
        public void WarnNotYet()
        {
            if (currentAlignmentScore < momentReadThreshold)
            {
                SayLine(waitingLines[Random.Range(0, waitingLines.Length)]);
                if (animator != null) animator.SetTrigger(HashReact);
            }
        }

        // ─── Cooldown ────────────────────────────────────────────────────────

        private void StartCooldown()
        {
            onCooldown = true;
            actionCooldownTimer = actionCooldown;
        }

        private void UpdateCooldown()
        {
            if (!onCooldown) return;
            actionCooldownTimer -= Time.deltaTime;
            if (actionCooldownTimer <= 0f)
                onCooldown = false;
        }

        // ─── Dialogue ────────────────────────────────────────────────────────

        public void SayLine(string line)
        {
            OnDialogueLine?.Invoke(line);
            Debug.Log($"[Nela] \"{line}\"");
        }

        // ─── Trio Integration ────────────────────────────────────────────────

        /// <summary>
        /// Called by TrioSystem. Nela contributes Alignment.
        /// Returns her current alignment contribution score.
        /// </summary>
        public float ContributeAlignment()
        {
            float contribution = momentIsOpen ? 1f : currentAlignmentScore;

            if (activationLogic != null)
                activationLogic.SetAlignment(contribution);

            if (animator != null) animator.SetTrigger(HashAct);
            return contribution;
        }

        public bool IsMomentOpen => momentIsOpen;
        public float AlignmentScore => currentAlignmentScore;
        public int MomentsDetected => momentsDetected;
        public int MomentsActedOn => momentsActedOn;

        // ─── Gizmos ──────────────────────────────────────────────────────────

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = momentIsOpen
                ? new Color(1f, 0.8f, 0f, 0.5f)
                : new Color(1f, 0.8f, 0f, 0.15f);
            Gizmos.DrawWireSphere(transform.position, instinctRadius);
        }
    }
}