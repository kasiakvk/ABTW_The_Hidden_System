using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using ABTW.Systems;

namespace ABTW.UI
{
    /// <summary>
    /// HUDController — In-game heads-up display.
    ///
    /// Elements (per GDD — minimal, non-RPG):
    ///   - 5 stat bars (Awareness, Clarity, Connection, Timing, Stability)
    ///   - Rank label + Chapter label
    ///   - Perception Mode ring (fades in/out)
    ///   - Trio readiness indicator (subtle)
    ///   - World state (time/weather icon — very subtle)
    ///   - NO minimap, NO health bar, NO quest markers
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        // ── Inspector — Stats ────────────────────────────────────────────────

        [Header("— STAT BARS —")]
        [SerializeField] private Slider awarenessBar;
        [SerializeField] private Slider clarityBar;
        [SerializeField] private Slider connectionBar;
        [SerializeField] private Slider timingBar;
        [SerializeField] private Slider stabilityBar;

        [Header("— STAT LABELS —")]
        [SerializeField] private TextMeshProUGUI awarenessLabel;
        [SerializeField] private TextMeshProUGUI clarityLabel;
        [SerializeField] private TextMeshProUGUI connectionLabel;
        [SerializeField] private TextMeshProUGUI timingLabel;
        [SerializeField] private TextMeshProUGUI stabilityLabel;

        [Header("— RANK & CHAPTER —")]
        [SerializeField] private TextMeshProUGUI rankText;
        [SerializeField] private TextMeshProUGUI chapterText;
        [SerializeField] private TextMeshProUGUI expText;

        [Header("— PERCEPTION RING —")]
        [SerializeField] private CanvasGroup perceptionRing;
        [SerializeField] private Image       perceptionFill;
        [SerializeField] private float       ringFadeSpeed = 3f;

        [Header("— TRIO INDICATOR —")]
        [SerializeField] private CanvasGroup trioIndicator;
        [SerializeField] private Image       lumiDot;
        [SerializeField] private Image       nelaDot;
        [SerializeField] private Image       astraDot;
        [SerializeField] private Color       dotReady    = new Color(0.85f, 0.75f, 0.30f);
        [SerializeField] private Color       dotNotReady = new Color(0.35f, 0.33f, 0.30f);

        [Header("— WORLD STATE —")]
        [SerializeField] private TextMeshProUGUI worldStateText;
        [SerializeField] private CanvasGroup     worldStateGroup;

        [Header("— ANOMALY COUNTER —")]
        [SerializeField] private TextMeshProUGUI anomalyCountText;

        // ── State ────────────────────────────────────────────────────────────

        private PlayerStats _stats;
        private bool        _perceptionActive;
        private bool        _lumiReady;
        private bool        _nelaReady;
        private float       _perceptionTimer;
        private float       _perceptionDuration;

        // ── Unity ────────────────────────────────────────────────────────────

        private void Start()
        {
            _stats = GameManager.Instance?.playerStats;

            // Subscribe to events
            if (_stats != null)
            {
                _stats.OnStatChanged  += OnStatChanged;
                _stats.OnRankUp       += OnRankUp;
            }

            var gm = GameManager.Instance;
            if (gm != null)
            {
                gm.OnPerceptionModeChanged += OnPerceptionModeChanged;
                gm.OnAnomalyFound          += _ => RefreshAnomalyCount();
            }

            var trio = GameManager.Instance?.trioSystem;
            if (trio != null)
            {
                trio.OnLumiReadyChanged += ready => { _lumiReady = ready; RefreshTrioDots(); };
                trio.OnNelaReadyChanged += ready => { _nelaReady = ready; RefreshTrioDots(); };
                trio.OnTrioActivated    += OnTrioActivated;
            }

            var world = WorldManager.Instance;
            if (world != null)
            {
                world.OnTimeChanged    += _ => RefreshWorldState();
                world.OnWeatherChanged += _ => RefreshWorldState();
            }

            // Initial refresh
            RefreshAllStats();
            RefreshRank();
            RefreshWorldState();
            RefreshAnomalyCount();

            // Hide perception ring initially
            if (perceptionRing != null) perceptionRing.alpha = 0f;
            if (trioIndicator  != null) trioIndicator.alpha  = 0f;
        }

        private void Update()
        {
            if (_perceptionActive)
                UpdatePerceptionRing();
        }

        // ── Private — Stats ──────────────────────────────────────────────────

        private void RefreshAllStats()
        {
            if (_stats == null) return;
            UpdateBar(awarenessBar,  awarenessLabel,  _stats.Awareness,  "👁");
            UpdateBar(clarityBar,    clarityLabel,    _stats.Clarity,    "🧩");
            UpdateBar(connectionBar, connectionLabel, _stats.Connection, "🔗");
            UpdateBar(timingBar,     timingLabel,     _stats.Timing,     "⏳");
            UpdateBar(stabilityBar,  stabilityLabel,  _stats.Stability,  "🌀");
        }

        private void UpdateBar(Slider bar, TextMeshProUGUI label, float value, string icon)
        {
            if (bar   != null) bar.value = value / 100f;
            if (label != null) label.text = $"{icon} {Mathf.RoundToInt(value)}";
        }

        private void OnStatChanged(string statName, float newValue)
        {
            switch (statName)
            {
                case "Awareness":
                    UpdateBar(awarenessBar, awarenessLabel, newValue, "👁"); break;
                case "Clarity":
                    UpdateBar(clarityBar, clarityLabel, newValue, "🧩"); break;
                case "Connection":
                    UpdateBar(connectionBar, connectionLabel, newValue, "🔗"); break;
                case "Timing":
                    UpdateBar(timingBar, timingLabel, newValue, "⏳"); break;
                case "Stability":
                    UpdateBar(stabilityBar, stabilityLabel, newValue, "🌀"); break;
            }
        }

        // ── Private — Rank ───────────────────────────────────────────────────

        private void RefreshRank()
        {
            if (_stats == null) return;
            if (rankText    != null) rankText.text    = _stats.CurrentRankName;
            if (chapterText != null) chapterText.text = "Chapter 1 — The First Glitch";
            if (expText     != null) expText.text     = $"EXP {_stats.CurrentEXP}";
        }

        private void OnRankUp(int newRank, string rankName)
        {
            RefreshRank();
            StartCoroutine(FlashRankText());
        }

        private IEnumerator FlashRankText()
        {
            if (rankText == null) yield break;
            Color original = rankText.color;
            Color flash    = new Color(0.95f, 0.88f, 0.40f);

            for (int i = 0; i < 3; i++)
            {
                rankText.color = flash;
                yield return new WaitForSeconds(0.2f);
                rankText.color = original;
                yield return new WaitForSeconds(0.2f);
            }
        }

        // ── Private — Perception Ring ────────────────────────────────────────

        private void OnPerceptionModeChanged(bool active)
        {
            _perceptionActive = active;

            if (active)
            {
                _perceptionTimer    = 0f;
                _perceptionDuration = GameManager.Instance?.perceptionSystem?.Duration ?? 15f;
                StartCoroutine(FadeGroup(perceptionRing, 0f, 1f, 0.4f));
            }
            else
            {
                StartCoroutine(FadeGroup(perceptionRing, 1f, 0f, 0.6f));
            }
        }

        private void UpdatePerceptionRing()
        {
            if (_perceptionDuration <= 0f) return;
            _perceptionTimer += Time.deltaTime;
            float fill = 1f - (_perceptionTimer / _perceptionDuration);
            if (perceptionFill != null)
                perceptionFill.fillAmount = Mathf.Clamp01(fill);
        }

        // ── Private — Trio ───────────────────────────────────────────────────

        private void RefreshTrioDots()
        {
            bool anyReady = _lumiReady || _nelaReady;

            if (trioIndicator != null)
                StartCoroutine(FadeGroup(trioIndicator, trioIndicator.alpha,
                    anyReady ? 1f : 0f, 0.5f));

            if (lumiDot  != null) lumiDot.color  = _lumiReady ? dotReady : dotNotReady;
            if (nelaDot  != null) nelaDot.color  = _nelaReady ? dotReady : dotNotReady;
            if (astraDot != null) astraDot.color = dotReady; // Astra always ready
        }

        private void OnTrioActivated()
        {
            StartCoroutine(TrioFlash());
        }

        private IEnumerator TrioFlash()
        {
            if (trioIndicator == null) yield break;
            Color gold = new Color(0.95f, 0.88f, 0.40f);

            for (int i = 0; i < 4; i++)
            {
                if (lumiDot  != null) lumiDot.color  = gold;
                if (nelaDot  != null) nelaDot.color  = gold;
                if (astraDot != null) astraDot.color = gold;
                yield return new WaitForSeconds(0.15f);
                RefreshTrioDots();
                yield return new WaitForSeconds(0.15f);
            }

            // Fade out trio indicator after activation
            yield return new WaitForSeconds(2f);
            StartCoroutine(FadeGroup(trioIndicator, 1f, 0f, 1f));
        }

        // ── Private — World State ────────────────────────────────────────────

        private void RefreshWorldState()
        {
            var world = WorldManager.Instance;
            if (world == null || worldStateText == null) return;

            string timeIcon = world.CurrentTime switch
            {
                TimeOfDay.Morning   => "🌅",
                TimeOfDay.Afternoon => "☀️",
                TimeOfDay.Sunset    => "🌇",
                TimeOfDay.Night     => "🌙",
                _                   => "☀️"
            };

            string weatherIcon = world.CurrentWeather switch
            {
                WeatherType.Rain     => "🌧",
                WeatherType.Fog      => "🌫",
                WeatherType.Overcast => "☁️",
                _                    => ""
            };

            worldStateText.text = $"{timeIcon} {weatherIcon}".Trim();

            // Fade in briefly then fade out
            StartCoroutine(WorldStatePulse());
        }

        private IEnumerator WorldStatePulse()
        {
            if (worldStateGroup == null) yield break;
            yield return StartCoroutine(FadeGroup(worldStateGroup, 0f, 1f, 0.5f));
            yield return new WaitForSeconds(3f);
            yield return StartCoroutine(FadeGroup(worldStateGroup, 1f, 0.3f, 1f));
        }

        // ── Private — Anomaly Count ──────────────────────────────────────────

        private void RefreshAnomalyCount()
        {
            if (anomalyCountText == null) return;
            int count = GameManager.Instance?.AnomaliesFound ?? 0;
            anomalyCountText.text = count > 0 ? $"◈ {count}" : "";
        }

        // ── Private — Utility ────────────────────────────────────────────────

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