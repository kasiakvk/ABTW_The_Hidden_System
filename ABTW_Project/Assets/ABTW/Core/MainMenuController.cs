using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

namespace ABTW.Core
{
    /// <summary>
    /// MainMenuController — Opening screen for ABTW.
    ///
    /// Design rule: calm, minimal, cinematic.
    /// No flashy animations — slow fade, subtle text reveal.
    ///
    /// Layout:
    ///   - Academy logo (centre)
    ///   - Title: "Academy Beyond This World"
    ///   - Subtitle: "The Hidden System"
    ///   - [ BEGIN ]  [ CONTINUE ]  [ SETTINGS ]
    ///   - Core line: "Some worlds are hidden. Others… are waiting to be noticed."
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("— PANELS —")]
        [SerializeField] private CanvasGroup mainGroup;
        [SerializeField] private CanvasGroup settingsGroup;

        [Header("— TITLE —")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI subtitleText;
        [SerializeField] private TextMeshProUGUI taglineText;
        [SerializeField] private TextMeshProUGUI versionText;

        [Header("— BUTTONS —")]
        [SerializeField] private Button beginButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button settingsBackButton;

        [Header("— SETTINGS —")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private TMP_Dropdown qualityDropdown;

        [Header("— SCENE —")]
        [SerializeField] private string chapter1SceneName = "Chapter1_TheFirstGlitch";

        [Header("— ANIMATION —")]
        [SerializeField] private float introFadeDuration = 2.5f;
        [SerializeField] private float buttonFadeDelay   = 1.8f;

        // ── Unity ────────────────────────────────────────────────────────────

        private void Awake()
        {
            beginButton?.onClick.AddListener(OnBeginClicked);
            continueButton?.onClick.AddListener(OnContinueClicked);
            settingsButton?.onClick.AddListener(OpenSettings);
            quitButton?.onClick.AddListener(OnQuitClicked);
            settingsBackButton?.onClick.AddListener(CloseSettings);

            masterVolumeSlider?.onValueChanged.AddListener(v =>
                AudioListener.volume = v);
            fullscreenToggle?.onValueChanged.AddListener(v =>
                Screen.fullScreen = v);
            qualityDropdown?.onValueChanged.AddListener(v =>
                QualitySettings.SetQualityLevel(v));

            // Hide settings initially
            if (settingsGroup != null)
            {
                settingsGroup.alpha          = 0f;
                settingsGroup.interactable   = false;
                settingsGroup.blocksRaycasts = false;
            }

            // Continue only if save exists
            if (continueButton != null)
                continueButton.interactable = SaveExists();
        }

        private void Start()
        {
            StartCoroutine(IntroSequence());
        }

        // ── Private — Intro ──────────────────────────────────────────────────

        private IEnumerator IntroSequence()
        {
            // Start fully transparent
            if (mainGroup != null) mainGroup.alpha = 0f;

            // Fade in title slowly
            yield return StartCoroutine(FadeGroup(mainGroup, 0f, 1f, introFadeDuration));

            // Typewriter on tagline
            if (taglineText != null)
                yield return StartCoroutine(TypewriterEffect(taglineText,
                    "Some worlds are hidden.\nOthers… are waiting to be noticed.",
                    0.04f));

            // Version label
            if (versionText != null)
                versionText.text = "v0.1 — Chapter 1: The First Glitch";
        }

        private IEnumerator TypewriterEffect(TextMeshProUGUI label, string text, float delay)
        {
            label.text = "";
            foreach (char c in text)
            {
                label.text += c;
                yield return new WaitForSeconds(delay);
            }
        }

        // ── Private — Buttons ────────────────────────────────────────────────

        private void OnBeginClicked()
        {
            StartCoroutine(LoadScene(chapter1SceneName));
        }

        private void OnContinueClicked()
        {
            // TODO: load save data
            StartCoroutine(LoadScene(chapter1SceneName));
        }

        private void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OpenSettings()
        {
            if (settingsGroup == null) return;
            StartCoroutine(FadeGroup(settingsGroup, 0f, 1f, 0.3f));
            settingsGroup.interactable   = true;
            settingsGroup.blocksRaycasts = true;

            // Sync current values
            if (masterVolumeSlider != null)
                masterVolumeSlider.value = AudioListener.volume;
            if (fullscreenToggle != null)
                fullscreenToggle.isOn = Screen.fullScreen;
            if (qualityDropdown != null)
                qualityDropdown.value = QualitySettings.GetQualityLevel();
        }

        private void CloseSettings()
        {
            if (settingsGroup == null) return;
            StartCoroutine(FadeGroup(settingsGroup, 1f, 0f, 0.3f));
            settingsGroup.interactable   = false;
            settingsGroup.blocksRaycasts = false;
        }

        private IEnumerator LoadScene(string sceneName)
        {
            // Fade out
            yield return StartCoroutine(FadeGroup(mainGroup, 1f, 0f, 1.2f));

            // Load
            SceneManager.LoadScene(sceneName);
        }

        // ── Private — Utility ────────────────────────────────────────────────

        private bool SaveExists()
        {
            return PlayerPrefs.HasKey("ABTW_SaveExists");
        }

        private IEnumerator FadeGroup(CanvasGroup group, float from, float to, float duration)
        {
            if (group == null) yield break;
            float elapsed = 0f;
            group.alpha = from;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                group.alpha = Mathf.Lerp(from, to, elapsed / duration);
                yield return null;
            }
            group.alpha = to;
        }
    }
}