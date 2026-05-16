using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ABTW.Core;

namespace ABTW.Systems
{
    /// <summary>
    /// Environment DNA — each room/space in the Academy has emotional and perceptual identity.
    /// Responds to player perception state: changes light, color, symbols, atmosphere.
    /// Core rule: "The Academy responds to perception, not force."
    /// </summary>
    public class EnvironmentDNA : MonoBehaviour
    {
        public enum RoomType
        {
            Corridor,       // Observation — transition space
            Classroom,      // Learning — structured warmth
            Library,        // Hidden truth — ancient, quiet
            Observatory,    // Wonder — cosmic awareness
            Dormitory,      // Belonging — emotional safety
            Garden,         // Reflection — calm, restorative
            Rooftop,        // Perspective — open, vast
            HiddenHall,     // Mystery — partially revealed
            OuterForest     // Unknown — partially explored
        }

        public enum PerceptionState
        {
            Dormant,        // No player attention
            Aware,          // Player is present
            Noticing,       // Player is actively perceiving
            Resonating,     // Trio synchronized
            Activated       // Full hidden layer visible
        }

        [Header("Room Identity")]
        public string roomID = "ROOM_001";
        public string roomName = "East Corridor";
        public RoomType roomType = RoomType.Corridor;
        [TextArea(2, 4)]
        public string emotionalDNA = "Observation. Transition. Subtle awareness.";

        [Header("Perception State")]
        [SerializeField] private PerceptionState currentState = PerceptionState.Dormant;
        public float perceptionDecayRate = 0.05f;  // how fast room returns to dormant
        public float perceptionBuildRate = 0.1f;   // how fast room responds to player

        [Header("Lighting DNA")]
        public Light[] roomLights;
        public Color dormantLightColor = new Color(0.8f, 0.75f, 0.65f);   // warm neutral
        public Color awareLightColor = new Color(0.85f, 0.82f, 0.75f);    // slightly brighter
        public Color noticingLightColor = new Color(0.7f, 0.8f, 1f);      // cool blue tint
        public Color resonatingLightColor = new Color(0.9f, 0.85f, 0.6f); // warm gold
        public Color activatedLightColor = new Color(1f, 0.95f, 0.8f);    // full warm gold
        public float lightTransitionSpeed = 1.5f;

        [Header("Atmosphere")]
        public ParticleSystem atmosphereParticles;
        public float dormantParticleRate = 2f;
        public float activatedParticleRate = 15f;

        [Header("Fog")]
        public bool controlFog = false;
        public float dormantFogDensity = 0.02f;
        public float activatedFogDensity = 0.005f;

        [Header("Symbol Slots")]
        public SymbolStateFlow[] embeddedSymbols;

        [Header("Sound")]
        public AudioSource ambientAudio;
        public AudioClip[] dormantAmbience;
        public AudioClip[] activatedAmbience;

        [Header("References")]
        public ActivationLogic activationLogic;

        [Header("Events")]
        public UnityEngine.Events.UnityEvent OnRoomActivated;
        public UnityEngine.Events.UnityEvent OnRoomDormant;
        public UnityEngine.Events.UnityEvent OnHiddenLayerRevealed;

        // Internal
        private float currentPerceptionLevel = 0f;
        private PerceptionState previousState = PerceptionState.Dormant;
        private Color targetLightColor;
        private bool hiddenLayerRevealed = false;

        // Room-type specific DNA profiles
        private static readonly Dictionary<RoomType, (float anomalyMultiplier, float symbolDensity, string atmosphere)>
            RoomProfiles = new Dictionary<RoomType, (float, float, string)>
        {
            { RoomType.Corridor,    (1.0f, 0.6f, "Cinematic. Larger than characters. Subtly too perfect.") },
            { RoomType.Classroom,   (0.6f, 0.4f, "Calm. Structured. Emotionally warm. Socially alive.") },
            { RoomType.Library,     (1.4f, 1.0f, "Intelligent. Quiet. Ancient. Emotionally weighted.") },
            { RoomType.Observatory, (0.8f, 0.8f, "Cool navy. Gold light. Layered silence. Distant stars.") },
            { RoomType.Dormitory,   (0.4f, 0.3f, "Personalized. Emotionally lived in. Layered with quiet detail.") },
            { RoomType.Garden,      (0.7f, 0.5f, "Peaceful. Intelligent. Emotionally alive. Symbolically layered.") },
            { RoomType.Rooftop,     (0.9f, 0.6f, "Perspective. Open. Vast. Emotionally clarifying.") },
            { RoomType.HiddenHall,  (1.8f, 1.2f, "Partially revealed. Ancient. Incomplete. Responsive.") },
            { RoomType.OuterForest, (1.2f, 0.7f, "Larger than understood. Partially hidden. Emotionally distant.") }
        };

        private void Start()
        {
            ApplyRoomDNA();
            SetLightColor(dormantLightColor, instant: true);
        }

        private void Update()
        {
            UpdatePerceptionLevel();
            UpdateLighting();
            UpdateAtmosphere();
        }

        // ─── Perception Level ────────────────────────────────────────────────

        private void UpdatePerceptionLevel()
        {
            // Decay toward dormant when no input
            currentPerceptionLevel = Mathf.MoveTowards(
                currentPerceptionLevel, 0f,
                perceptionDecayRate * Time.deltaTime);

            // Update state based on level
            PerceptionState newState = GetStateFromLevel(currentPerceptionLevel);
            if (newState != currentState)
            {
                TransitionToState(newState);
            }
        }

        private PerceptionState GetStateFromLevel(float level)
        {
            if (level >= 0.9f) return PerceptionState.Activated;
            if (level >= 0.7f) return PerceptionState.Resonating;
            if (level >= 0.4f) return PerceptionState.Noticing;
            if (level >= 0.1f) return PerceptionState.Aware;
            return PerceptionState.Dormant;
        }

        // ─── State Transitions ───────────────────────────────────────────────

        private void TransitionToState(PerceptionState newState)
        {
            previousState = currentState;
            currentState = newState;

            Debug.Log($"[EnvironmentDNA] {roomName}: {previousState} → {newState}");

            switch (newState)
            {
                case PerceptionState.Dormant:
                    targetLightColor = dormantLightColor;
                    OnRoomDormant?.Invoke();
                    break;

                case PerceptionState.Aware:
                    targetLightColor = awareLightColor;
                    break;

                case PerceptionState.Noticing:
                    targetLightColor = noticingLightColor;
                    ActivateNearbySymbols(SymbolStateFlow.SymbolState.Glitch);
                    break;

                case PerceptionState.Resonating:
                    targetLightColor = resonatingLightColor;
                    ActivateNearbySymbols(SymbolStateFlow.SymbolState.Forming);
                    break;

                case PerceptionState.Activated:
                    targetLightColor = activatedLightColor;
                    ActivateNearbySymbols(SymbolStateFlow.SymbolState.Activation);
                    OnRoomActivated?.Invoke();
                    if (!hiddenLayerRevealed)
                    {
                        hiddenLayerRevealed = true;
                        OnHiddenLayerRevealed?.Invoke();
                    }
                    break;
            }
        }

        // ─── Lighting ────────────────────────────────────────────────────────

        private void UpdateLighting()
        {
            if (roomLights == null) return;
            foreach (var light in roomLights)
            {
                if (light == null) continue;
                light.color = Color.Lerp(light.color, targetLightColor,
                    lightTransitionSpeed * Time.deltaTime);
            }
        }

        private void SetLightColor(Color color, bool instant = false)
        {
            targetLightColor = color;
            if (instant && roomLights != null)
            {
                foreach (var light in roomLights)
                    if (light != null) light.color = color;
            }
        }

        // ─── Atmosphere ──────────────────────────────────────────────────────

        private void UpdateAtmosphere()
        {
            if (atmosphereParticles == null) return;

            var emission = atmosphereParticles.emission;
            float targetRate = Mathf.Lerp(dormantParticleRate, activatedParticleRate,
                currentPerceptionLevel);
            emission.rateOverTime = targetRate;

            if (controlFog)
            {
                RenderSettings.fogDensity = Mathf.Lerp(
                    dormantFogDensity, activatedFogDensity, currentPerceptionLevel);
            }
        }

        // ─── Symbol Activation ───────────────────────────────────────────────

        private void ActivateNearbySymbols(SymbolStateFlow.SymbolState targetState)
        {
            if (embeddedSymbols == null) return;

            foreach (var symbol in embeddedSymbols)
            {
                if (symbol == null) continue;

                switch (targetState)
                {
                    case SymbolStateFlow.SymbolState.Glitch:
                        symbol.TriggerGlitch();
                        break;
                    case SymbolStateFlow.SymbolState.Forming:
                        symbol.TriggerForming();
                        break;
                    case SymbolStateFlow.SymbolState.Activation:
                        symbol.TriggerActivation();
                        break;
                }
            }
        }

        // ─── Room DNA Application ────────────────────────────────────────────

        private void ApplyRoomDNA()
        {
            if (!RoomProfiles.ContainsKey(roomType)) return;

            var profile = RoomProfiles[roomType];
            Debug.Log($"[EnvironmentDNA] {roomName} DNA loaded — " +
                      $"AnomalyMult: {profile.anomalyMultiplier}, " +
                      $"Atmosphere: {profile.atmosphere}");
        }

        public float GetAnomalyMultiplier()
        {
            return RoomProfiles.ContainsKey(roomType)
                ? RoomProfiles[roomType].anomalyMultiplier
                : 1f;
        }

        // ─── Public Interface ────────────────────────────────────────────────

        /// <summary>
        /// Called by PerceptionSystem when player enters and perceives.
        /// </summary>
        public void ReceivePerception(float amount)
        {
            currentPerceptionLevel = Mathf.Clamp01(
                currentPerceptionLevel + amount * perceptionBuildRate);
        }

        /// <summary>
        /// Called by TrioSystem on full synchronization.
        /// </summary>
        public void ReceiveTrioResonance()
        {
            currentPerceptionLevel = Mathf.Clamp01(currentPerceptionLevel + 0.4f);
        }

        public PerceptionState CurrentState => currentState;
        public float PerceptionLevel => currentPerceptionLevel;
        public bool IsHiddenLayerRevealed => hiddenLayerRevealed;

        // ─── Trigger Zone ────────────────────────────────────────────────────

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ReceivePerception(0.2f);
                Debug.Log($"[EnvironmentDNA] Player entered: {roomName}");
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ReceivePerception(0.01f * Time.deltaTime);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Color gizmoColor = currentState switch
            {
                PerceptionState.Activated  => Color.yellow,
                PerceptionState.Resonating => new Color(1f, 0.8f, 0f),
                PerceptionState.Noticing   => Color.cyan,
                PerceptionState.Aware      => Color.white,
                _                          => Color.gray
            };

            Gizmos.color = gizmoColor;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
    }
}