using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ABTW.Systems
{
    /// <summary>
    /// ABTW Lighting Manager — controls all atmospheric lighting.
    /// Soft light, no harsh shadows, subtle transitions.
    /// Responds to: time of day, weather, perception state, anomaly presence.
    /// </summary>
    public class LightingManager : MonoBehaviour
    {
        public enum LightingPreset
        {
            Morning,        // Quiet awareness — soft warm light
            Afternoon,      // Warm social rhythm — golden
            Sunset,         // Transition — amber, emotional shift
            Night,          // Perception depth — deep navy, silver
            Rain,           // Reflection — desaturated, cool
            Fog,            // Instability — diffused, pale
            PerceptionMode, // Player in perception mode — slight blue tint
            TrioActivation, // Trio synchronized — warm gold pulse
            HiddenLayer     // Hidden layer revealed — ethereal white-gold
        }

        [Header("Directional Light")]
        public Light directionalLight;
        public float transitionSpeed = 1.2f;

        [Header("Ambient")]
        public bool controlAmbient = true;
        public float ambientIntensityBase = 0.4f;

        [Header("Lighting Presets")]
        public LightingPreset currentPreset = LightingPreset.Morning;
        [SerializeField] private LightingPreset targetPreset = LightingPreset.Morning;

        [Header("Preset Override")]
        public bool allowPerceptionOverride = true;
        public bool allowTrioOverride = true;

        [Header("Post Processing")]
        public bool controlColorGrading = false;
        // Color grading values (used if post-processing is available)
        [Range(-1f, 1f)] public float colorTemperature = 0f;
        [Range(0f, 2f)] public float saturation = 1f;
        [Range(0f, 2f)] public float contrast = 1f;

        // Preset definitions
        private struct LightPresetData
        {
            public Color lightColor;
            public float lightIntensity;
            public Color ambientColor;
            public float ambientIntensity;
            public Color fogColor;
            public float fogDensity;
            public bool enableFog;
        }

        private static readonly Dictionary<LightingPreset, LightPresetData> Presets
            = new Dictionary<LightingPreset, LightPresetData>
        {
            [LightingPreset.Morning] = new LightPresetData {
                lightColor      = new Color(1f, 0.95f, 0.85f),
                lightIntensity  = 0.8f,
                ambientColor    = new Color(0.6f, 0.65f, 0.75f),
                ambientIntensity = 0.5f,
                fogColor        = new Color(0.85f, 0.88f, 0.92f),
                fogDensity      = 0.008f,
                enableFog       = true
            },
            [LightingPreset.Afternoon] = new LightPresetData {
                lightColor      = new Color(1f, 0.92f, 0.7f),
                lightIntensity  = 1.0f,
                ambientColor    = new Color(0.65f, 0.6f, 0.5f),
                ambientIntensity = 0.55f,
                fogColor        = new Color(0.9f, 0.85f, 0.75f),
                fogDensity      = 0.005f,
                enableFog       = false
            },
            [LightingPreset.Sunset] = new LightPresetData {
                lightColor      = new Color(1f, 0.6f, 0.3f),
                lightIntensity  = 0.7f,
                ambientColor    = new Color(0.5f, 0.35f, 0.3f),
                ambientIntensity = 0.4f,
                fogColor        = new Color(0.8f, 0.5f, 0.3f),
                fogDensity      = 0.012f,
                enableFog       = true
            },
            [LightingPreset.Night] = new LightPresetData {
                lightColor      = new Color(0.3f, 0.35f, 0.6f),
                lightIntensity  = 0.3f,
                ambientColor    = new Color(0.1f, 0.12f, 0.25f),
                ambientIntensity = 0.2f,
                fogColor        = new Color(0.05f, 0.05f, 0.15f),
                fogDensity      = 0.02f,
                enableFog       = true
            },
            [LightingPreset.Rain] = new LightPresetData {
                lightColor      = new Color(0.7f, 0.75f, 0.85f),
                lightIntensity  = 0.5f,
                ambientColor    = new Color(0.4f, 0.45f, 0.55f),
                ambientIntensity = 0.35f,
                fogColor        = new Color(0.6f, 0.65f, 0.7f),
                fogDensity      = 0.025f,
                enableFog       = true
            },
            [LightingPreset.Fog] = new LightPresetData {
                lightColor      = new Color(0.85f, 0.85f, 0.9f),
                lightIntensity  = 0.4f,
                ambientColor    = new Color(0.55f, 0.55f, 0.6f),
                ambientIntensity = 0.45f,
                fogColor        = new Color(0.8f, 0.82f, 0.85f),
                fogDensity      = 0.04f,
                enableFog       = true
            },
            [LightingPreset.PerceptionMode] = new LightPresetData {
                lightColor      = new Color(0.75f, 0.85f, 1f),
                lightIntensity  = 0.85f,
                ambientColor    = new Color(0.3f, 0.4f, 0.6f),
                ambientIntensity = 0.5f,
                fogColor        = new Color(0.5f, 0.6f, 0.8f),
                fogDensity      = 0.01f,
                enableFog       = true
            },
            [LightingPreset.TrioActivation] = new LightPresetData {
                lightColor      = new Color(1f, 0.95f, 0.7f),
                lightIntensity  = 1.2f,
                ambientColor    = new Color(0.7f, 0.65f, 0.4f),
                ambientIntensity = 0.7f,
                fogColor        = new Color(0.9f, 0.85f, 0.6f),
                fogDensity      = 0.003f,
                enableFog       = true
            },
            [LightingPreset.HiddenLayer] = new LightPresetData {
                lightColor      = new Color(0.95f, 0.98f, 1f),
                lightIntensity  = 1.4f,
                ambientColor    = new Color(0.8f, 0.82f, 0.9f),
                ambientIntensity = 0.8f,
                fogColor        = new Color(0.9f, 0.92f, 1f),
                fogDensity      = 0.002f,
                enableFog       = true
            }
        };

        // Transition state
        private LightPresetData currentData;
        private LightPresetData targetData;
        private float transitionProgress = 1f;
        private Coroutine transitionCoroutine;

        // Override stack
        private LightingPreset? perceptionOverride = null;
        private LightingPreset? trioOverride = null;
        private LightingPreset basePreset = LightingPreset.Morning;

        private void Start()
        {
            if (directionalLight == null)
                directionalLight = FindObjectOfType<Light>();

            ApplyPresetInstant(currentPreset);
            basePreset = currentPreset;
        }

        private void Update()
        {
            if (transitionProgress < 1f)
                ApplyInterpolatedLighting();
        }

        // ─── Preset Control ──────────────────────────────────────────────────

        public void SetPreset(LightingPreset preset, bool instant = false)
        {
            basePreset = preset;
            ApplyEffectivePreset(instant);
        }

        public void SetPerceptionOverride(bool active)
        {
            if (!allowPerceptionOverride) return;
            perceptionOverride = active ? LightingPreset.PerceptionMode : (LightingPreset?)null;
            ApplyEffectivePreset(false);
        }

        public void SetTrioOverride(bool active)
        {
            if (!allowTrioOverride) return;
            trioOverride = active ? LightingPreset.TrioActivation : (LightingPreset?)null;
            ApplyEffectivePreset(false);
        }

        public void SetHiddenLayerReveal(bool active)
        {
            if (active)
                StartCoroutine(HiddenLayerRevealRoutine());
        }

        private void ApplyEffectivePreset(bool instant)
        {
            // Priority: TrioOverride > PerceptionOverride > Base
            LightingPreset effective = trioOverride ?? perceptionOverride ?? basePreset;

            if (effective == currentPreset && !instant) return;

            currentPreset = effective;

            if (instant)
                ApplyPresetInstant(effective);
            else
                StartTransition(effective);
        }

        // ─── Transitions ─────────────────────────────────────────────────────

        private void StartTransition(LightingPreset preset)
        {
            if (!Presets.ContainsKey(preset)) return;

            currentData = GetCurrentLightData();
            targetData = Presets[preset];
            transitionProgress = 0f;
            targetPreset = preset;
        }

        private void ApplyInterpolatedLighting()
        {
            transitionProgress = Mathf.MoveTowards(transitionProgress, 1f,
                transitionSpeed * Time.deltaTime);

            float t = transitionProgress;

            if (directionalLight != null)
            {
                directionalLight.color = Color.Lerp(currentData.lightColor, targetData.lightColor, t);
                directionalLight.intensity = Mathf.Lerp(currentData.lightIntensity, targetData.lightIntensity, t);
            }

            if (controlAmbient)
            {
                RenderSettings.ambientLight = Color.Lerp(currentData.ambientColor, targetData.ambientColor, t);
                RenderSettings.ambientIntensity = Mathf.Lerp(currentData.ambientIntensity, targetData.ambientIntensity, t);
            }

            RenderSettings.fogColor = Color.Lerp(currentData.fogColor, targetData.fogColor, t);
            RenderSettings.fogDensity = Mathf.Lerp(currentData.fogDensity, targetData.fogDensity, t);

            if (transitionProgress >= 1f)
                RenderSettings.fog = targetData.enableFog;
        }

        private void ApplyPresetInstant(LightingPreset preset)
        {
            if (!Presets.ContainsKey(preset)) return;
            var data = Presets[preset];

            if (directionalLight != null)
            {
                directionalLight.color = data.lightColor;
                directionalLight.intensity = data.lightIntensity;
            }

            if (controlAmbient)
            {
                RenderSettings.ambientLight = data.ambientColor;
                RenderSettings.ambientIntensity = data.ambientIntensity;
            }

            RenderSettings.fogColor = data.fogColor;
            RenderSettings.fogDensity = data.fogDensity;
            RenderSettings.fog = data.enableFog;

            transitionProgress = 1f;
        }

        private LightPresetData GetCurrentLightData()
        {
            return new LightPresetData
            {
                lightColor       = directionalLight != null ? directionalLight.color : Color.white,
                lightIntensity   = directionalLight != null ? directionalLight.intensity : 1f,
                ambientColor     = RenderSettings.ambientLight,
                ambientIntensity = RenderSettings.ambientIntensity,
                fogColor         = RenderSettings.fogColor,
                fogDensity       = RenderSettings.fogDensity,
                enableFog        = RenderSettings.fog
            };
        }

        // ─── Special Effects ─────────────────────────────────────────────────

        private IEnumerator HiddenLayerRevealRoutine()
        {
            // Brief flash to hidden layer, then settle
            StartTransition(LightingPreset.HiddenLayer);
            yield return new WaitForSeconds(3f);
            // Return to base with slight elevation
            StartTransition(basePreset);
        }

        public void PulseLight(float duration = 0.5f, float intensity = 1.5f)
        {
            StartCoroutine(PulseLightRoutine(duration, intensity));
        }

        private IEnumerator PulseLightRoutine(float duration, float intensity)
        {
            if (directionalLight == null) yield break;

            float baseIntensity = directionalLight.intensity;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float pulse = Mathf.Sin(t * Mathf.PI);
                directionalLight.intensity = Mathf.Lerp(baseIntensity, intensity, pulse);
                elapsed += Time.deltaTime;
                yield return null;
            }

            directionalLight.intensity = baseIntensity;
        }

        // ─── WorldManager Integration ────────────────────────────────────────

        public void OnTimeOfDayChanged(WorldManager.TimeOfDay time)
        {
            LightingPreset preset = time switch
            {
                WorldManager.TimeOfDay.Morning   => LightingPreset.Morning,
                WorldManager.TimeOfDay.Afternoon => LightingPreset.Afternoon,
                WorldManager.TimeOfDay.Sunset    => LightingPreset.Sunset,
                WorldManager.TimeOfDay.Night     => LightingPreset.Night,
                _                                => LightingPreset.Morning
            };
            SetPreset(preset);
        }

        public void OnWeatherChanged(WorldManager.WeatherType weather)
        {
            if (weather == WorldManager.WeatherType.Rain)
                SetPreset(LightingPreset.Rain);
            else if (weather == WorldManager.WeatherType.Fog)
                SetPreset(LightingPreset.Fog);
        }
    }
}