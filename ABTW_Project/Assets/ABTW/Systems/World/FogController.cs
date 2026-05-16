using UnityEngine;
using System.Collections;

namespace ABTW.Systems
{
    /// <summary>
    /// ABTW Fog Controller — atmospheric fog that responds to perception state.
    /// Fog is never decorative — it is a perception mechanic.
    /// More fog = harder to notice anomalies, but hidden layers more likely.
    /// </summary>
    public class FogController : MonoBehaviour
    {
        public enum FogState
        {
            Clear,          // No fog — full visibility
            Subtle,         // Light atmospheric haze
            Atmospheric,    // Normal Academy atmosphere
            Dense,          // Heavy fog — perception challenge
            Perception,     // Player in perception mode — fog clears slightly
            HiddenLayer     // Hidden layer active — ethereal mist
        }

        [Header("Fog Settings")]
        public bool controlSceneFog = true;
        public FogMode fogMode = FogMode.Exponential;
        public float transitionSpeed = 1.0f;

        [Header("Fog State Presets")]
        public FogState currentState = FogState.Atmospheric;

        [Header("Density Values")]
        public float clearDensity       = 0.000f;
        public float subtleDensity      = 0.005f;
        public float atmosphericDensity = 0.012f;
        public float denseDensity       = 0.035f;
        public float perceptionDensity  = 0.006f;
        public float hiddenLayerDensity = 0.003f;

        [Header("Color Values")]
        public Color clearColor       = new Color(0.85f, 0.88f, 0.92f);
        public Color subtleColor      = new Color(0.82f, 0.85f, 0.90f);
        public Color atmosphericColor = new Color(0.75f, 0.80f, 0.88f);
        public Color denseColor       = new Color(0.60f, 0.65f, 0.72f);
        public Color perceptionColor  = new Color(0.70f, 0.82f, 1.00f);
        public Color hiddenLayerColor = new Color(0.90f, 0.92f, 1.00f);

        [Header("Perception Response")]
        public bool respondToPerception = true;
        public float perceptionClearFactor = 0.4f; // how much perception clears fog

        [Header("Dynamic Fog")]
        public bool enableDynamicPulse = false;
        public float pulseSpeed = 0.3f;
        public float pulseAmplitude = 0.002f;

        // Internal
        private float currentDensity;
        private float targetDensity;
        private Color currentColor;
        private Color targetColor;
        private bool isTransitioning = false;
        private Coroutine transitionCoroutine;

        // Fog state before perception override
        private FogState baseState = FogState.Atmospheric;
        private bool perceptionOverrideActive = false;

        private void Start()
        {
            if (controlSceneFog)
            {
                RenderSettings.fog = true;
                RenderSettings.fogMode = fogMode;
            }

            ApplyStateInstant(currentState);
            baseState = currentState;
        }

        private void Update()
        {
            if (enableDynamicPulse && !isTransitioning)
                ApplyDynamicPulse();
        }

        // ─── State Control ───────────────────────────────────────────────────

        public void SetState(FogState newState, bool instant = false)
        {
            baseState = newState;
            ApplyEffectiveState(instant);
        }

        public void SetPerceptionOverride(bool active, float perceptionLevel = 1f)
        {
            if (!respondToPerception) return;

            perceptionOverrideActive = active;

            if (active)
            {
                // Partially clear fog based on perception level
                float overrideDensity = Mathf.Lerp(
                    GetDensityForState(baseState),
                    perceptionDensity,
                    perceptionLevel * perceptionClearFactor);

                StartTransition(overrideDensity, perceptionColor);
            }
            else
            {
                ApplyEffectiveState(false);
            }
        }

        public void SetHiddenLayerActive(bool active)
        {
            if (active)
                StartTransition(hiddenLayerDensity, hiddenLayerColor);
            else
                ApplyEffectiveState(false);
        }

        private void ApplyEffectiveState(bool instant)
        {
            FogState effective = perceptionOverrideActive ? FogState.Perception : baseState;
            currentState = effective;

            if (instant)
                ApplyStateInstant(effective);
            else
                StartTransitionToState(effective);
        }

        // ─── Transitions ─────────────────────────────────────────────────────

        private void StartTransitionToState(FogState state)
        {
            StartTransition(GetDensityForState(state), GetColorForState(state));
        }

        private void StartTransition(float density, Color color)
        {
            targetDensity = density;
            targetColor = color;

            if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
            transitionCoroutine = StartCoroutine(TransitionRoutine());
        }

        private IEnumerator TransitionRoutine()
        {
            isTransitioning = true;

            float startDensity = RenderSettings.fogDensity;
            Color startColor = RenderSettings.fogColor;
            float elapsed = 0f;
            float duration = 1f / transitionSpeed;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float smoothT = Mathf.SmoothStep(0f, 1f, t);

                if (controlSceneFog)
                {
                    RenderSettings.fogDensity = Mathf.Lerp(startDensity, targetDensity, smoothT);
                    RenderSettings.fogColor = Color.Lerp(startColor, targetColor, smoothT);
                }

                currentDensity = RenderSettings.fogDensity;
                currentColor = RenderSettings.fogColor;

                elapsed += Time.deltaTime;
                yield return null;
            }

            if (controlSceneFog)
            {
                RenderSettings.fogDensity = targetDensity;
                RenderSettings.fogColor = targetColor;
            }

            isTransitioning = false;
        }

        private void ApplyStateInstant(FogState state)
        {
            float density = GetDensityForState(state);
            Color color = GetColorForState(state);

            if (controlSceneFog)
            {
                RenderSettings.fogDensity = density;
                RenderSettings.fogColor = color;
                RenderSettings.fog = density > 0f;
            }

            currentDensity = density;
            currentColor = color;
            targetDensity = density;
            targetColor = color;
        }

        // ─── Dynamic Pulse ───────────────────────────────────────────────────

        private void ApplyDynamicPulse()
        {
            if (!controlSceneFog) return;

            float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseAmplitude;
            RenderSettings.fogDensity = Mathf.Max(0f, currentDensity + pulse);
        }

        // ─── WorldManager Integration ────────────────────────────────────────

        public void OnWeatherChanged(WorldManager.WeatherType weather)
        {
            FogState newState = weather switch
            {
                WorldManager.WeatherType.Clear    => FogState.Subtle,
                WorldManager.WeatherType.Overcast => FogState.Atmospheric,
                WorldManager.WeatherType.Rain     => FogState.Atmospheric,
                WorldManager.WeatherType.Fog      => FogState.Dense,
                _ => FogState.Atmospheric
            };
            SetState(newState);
        }

        public void OnTimeOfDayChanged(WorldManager.TimeOfDay time)
        {
            // Night adds more atmospheric fog
            if (time == WorldManager.TimeOfDay.Night && currentState != FogState.Dense)
            {
                float nightBoost = GetDensityForState(currentState) * 1.3f;
                StartTransition(nightBoost, GetColorForState(currentState));
            }
        }

        // ─── Helpers ─────────────────────────────────────────────────────────

        private float GetDensityForState(FogState state) => state switch
        {
            FogState.Clear       => clearDensity,
            FogState.Subtle      => subtleDensity,
            FogState.Atmospheric => atmosphericDensity,
            FogState.Dense       => denseDensity,
            FogState.Perception  => perceptionDensity,
            FogState.HiddenLayer => hiddenLayerDensity,
            _ => atmosphericDensity
        };

        private Color GetColorForState(FogState state) => state switch
        {
            FogState.Clear       => clearColor,
            FogState.Subtle      => subtleColor,
            FogState.Atmospheric => atmosphericColor,
            FogState.Dense       => denseColor,
            FogState.Perception  => perceptionColor,
            FogState.HiddenLayer => hiddenLayerColor,
            _ => atmosphericColor
        };

        public float CurrentDensity => currentDensity;
        public FogState CurrentState => currentState;
        public bool IsTransitioning => isTransitioning;
    }
}