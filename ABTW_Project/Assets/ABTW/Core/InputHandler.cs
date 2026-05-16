using UnityEngine;
using ABTW.Systems;
using ABTW.UI;

namespace ABTW.Core
{
    /// <summary>
    /// Central input handler — all key bindings in one place.
    /// Designed for calm, deliberate interaction. No rapid-fire inputs.
    /// </summary>
    public class InputHandler : MonoBehaviour
    {
        [Header("— KEY BINDINGS —")]
        [SerializeField] private KeyCode perceptionKey  = KeyCode.P;
        [SerializeField] private KeyCode notebookKey    = KeyCode.N;
        [SerializeField] private KeyCode memoryWalkKey  = KeyCode.M;
        [SerializeField] private KeyCode trioKey        = KeyCode.T;
        [SerializeField] private KeyCode interactKey    = KeyCode.E;

        [Header("— COOLDOWNS —")]
        [SerializeField] private float perceptionCooldown = 1.0f;

        private float _perceptionCooldownTimer;

        // ── Unity ────────────────────────────────────────────────────────────

        private void Update()
        {
            _perceptionCooldownTimer -= Time.deltaTime;

            HandlePerception();
            HandleNotebook();
            HandleMemoryWalk();
            HandleTrio();
            HandleInteract();
        }

        // ── Private ──────────────────────────────────────────────────────────

        private void HandlePerception()
        {
            if (!Input.GetKeyDown(perceptionKey)) return;
            if (_perceptionCooldownTimer > 0f) return;

            _perceptionCooldownTimer = perceptionCooldown;
            GameManager.Instance?.SetPerceptionMode(
                !GameManager.Instance.IsPerceptionActive);
        }

        private void HandleNotebook()
        {
            if (!Input.GetKeyDown(notebookKey)) return;
            GameManager.Instance?.uiManager?.ToggleNotebook();
        }

        private void HandleMemoryWalk()
        {
            if (!Input.GetKeyDown(memoryWalkKey)) return;

            bool isActive = GameManager.Instance?.IsMemoryWalkActive ?? false;
            GameManager.Instance?.SetMemoryWalk(!isActive);
        }

        private void HandleTrio()
        {
            if (!Input.GetKeyDown(trioKey)) return;
            FindObjectOfType<TrioSystem>()?.PlayerTriggerTrio();
        }

        private void HandleInteract()
        {
            if (!Input.GetKeyDown(interactKey)) return;

            // Raycast for interactable objects
            Ray ray = Camera.main.ScreenPointToRay(
                new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));

            if (Physics.Raycast(ray, out RaycastHit hit, 3f))
            {
                var anomaly = hit.collider.GetComponent<AnomalyBase>();
                if (anomaly != null && anomaly.IsDetected && !anomaly.IsResolved)
                {
                    // Open notebook for this anomaly
                    var notebook = GameManager.Instance?.notebookSystem;
                    if (notebook?.PendingPattern != null)
                        GameManager.Instance?.uiManager?.OpenNotebook(notebook.PendingPattern);
                }
            }
        }
    }
}