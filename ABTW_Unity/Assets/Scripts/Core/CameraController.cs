using UnityEngine;

namespace ABTW.Core
{
    /// <summary>
    /// Third-person cinematic camera for ABTW.
    /// Follows Astra with smooth lag, supports:
    ///   - Normal follow mode
    ///   - Perception Mode (slight zoom-in + breath effect)
    ///   - Memory Walk mode (desaturated, slow drift)
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;

        [Header("Normal Follow")]
        [SerializeField] private Vector3  offset        = new Vector3(0f, 2.2f, -4.5f);
        [SerializeField] private float    followSpeed   = 6f;
        [SerializeField] private float    rotationSpeed = 4f;

        [Header("Perception Mode")]
        [SerializeField] private float perceptionFOV    = 58f;
        [SerializeField] private float normalFOV        = 65f;
        [SerializeField] private float fovTransSpeed    = 2f;
        [SerializeField] private float breathAmplitude  = 0.04f;
        [SerializeField] private float breathFrequency  = 0.8f;

        [Header("Memory Walk")]
        [SerializeField] private float memoryFOV        = 50f;
        [SerializeField] private float memoryDriftSpeed = 0.3f;

        // State
        private Camera  _cam;
        private float   _targetFOV;
        private bool    _perceptionActive;
        private bool    _memoryActive;
        private float   _breathTimer;
        private Vector3 _memoryDriftOffset;

        // ── Unity ────────────────────────────────────────────────────────────

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            if (_cam == null) _cam = Camera.main;
            _targetFOV = normalFOV;
        }

        private void Start()
        {
            // Auto-find player if not assigned
            if (target == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) target = player.transform;
            }

            // Subscribe to game events
            var gm = GameManager.Instance;
            if (gm != null)
            {
                gm.OnPerceptionModeChanged += SetPerceptionMode;
                gm.OnMemoryWalkChanged     += SetMemoryMode;
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            UpdatePosition();
            UpdateFOV();
            ApplyBreath();
        }

        // ── Public ───────────────────────────────────────────────────────────

        public void SetPerceptionMode(bool active)
        {
            _perceptionActive = active;
            _targetFOV = active ? perceptionFOV : normalFOV;
        }

        public void SetMemoryMode(bool active)
        {
            _memoryActive = active;
            _targetFOV = active ? memoryFOV : normalFOV;
            _memoryDriftOffset = Vector3.zero;
        }

        // ── Private ──────────────────────────────────────────────────────────

        private void UpdatePosition()
        {
            Vector3 desiredPos = target.position + target.TransformDirection(offset);

            if (_memoryActive)
            {
                // Slow cinematic drift in memory walk
                _memoryDriftOffset += new Vector3(
                    Mathf.Sin(Time.time * memoryDriftSpeed) * 0.002f,
                    Mathf.Cos(Time.time * memoryDriftSpeed * 0.7f) * 0.001f,
                    0f);
                desiredPos += _memoryDriftOffset;
            }

            transform.position = Vector3.Lerp(transform.position, desiredPos,
                followSpeed * Time.deltaTime);

            // Look at target (slightly above feet)
            Vector3 lookTarget = target.position + Vector3.up * 1.2f;
            Quaternion desiredRot = Quaternion.LookRotation(lookTarget - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot,
                rotationSpeed * Time.deltaTime);
        }

        private void UpdateFOV()
        {
            if (_cam == null) return;
            _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, _targetFOV,
                fovTransSpeed * Time.deltaTime);
        }

        private void ApplyBreath()
        {
            if (!_perceptionActive) return;

            _breathTimer += Time.deltaTime * breathFrequency;
            float breathY = Mathf.Sin(_breathTimer * Mathf.PI * 2f) * breathAmplitude;
            transform.position += new Vector3(0f, breathY, 0f);
        }
    }
}