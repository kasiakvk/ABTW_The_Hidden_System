using UnityEngine;
using System;
using System.Collections;

namespace ABTW.Systems
{
    // ── Enums ────────────────────────────────────────────────────────────────

    public enum TimeOfDay   { Morning, Afternoon, Sunset, Night }
    public enum WeatherType { Clear, Overcast, Rain, Fog }
    public enum Season      { Autumn, Winter, Spring, Summer }

    /// <summary>
    /// WorldManager — Living World System.
    ///
    /// Controls time, weather and seasons as PERCEPTION MECHANICS,
    /// not just visual decoration.
    ///
    /// GDD rules:
    ///   Night   → more anomalies visible
    ///   Rain    → SoundEcho easier to detect
    ///   Winter  → Stability +10%
    ///   Fog     → Awareness check harder
    ///   Autumn  → Memory traces more frequent
    /// </summary>
    public class WorldManager : MonoBehaviour
    {
        // ── Singleton ────────────────────────────────────────────────────────

        public static WorldManager Instance { get; private set; }

        // ── Inspector ────────────────────────────────────────────────────────

        [Header("— TIME —")]
        [SerializeField] private TimeOfDay  startTime    = TimeOfDay.Afternoon;
        [SerializeField] private float      dayDuration  = 600f;   // seconds per full day
        [SerializeField] private bool       autoAdvance  = true;

        [Header("— WEATHER —")]
        [SerializeField] private WeatherType startWeather = WeatherType.Clear;
        [SerializeField] private float       weatherChangeInterval = 180f;

        [Header("— SEASON —")]
        [SerializeField] private Season startSeason = Season.Autumn;

        [Header("— LIGHTING REFERENCES —")]
        [SerializeField] private Light  sunLight;
        [SerializeField] private float  morningIntensity   = 0.6f;
        [SerializeField] private float  afternoonIntensity = 1.0f;
        [SerializeField] private float  sunsetIntensity    = 0.7f;
        [SerializeField] private float  nightIntensity     = 0.15f;

        [Header("— LIGHTING COLORS —")]
        [SerializeField] private Color morningColor   = new Color(0.95f, 0.90f, 0.80f);
        [SerializeField] private Color afternoonColor = new Color(1.00f, 0.98f, 0.92f);
        [SerializeField] private Color sunsetColor    = new Color(1.00f, 0.65f, 0.40f);
        [SerializeField] private Color nightColor     = new Color(0.20f, 0.22f, 0.40f);

        [Header("— WEATHER FX —")]
        [SerializeField] private ParticleSystem rainParticles;
        [SerializeField] private ParticleSystem fogParticles;

        // ── State ────────────────────────────────────────────────────────────

        public TimeOfDay  CurrentTime    { get; private set; }
        public WeatherType CurrentWeather { get; private set; }
        public Season     CurrentSeason  { get; private set; }

        private float _dayTimer;
        private float _weatherTimer;

        // ── Events ───────────────────────────────────────────────────────────

        public event Action<TimeOfDay>   OnTimeChanged;
        public event Action<WeatherType> OnWeatherChanged;
        public event Action<Season>      OnSeasonChanged;

        // ── Unity ────────────────────────────────────────────────────────────

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            CurrentTime    = startTime;
            CurrentWeather = startWeather;
            CurrentSeason  = startSeason;

            ApplyTimeOfDay(CurrentTime, instant: true);
            ApplyWeather(CurrentWeather, instant: true);

            if (autoAdvance)
                StartCoroutine(DayCycleRoutine());

            StartCoroutine(WeatherCycleRoutine());
        }

        // ── Public API ───────────────────────────────────────────────────────

        /// <summary>Force a specific time of day (e.g. from ChapterDirector).</summary>
        public void SetTime(TimeOfDay time)
        {
            if (CurrentTime == time) return;
            CurrentTime = time;
            StartCoroutine(TransitionLighting(time));
            OnTimeChanged?.Invoke(time);
            ApplyWorldPerceptionModifiers();
            Debug.Log($"[World] Time → {time}");
        }

        public void SetWeather(WeatherType weather)
        {
            if (CurrentWeather == weather) return;
            CurrentWeather = weather;
            ApplyWeather(weather);
            OnWeatherChanged?.Invoke(weather);
            ApplyWorldPerceptionModifiers();
            Debug.Log($"[World] Weather → {weather}");
        }

        public void SetSeason(Season season)
        {
            if (CurrentSeason == season) return;
            CurrentSeason = season;
            OnSeasonChanged?.Invoke(season);
            ApplyWorldPerceptionModifiers();
            Debug.Log($"[World] Season → {season}");
        }

        /// <summary>
        /// Returns anomaly spawn multiplier for current world state.
        /// Night = 1.8x, Rain = 1.3x, Fog = 0.7x (harder to see), etc.
        /// </summary>
        public float GetAnomalyMultiplier()
        {
            float mult = 1f;

            // Time modifiers
            switch (CurrentTime)
            {
                case TimeOfDay.Night:     mult *= 1.8f; break;
                case TimeOfDay.Sunset:    mult *= 1.3f; break;
                case TimeOfDay.Morning:   mult *= 0.9f; break;
                case TimeOfDay.Afternoon: mult *= 1.0f; break;
            }

            // Weather modifiers
            switch (CurrentWeather)
            {
                case WeatherType.Rain:     mult *= 1.3f; break;  // SoundEcho easier
                case WeatherType.Fog:      mult *= 0.7f; break;  // harder to see
                case WeatherType.Overcast: mult *= 1.1f; break;
            }

            // Season modifiers
            switch (CurrentSeason)
            {
                case Season.Autumn: mult *= 1.2f; break;  // memory traces more frequent
                case Season.Winter: mult *= 0.9f; break;  // stability bonus
            }

            return mult;
        }

        /// <summary>
        /// Returns Stability bonus from current world state.
        /// Winter gives +10% stability.
        /// </summary>
        public float GetStabilityBonus()
        {
            float bonus = 0f;
            if (CurrentSeason == Season.Winter)  bonus += 0.10f;
            if (CurrentWeather == WeatherType.Clear) bonus += 0.05f;
            return bonus;
        }

        // ── Private ──────────────────────────────────────────────────────────

        private IEnumerator DayCycleRoutine()
        {
            float timePerPhase = dayDuration / 4f;

            while (true)
            {
                yield return new WaitForSeconds(timePerPhase);
                AdvanceTime();
            }
        }

        private IEnumerator WeatherCycleRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(weatherChangeInterval);
                if (UnityEngine.Random.value < 0.4f)
                    SetWeather(GetNextWeather());
            }
        }

        private void AdvanceTime()
        {
            TimeOfDay next = (TimeOfDay)(((int)CurrentTime + 1) % 4);
            SetTime(next);
        }

        private WeatherType GetNextWeather()
        {
            // Weighted random — Clear most common
            float r = UnityEngine.Random.value;
            if (r < 0.50f) return WeatherType.Clear;
            if (r < 0.75f) return WeatherType.Overcast;
            if (r < 0.90f) return WeatherType.Rain;
            return WeatherType.Fog;
        }

        private void ApplyTimeOfDay(TimeOfDay time, bool instant = false)
        {
            if (instant)
            {
                SetLighting(GetTargetIntensity(time), GetTargetColor(time));
            }
            else
            {
                StartCoroutine(TransitionLighting(time));
            }
        }

        private IEnumerator TransitionLighting(TimeOfDay time)
        {
            if (sunLight == null) yield break;

            float targetIntensity = GetTargetIntensity(time);
            Color targetColor     = GetTargetColor(time);
            float elapsed = 0f;
            float duration = 8f;

            float startIntensity = sunLight.intensity;
            Color startColor     = sunLight.color;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                sunLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);
                sunLight.color     = Color.Lerp(startColor, targetColor, t);
                yield return null;
            }

            SetLighting(targetIntensity, targetColor);
        }

        private void SetLighting(float intensity, Color color)
        {
            if (sunLight == null) return;
            sunLight.intensity = intensity;
            sunLight.color     = color;
        }

        private float GetTargetIntensity(TimeOfDay time)
        {
            switch (time)
            {
                case TimeOfDay.Morning:   return morningIntensity;
                case TimeOfDay.Afternoon: return afternoonIntensity;
                case TimeOfDay.Sunset:    return sunsetIntensity;
                case TimeOfDay.Night:     return nightIntensity;
                default: return 1f;
            }
        }

        private Color GetTargetColor(TimeOfDay time)
        {
            switch (time)
            {
                case TimeOfDay.Morning:   return morningColor;
                case TimeOfDay.Afternoon: return afternoonColor;
                case TimeOfDay.Sunset:    return sunsetColor;
                case TimeOfDay.Night:     return nightColor;
                default: return Color.white;
            }
        }

        private void ApplyWeather(WeatherType weather, bool instant = false)
        {
            // Rain particles
            if (rainParticles != null)
            {
                if (weather == WeatherType.Rain)
                    rainParticles.Play();
                else
                    rainParticles.Stop();
            }

            // Fog particles
            if (fogParticles != null)
            {
                if (weather == WeatherType.Fog)
                    fogParticles.Play();
                else
                    fogParticles.Stop();
            }

            // Unity built-in fog
            RenderSettings.fog = (weather == WeatherType.Fog || weather == WeatherType.Overcast);
            RenderSettings.fogDensity = weather == WeatherType.Fog ? 0.04f : 0.01f;
        }

        private void ApplyWorldPerceptionModifiers()
        {
            var stats = GameManager.Instance?.playerStats;
            if (stats == null) return;

            // Winter stability bonus
            float stabilityBonus = GetStabilityBonus();
            if (stabilityBonus > 0f)
            {
                // Passive bonus — applied as a multiplier in PerceptionSystem
                Debug.Log($"[World] Stability bonus active: +{stabilityBonus * 100f:F0}%");
            }
        }
    }
}