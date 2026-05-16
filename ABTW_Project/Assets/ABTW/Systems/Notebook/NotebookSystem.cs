using UnityEngine;
using System.Collections.Generic;
using System;
using ABTW.Core;

namespace ABTW.Systems
{
    /// <summary>
    /// The Notebook — heart of ABTW.
    /// NOT a quest log. NOT a UI tool.
    /// A reality-affecting system that stores decisions and changes the world.
    /// </summary>
    public class NotebookSystem : MonoBehaviour
    {
        [Header("— STATE —")]
        [SerializeField] private bool isUnlocked = false;

        // All patterns ever observed
        private readonly List<PatternData> _patterns = new();

        // Active pattern awaiting decision
        private PatternData _pendingPattern;

        // Events
        public event Action<PatternData>  OnPatternAdded;
        public event Action<PatternData>  OnPatternRecorded;
        public event Action<PatternData>  OnPatternWaited;
        public event Action               OnNotebookUnlocked;
        public event Action<PatternData>  OnDeepRecordAvailable;

        // ── Public API ───────────────────────────────────────────────────────

        /// <summary>Unlock notebook — happens after first anomaly found.</summary>
        public void UnlockNotebook()
        {
            if (isUnlocked) return;
            isUnlocked = true;
            OnNotebookUnlocked?.Invoke();
            Debug.Log("[Notebook] Unlocked.");
        }

        /// <summary>Add a newly observed pattern to the notebook.</summary>
        public void AddPattern(PatternData pattern)
        {
            if (!isUnlocked) return;

            pattern.status = PatternStatus.Unrecorded;
            pattern.observedAt = Time.time;
            _patterns.Add(pattern);
            _pendingPattern = pattern;

            OnPatternAdded?.Invoke(pattern);
            Debug.Log($"[Notebook] Pattern added: {pattern.name}");
        }

        /// <summary>Player chooses RECORD — stabilises reality, closes deeper path.</summary>
        public void Record()
        {
            if (_pendingPattern == null) return;

            _pendingPattern.status = PatternStatus.Recorded;
            _pendingPattern.recordedBeforeTrio = !TrioSystem.WasTrioActiveRecently;
            _pendingPattern.recordedAt = Time.time;

            OnPatternRecorded?.Invoke(_pendingPattern);
            GameManager.Instance?.OnPatternRecorded(_pendingPattern);

            Debug.Log($"[Notebook] RECORD: {_pendingPattern.name} " +
                      $"(beforeTrio={_pendingPattern.recordedBeforeTrio})");

            _pendingPattern = null;
        }

        /// <summary>Player chooses WAIT — pattern evolves, deeper layers open.</summary>
        public void Wait()
        {
            if (_pendingPattern == null) return;

            _pendingPattern.status = PatternStatus.Evolving;
            _pendingPattern.waitStartTime = Time.time;

            OnPatternWaited?.Invoke(_pendingPattern);
            GameManager.Instance?.OnPatternWaited(_pendingPattern);

            Debug.Log($"[Notebook] WAIT: {_pendingPattern.name}");
            // Pattern stays pending — Trio system will pick it up
        }

        /// <summary>Deep Record — only available after Trio activation.</summary>
        public void DeepRecord()
        {
            if (_pendingPattern == null) return;

            _pendingPattern.status = PatternStatus.DeepRecorded;
            _pendingPattern.hasDeepRecord = true;
            _pendingPattern.recordedAt = Time.time;

            // Deep record gives full stats
            var stats = GameManager.Instance?.playerStats;
            stats?.RaiseStat(StatType.Clarity,    5f);
            stats?.RaiseStat(StatType.Connection, 3f);
            stats?.RaiseStat(StatType.Stability,  2f);
            GameManager.Instance?.playerStats.AddEXP(40);

            OnPatternRecorded?.Invoke(_pendingPattern);
            Debug.Log($"[Notebook] DEEP RECORD: {_pendingPattern.name}");

            _pendingPattern = null;
        }

        /// <summary>Mark a pattern as an Echo (incomplete, recorded too early).</summary>
        public void MarkAsEcho(PatternData pattern)
        {
            pattern.status = PatternStatus.Echo;
            OnPatternAdded?.Invoke(pattern);
            Debug.Log($"[Notebook] Echo detected: {pattern.name}");
        }

        // ── Queries ──────────────────────────────────────────────────────────

        public bool IsUnlocked => isUnlocked;
        public PatternData PendingPattern => _pendingPattern;
        public IReadOnlyList<PatternData> AllPatterns => _patterns;

        public List<PatternData> GetByStatus(PatternStatus status)
        {
            return _patterns.FindAll(p => p.status == status);
        }

        public bool HasPendingDecision => _pendingPattern != null &&
            (_pendingPattern.status == PatternStatus.Unrecorded ||
             _pendingPattern.status == PatternStatus.Evolving);
    }

    // ── Data Types ───────────────────────────────────────────────────────────

    [Serializable]
    public class PatternData
    {
        public string        name;
        public string        description;
        public AnomalyType   anomalyType;
        public PatternStatus status;

        [Header("Lumi & Nela notes")]
        public string lumiNote;
        public string nelaNote;

        [Header("Observations")]
        public List<string> observations = new();

        [Header("EXP")]
        public int expOnRecord = 25;
        public int expOnDeep   = 40;

        [Header("Runtime")]
        public float observedAt;
        public float recordedAt;
        public float waitStartTime;
        public bool  recordedBeforeTrio;
        public bool  hasDeepRecord;
        public bool  echoScheduled;
    }

    public enum PatternStatus
    {
        Unrecorded,   // observed, awaiting decision
        Evolving,     // player chose Wait
        Recorded,     // player chose Record
        DeepRecorded, // recorded after Trio
        Echo          // incomplete, recorded too early
    }

    public enum AnomalyType
    {
        ShadowDelay,
        LightInstability,
        SymbolFragment,
        ReflectionMismatch,
        SoundEcho,
        SpaceFold,
        MemoryTrace,
        SystemPulse
    }
}