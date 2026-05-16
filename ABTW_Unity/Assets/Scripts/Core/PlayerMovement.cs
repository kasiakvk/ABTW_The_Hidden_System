using UnityEngine;

namespace ABTW.Core
{
    /// <summary>
    /// Astra's movement — calm, observational, never heroic.
    /// Speed is intentionally slow. Astra thinks, she doesn't run.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("— MOVEMENT —")]
        [SerializeField] private float walkSpeed       = 2.8f;   // slow, deliberate
        [SerializeField] private float memoryWalkSpeed = 1.4f;   // even slower in memory
        [SerializeField] private float rotationSpeed   = 6f;
        [SerializeField] private float gravity         = -9.81f;

        [Header("— PERCEPTION SLOW —")]
        [SerializeField] private float perceptionSpeedMult = 0.6f; // slows when observing

        [Header("— CAMERA —")]
        [SerializeField] private Transform cameraTarget;           // assign in Inspector
        [SerializeField] private float cameraDistance  = 4.5f;
        [SerializeField] private float cameraHeight    = 1.8f;
        [SerializeField] private float cameraSmoothTime = 0.15f;

        // State
        private CharacterController _cc;
        private Vector3             _velocity;
        private Vector3             _camVelocity;
        private Camera              _cam;
        private bool                _isInMemoryWalk;
        private bool                _isPerceiving;

        // Notice animation
        private Animator            _animator;
        private static readonly int AnimNotice = Animator.StringToHash("Notice");
        private static readonly int AnimWalk   = Animator.StringToHash("Walk");

        // Events
        public System.Action OnPlayerStopped;   // fired when player stands still ≥ 2 sec
        private float _stillTimer;

        // ── Unity ───────────────────────────────────────────────────────────

        private void Awake()
        {
            _cc       = GetComponent<CharacterController>();
            _cam      = Camera.main;
            _animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            HandleMovement();
            HandleStillTimer();
        }

        private void LateUpdate()
        {
            SmoothCamera();
        }

        // ── Public API ──────────────────────────────────────────────────────

        public void SetMemoryWalk(bool active)
        {
            _isInMemoryWalk = active;
        }

        public void SetPerceiving(bool active)
        {
            _isPerceiving = active;
        }

        /// <summary>Trigger the "Notice" head-tilt animation.</summary>
        public void PlayNoticeAnimation()
        {
            _animator?.SetTrigger(AnimNotice);
        }

        // ── Private ─────────────────────────────────────────────────────────

        private void HandleMovement()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            // Camera-relative direction
            Vector3 camForward = _cam.transform.forward;
            Vector3 camRight   = _cam.transform.right;
            camForward.y = 0f; camForward.Normalize();
            camRight.y   = 0f; camRight.Normalize();

            Vector3 move = camForward * v + camRight * h;

            float speed = _isInMemoryWalk ? memoryWalkSpeed : walkSpeed;
            if (_isPerceiving) speed *= perceptionSpeedMult;

            // Rotate toward movement
            if (move.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }

            // Gravity
            if (_cc.isGrounded && _velocity.y < 0f) _velocity.y = -2f;
            _velocity.y += gravity * Time.deltaTime;

            _cc.Move((move * speed + _velocity) * Time.deltaTime);

            // Animator
            _animator?.SetFloat(AnimWalk, move.magnitude);
        }

        private void HandleStillTimer()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            bool moving = Mathf.Abs(h) > 0.05f || Mathf.Abs(v) > 0.05f;

            if (!moving)
            {
                _stillTimer += Time.deltaTime;
                if (_stillTimer >= 2f)
                {
                    OnPlayerStopped?.Invoke();
                    _stillTimer = 0f; // reset so it fires once per 2-sec window
                }
            }
            else
            {
                _stillTimer = 0f;
            }
        }

        private void SmoothCamera()
        {
            if (_cam == null || cameraTarget == null) return;

            Vector3 desiredPos = cameraTarget.position
                - transform.forward * cameraDistance
                + Vector3.up * cameraHeight;

            _cam.transform.position = Vector3.SmoothDamp(
                _cam.transform.position, desiredPos,
                ref _camVelocity, cameraSmoothTime);

            _cam.transform.LookAt(cameraTarget.position + Vector3.up * 0.5f);
        }
    }
}