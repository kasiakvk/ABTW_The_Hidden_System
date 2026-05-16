using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ABTW.Core;

namespace ABTW.UI
{
    /// <summary>
    /// Symbol HUD — displays active symbols, their states, and perception indicators.
    /// Minimal, non-intrusive design. Fades in only when relevant.
    /// </summary>
    public class SymbolHUD : MonoBehaviour
    {
        [System.Serializable]
        public class SymbolSlot
        {
            public string symbolID;
            public Image symbolIcon;
            public Image stateIndicator;
            public Text symbolLabel;
            public CanvasGroup slotGroup;
            public Animator slotAnimator;
        }

        [Header("HUD Layout")]
        public CanvasGroup hudGroup;
        public float hudFadeSpeed = 2f;
        public float hudAutoHideDelay = 4f;
        public bool autoHide = true;

        [Header("Symbol Slots")]
        public List<SymbolSlot> symbolSlots = new List<SymbolSlot>();
        public int maxVisibleSymbols = 4;

        [Header("State Colors")]
        public Color hiddenColor     = new Color(0.3f, 0.3f, 0.3f, 0.3f);
        public Color glitchColor     = new Color(0.4f, 0.4f, 1f, 0.8f);
        public Color formingColor    = new Color(0.6f, 0.8f, 1f, 0.9f);
        public Color activationColor = new Color(1f, 0.95f, 0.6f, 1f);
        public Color resolvedColor   = new Color(0.5f, 1f, 0.5f, 0.8f);
        public Color echoColor       = new Color(0.6f, 0.6f, 0.6f, 0.5f);

        [Header("Perception Ring")]
        public Image perceptionRing;
        public Image perceptionRingFill;
        public Color perceptionActiveColor = new Color(0.6f, 0.85f, 1f, 0.9f);
        public Color perceptionInactiveColor = new Color(0.4f, 0.4f, 0.4f, 0.4f);

        [Header("Activation Indicator")]
        public CanvasGroup activationIndicator;
        public Text activationLayerText;
        public Image activationLayerIcon;

        [Header("Trio Status")]
        public Image[] trioStatusDots; // 0=Astra, 1=Lumi, 2=Nela
        public Color trioActiveColor   = new Color(1f, 0.9f, 0.5f, 1f);
        public Color trioInactiveColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

        // Internal state
        private Dictionary<string, SymbolStateFlow> trackedSymbols
            = new Dictionary<string, SymbolStateFlow>();
        private float autoHideTimer = 0f;
        private bool hudVisible = false;
        private Coroutine hideCoroutine;

        // Animator hashes
        private static readonly int HashGlitch    = Animator.StringToHash("Glitch");
        private static readonly int HashActivate  = Animator.StringToHash("Activate");
        private static readonly int HashResolve   = Animator.StringToHash("Resolve");

        private void Start()
        {
            if (hudGroup != null) hudGroup.alpha = 0f;
            if (activationIndicator != null) activationIndicator.alpha = 0f;
            SetTrioStatus(false, false, false);
        }

        private void Update()
        {
            if (autoHide && hudVisible)
            {
                autoHideTimer -= Time.deltaTime;
                if (autoHideTimer <= 0f)
                    HideHUD();
            }
        }

        // ─── Symbol Registration ─────────────────────────────────────────────

        public void RegisterSymbol(SymbolStateFlow symbol)
        {
            if (symbol == null || trackedSymbols.ContainsKey(symbol.symbolID)) return;

            trackedSymbols[symbol.symbolID] = symbol;

            // Subscribe to state changes
            symbol.OnGlitchStart.AddListener(() => OnSymbolStateChanged(symbol.symbolID,
                SymbolStateFlow.SymbolState.Glitch));
            symbol.OnForming.AddListener(() => OnSymbolStateChanged(symbol.symbolID,
                SymbolStateFlow.SymbolState.Forming));
            symbol.OnActivation.AddListener(() => OnSymbolStateChanged(symbol.symbolID,
                SymbolStateFlow.SymbolState.Activation));
            symbol.OnResolved.AddListener(() => OnSymbolStateChanged(symbol.symbolID,
                SymbolStateFlow.SymbolState.Resolved));
            symbol.OnEcho.AddListener(() => OnSymbolStateChanged(symbol.symbolID,
                SymbolStateFlow.SymbolState.Echo));

            Debug.Log($"[SymbolHUD] Registered symbol: {symbol.symbolName}");
        }

        public void UnregisterSymbol(string symbolID)
        {
            trackedSymbols.Remove(symbolID);
        }

        // ─── State Updates ───────────────────────────────────────────────────

        private void OnSymbolStateChanged(string symbolID, SymbolStateFlow.SymbolState newState)
        {
            ShowHUD();
            UpdateSymbolSlot(symbolID, newState);

            // Special handling for activation
            if (newState == SymbolStateFlow.SymbolState.Activation)
                ShowActivationIndicator(symbolID);
        }

        private void UpdateSymbolSlot(string symbolID, SymbolStateFlow.SymbolState state)
        {
            // Find or assign a slot
            int slotIndex = GetSlotIndex(symbolID);
            if (slotIndex < 0 || slotIndex >= symbolSlots.Count) return;

            var slot = symbolSlots[slotIndex];
            if (slot == null) return;

            // Update color
            Color stateColor = GetStateColor(state);
            if (slot.stateIndicator != null)
                slot.stateIndicator.color = stateColor;

            // Update label
            if (slot.symbolLabel != null && trackedSymbols.ContainsKey(symbolID))
                slot.symbolLabel.text = trackedSymbols[symbolID].symbolName;

            // Trigger animation
            if (slot.slotAnimator != null)
            {
                switch (state)
                {
                    case SymbolStateFlow.SymbolState.Glitch:
                        slot.slotAnimator.SetTrigger(HashGlitch); break;
                    case SymbolStateFlow.SymbolState.Activation:
                        slot.slotAnimator.SetTrigger(HashActivate); break;
                    case SymbolStateFlow.SymbolState.Resolved:
                        slot.slotAnimator.SetTrigger(HashResolve); break;
                }
            }

            // Fade in slot
            if (slot.slotGroup != null)
                StartCoroutine(FadeCanvasGroup(slot.slotGroup, 1f, 0.3f));
        }

        private int GetSlotIndex(string symbolID)
        {
            // Check if already assigned
            for (int i = 0; i < symbolSlots.Count; i++)
            {
                if (symbolSlots[i].symbolID == symbolID) return i;
            }

            // Assign new slot
            for (int i = 0; i < Mathf.Min(symbolSlots.Count, maxVisibleSymbols); i++)
            {
                if (string.IsNullOrEmpty(symbolSlots[i].symbolID))
                {
                    symbolSlots[i].symbolID = symbolID;
                    return i;
                }
            }

            return -1; // No slot available
        }

        // ─── Perception Ring ─────────────────────────────────────────────────

        public void UpdatePerceptionRing(float fillAmount, bool isActive)
        {
            if (perceptionRingFill != null)
                perceptionRingFill.fillAmount = fillAmount;

            if (perceptionRing != null)
                perceptionRing.color = isActive ? perceptionActiveColor : perceptionInactiveColor;
        }

        // ─── Activation Indicator ────────────────────────────────────────────

        private void ShowActivationIndicator(string symbolID)
        {
            if (activationIndicator == null) return;

            if (activationLayerText != null && trackedSymbols.ContainsKey(symbolID))
                activationLayerText.text = trackedSymbols[symbolID].symbolName.ToUpper();

            StartCoroutine(FlashActivationIndicator());
        }

        private IEnumerator FlashActivationIndicator()
        {
            if (activationIndicator == null) yield break;

            yield return StartCoroutine(FadeCanvasGroup(activationIndicator, 1f, 0.3f));
            yield return new WaitForSeconds(2f);
            yield return StartCoroutine(FadeCanvasGroup(activationIndicator, 0f, 0.8f));
        }

        // ─── Activation Layer Display ────────────────────────────────────────

        public void UpdateActivationLayer(ActivationLogic.ActivationLayer layer)
        {
            if (activationLayerText == null) return;

            string layerText = layer switch
            {
                ActivationLogic.ActivationLayer.None        => "",
                ActivationLogic.ActivationLayer.Attention   => "ATTENTION",
                ActivationLogic.ActivationLayer.Understanding => "UNDERSTANDING",
                ActivationLogic.ActivationLayer.Alignment   => "ALIGNMENT",
                ActivationLogic.ActivationLayer.Full        => "FULL ACTIVATION",
                _ => ""
            };

            activationLayerText.text = layerText;

            if (!string.IsNullOrEmpty(layerText))
                ShowHUD();
        }

        // ─── Trio Status ─────────────────────────────────────────────────────

        public void SetTrioStatus(bool astraReady, bool lumiReady, bool nelaReady)
        {
            if (trioStatusDots == null || trioStatusDots.Length < 3) return;

            bool[] states = { astraReady, lumiReady, nelaReady };
            for (int i = 0; i < 3 && i < trioStatusDots.Length; i++)
            {
                if (trioStatusDots[i] != null)
                    trioStatusDots[i].color = states[i] ? trioActiveColor : trioInactiveColor;
            }
        }

        // ─── HUD Visibility ──────────────────────────────────────────────────

        public void ShowHUD()
        {
            if (hudVisible) { autoHideTimer = hudAutoHideDelay; return; }

            hudVisible = true;
            autoHideTimer = hudAutoHideDelay;

            if (hideCoroutine != null) StopCoroutine(hideCoroutine);
            StartCoroutine(FadeCanvasGroup(hudGroup, 1f, 0.4f));
        }

        public void HideHUD()
        {
            if (!hudVisible) return;
            hudVisible = false;
            hideCoroutine = StartCoroutine(FadeCanvasGroup(hudGroup, 0f, 1f));
        }

        public void ForceShowHUD() { autoHide = false; ShowHUD(); }
        public void ForceHideHUD() { autoHide = true; HideHUD(); }

        // ─── Utilities ───────────────────────────────────────────────────────

        private Color GetStateColor(SymbolStateFlow.SymbolState state)
        {
            return state switch
            {
                SymbolStateFlow.SymbolState.Hidden     => hiddenColor,
                SymbolStateFlow.SymbolState.Glitch     => glitchColor,
                SymbolStateFlow.SymbolState.Forming    => formingColor,
                SymbolStateFlow.SymbolState.Activation => activationColor,
                SymbolStateFlow.SymbolState.Resolved   => resolvedColor,
                SymbolStateFlow.SymbolState.Echo       => echoColor,
                _ => hiddenColor
            };
        }

        private IEnumerator FadeCanvasGroup(CanvasGroup group, float target, float duration)
        {
            if (group == null) yield break;

            float start = group.alpha;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                group.alpha = Mathf.Lerp(start, target, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            group.alpha = target;
            group.interactable = target > 0.5f;
            group.blocksRaycasts = target > 0.5f;
        }
    }
}