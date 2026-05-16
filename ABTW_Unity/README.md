# 🌌 Academy Beyond This World — Unity Prototype
### Version: 0.1 — Chapter 1: "The First Glitch"

> *"Reality is not fixed. It responds to how it is seen."*

---

## 📋 SPIS TREŚCI
1. [Wymagania](#wymagania)
2. [Instalacja](#instalacja)
3. [Struktura projektu](#struktura-projektu)
4. [Pierwsze uruchomienie](#pierwsze-uruchomienie)
5. [Sterowanie](#sterowanie)
6. [Systemy gry](#systemy-gry)
7. [Rozszerzanie projektu](#rozszerzanie-projektu)
8. [Znane problemy](#znane-problemy)

---

## ⚙️ WYMAGANIA

| Komponent | Minimalna wersja |
|-----------|-----------------|
| **Unity** | 2022.3 LTS (Long Term Support) |
| **C#** | .NET Standard 2.1 (wbudowany w Unity) |
| **System** | Windows 10/11, macOS 12+, Ubuntu 20.04+ |
| **RAM** | 8 GB (16 GB zalecane) |
| **GPU** | DirectX 11 / Metal / Vulkan |

> ✅ **Zalecana wersja Unity:** 2022.3.x LTS — pobierz z [unity.com/download](https://unity.com/download)

---

## 🚀 INSTALACJA

### Krok 1 — Pobierz Unity Hub
1. Wejdź na [unity.com/download](https://unity.com/download)
2. Pobierz i zainstaluj **Unity Hub**
3. Zaloguj się (konto Unity jest darmowe)

### Krok 2 — Zainstaluj Unity 2022.3 LTS
1. W Unity Hub → **Installs** → **Install Editor**
2. Wybierz **Unity 2022.3.x LTS**
3. Zaznacz moduły:
   - ✅ **Windows Build Support** (lub Mac/Linux zależnie od systemu)
   - ✅ **Visual Studio** (lub VS Code)
4. Kliknij **Install** i poczekaj (~15-30 min)

### Krok 3 — Otwórz projekt ABTW
1. W Unity Hub → **Projects** → **Open**
2. Nawiguj do folderu `ABTW_Unity/`
3. Kliknij **Open**
4. Unity zaimportuje projekt (~2-5 min przy pierwszym otwarciu)

### Krok 4 — Otwórz scenę Chapter 1
1. W Unity Editor → **Project** panel (dół ekranu)
2. Nawiguj do: `Assets/Scenes/`
3. Dwuklik na `Chapter1_TheFirstGlitch`

> 💡 **Jeśli scena nie istnieje:** Użyj menu **ABTW → Setup → Create Chapter 1 Scene Structure**

---

## 📁 STRUKTURA PROJEKTU

```
ABTW_Unity/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/                    # Główne systemy
│   │   │   ├── GameManager.cs       # Singleton — centrum gry
│   │   │   ├── PlayerMovement.cs    # Ruch Astry
│   │   │   ├── PlayerStats.cs       # 5 statystyk + rangi
│   │   │   ├── CameraController.cs  # Kamera 3rd person
│   │   │   ├── InputHandler.cs      # Klawisze
│   │   │   ├── SceneSetup.cs        # Auto-setup sceny
│   │   │   └── ChapterOneDirector.cs # Narracja Rozdziału 1
│   │   │
│   │   ├── Systems/                 # Mechaniki gry
│   │   │   ├── PerceptionSystem.cs  # Tryb percepcji (P)
│   │   │   ├── NotebookSystem.cs    # Notatnik (N)
│   │   │   ├── GlitchSystem.cs      # Efekty wizualne
│   │   │   ├── EchoSystem.cs        # System echa
│   │   │   ├── MemoryWalkSystem.cs  # Spacer po pamięci (M)
│   │   │   └── TrioSystem.cs        # System Trio
│   │   │
│   │   ├── Anomalies/               # Typy anomalii
│   │   │   ├── AnomalyBase.cs       # Klasa bazowa
│   │   │   ├── ShadowDelayAnomaly.cs
│   │   │   ├── LightInstabilityAnomaly.cs
│   │   │   ├── SymbolFragmentAnomaly.cs
│   │   │   └── ReflectionMismatchAnomaly.cs
│   │   │
│   │   ├── Companions/              # AI towarzysze
│   │   │   ├── LumiCompanion.cs     # Lumi — analiza
│   │   │   └── NelaCompanion.cs     # Nela — intuicja
│   │   │
│   │   └── UI/
│   │       └── UIManager.cs         # HUD, Notatnik, Powiadomienia
│   │
│   ├── Editor/
│   │   └── ABTWEditorTools.cs       # Narzędzia edytora
│   │
│   ├── Scenes/                      # Sceny Unity
│   ├── Resources/
│   │   ├── Prefabs/                 # Prefaby
│   │   ├── Materials/               # Materiały
│   │   ├── Audio/                   # Dźwięki
│   │   ├── Fonts/                   # Czcionki
│   │   └── Textures/                # Tekstury
│   └── Animations/                  # Animacje
│
├── ProjectSettings/
│   └── ProjectSettings.asset        # Ustawienia projektu
├── .gitignore
└── README.md                        ← jesteś tutaj
```

---

## ▶️ PIERWSZE URUCHOMIENIE

### Opcja A — Auto Setup (zalecane)
1. Otwórz Unity Editor
2. Menu górne → **ABTW → Setup → Create Chapter 1 Scene Structure**
3. Scena zostanie automatycznie zbudowana
4. Kliknij ▶️ **Play** w Unity Editor

### Opcja B — Ręczne ustawienie
1. Utwórz nową scenę: **File → New Scene**
2. Dodaj pusty GameObject → nazwij go `GameManager`
3. Dodaj komponenty: `GameManager`, `PerceptionSystem`, `NotebookSystem`, `UIManager`
4. Dodaj GameObject `Player` z tagiem `Player` + komponent `PlayerMovement`
5. Kliknij ▶️ **Play**

---

## 🎮 STEROWANIE

| Klawisz | Akcja |
|---------|-------|
| **WASD** / **Strzałki** | Ruch Astry |
| **P** | Włącz/wyłącz Tryb Percepcji |
| **N** | Otwórz/zamknij Notatnik |
| **M** | Wejdź w Memory Walk (Spacer po Pamięci) |
| **T** | Aktywuj Trio (jeśli warunki spełnione) |
| **E** | Interakcja z obiektem |
| **Esc** | Pauza / Menu |

---

## 🧠 SYSTEMY GRY

### 📊 System Statystyk (PlayerStats)
Astra posiada 5 statystyk rozwijanych przez obserwację:

| Statystyka | Funkcja | Jak zdobyć |
|-----------|---------|-----------|
| 👁 **Awareness** | Wykrywanie anomalii | Znajdowanie glitchy |
| 🧩 **Clarity** | Rozumienie wzorców | Zapisywanie w notatniku |
| 🔗 **Connection** | Łączenie elementów | Aktywacja Trio |
| ⏳ **Timing** | Właściwy moment | Czekanie zamiast zapisu |
| 🌀 **Stability** | Odporność na zniekształcenia | Memory Walk |

**Rangi (1-10):**
`Observer → Noticer → Pattern Seeker → Interpreter → Aligner → System Aware → Influence Initiate → Reality Reader → System Shifter → Beyond Observer`

---

### 📓 System Notatnika (NotebookSystem)
Serce mechaniki gry. Naciśnij **N** aby otworzyć.

**Decyzje przy każdej anomalii:**

| Opcja | Efekt | Konsekwencja |
|-------|-------|-------------|
| **RECORD** | +Clarity, +EXP, anomalia znika | Zamknięcie głębszej ścieżki |
| **WAIT** | Wzorzec ewoluuje | Możliwość aktywacji Trio |
| **DEEP RECORD** | Pełny zapis (po Trio) | Maksymalny EXP |
| **REVISIT** | Memory Walk do tej anomalii | Tylko obserwacja |

---

### 🔁 System Echa (EchoSystem)
Jeśli gracz zapisał anomalię zbyt wcześnie, po czasie pojawi się jej **echo** — słabsza, niepełna wersja.

```
Notebook: "Echo Detected — This is not the original pattern. A trace of something recorded too early."
```

---

### 🧠 Memory Walk (MemoryWalkSystem)
Naciśnij **M** aby wejść w tryb wspomnień:
- Czas zwalnia do 50%
- Oświetlenie staje się desaturowane
- Gracz może **obserwować** przeszłe zdarzenia
- Gracz **NIE może** zmieniać decyzji
- Wyjście zawsze daje +Stability

---

### 🤝 System Trio (TrioSystem)
Aktywuje się gdy:
1. Astra obserwuje anomalię
2. Lumi zakończy analizę (auto, ~8 sekund)
3. Nela wyczuje moment (auto, timing)
4. Gracz czeka ≥3 sekundy bez klikania

**Efekt:** Pojawia się ukryta warstwa rzeczywistości, symbol, specjalna nagroda.

---

## 🛠️ ROZSZERZANIE PROJEKTU

### Dodanie nowej anomalii
1. Utwórz nowy skrypt dziedziczący po `AnomalyBase`:
```csharp
public class MyAnomaly : AnomalyBase
{
    protected override void OnDetected() { /* co się dzieje po wykryciu */ }
    protected override void OnEvolve()   { /* ewolucja po WAIT */ }
    protected override void OnResolve()  { /* rozwiązanie */ }
    protected override PatternData BuildPatternData() { return new PatternData { ... }; }
}
```
2. Dodaj nowy typ do enum `AnomalyType` w `NotebookSystem.cs`
3. Umieść obiekt w scenie i dodaj komponent

### Dodanie nowej sceny/rozdziału
1. Utwórz nową scenę Unity
2. Dodaj `SceneSetup.cs` do pustego GameObject
3. Skonfiguruj referencje w Inspectorze
4. Utwórz nowy `ChapterDirector` dziedziczący wzorzec z `ChapterOneDirector.cs`

### Narzędzia edytora
Menu **ABTW** w Unity Editor zawiera:
- `Setup → Create Chapter 1 Scene Structure` — auto-budowanie sceny
- `Setup → Create Folder Structure` — tworzenie folderów
- `Debug → Log All Anomalies` — lista anomalii w scenie
- `Debug → Simulate Trio Activation` — test Trio
- `Debug → Add Max EXP` — test statystyk
- `Open ABTW Dashboard` — panel narzędzi

---

## ⚠️ ZNANE PROBLEMY

| Problem | Rozwiązanie |
|---------|------------|
| Błąd kompilacji przy pierwszym otwarciu | Poczekaj na zakończenie importu Unity |
| `GameManager.Instance` jest null | Upewnij się że GameManager jest w scenie |
| Brak UI w grze | Sprawdź czy UIManager ma przypisany Canvas |
| Anomalie nie są wykrywane | Sprawdź czy PerceptionSystem jest aktywny (P) |
| Trio nie aktywuje się | Lumi i Nela muszą być w scenie jako GameObject |

---

## 📞 KONTAKT / DOKUMENTACJA

- **World Bible:** `ABTW LEGENDARY MASTER WORLD BIBLE v1.0.docx`
- **Characters Bible:** `ABTW LEGENDARY MASTER CHARACTERS BIBLE v1.0.docx`
- **Systems Bible:** `ABWT LEGENDARY MASTER SYSTEMS BIBLE v1.0.docx`
- **GDD (Game Design Document):** zawarty w dokumentacji projektu

---

## 🔒 CORE DESIGN RULES (nie zmieniaj!)

```
❌ Brak walki jako core mechanic
✅ Percepcja > Siła
✅ Decyzje zmieniają świat
❌ Brak resetów "na klik"
✅ Echo zamiast cofania
✅ Notebook = decyzja
✅ Trio = pełne zrozumienie
```

> *"This is not a game about winning.*
> *This is a game about seeing differently."*

---

*ABTW Prototype v0.1 — Academy Beyond This World © 2025*