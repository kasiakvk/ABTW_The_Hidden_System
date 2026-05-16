using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using ABTW.Systems;

namespace ABTW.Core
{
    /// <summary>
    /// PauseMenu — Esc key pause system.
    ///
    /// ABTW design rule: pause menu must feel like the notebook —
    /// calm, minimal, no flashy transitions.
    ///
    /// Options:
    ///   - Resume
    ///   - Settings (audio/graphics stub)
    ///   - Memory Walk (quick access)
    ///   - Return to Main Menu
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        [Header("— PANELS —")]
        [SerializeField] private GameObject pauseRoot;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject settingsPanel;

        [Header("— BUTTONS —")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button memoryWalkButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button settingsBackButton;

        [Header("— LABELS —")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI rankLabel;
        [SerializeField] private TextMeshProUGUI timeLabel;

        [Header("— SETTINGS SLIDERS —")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        [Header("— ANIMATION —")]
        [SerializeField] private float fadeDuration = 0.3f;

        // ── State ────────────────────────────────────────────────────────────

        private bool _isPaused;

        // ── Unity ────────────────────────────────────────────────────────────

        private void Awake()
        {
            resumeButton?.onClick.AddListener(Resume);
            settingsButton?.onClick.AddListener(OpenSettings);
            memoryWalkButton?.onClick.AddListener(OpenMemoryWalk);
            mainMenuButton?.onClick.AddListener(ReturnToMainMenu);
            settingsBackButton?.onClick.AddListener(CloseSettings);

            masterVolumeSlider?.onValueChanged.AddListener(v =>
                AudioListener.volume = v);

            if (pauseRoot != null) pauseRoot.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                TogglePause();
        }

        // ── Public ───────────────────────────────────────────────────────────

        public void TogglePause()
        {
            if (_isPaused) Resume();
            else           Pause();
        }

        public void Pause()
        {
            _isPaused = true;
            Time.timeScale = 0f;

            if (pauseRoot != null) pauseRoot.SetActive(true);
            StartCoroutine(FadeIn());

            // Update labels
            if (titleText != null) titleText.text = "PAUSED";

            var stats = GameManager.Instance?.playerStats;
            if (rankLabel != null && stats != null)
                rankLabel.text = $"Rank: {stats.CurrentRankName}";

            if (timeLabel != null)
            {
                var world = WorldManager.Instance;
                timeLabel.text = world != null
                    ? $"{world.CurrentTime}  ·  {world.CurrentSeason}"
                    : "";
            }
        }

        public void Resume()
        {
            _isPaused = false;
            StartCoroutine(FadeOut(() =>
            {
                Time.timeScale = 1f;
                if (pauseRoot != null) pauseRoot.SetActive(false);
            }));
        }

        // ── Private ──────────────────────────────────────────────────────────

        private void OpenSettings()
        {
            if (settingsPanel != null) settingsPanel.SetActive(true);
        }

        private void CloseSettings()
        {
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }

        private void OpenMemoryWalk()
        {
            Resume();
            GameManager.Instance?.memoryWalkSystem?.EnterMemoryWalk(Vector3.zero);
        }

        private void ReturnToMainMenu()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        private IEnumerator FadeIn()
        {
            if (canvasGroup == null) yield break;
            float elapsed = 0f;
            canvasGroup.alpha = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }

        private IEnumerator FadeOut(System.Action onComplete = null)
        {
            if (canvasGroup == null) { onComplete?.Invoke(); yield break; }
            float elapsed = 0f;
            canvasGroup.alpha = 1f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                yield return null;
            }
            canvasGroup.alpha = 0f;
            onComplete?.Invoke();
        }
    }
}