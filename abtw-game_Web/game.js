
// ═══════════════════════════════════════════════════════════
//  ABTW — THE HIDDEN SYSTEM: PART I  |  Game Engine v2
//  Now with real ABTW artwork as scene backgrounds
// ═══════════════════════════════════════════════════════════

const Game = {
  currentScene: 0,
  perceptionLevel: 0,
  perceptionActive: false,
  anomaliesFound: { corridor: 0, archive: 0, observatory: 0 },
  dialogueQueue: [],
  dialogueIndex: 0,
  isDialogueRunning: false,
  dialogueOnComplete: null,
  stats: { totalAnomalies: 0, perceptionActivations: 0, choicesMade: 0 },
  _typeInterval: null,
  _floatInterval: null,
  _starPulseInterval: null,
  _archiveFound: {},
  _obsStarsFound: [],
  _obsStarPositions: [],
  _archiveHotspots: [],
  _corridorAnomaliesRevealed: false,
  _archiveAnomaliesRevealed: false,

  init() {
    this.buildStars('title-stars', 140);
    document.addEventListener('keydown', (e) => {
      if (e.code === 'Space' || e.code === 'Enter') this.advanceDialogue();
    });
  },

  buildStars(containerId, count) {
    const container = document.getElementById(containerId);
    if (!container) return;
    container.innerHTML = '';
    for (let i = 0; i < count; i++) {
      const star = document.createElement('div');
      star.className = 'star';
      const size = Math.random() * 2.5 + 0.5;
      star.style.cssText = `width:${size}px;height:${size}px;left:${Math.random()*100}%;top:${Math.random()*100}%;--dur:${2+Math.random()*4}s;--delay:${Math.random()*5}s;--min-op:${0.1+Math.random()*0.2};--max-op:${0.5+Math.random()*0.5};`;
      container.appendChild(star);
    }
  },

  showScreen(id) {
    const overlay = document.getElementById('transition-overlay');
    overlay.classList.add('fade-in');
    setTimeout(() => {
      document.querySelectorAll('.screen').forEach(s => s.classList.remove('active'));
      document.getElementById(id).classList.add('active');
      overlay.classList.remove('fade-in');
    }, 800);
  },

  start() {
    this.showScreen('screen-game');
    setTimeout(() => this.loadScene(0), 900);
  },

  loadScene(index) {
    this.currentScene = index;
    this.perceptionActive = false;
    this._corridorAnomaliesRevealed = false;
    this._archiveAnomaliesRevealed = false;
    clearInterval(this._floatInterval);
    clearInterval(this._starPulseInterval);

    const sd = SCENES[index];
    document.querySelectorAll('.scene').forEach(s => s.classList.remove('active'));
    document.getElementById('scene-name').textContent = sd.sceneName;
    this.hideDialogue();
    this.hideChoices();
    const btn = document.getElementById('perception-btn');
    btn.classList.remove('active');
    this.updateFoundCounter(sd.id, 0, sd.anomaliesTotal);

    const sceneEl = document.getElementById('scene-' + sd.id);
    if (sceneEl) sceneEl.classList.add('active');

    if (sd.id === 'corridor')     this.initCorridor();
    if (sd.id === 'archive')      this.initArchive();
    if (sd.id === 'observatory')  this.initObservatory();

    setTimeout(() => this.showSceneIntro(sd), 400);
  },

  showSceneIntro(sd) {
    document.getElementById('scene-intro-number').textContent = sd.number;
    document.getElementById('scene-intro-title').textContent  = sd.title;
    document.getElementById('scene-intro-desc').textContent   = sd.desc;
    document.getElementById('scene-intro').classList.add('visible');
  },

  dismissSceneIntro() {
    document.getElementById('scene-intro').classList.remove('visible');
    const sd = SCENES[this.currentScene];
    setTimeout(() => {
      this.startDialogue(DIALOGUES[sd.id + '_intro'], () => {
        this.showHint('Activate Perception Mode — Eye Button');
      });
    }, 400);
  },

  togglePerception() {
    this.perceptionActive = !this.perceptionActive;
    const btn = document.getElementById('perception-btn');
    const sceneEl = document.getElementById('scene-' + SCENES[this.currentScene].id);
    if (this.perceptionActive) {
      btn.classList.add('active');
      sceneEl.classList.add('perception-active');
      this.stats.perceptionActivations++;
      this.hideHint();
      this.activatePerceptionEffects();
    } else {
      btn.classList.remove('active');
      sceneEl.classList.remove('perception-active');
    }
  },

  activatePerceptionEffects() {
    const id = SCENES[this.currentScene].id;
    if (id === 'corridor')    this.revealCorridorAnomalies();
    if (id === 'archive')     this.revealArchiveAnomalies();
    if (id === 'observatory') this.revealObservatoryAnomalies();
  },

  // ── DIALOGUE ──────────────────────────────────────────────
  startDialogue(lines, onComplete) {
    if (!lines || lines.length === 0) { if (onComplete) onComplete(); return; }
    this.dialogueQueue = lines;
    this.dialogueIndex = 0;
    this.dialogueOnComplete = onComplete || null;
    this.isDialogueRunning = true;
    this.showDialogueLine(lines[0]);
  },

  showDialogueLine(line) {
    const box      = document.getElementById('dialogue-box');
    const speaker  = document.getElementById('dialogue-speaker');
    const text     = document.getElementById('dialogue-text');
    const cont     = document.getElementById('dialogue-continue');
    box.classList.add('visible');
    speaker.textContent = line.speaker ? line.speaker.toUpperCase() : '— NARRATOR —';
    speaker.style.color = line.speaker === 'Narrator' ? 'var(--silver)' : 'var(--gold)';
    text.textContent = '';
    cont.style.display = 'none';
    let i = 0;
    clearInterval(this._typeInterval);
    this._typeInterval = setInterval(() => {
      text.textContent += line.text[i];
      i++;
      if (i >= line.text.length) {
        clearInterval(this._typeInterval);
        cont.style.display = 'block';
      }
    }, 24);
  },

  advanceDialogue() {
    if (!this.isDialogueRunning) return;
    clearInterval(this._typeInterval);
    const currentLine = this.dialogueQueue[this.dialogueIndex];
    const text = document.getElementById('dialogue-text');
    if (text.textContent.length < currentLine.text.length) {
      text.textContent = currentLine.text;
      document.getElementById('dialogue-continue').style.display = 'block';
      return;
    }
    this.dialogueIndex++;
    if (this.dialogueIndex < this.dialogueQueue.length) {
      this.showDialogueLine(this.dialogueQueue[this.dialogueIndex]);
    } else {
      this.isDialogueRunning = false;
      this.hideDialogue();
      if (this.dialogueOnComplete) {
        const cb = this.dialogueOnComplete;
        this.dialogueOnComplete = null;
        setTimeout(cb, 300);
      }
    }
  },

  hideDialogue() {
    document.getElementById('dialogue-box').classList.remove('visible');
    this.isDialogueRunning = false;
    clearInterval(this._typeInterval);
  },

  // ── CHOICES ───────────────────────────────────────────────
  showChoices(choiceData) {
    const container = document.getElementById('choices');
    container.innerHTML = '';
    this.hideDialogue();

    const prompt = document.createElement('div');
    prompt.className = 'choice-prompt';
    prompt.textContent = choiceData.text;
    container.appendChild(prompt);

    choiceData.choices.forEach((choice, i) => {
      const btn = document.createElement('button');
      btn.className = 'choice-btn';
      btn.textContent = `${i + 1}. ${choice.label}`;
      btn.onclick = () => {
        this.stats.choicesMade++;
        if (choice.perception) this.addPerception(choice.perception);
        this.hideChoices();
        setTimeout(() => {
          if (DIALOGUES[choice.next]) {
            this.startDialogue(DIALOGUES[choice.next], () => this.afterChoice(choice.next));
          } else {
            this.afterChoice(choice.next);
          }
        }, 300);
      };
      container.appendChild(btn);
    });
    container.classList.add('visible');
  },

  hideChoices() {
    document.getElementById('choices').classList.remove('visible');
  },

  afterChoice(key) {
    if (key.startsWith('ending_')) {
      setTimeout(() => this.showEndScreen(), 1200);
    } else {
      this.proceedToNextScene();
    }
  },

  // ── ANOMALY FOUND ─────────────────────────────────────────
  foundAnomaly(sceneId, anomalyId, dialogueKey) {
    const el = document.getElementById(anomalyId);
    if (el) {
      if (el.dataset.found === 'true') return;
      el.dataset.found = 'true';
      el.classList.add('found');
    }
    this.anomaliesFound[sceneId]++;
    this.stats.totalAnomalies++;
    const sd = SCENES[this.currentScene];
    this.addPerception(sd.perceptionGain / sd.anomaliesTotal);
    this.updateFoundCounter(sceneId, this.anomaliesFound[sceneId], sd.anomaliesTotal);
    if (el) this.spawnParticlesFromEl(el);
    this.showNotification('Anomaly Detected');

    setTimeout(() => {
      if (DIALOGUES[dialogueKey]) {
        this.startDialogue(DIALOGUES[dialogueKey], () => {
          if (this.anomaliesFound[sceneId] >= sd.anomaliesTotal) this.onSceneComplete(sceneId);
        });
      } else {
        if (this.anomaliesFound[sceneId] >= sd.anomaliesTotal) this.onSceneComplete(sceneId);
      }
    }, 400);
  },

  onSceneComplete(sceneId) {
    const ck = sceneId + '_complete';
    if (DIALOGUES[ck]) {
      this.startDialogue(DIALOGUES[ck], () => {
        const fk = sceneId + '_final_choice';
        if (DIALOGUES[fk] && DIALOGUES[fk][0]) {
          this.showChoices(DIALOGUES[fk][0]);
        } else {
          this.proceedToNextScene();
        }
      });
    } else {
      this.proceedToNextScene();
    }
  },

  proceedToNextScene() {
    const next = this.currentScene + 1;
    if (next < SCENES.length) {
      const overlay = document.getElementById('transition-overlay');
      overlay.classList.add('fade-in');
      setTimeout(() => {
        overlay.classList.remove('fade-in');
        this.loadScene(next);
      }, 900);
    } else {
      this.showEndScreen();
    }
  },

  addPerception(amount) {
    this.perceptionLevel = Math.min(100, this.perceptionLevel + amount);
    document.getElementById('perception-fill').style.width = this.perceptionLevel + '%';
  },

  updateFoundCounter(sceneId, found, total) {
    const el = document.getElementById('found-counter');
    el.textContent = `Anomalies: ${found} / ${total}`;
    el.classList.add('visible');
  },

  showHint(text) {
    const h = document.getElementById('hint');
    h.textContent = text;
    h.style.opacity = '1';
  },
  hideHint() {
    document.getElementById('hint').style.opacity = '0';
  },

  showNotification(text) {
    const el = document.getElementById('notification');
    el.textContent = text;
    el.classList.add('show');
    setTimeout(() => el.classList.remove('show'), 2500);
  },

  spawnParticlesFromEl(sourceEl) {
    const rect = sourceEl.getBoundingClientRect();
    this.spawnParticlesAt(rect.left + rect.width / 2, rect.top + rect.height / 2);
  },

  spawnParticlesAt(cx, cy) {
    const area = document.getElementById('scene-area');
    const ar = area.getBoundingClientRect();
    for (let i = 0; i < 14; i++) {
      const p = document.createElement('div');
      p.className = 'particle';
      const size = 3 + Math.random() * 5;
      const angle = Math.random() * Math.PI * 2;
      const dist = 40 + Math.random() * 90;
      p.style.cssText = `width:${size}px;height:${size}px;left:${cx - ar.left}px;top:${cy - ar.top}px;background:${Math.random() > 0.5 ? 'var(--perception)' : 'var(--gold)'};--dur:${0.8 + Math.random() * 0.8}s;--tx:${Math.cos(angle) * dist}px;--ty:${Math.sin(angle) * dist}px;z-index:20;`;
      area.appendChild(p);
      setTimeout(() => p.remove(), 1600);
    }
  },

  // ═══════════════════════════════════════════════════════
  //  SCENE 1: CORRIDOR  (real photo bg + SVG overlays)
  // ═══════════════════════════════════════════════════════
  initCorridor() {
    this.anomaliesFound.corridor = 0;
    // Reset anomaly overlays
    ['shadow-overlay', 'symbol-overlay', 'light-overlay'].forEach(id => {
      const el = document.getElementById(id);
      if (el) {
        el.classList.remove('visible', 'found');
        el.dataset.found = 'false';
      }
    });
  },

  revealCorridorAnomalies() {
    if (this._corridorAnomaliesRevealed) return;
    this._corridorAnomaliesRevealed = true;
    setTimeout(() => {
      const s = document.getElementById('shadow-overlay');
      if (s) s.classList.add('visible');
    }, 400);
    setTimeout(() => {
      const sym = document.getElementById('symbol-overlay');
      if (sym) sym.classList.add('visible');
    }, 900);
    setTimeout(() => {
      const l = document.getElementById('light-overlay');
      if (l) l.classList.add('visible');
    }, 1400);
  },

  clickShadow() {
    if (!this.perceptionActive) { this.showNotification('Activate Perception Mode First'); return; }
    this.foundAnomaly('corridor', 'shadow-overlay', 'corridor_shadow');
  },
  clickSymbol() {
    if (!this.perceptionActive) { this.showNotification('Activate Perception Mode First'); return; }
    this.foundAnomaly('corridor', 'symbol-overlay', 'corridor_symbol');
  },
  clickLight() {
    if (!this.perceptionActive) { this.showNotification('Activate Perception Mode First'); return; }
    this.foundAnomaly('corridor', 'light-overlay', 'corridor_light');
  },

  // ═══════════════════════════════════════════════════════
  //  SCENE 2: ARCHIVE  (real photo bg + canvas overlay)
  // ═══════════════════════════════════════════════════════
  initArchive() {
    this.anomaliesFound.archive = 0;
    this._archiveFound = {};
    this._archiveHotspots = [];
    clearInterval(this._floatInterval);
    setTimeout(() => {
      const canvas = document.getElementById('archive-canvas');
      if (!canvas) return;
      canvas.width  = canvas.offsetWidth  || canvas.clientWidth;
      canvas.height = canvas.offsetHeight || canvas.clientHeight;
      this.drawArchiveOverlay(canvas);
    }, 150);
  },

  drawArchiveOverlay(canvas) {
    // Canvas is transparent overlay on top of the photo background
    const ctx = canvas.getContext('2d');
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    // Hotspot positions as fractions of canvas size
    this._archiveHotspots = [
      { x: canvas.width * 0.50, y: canvas.height * 0.38, r: 40, key: 0 }, // door symbol
      { x: canvas.width * 0.72, y: canvas.height * 0.30, r: 35, key: 1 }, // floating book
      { x: canvas.width * 0.18, y: canvas.height * 0.42, r: 38, key: 2 }, // window reflection
    ];
  },

  revealArchiveAnomalies() {
    if (this._archiveAnomaliesRevealed) return;
    this._archiveAnomaliesRevealed = true;
    const canvas = document.getElementById('archive-canvas');
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    const W = canvas.width, H = canvas.height;

    // 1 — Door symbol (centre)
    setTimeout(() => {
      const hs = this._archiveHotspots[0];
      if (!hs || this._archiveFound[0]) return;
      ctx.strokeStyle = 'rgba(123,79,255,0.75)';
      ctx.lineWidth = 1.5;
      ctx.beginPath(); ctx.arc(hs.x, hs.y, 28, 0, Math.PI * 2); ctx.stroke();
      ctx.beginPath(); ctx.arc(hs.x, hs.y, 16, 0, Math.PI * 2); ctx.stroke();
      for (let a = 0; a < 8; a++) {
        const ang = (a / 8) * Math.PI * 2;
        ctx.beginPath();
        ctx.moveTo(hs.x + Math.cos(ang) * 16, hs.y + Math.sin(ang) * 16);
        ctx.lineTo(hs.x + Math.cos(ang) * 28, hs.y + Math.sin(ang) * 28);
        ctx.stroke();
      }
      const g = ctx.createRadialGradient(hs.x, hs.y, 5, hs.x, hs.y, 50);
      g.addColorStop(0, 'rgba(123,79,255,0.18)');
      g.addColorStop(1, 'rgba(123,79,255,0)');
      ctx.fillStyle = g;
      ctx.beginPath(); ctx.arc(hs.x, hs.y, 50, 0, Math.PI * 2); ctx.fill();
    }, 500);

    // 2 — Floating book (right shelf)
    setTimeout(() => {
      const hs = this._archiveHotspots[1];
      if (!hs || this._archiveFound[1]) return;
      this._startFloatingBookAnim(ctx, hs.x, hs.y);
    }, 1000);

    // 3 — Window reflection (left)
    setTimeout(() => {
      const hs = this._archiveHotspots[2];
      if (!hs || this._archiveFound[2]) return;
      ctx.strokeStyle = 'rgba(123,79,255,0.6)';
      ctx.lineWidth = 1.5;
      ctx.strokeRect(hs.x - 30, hs.y - 45, 60, 90);
      ctx.fillStyle = 'rgba(123,79,255,0.08)';
      ctx.fillRect(hs.x - 30, hs.y - 45, 60, 90);
      // Reflection figure
      ctx.fillStyle = 'rgba(123,79,255,0.35)';
      ctx.beginPath(); ctx.arc(hs.x, hs.y - 20, 10, 0, Math.PI * 2); ctx.fill();
      ctx.fillRect(hs.x - 6, hs.y - 10, 12, 30);
    }, 1600);
  },

  _startFloatingBookAnim(ctx, bx, by) {
    let t = 0;
    const drawBook = (offset) => {
      ctx.clearRect(bx - 22, by - 40, 44, 60);
      // Redraw transparent bg patch
      ctx.clearRect(bx - 22, by - 40, 44, 60);
      ctx.fillStyle = 'rgba(123,79,255,0.12)';
      ctx.fillRect(bx - 14, by - 35 - offset, 28, 42);
      ctx.strokeStyle = 'rgba(123,79,255,0.7)';
      ctx.lineWidth = 1.5;
      ctx.strokeRect(bx - 14, by - 35 - offset, 28, 42);
      // Symbol on book cover
      ctx.strokeStyle = 'rgba(201,168,76,0.6)';
      ctx.lineWidth = 1;
      ctx.beginPath(); ctx.arc(bx, by - 14 - offset, 8, 0, Math.PI * 2); ctx.stroke();
      // Shadow
      const sg = ctx.createRadialGradient(bx, by + 8, 1, bx, by + 8, 18);
      sg.addColorStop(0, 'rgba(123,79,255,0.2)');
      sg.addColorStop(1, 'rgba(123,79,255,0)');
      ctx.fillStyle = sg;
      ctx.beginPath(); ctx.ellipse(bx, by + 8, 18, 6, 0, 0, Math.PI * 2); ctx.fill();
    };
    this._floatInterval = setInterval(() => {
      if (this._archiveFound[1]) { clearInterval(this._floatInterval); return; }
      drawBook(Math.sin(t) * 5);
      t += 0.05;
    }, 50);
  },

  handleArchiveClick(e) {
    if (!this.perceptionActive) { this.showNotification('Activate Perception Mode First'); return; }
    const canvas = document.getElementById('archive-canvas');
    const rect = canvas.getBoundingClientRect();
    const scaleX = canvas.width  / rect.width;
    const scaleY = canvas.height / rect.height;
    const mx = (e.clientX - rect.left) * scaleX;
    const my = (e.clientY - rect.top)  * scaleY;

    const dialogues = ['archive_door_symbol', 'archive_floating', 'archive_reflection'];
    this._archiveHotspots.forEach((hs, i) => {
      if (this._archiveFound[i]) return;
      const dist = Math.sqrt((mx - hs.x) ** 2 + (my - hs.y) ** 2);
      if (dist < hs.r + 10) {
        this._archiveFound[i] = true;
        clearInterval(this._floatInterval);
        // Clear the hotspot visually
        const canvas2 = document.getElementById('archive-canvas');
        const ctx = canvas2.getContext('2d');
        ctx.clearRect(hs.x - 60, hs.y - 60, 120, 120);
        // Gold confirmation ring
        ctx.strokeStyle = 'rgba(201,168,76,0.8)';
        ctx.lineWidth = 2;
        ctx.beginPath(); ctx.arc(hs.x, hs.y, 22, 0, Math.PI * 2); ctx.stroke();
        ctx.fillStyle = 'rgba(201,168,76,0.1)';
        ctx.beginPath(); ctx.arc(hs.x, hs.y, 22, 0, Math.PI * 2); ctx.fill();

        this.anomaliesFound.archive++;
        this.stats.totalAnomalies++;
        const sd = SCENES[this.currentScene];
        this.addPerception(sd.perceptionGain / sd.anomaliesTotal);
        this.updateFoundCounter('archive', this.anomaliesFound.archive, sd.anomaliesTotal);
        this.showNotification('Anomaly Detected');
        this.spawnParticlesAt(e.clientX, e.clientY);

        setTimeout(() => {
          this.startDialogue(DIALOGUES[dialogues[i]], () => {
            if (this.anomaliesFound.archive >= 3) this.onSceneComplete('archive');
          });
        }, 300);
      }
    });
  },

  // ═══════════════════════════════════════════════════════
  //  SCENE 3: OBSERVATORY  (real photo bg + canvas overlay)
  // ═══════════════════════════════════════════════════════
  initObservatory() {
    this.anomaliesFound.observatory = 0;
    this._obsStarsFound = [];
    this._obsStarPositions = [];
    clearInterval(this._starPulseInterval);
    setTimeout(() => {
      const canvas = document.getElementById('obs-canvas');
      if (!canvas) return;
      canvas.width  = canvas.offsetWidth  || canvas.clientWidth;
      canvas.height = canvas.offsetHeight || canvas.clientHeight;
      this.drawObservatoryOverlay(canvas);
    }, 150);
  },

  drawObservatoryOverlay(canvas) {
    const ctx = canvas.getContext('2d');
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    const W = canvas.width, H = canvas.height;
    // Three anomalous stars positioned in upper sky area of the photo
    this._obsStarPositions = [
      { x: W * 0.25, y: H * 0.18 },
      { x: W * 0.62, y: H * 0.12 },
      { x: W * 0.45, y: H * 0.30 }
    ];
    this._startStarPulseAnim(ctx);
  },

  revealObservatoryAnomalies() {
    // Stars are always pulsing once perception is active — already drawn
    this.showNotification('Stars Responding...');
  },

  _startStarPulseAnim(ctx) {
    let t = 0;
    const canvas = document.getElementById('obs-canvas');
    const W = canvas.width, H = canvas.height;

    this._starPulseInterval = setInterval(() => {
      ctx.clearRect(0, 0, W, H);
      this._obsStarPositions.forEach((star, i) => {
        if (this._obsStarsFound.includes(i)) {
          // Found — draw gold ring
          ctx.strokeStyle = 'rgba(201,168,76,0.9)';
          ctx.lineWidth = 2;
          ctx.beginPath(); ctx.arc(star.x, star.y, 20, 0, Math.PI * 2); ctx.stroke();
          ctx.fillStyle = 'rgba(201,168,76,0.15)';
          ctx.beginPath(); ctx.arc(star.x, star.y, 20, 0, Math.PI * 2); ctx.fill();
          return;
        }
        const pulse = 4 + Math.sin(t + i * 2.1) * 3;
        const alpha = 0.5 + Math.sin(t + i * 2.1) * 0.3;

        // Outer glow
        const g = ctx.createRadialGradient(star.x, star.y, 1, star.x, star.y, pulse * 5);
        g.addColorStop(0, `rgba(123,79,255,${alpha * 0.5})`);
        g.addColorStop(1, 'rgba(123,79,255,0)');
        ctx.fillStyle = g;
        ctx.beginPath(); ctx.arc(star.x, star.y, pulse * 5, 0, Math.PI * 2); ctx.fill();

        // Star core
        ctx.fillStyle = `rgba(200,180,255,${alpha})`;
        ctx.beginPath(); ctx.arc(star.x, star.y, pulse, 0, Math.PI * 2); ctx.fill();
        ctx.fillStyle = 'rgba(255,255,255,0.9)';
        ctx.beginPath(); ctx.arc(star.x, star.y, pulse * 0.4, 0, Math.PI * 2); ctx.fill();

        // Cross sparkle
        ctx.strokeStyle = `rgba(200,180,255,${alpha * 0.6})`;
        ctx.lineWidth = 1;
        const sp = pulse * 2.5;
        ctx.beginPath(); ctx.moveTo(star.x - sp, star.y); ctx.lineTo(star.x + sp, star.y); ctx.stroke();
        ctx.beginPath(); ctx.moveTo(star.x, star.y - sp); ctx.lineTo(star.x, star.y + sp); ctx.stroke();
      });

      // Draw constellation lines if 2+ found
      if (this._obsStarsFound.length >= 2) {
        this._drawConstellationLines(ctx);
      }
      t += 0.04;
    }, 50);
  },

  handleObservatoryClick(e) {
    if (!this.perceptionActive) { this.showNotification('Activate Perception Mode First'); return; }
    const canvas = document.getElementById('obs-canvas');
    const rect = canvas.getBoundingClientRect();
    const scaleX = canvas.width  / rect.width;
    const scaleY = canvas.height / rect.height;
    const mx = (e.clientX - rect.left) * scaleX;
    const my = (e.clientY - rect.top)  * scaleY;

    const dialogues = ['observatory_star1', 'observatory_star2', 'observatory_star3'];
    this._obsStarPositions.forEach((star, i) => {
      if (this._obsStarsFound.includes(i)) return;
      const dist = Math.sqrt((mx - star.x) ** 2 + (my - star.y) ** 2);
      if (dist < 40) {
        this._obsStarsFound.push(i);
        this.anomaliesFound.observatory++;
        this.stats.totalAnomalies++;
        const sd = SCENES[this.currentScene];
        this.addPerception(sd.perceptionGain / sd.anomaliesTotal);
        this.updateFoundCounter('observatory', this.anomaliesFound.observatory, sd.anomaliesTotal);
        this.showNotification('Star Aligned');
        this.spawnParticlesAt(e.clientX, e.clientY);

        if (this._obsStarsFound.length === 3) {
          setTimeout(() => this._drawFinalConstellation(), 400);
        }

        setTimeout(() => {
          this.startDialogue(DIALOGUES[dialogues[i]], () => {
            if (this.anomaliesFound.observatory >= 3) {
              setTimeout(() => this.onSceneComplete('observatory'), 800);
            }
          });
        }, 300);
      }
    });
  },

  _drawConstellationLines(ctx) {
    const found = this._obsStarsFound;
    const stars = this._obsStarPositions;
    ctx.strokeStyle = 'rgba(123,79,255,0.35)';
    ctx.lineWidth = 1;
    ctx.setLineDash([5, 5]);
    for (let i = 0; i < found.length - 1; i++) {
      const a = stars[found[i]], b = stars[found[i + 1]];
      ctx.beginPath(); ctx.moveTo(a.x, a.y); ctx.lineTo(b.x, b.y); ctx.stroke();
    }
    ctx.setLineDash([]);
  },

  _drawFinalConstellation() {
    const canvas = document.getElementById('obs-canvas');
    const ctx = canvas.getContext('2d');
    const stars = this._obsStarPositions;
    ctx.strokeStyle = 'rgba(201,168,76,0.8)';
    ctx.lineWidth = 1.5;
    ctx.setLineDash([]);
    ctx.beginPath();
    ctx.moveTo(stars[0].x, stars[0].y);
    ctx.lineTo(stars[1].x, stars[1].y);
    ctx.lineTo(stars[2].x, stars[2].y);
    ctx.closePath();
    ctx.stroke();
    // Centre glow
    const cx = (stars[0].x + stars[1].x + stars[2].x) / 3;
    const cy = (stars[0].y + stars[1].y + stars[2].y) / 3;
    const g = ctx.createRadialGradient(cx, cy, 5, cx, cy, 90);
    g.addColorStop(0, 'rgba(201,168,76,0.2)');
    g.addColorStop(1, 'rgba(201,168,76,0)');
    ctx.fillStyle = g;
    ctx.beginPath(); ctx.arc(cx, cy, 90, 0, Math.PI * 2); ctx.fill();
    ctx.strokeStyle = 'rgba(201,168,76,0.5)';
    ctx.lineWidth = 1;
    ctx.beginPath(); ctx.arc(cx, cy, 18, 0, Math.PI * 2); ctx.stroke();
  },

  // ═══════════════════════════════════════════════════════
  //  END SCREEN
  // ═══════════════════════════════════════════════════════
  showEndScreen() {
    clearInterval(this._floatInterval);
    clearInterval(this._starPulseInterval);
    const overlay = document.getElementById('transition-overlay');
    overlay.classList.add('fade-in');
    setTimeout(() => {
      document.querySelectorAll('.screen').forEach(s => s.classList.remove('active'));
      const end = document.getElementById('screen-end');
      end.innerHTML = `
        <div class="stars-bg" id="end-stars"></div>
        <div class="end-label">Academy Beyond This World</div>
        <div class="end-title">The Hidden System</div>
        <div class="end-quote">"Reality is not fixed.<br>It responds to how you see it."</div>
        <div class="end-gold">— Part I Complete —</div>
        <div class="end-stats">
          <div class="stat-item">
            <span class="stat-value">${this.stats.totalAnomalies}</span>
            <span class="stat-label">Anomalies Found</span>
          </div>
          <div class="stat-item">
            <span class="stat-value">${Math.round(this.perceptionLevel)}%</span>
            <span class="stat-label">Perception</span>
          </div>
          <div class="stat-item">
            <span class="stat-value">${this.stats.choicesMade}</span>
            <span class="stat-label">Choices Made</span>
          </div>
        </div>
        <button class="btn-replay" onclick="Game.replay()">Play Again</button>
      `;
      end.classList.add('active');
      overlay.classList.remove('fade-in');
      this.buildStars('end-stars', 120);
    }, 900);
  },

  replay() {
    this.currentScene = 0;
    this.perceptionLevel = 0;
    this.perceptionActive = false;
    this.anomaliesFound = { corridor: 0, archive: 0, observatory: 0 };
    this.stats = { totalAnomalies: 0, perceptionActivations: 0, choicesMade: 0 };
    this._archiveFound = {};
    this._obsStarsFound = [];
    this._corridorAnomaliesRevealed = false;
    this._archiveAnomaliesRevealed = false;
    document.getElementById('perception-fill').style.width = '0%';
    this.showScreen('screen-title');
  }
};
