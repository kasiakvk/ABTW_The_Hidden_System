using UnityEngine;
using System.Collections;
using ABTW.Core;

namespace ABTW.Companions
{
    /// <summary>
    /// Astra — main playable character.
    /// Role: PERCEIVER — she sees what others cannot.
    /// Controls: movement, perception mode, interaction with anomalies.
    /// </summary>
    public class AstraController : MonoBehaviour
    {
        [Header("Movement")]
        public float walkSpeed = 3.5f;
        public float perceptionWalkSpeed = 1.8f; // slower when in perception mode
        public float rotationSpeed = 8f;
        public float groundCheckDistance = 0.2f;
        public LayerMask groundLayer;

        [Header("Perception")]
        public float perceptionRadius = 6f;
        public float perceptionFOV = 110f;
        public float noticeDelay = 0.8f;       // time before anomaly is "noticed"
        public LayerMask anomalyLayer;

        [Header("Perception Mode")]
        public bool isInPerceptionMode = false;
        public float perceptionModeDuration = 5f;
        public float perceptionModeCooldown = 8f;

        [Header("References")]
        public ActivationLogic activationLogic;
        public Transform cameraTransform;
        public Animator animator;

        [Header("State")]
        [SerializeField] private bool isGrounded;
        [SerializeField] private bool isMoving;
        [SerializeField] private bool isInteracting;
        [SerializeField] private int anomaliesNoticed = 0;

        // Internal
        private CharacterController characterController;
        private Vector3 moveDirection;
        private float perceptionModeTimer = 0f;
        private float perceptionCooldownTimer = 0f;
        private bool perceptionOnCooldown = false;
        private Coroutine perceptionCoroutine;

        // Animator parameter hashes
        private static readonly int HashSpeed = Animator.StringToHash("Speed");
        private static readonly int HashIsPerceiving = Animator.StringToHash("IsPerceiving");
        private static readonly int HashNotice = Animator.StringToHash("Notice");
        private static readonly int HashInteract = Animator.StringToHash("Interact");

        // Events
        public System.Action<float> OnAwarenessGained;
        public System.Action OnAnomalyNoticed;
        public System.Action OnPerceptionModeEnter;
        public System.Action OnPerceptionModeExit;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            if (characterController == null)
                characterController = gameObject.AddComponent<CharacterController>();
        }

        private void Start()
        {
            if (cameraTransform == null && Camera.main != null)
                cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            HandleMovement();
            HandlePerceptionMode();
            HandlePerceptionScan();
            UpdateCooldowns();
            UpdateAnimator();
        }

        // ─── Movement ────────────────────────────────────────────────────────

        private void HandleMovement()
        {
            if (isInteracting) return;

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            isMoving = Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;

            if (isMoving)
            {
                // Camera-relative movement
                Vector3 camForward = cameraTransform != null
                    ? Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized
                    : Vector3.forward;
                Vector3 camRight = cameraTransform != null
                    ? Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized
                    : Vector3.right;

                moveDirection = (camForward * v + camRight * h).normalized;

                float currentSpeed = isInPerceptionMode ? perceptionWalkSpeed : walkSpeed;
                characterController.Move(moveDirection * currentSpeed * Time.deltaTime);

                // Smooth rotation toward movement direction
                Quaternion targetRot = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot,
                    rotationSpeed * Time.deltaTime);
            }

            // Gravity
            if (!characterController.isGrounded)
                characterController.Move(Vector3.down * 9.81f * Time.deltaTime);

            isGrounded = characterController.isGrounded;
        }

        // ─── Perception Mode ─────────────────────────────────────────────────

        private void HandlePerceptionMode()
        {
            // Toggle with F key (or controller button)
            if (Input.GetKeyDown(KeyCode.F) && !perceptionOnCooldown)
            {
                if (!isInPerceptionMode)
                    EnterPerceptionMode();
                else
                    ExitPerceptionMode();
            }

            if (isInPerceptionMode)
            {
                perceptionModeTimer -= Time.deltaTime;
                if (perceptionModeTimer <= 0f)
                    ExitPerceptionMode();
            }
        }

        private void EnterPerceptionMode()
        {
            isInPerceptionMode = true;
            perceptionModeTimer = perceptionModeDuration;
            OnPerceptionModeEnter?.Invoke();

            // Boost attention in ActivationLogic
            if (activationLogic != null)
                activationLogic.AddAttention(0.3f);

            if (perceptionCoroutine != null) StopCoroutine(perceptionCoroutine);
            perceptionCoroutine = StartCoroutine(PerceptionModeEffect());

            Debug.Log("[AstraController] Perception Mode ACTIVE");
        }

        private void ExitPerceptionMode()
        {
            isInPerceptionMode = false;
            perceptionOnCooldown = true;
            perceptionCooldownTimer = perceptionModeCooldown;
            OnPerceptionModeExit?.Invoke();
            Debug.Log("[AstraController] Perception Mode OFF — cooldown started");
        }

        private IEnumerator PerceptionModeEffect()
        {
            // Subtle camera + time effect handled by CameraController
            // Here we just pulse awareness
            while (isInPerceptionMode)
            {
                ScanForAnomalies();
                yield return new WaitForSeconds(0.5f);
            }
        }

        // ─── Perception Scan ─────────────────────────────────────────────────

        private void HandlePerceptionScan()
        {
            // Passive scan every frame (lighter version)
            if (!isInPerceptionMode) return;
        }

        private void ScanForAnomalies()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, perceptionRadius, anomalyLayer);

            foreach (var hit in hits)
            {
                // Check if within FOV
                Vector3 dirToAnomaly = (hit.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, dirToAnomaly);

                if (angle <= perceptionFOV * 0.5f)
                {
                    StartCoroutine(NoticeAnomaly(hit.gameObject));
                }
            }
        }

        private IEnumerator NoticeAnomaly(GameObject anomaly)
        {
            yield return new WaitForSeconds(noticeDelay);

            if (anomaly == null) return;

            anomaliesNoticed++;
            OnAnomalyNoticed?.Invoke();

            // Gain awareness
            float awarenessGain = 0.1f;
            OnAwarenessGained?.Invoke(awarenessGain);

            if (activationLogic != null)
                activationLogic.AddAttention(awarenessGain);

            // Trigger notice animation
            if (animator != null)
                animator.SetTrigger(HashNotice);

            Debug.Log($"[AstraController] Anomaly noticed: {anomaly.name} (total: {anomaliesNoticed})");
        }

        // ─── Interaction ─────────────────────────────────────────────────────

        public void StartInteraction()
        {
            isInteracting = true;
            if (animator != null) animator.SetTrigger(HashInteract);
        }

        public void EndInteraction()
        {
            isInteracting = false;
        }

        // ─── Cooldowns ───────────────────────────────────────────────────────

        private void UpdateCooldowns()
        {
            if (perceptionOnCooldown)
            {
                perceptionCooldownTimer -= Time.deltaTime;
                if (perceptionCooldownTimer <= 0f)
                    perceptionOnCooldown = false;
            }
        }

        // ─── Animator ────────────────────────────────────────────────────────

        private void UpdateAnimator()
        {
            if (animator == null) return;

            float speed = isMoving ? (isInPerceptionMode ? 0.5f : 1f) : 0f;
            animator.SetFloat(HashSpeed, speed, 0.1f, Time.deltaTime);
            animator.SetBool(HashIsPerceiving, isInPerceptionMode);
        }

        // ─── Gizmos ──────────────────────────────────────────────────────────

        private void OnDrawGizmosSelected()
        {
            // Perception radius
            Gizmos.color = isInPerceptionMode ? Color.cyan : new Color(0f, 1f, 1f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, perceptionRadius);

            // FOV lines
            Vector3 fovLeft = Quaternion.Euler(0, -perceptionFOV * 0.5f, 0) * transform.forward;
            Vector3 fovRight = Quaternion.Euler(0, perceptionFOV * 0.5f, 0) * transform.forward;
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, fovLeft * perceptionRadius);
            Gizmos.DrawRay(transform.position, fovRight * perceptionRadius);
        }
    }
}