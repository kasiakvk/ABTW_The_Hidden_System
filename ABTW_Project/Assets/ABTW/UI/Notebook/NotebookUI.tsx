import React, { useState, useEffect, useCallback } from "react";

// ─── Types ────────────────────────────────────────────────────────────────────

type PatternStatus = "Unrecorded" | "Recorded" | "Echo" | "DeepRecord";
type NotebookTab = "PATTERNS" | "SYMBOLS" | "CONNECTIONS" | "MEMORY";

interface Observation {
  text: string;
  timestamp?: string;
}

interface NotebookEntry {
  id: string;
  patternName: string;
  status: PatternStatus;
  location: string;
  observations: Observation[];
  lumiNote?: string;
  nelaNote?: string;
  echoNote?: string;
  isEcho: boolean;
  canRecord: boolean;
  canWait: boolean;
  canDeepRecord: boolean;
  recordedAt?: string;
}

interface SymbolEntry {
  id: string;
  name: string;
  category: "Layer1_Anomaly" | "Layer2_Symbol" | "Layer3_Hidden";
  location: string;
  meaning?: string;
  discovered: boolean;
  activated: boolean;
}

interface ConnectionEntry {
  id: string;
  elementA: string;
  elementB: string;
  connectionType: string;
  strength: number; // 0–1
  discovered: boolean;
}

interface MemoryEntry {
  id: string;
  title: string;
  description: string;
  canRevisit: boolean;
  isDistorted: boolean;
  timestamp: string;
}

interface NotebookUIProps {
  entries?: NotebookEntry[];
  symbols?: SymbolEntry[];
  connections?: ConnectionEntry[];
  memories?: MemoryEntry[];
  trioActive?: boolean;
  onRecord?: (entryId: string) => void;
  onWait?: (entryId: string) => void;
  onDeepRecord?: (entryId: string) => void;
  onRevisitMemory?: (memoryId: string) => void;
  onClose?: () => void;
}

// ─── Status Badge ─────────────────────────────────────────────────────────────

const StatusBadge: React.FC<{ status: PatternStatus }> = ({ status }) => {
  const config: Record<PatternStatus, { color: string; dot: string; label: string }> = {
    Unrecorded: { color: "#8899CC", dot: "#6677AA", label: "Unrecorded" },
    Recorded:   { color: "#88BB88", dot: "#66AA66", label: "Recorded" },
    Echo:       { color: "#AA88BB", dot: "#886699", label: "Echo (Incomplete)" },
    DeepRecord: { color: "#DDBB44", dot: "#CCAA22", label: "Deep Record" },
  };
  const c = config[status];
  return (
    <span style={{ display: "inline-flex", alignItems: "center", gap: 6, fontSize: 12, color: c.color }}>
      <span style={{
        width: 8, height: 8, borderRadius: "50%",
        background: c.dot, display: "inline-block",
        boxShadow: `0 0 6px ${c.dot}`
      }} />
      {c.label}
    </span>
  );
};

// ─── Pattern Entry Card ───────────────────────────────────────────────────────

const PatternCard: React.FC<{
  entry: NotebookEntry;
  trioActive: boolean;
  onRecord: () => void;
  onWait: () => void;
  onDeepRecord: () => void;
}> = ({ entry, trioActive, onRecord, onWait, onDeepRecord }) => {
  const [expanded, setExpanded] = useState(false);

  return (
    <div style={{
      background: "rgba(255,255,255,0.04)",
      border: "1px solid rgba(180,190,220,0.15)",
      borderRadius: 8,
      padding: "14px 16px",
      marginBottom: 10,
      cursor: "pointer",
      transition: "border-color 0.3s",
    }}
      onClick={() => setExpanded(e => !e)}
    >
      {/* Header */}
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
        <span style={{ color: "#D0D8F0", fontFamily: "Georgia, serif", fontSize: 14, fontWeight: 600 }}>
          {entry.isEcho ? "⟳ " : ""}{entry.patternName}
        </span>
        <StatusBadge status={entry.status} />
      </div>

      <div style={{ color: "#8899BB", fontSize: 11, marginTop: 3 }}>
        📍 {entry.location}
      </div>

      {/* Expanded content */}
      {expanded && (
        <div style={{ marginTop: 12 }}>
          {/* Observations */}
          <div style={{ marginBottom: 10 }}>
            <div style={{ color: "#9AAABB", fontSize: 11, marginBottom: 4, letterSpacing: 1, textTransform: "uppercase" }}>
              Observed
            </div>
            {entry.observations.map((obs, i) => (
              <div key={i} style={{ color: "#B0BBCC", fontSize: 12, paddingLeft: 10, marginBottom: 2 }}>
                — {obs.text}
              </div>
            ))}
          </div>

          {/* Companion notes */}
          {entry.lumiNote && (
            <div style={{ marginBottom: 6 }}>
              <span style={{ color: "#88AADD", fontSize: 11 }}>Lumi: </span>
              <span style={{ color: "#AABBCC", fontSize: 12, fontStyle: "italic" }}>"{entry.lumiNote}"</span>
            </div>
          )}
          {entry.nelaNote && (
            <div style={{ marginBottom: 6 }}>
              <span style={{ color: "#DDAA88", fontSize: 11 }}>Nela: </span>
              <span style={{ color: "#AABBCC", fontSize: 12, fontStyle: "italic" }}>"{entry.nelaNote}"</span>
            </div>
          )}

          {/* Echo note */}
          {entry.isEcho && entry.echoNote && (
            <div style={{
              background: "rgba(150,100,180,0.1)",
              border: "1px solid rgba(150,100,180,0.2)",
              borderRadius: 6,
              padding: "8px 10px",
              marginBottom: 10,
              color: "#BB99CC",
              fontSize: 12,
              fontStyle: "italic"
            }}>
              {entry.echoNote}
            </div>
          )}

          {/* Action buttons */}
          {entry.status === "Unrecorded" && (
            <div style={{ display: "flex", gap: 8, marginTop: 12 }}>
              {entry.canRecord && (
                <ActionButton
                  label="RECORD"
                  color="#6688AA"
                  hoverColor="#88AACC"
                  onClick={(e) => { e.stopPropagation(); onRecord(); }}
                />
              )}
              {entry.canWait && (
                <ActionButton
                  label="WAIT"
                  color="#557755"
                  hoverColor="#779977"
                  onClick={(e) => { e.stopPropagation(); onWait(); }}
                />
              )}
              {entry.canDeepRecord && trioActive && (
                <ActionButton
                  label="DEEP RECORD"
                  color="#AA8833"
                  hoverColor="#CCAA44"
                  onClick={(e) => { e.stopPropagation(); onDeepRecord(); }}
                  glow
                />
              )}
            </div>
          )}

          {entry.status === "Echo" && (
            <div style={{ display: "flex", gap: 8, marginTop: 12 }}>
              <ActionButton label="VIEW MEMORY" color="#886699" hoverColor="#AA88BB"
                onClick={(e) => { e.stopPropagation(); }} />
              <ActionButton label="ACCEPT" color="#556677" hoverColor="#778899"
                onClick={(e) => { e.stopPropagation(); }} />
            </div>
          )}
        </div>
      )}
    </div>
  );
};

// ─── Action Button ────────────────────────────────────────────────────────────

const ActionButton: React.FC<{
  label: string;
  color: string;
  hoverColor: string;
  onClick: (e: React.MouseEvent) => void;
  glow?: boolean;
}> = ({ label, color, hoverColor, onClick, glow }) => {
  const [hovered, setHovered] = useState(false);
  return (
    <button
      onClick={onClick}
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={() => setHovered(false)}
      style={{
        background: "transparent",
        border: `1px solid ${hovered ? hoverColor : color}`,
        color: hovered ? hoverColor : color,
        borderRadius: 4,
        padding: "6px 14px",
        fontSize: 11,
        letterSpacing: 1.5,
        cursor: "pointer",
        transition: "all 0.2s",
        boxShadow: glow && hovered ? `0 0 12px ${hoverColor}55` : "none",
        fontFamily: "Georgia, serif",
      }}
    >
      {label}
    </button>
  );
};

// ─── Memory Walk Panel ────────────────────────────────────────────────────────

const MemoryPanel: React.FC<{
  memories: MemoryEntry[];
  onRevisit: (id: string) => void;
}> = ({ memories, onRevisit }) => (
  <div>
    {memories.length === 0 && (
      <div style={{ color: "#667788", fontSize: 13, textAlign: "center", marginTop: 30 }}>
        No memories recorded yet.
      </div>
    )}
    {memories.map(mem => (
      <div key={mem.id} style={{
        background: "rgba(255,255,255,0.03)",
        border: "1px solid rgba(180,190,220,0.12)",
        borderRadius: 8,
        padding: "12px 14px",
        marginBottom: 10,
      }}>
        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
          <span style={{ color: mem.isDistorted ? "#AA99BB" : "#C0C8E0", fontSize: 13 }}>
            {mem.isDistorted ? "⟳ " : ""}{mem.title}
          </span>
          <span style={{ color: "#556677", fontSize: 10 }}>{mem.timestamp}</span>
        </div>
        <div style={{ color: "#8899AA", fontSize: 12, marginTop: 4 }}>{mem.description}</div>
        {mem.canRevisit && (
          <div style={{ marginTop: 10 }}>
            <ActionButton label="REVISIT" color="#667788" hoverColor="#8899AA"
              onClick={() => onRevisit(mem.id)} />
          </div>
        )}
        {mem.isDistorted && (
          <div style={{ color: "#776688", fontSize: 11, marginTop: 6, fontStyle: "italic" }}>
            Memory Reconstruction — This is not the original moment.
          </div>
        )}
      </div>
    ))}
  </div>
);

// ─── Main NotebookUI Component ────────────────────────────────────────────────

const NotebookUI: React.FC<NotebookUIProps> = ({
  entries = [],
  symbols = [],
  connections = [],
  memories = [],
  trioActive = false,
  onRecord,
  onWait,
  onDeepRecord,
  onRevisitMemory,
  onClose,
}) => {
  const [activeTab, setActiveTab] = useState<NotebookTab>("PATTERNS");
  const [visible, setVisible] = useState(true);

  const tabs: NotebookTab[] = ["PATTERNS", "SYMBOLS", "CONNECTIONS", "MEMORY"];

  const handleClose = useCallback(() => {
    setVisible(false);
    onClose?.();
  }, [onClose]);

  if (!visible) return null;

  return (
    <div style={{
      position: "fixed",
      top: "50%",
      left: "50%",
      transform: "translate(-50%, -50%)",
      width: 420,
      maxHeight: "80vh",
      background: "linear-gradient(160deg, #0D1220 0%, #111828 100%)",
      border: "1px solid rgba(180,200,240,0.18)",
      borderRadius: 12,
      boxShadow: "0 8px 40px rgba(0,0,0,0.7), 0 0 60px rgba(80,100,180,0.08)",
      display: "flex",
      flexDirection: "column",
      fontFamily: "Georgia, serif",
      zIndex: 1000,
      overflow: "hidden",
    }}>

      {/* Header */}
      <div style={{
        padding: "16px 20px 12px",
        borderBottom: "1px solid rgba(180,200,240,0.1)",
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
      }}>
        <div>
          <div style={{ color: "#C8D4F0", fontSize: 15, letterSpacing: 2, textTransform: "uppercase" }}>
            📓 Astra's Notebook
          </div>
          {trioActive && (
            <div style={{ color: "#DDBB44", fontSize: 10, marginTop: 2, letterSpacing: 1 }}>
              ✦ TRIO SYNCHRONIZED
            </div>
          )}
        </div>
        <button onClick={handleClose} style={{
          background: "none", border: "none", color: "#556677",
          fontSize: 18, cursor: "pointer", lineHeight: 1,
        }}>✕</button>
      </div>

      {/* Tabs */}
      <div style={{
        display: "flex",
        borderBottom: "1px solid rgba(180,200,240,0.08)",
        padding: "0 8px",
      }}>
        {tabs.map(tab => (
          <button key={tab} onClick={() => setActiveTab(tab)} style={{
            flex: 1,
            background: "none",
            border: "none",
            borderBottom: activeTab === tab
              ? "2px solid #8899CC"
              : "2px solid transparent",
            color: activeTab === tab ? "#B0BBDD" : "#556677",
            fontSize: 10,
            letterSpacing: 1.2,
            padding: "10px 4px",
            cursor: "pointer",
            transition: "all 0.2s",
          }}>
            {tab}
          </button>
        ))}
      </div>

      {/* Content */}
      <div style={{ flex: 1, overflowY: "auto", padding: "14px 16px" }}>

        {activeTab === "PATTERNS" && (
          <div>
            {entries.length === 0 && (
              <div style={{ color: "#667788", fontSize: 13, textAlign: "center", marginTop: 30 }}>
                Nothing recorded yet.<br />
                <span style={{ fontSize: 11, color: "#445566" }}>Keep observing.</span>
              </div>
            )}
            {entries.map(entry => (
              <PatternCard
                key={entry.id}
                entry={entry}
                trioActive={trioActive}
                onRecord={() => onRecord?.(entry.id)}
                onWait={() => onWait?.(entry.id)}
                onDeepRecord={() => onDeepRecord?.(entry.id)}
              />
            ))}
          </div>
        )}

        {activeTab === "SYMBOLS" && (
          <div>
            {symbols.map(sym => (
              <div key={sym.id} style={{
                background: "rgba(255,255,255,0.03)",
                border: `1px solid ${sym.activated ? "rgba(220,180,60,0.25)" : "rgba(180,190,220,0.1)"}`,
                borderRadius: 8,
                padding: "10px 14px",
                marginBottom: 8,
              }}>
                <div style={{ display: "flex", justifyContent: "space-between" }}>
                  <span style={{ color: sym.activated ? "#DDBB44" : "#A0AABB", fontSize: 13 }}>
                    {sym.name}
                  </span>
                  <span style={{ color: "#556677", fontSize: 10 }}>{sym.category}</span>
                </div>
                <div style={{ color: "#667788", fontSize: 11, marginTop: 3 }}>📍 {sym.location}</div>
                {sym.meaning && sym.activated && (
                  <div style={{ color: "#8899AA", fontSize: 12, marginTop: 6, fontStyle: "italic" }}>
                    {sym.meaning}
                  </div>
                )}
              </div>
            ))}
          </div>
        )}

        {activeTab === "CONNECTIONS" && (
          <div>
            {connections.map(conn => (
              <div key={conn.id} style={{
                background: "rgba(255,255,255,0.03)",
                border: "1px solid rgba(180,190,220,0.1)",
                borderRadius: 8,
                padding: "10px 14px",
                marginBottom: 8,
              }}>
                <div style={{ color: "#A0AABB", fontSize: 13 }}>
                  {conn.elementA} <span style={{ color: "#556677" }}>↔</span> {conn.elementB}
                </div>
                <div style={{ display: "flex", justifyContent: "space-between", marginTop: 4 }}>
                  <span style={{ color: "#667788", fontSize: 11 }}>{conn.connectionType}</span>
                  <span style={{ color: "#8899AA", fontSize: 11 }}>
                    {"▓".repeat(Math.round(conn.strength * 5))}{"░".repeat(5 - Math.round(conn.strength * 5))}
                  </span>
                </div>
              </div>
            ))}
          </div>
        )}

        {activeTab === "MEMORY" && (
          <MemoryPanel memories={memories} onRevisit={(id) => onRevisitMemory?.(id)} />
        )}
      </div>

      {/* Footer */}
      <div style={{
        padding: "10px 16px",
        borderTop: "1px solid rgba(180,200,240,0.08)",
        color: "#445566",
        fontSize: 10,
        letterSpacing: 0.8,
        textAlign: "center",
      }}>
        {activeTab === "PATTERNS" && `${entries.filter(e => e.status === "Recorded").length} / ${entries.length} recorded`}
        {activeTab === "SYMBOLS" && `${symbols.filter(s => s.discovered).length} / ${symbols.length} discovered`}
        {activeTab === "CONNECTIONS" && `${connections.filter(c => c.discovered).length} connections found`}
        {activeTab === "MEMORY" && `${memories.length} memory traces`}
      </div>
    </div>
  );
};

export default NotebookUI;
export type { NotebookEntry, SymbolEntry, ConnectionEntry, MemoryEntry, NotebookUIProps };