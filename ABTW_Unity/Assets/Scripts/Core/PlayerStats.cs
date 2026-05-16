using UnityEngine;
using System;

namespace ABTW.Core
{
    /// <summary>
    /// Five perception stats that drive all game mechanics.
    /// Stats grow through observation and understanding — never through combat.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "ABTW/Player Stats")]
    public class PlayerStats : ScriptableObject
    {
        [Header("— PERCEPTION STATS —")]
        [Range(0, 100)] public float awareness   = 0f;  // detect anomalies
        [Range(0, 100)] public float clarity     = 0f;  // understand patterns
        [Range(0, 100)] public float connection  = 0f;  // link elements
        [Range(0, 100)] public float timing      = 0f;  // correct moment
        [Range(0, 100)] public float stability   = 0f;  // resist distortion

        [Header("— PROGRESSION —")]
        public int   totalEXP    = 0;
        public int   currentRank = 1;   // 1 = Observer … 10 = Beyond Observer

        // EXP thresholds per rank (index 0 = rank 1→2, etc.)
        private static readonly int[] RankThresholds =
            { 100, 250, 500, 900, 1400, 2000, 2800, 3800, 5000 };

        public static readonly string[] RankNames =
        {
            "Observer", "Noticer", "Pattern Seeker", "Interpreter",
            "Aligner", "System Aware", "Influence Initiate",
            "Reality Reader", "System Shifter", "Beyond Observer"
        };

        // Events
        public event Action<int>   OnRankUp;
        public event Action<float> OnStatChanged;

        // ── Public API ──────────────────────────────────────────────────────

        public void AddEXP(int amount)
        {
            totalEXP += amount;
            CheckRankUp();
        }

        public void RaiseStat(StatType stat, float amount)
        {
            switch (stat)
            {
                case StatType.Awareness:  awareness  = Mathf.Clamp(awareness  + amount, 0, 100); break;
                case StatType.Clarity:    clarity    = Mathf.Clamp(clarity    + amount, 0, 100); break;
                case StatType.Connection: connection = Mathf.Clamp(connection + amount, 0, 100); break;
                case StatType.Timing:     timing     = Mathf.Clamp(timing     + amount, 0, 100); break;
                case StatType.Stability:  stability  = Mathf.Clamp(stability  + amount, 0, 100); break;
            }
            OnStatChanged?.Invoke(amount);
        }

        public string CurrentRankName =>
            currentRank <= RankNames.Length ? RankNames[currentRank - 1] : "Beyond Observer";

        public float PerceptionModeMaxDuration => 8f + (stability / 100f) * 22f; // 8–30 sec

        public float AnomalyDetectionRadius => 3f + (awareness / 100f) * 12f;   // 3–15 units

        // ── Private ─────────────────────────────────────────────────────────

        private void CheckRankUp()
        {
            if (currentRank >= 10) return;
            int threshold = RankThresholds[currentRank - 1];
            if (totalEXP >= threshold)
            {
                currentRank++;
                OnRankUp?.Invoke(currentRank);
                Debug.Log($"[ABTW] Rank Up → {CurrentRankName}");
            }
        }

        public void Reset()
        {
            awareness = clarity = connection = timing = stability = 0f;
            totalEXP = 0;
            currentRank = 1;
        }
    }

    public enum StatType { Awareness, Clarity, Connection, Timing, Stability }
}