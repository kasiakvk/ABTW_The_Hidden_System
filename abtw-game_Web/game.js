// ═══════════════════════════════════════════════════════════
//  ABTW — THE HIDDEN SYSTEM: PART I  |  Game Engine
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
  _archiveHotspot1: null,
  _archiveHotspot2: null,
  _archiveHotspot3: null,

  init() {
    this.buildStars('title-stars', 120);
    document.addEventListener('keydown', (e) => {
      if (e.code === 'Space' || e.code === 'Enter') this.advanceDialogue();
    });
  },

  buildStars(containerId, count) {
    const container = document.getElementById(containerId);
    if (!container) return;
    for (let i = 0; i < count; i++) {
      const star = document.createElement('div');
      star.className = 'star';
      const size = Math.random() * 2.5 + 0.5;
      star.style.cssText = `width:${size}px;height:${size}px;left:${Math.random()*100}%;top:${Math.random()*100}%;--dur:${2+Math.random()*4}s;--delay:${Math.random()*5}s;--min-op:${0.1+Math.random()*0.2};--max-op:${0.6+Math.random()*0.4};`;
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
    clearInterval(this._floatInterval);
    clearInterval(this._starPulseInterval);

    const sceneData = SCENES[index];
    document.querySelectorAll('.scene').forEach(s => s.classList.remove('active'));
    document.getElementById('scene-name').textContent = sceneData.sceneName;
    this.hideDialogue();
    this.hideChoices();
    document.getElementById('perception-btn').classList.remove('active');
    this.updateFoundCounter(sceneData.id, 0, sceneData.anomaliesTotal);

    const sceneEl = document.getElementById('scene-' + sceneData.id);
    if (sceneEl) sceneEl.classList.add('active');

    if (sceneData.id === 'corridor') this.initCorridor();
    if (sceneData.id === 'archive') this.initArchive();
    if (sceneData.id === 'observatory') this.initObservatory();

    setTimeout(() => this.showSceneIntro(sceneData), 400);
  },

  showSceneIntro(sceneData) {
    const intro = document.getElementById('scene-intro');
    document.getElementById('scene-intro-number').textContent = sceneData.number;
    document.getElementById('scene-intro-title').textContent = sceneData.title;
    document.getElementById('scene-intro-desc').textContent = sceneData.desc;
    intro.classList.add('visible');
  },

  dismissSceneIntro() {
    document.getElementById('scene-intro').classList.remove('visible');
    const sceneData = SCENES[this.currentScene];
    setTimeout(() => {
      this.startDialogue(DIALOGUES[sceneData.id + '_intro'], () => {
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
    const sceneId = SCENES[this.currentScene].id;
    if (sceneId === 'corridor') {
      setTimeout(() => {
        const s = document.getElementById('delayed-shadow');
        const sym = document.getElementById('wall-symbol');
        const l = document.getElementById('light-flicker');
        if (s) s.style.opacity = '1';
        if (sym) sym.style.opacity = '1';
        if (l) l.style.opacity = '1';
      }, 600);
    }
    if (sceneId === 'archive') this.revealArchiveAnomalies();
    if (sceneId === 'observatory') this.revealObservatoryAnomalies();
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
    const box = document.getElementById('dialogue-box');
    const speaker = document.getElementById('dialogue-speaker');
    const text = document.getElementById('dialogue-text');
    const cont = document.getElementById('dialogue-continue');
    box.classList.add('visible');
    speaker.textContent = line.speaker ? line.speaker.toUpperCase() : '— NARRATOR —';
    speaker.style.color = line.speaker ? 'var(--gold)' : 'var(--silver)';
    text.textContent = '';
    cont.style.display = 'none';
    let i = 0;
    clearInterval(this._typeInterval);
    this._typeInterval = setInterval(() => {
      text.textContent += line.text[i];
      i++;
      if (i >= line.text.length) { clearInterval(this._typeInterval); cont.style.display = 'block'; }
    }, 28);
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
    prompt.style.cssText = 'font-family:Cinzel,serif;font-size:11px;letter-spacing:.25em;color:var(--silver);text-transform:uppercase;margin-bottom:8px;';
    prompt.textContent = choiceData.text;
    container.appendChild(prompt);
    choiceData.choices.forEach((choice, i) => {
      const btn = document.createElement('button');
      btn.className = 'choice-btn';
      btn.textContent = `${i+1}. ${choice.label}`;
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
      setTimeout(() => this.showEndScreen(), 800);
    } else {
      this.proceedToNextScene();
    }
  },

  // ── ANOMALY FOUND ─────────────────────────────────────────
  foundAnomaly(sceneId, anomalyId, dialogueKey) {
    const el = document.getElementById(anomalyId);
    if (el && el.classList.contains('found')) return;
    if (el) { el.classList.add('found'); el.style.cursor = 'default'; }
    this.anomaliesFound[sceneId]++;
    this.stats.totalAnomalies++;
    const sd = SCENES[this.currentScene];
    this.addPerception(sd.perceptionGain / sd.anomaliesTotal);
    this.updateFoundCounter(sceneId, this.anomaliesFound[sceneId], sd.anomaliesTotal);
    if (el) this.spawnParticles(el);
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
        const choiceKey = sceneId + '_choice';
        const finalKey = sceneId + '_final_choice';
        const key = DIALOGUES[finalKey] ? finalKey : (DIALOGUES[choiceKey] ? choiceKey : null);
        if (key && DIALOGUES[key][0]) this.showChoices(DIALOGUES[key][0]);
        else this.proceedToNextScene();
      });
    } else this.proceedToNextScene();
  },

  proceedToNextScene() {
    const next = this.currentScene + 1;
    if (next < SCENES.length) {
      const overlay = document.getElementById('transition-overlay');
      overlay.classList.add('fade-in');
      setTimeout(() => { overlay.classList.remove('fade-in'); this.loadScene(next); }, 900);
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

  showHint(text) { const h = document.getElementById('hint'); h.textContent = text; h.style.opacity = '1'; },
  hideHint() { document.getElementById('hint').style.opacity = '0'; },

  showNotification(text) {
    const el = document.getElementById('notification');
    el.textContent = text;
    el.classList.add('show');
    setTimeout(() => el.classList.remove('show'), 2500);
  },

  spawnParticles(sourceEl) {
    const rect = sourceEl.getBoundingClientRect();
    this.spawnParticlesAt(rect.left + rect.width/2, rect.top + rect.height/2);
  },

  spawnParticlesAt(cx, cy) {
    const area = document.getElementById('scene-area');
    const ar = area.getBoundingClientRect();
    for (let i = 0; i < 12; i++) {
      const p = document.createElement('div');
      p.className = 'particle';
      const size = 3 + Math.random() * 5;
      const angle = Math.random() * Math.PI * 2;
      const dist = 40 + Math.random() * 80;
      p.style.cssText = `width:${size}px;height:${size}px;left:${cx-ar.left}px;top:${cy-ar.top}px;background:${Math.random()>.5?'var(--perception)':'var(--gold)'};--dur:${0.8+Math.random()*0.8}s;--tx:${Math.cos(angle)*dist}px;--ty:${Math.sin(angle)*dist}px;z-index:20;`;
      area.appendChild(p);
      setTimeout(() => p.remove(), 1600);
    }
  },

  // ═══════════════════════════════════════════════════════
  //  SCENE 1: CORRIDOR
  // ═══════════════════════════════════════════════════════
  initCorridor() {
    this.anomaliesFound.corridor = 0;
    ['delayed-shadow','wall-symbol','light-flicker'].forEach(id => {
      const el = document.getElementById(id);
      if (el) { el.style.opacity = '0'; el.classList.remove('found'); el.style.cursor = 'pointer'; }
    });
  },

  clickShadow() {
    if (!this.perceptionActive) { this.showNotification('Activate Perception Mode First'); return; }
    this.foundAnomaly('corridor', 'delayed-shadow', 'corridor_shadow');
  },
  clickSymbol() {
    if (!this.perceptionActive) { this.showNotification('Activate Perception Mode First'); return; }
    this.foundAnomaly('corridor', 'wall-symbol', 'corridor_symbol');
  },
  clickLight() {
    if (!this.perceptionActive) { this.showNotification('Activate Perception Mode First'); return; }
    this.foundAnomaly('corridor', 'light-flicker', 'corridor_light');
  },

  // ═══════════════════════════════════════════════════════
  //  SCENE 2: ARCHIVE
  // ═══════════════════════════════════════════════════════
  initArchive() {
    this.anomaliesFound.archive = 0;
    this._archiveFound = {};
    this._archiveAnomaliesRevealed = false;
    this._archiveHotspot1 = null;
    this._archiveHotspot2 = null;
    this._archiveHotspot3 = null;
    clearInterval(this._floatInterval);
    setTimeout(() => {
      const canvas = document.getElementById('archive-canvas');
      if (!canvas) return;
      canvas.width = canvas.offsetWidth || canvas.clientWidth;
      canvas.height = canvas.offsetHeight || canvas.clientHeight;
      this.drawArchiveScene(canvas);
    }, 100);
  },

  drawArchiveScene(canvas) {
    const ctx = canvas.getContext('2d');
    const W = canvas.width, H = canvas.height;
    const bg = ctx.createRadialGradient(W/2,H*.4,50,W/2,H/2,W*.7);
    bg.addColorStop(0,'#0f1525'); bg.addColorStop(1,'#060910');
    ctx.fillStyle = bg; ctx.fillRect(0,0,W,H);
    ctx.fillStyle = '#080c18'; ctx.fillRect(0,H*.72,W,H*.28);
    ctx.strokeStyle = 'rgba(201,168,76,0.15)'; ctx.lineWidth = 1;
    ctx.beginPath(); ctx.moveTo(0,H*.72); ctx.lineTo(W,H*.72); ctx.stroke();
    ctx.strokeStyle = 'rgba(201,168,76,0.05)';
    for (let i=0;i<=8;i++) { ctx.beginPath(); ctx.moveTo(W/2,H*.72); ctx.lineTo(i*(W/8),H); ctx.stroke(); }
    // Door
    const dX=W/2-80, dY=H*.15, dW=160, dH=H*.57;
    ctx.fillStyle='#0a0e1c'; ctx.fillRect(dX-20,dY-20,dW+40,dH+20);
    ctx.fillStyle='#0d1228'; ctx.fillRect(dX,dY,dW,dH);
    ctx.strokeStyle='rgba(201,168,76,0.3)'; ctx.lineWidth=2; ctx.strokeRect(dX,dY,dW,dH);
    ctx.strokeStyle='rgba(201,168,76,0.15)'; ctx.lineWidth=1; ctx.strokeRect(dX+12,dY+12,dW-24,dH-24);
    ctx.fillStyle='rgba(201,168,76,0.4)'; ctx.beginPath(); ctx.arc(dX+dW-22,dY+dH/2,6,0,Math.PI*2); ctx.fill();
    // Shelves
    this._drawShelf(ctx,W*.05,H*.1,W*.22,H*.65);
    this._drawShelf(ctx,W*.73,H*.1,W*.22,H*.65);
    // Door glow
    const dg=ctx.createRadialGradient(W/2,dY+dH/2,10,W/2,dY+dH/2,200);
    dg.addColorStop(0,'rgba(201,168,76,0.06)'); dg.addColorStop(1,'rgba(201,168,76,0)');
    ctx.fillStyle=dg; ctx.fillRect(0,0,W,H);
    // Window
    ctx.fillStyle='#0a0f20'; ctx.fillRect(W*.78,H*.2,W*.12,H*.3);
    ctx.strokeStyle='rgba(168,180,200,0.2)'; ctx.lineWidth=1; ctx.strokeRect(W*.78,H*.2,W*.12,H*.3);
    ctx.beginPath(); ctx.moveTo(W*.84,H*.2); ctx.lineTo(W*.84,H*.5); ctx.stroke();
    ctx.beginPath(); ctx.moveTo(W*.78,H*.35); ctx.lineTo(W*.9,H*.35); ctx.stroke();
    // Candle
    ctx.fillStyle='#0d1228'; ctx.fillRect(W*.44,H*.68,W*.12,H*.04);
    ctx.fillStyle='rgba(201,168,76,0.6)'; ctx.fillRect(W*.495,H*.64,4,H*.04);
    const cg=ctx.createRadialGradient(W*.497,H*.64,2,W*.497,H*.64,60);
    cg.addColorStop(0,'rgba(201,168,76,0.2)'); cg.addColorStop(1,'rgba(201,168,76,0)');
    ctx.fillStyle=cg; ctx.beginPath(); ctx.arc(W*.497,H*.64,60,0,Math.PI*2); ctx.fill();
    this._archiveDoor = {x:dX,y:dY,w:dW,h:dH};
    this._archiveWindow = {x:W*.78,y:H*.2,w:W*.12,h:H*.3};
  },

  _drawShelf(ctx,x,y,w,h) {
    ctx.fillStyle='#0a0d1a'; ctx.fillRect(x,y,w,h);
    ctx.strokeStyle='rgba(201,168,76,0.12)'; ctx.lineWidth=1; ctx.strokeRect(x,y,w,h);
    const colors=['#1a2040','#0f1830','#151c35','#0d1428','#1a1535','#0a1020'];
    for (let s=0;s<5;s++) {
      const sy=y+s*(h/5);
      ctx.strokeStyle='rgba(201,168,76,0.1)';
      ctx.beginPath(); ctx.moveTo(x,sy+h/5-4); ctx.lineTo(x+w,sy+h/5-4); ctx.stroke();
      let bx=x+4;
      while(bx<x+w-8) {
        const bw=8+Math.random()*14, bh=(h/5)*(0.6+Math.random()*0.3);
        ctx.fillStyle=colors[Math.floor(Math.random()*colors.length)];
        ctx.fillRect(bx,sy+h/5-4-bh,bw,bh);
        bx+=bw+1;
      }
    }
  },

  revealArchiveAnomalies() {
    if (this._archiveAnomaliesRevealed) return;
    this._archiveAnomaliesRevealed = true;
    const canvas = document.getElementById('archive-canvas');
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    const W=canvas.width, H=canvas.height;
    // 1. Door symbol
    setTimeout(() => {
      const d=this._archiveDoor;
      const cx=d.x+d.w/2, cy=d.y+40;
      ctx.strokeStyle='rgba(123,79,255,0.7)'; ctx.lineWidth=1.5;
      ctx.beginPath(); ctx.arc(cx,cy,22,0,Math.PI*2); ctx.stroke();
      ctx.beginPath(); ctx.arc(cx,cy,14,0,Math.PI*2); ctx.stroke();
      [[cx,cy-22,cx,cy+22],[cx-22,cy,cx+22,cy],[cx-16,cy-16,cx+16,cy+16],[cx+16,cy-16,cx-16,cy+16]].forEach(([x1,y1,x2,y2])=>{
        ctx.beginPath(); ctx.moveTo(x1,y1); ctx.lineTo(x2,y2); ctx.stroke();
      });
      const g=ctx.createRadialGradient(cx,cy,5,cx,cy,40);
      g.addColorStop(0,'rgba(123,79,255,0.15)'); g.addColorStop(1,'rgba(123,79,255,0)');
      ctx.fillStyle=g; ctx.beginPath(); ctx.arc(cx,cy,40,0,Math.PI*2); ctx.fill();
      this._archiveHotspot1={x:cx-30,y:cy-30,w:60,h:60};
    },500);
    // 2. Floating book
    setTimeout(() => {
      this._floatingBookX=W*.73+W*.05; this._floatingBookY=H*.25;
      this._drawFloatingBook(ctx,this._floatingBookX,this._floatingBookY,0);
      this._archiveHotspot2={x:this._floatingBookX-15,y:this._floatingBookY-25,w:30,h:50};
      this._startFloatingBookAnim(ctx);
    },1000);
    // 3. Window reflection
    setTimeout(() => {
      const win=this._archiveWindow;
      ctx.fillStyle='rgba(123,79,255,0.12)'; ctx.fillRect(win.x,win.y,win.w,win.h);
      ctx.strokeStyle='rgba(123,79,255,0.5)'; ctx.lineWidth=1; ctx.strokeRect(win.x+2,win.y+2,win.w-4,win.h-4);
      ctx.fillStyle='rgba(123,79,255,0.3)';
      ctx.beginPath(); ctx.arc(win.x+win.w/2,win.y+win.h*.35,8,0,Math.PI*2); ctx.fill();
      ctx.fillRect(win.x+win.w/2-6,win.y+win.h*.45,12,25);
      this._archiveHotspot3={x:win.x,y:win.y,w:win.w,h:win.h};
    },1600);
  },

  _drawFloatingBook(ctx,x,y,offset) {
    const bw=28,bh=40;
    ctx.clearRect(x-2,y-offset-8,bw+4,bh+16);
    // Redraw bg patch
    const bg=ctx.createRadialGradient(x+bw/2,y+bh/2,10,x+bw/2,y+bh/2,60);
    bg.addColorStop(0,'#0f1525'); bg.addColorStop(1,'#060910');
    ctx.fillStyle=bg; ctx.fillRect(x-2,y-offset-8,bw+4,bh+16);
    ctx.fillStyle='#2a1a4a'; ctx.fillRect(x,y-offset,bw,bh);
    ctx.strokeStyle='rgba(123,79,255,0.6)'; ctx.lineWidth=1; ctx.strokeRect(x,y-offset,bw,bh);
    const g=ctx.createRadialGradient(x+bw/2,y-offset+bh+5,2,x+bw/2,y-offset+bh+5,20);
    g.addColorStop(0,'rgba(123,79,255,0.2)'); g.addColorStop(1,'rgba(123,79,255,0)');
    ctx.fillStyle=g; ctx.beginPath(); ctx.ellipse(x+bw/2,y-offset+bh+5,20,8,0,0,Math.PI*2); ctx.fill();
  },

  _startFloatingBookAnim(ctx) {
    let t=0;
    this._floatInterval=setInterval(()=>{
      if (!this._archiveAnomaliesRevealed) { clearInterval(this._floatInterval); return; }
      if (this._archiveFound && this._archiveFound[1]) return;
      const offset=Math.sin(t)*6;
      if (this._floatingBookX) this._drawFloatingBook(ctx,this._floatingBookX,this._floatingBookY,offset);
      t+=0.05;
    },50);
  },

  handleArchiveClick(e) {
    if (!this.perceptionActive) { this.showNotification('Activate Perception Mode First'); return; }
    const canvas=document.getElementById('archive-canvas');
    const rect=canvas.getBoundingClientRect();
    const mx=e.clientX-rect.left, my=e.clientY-rect.top;
    const hotspots=[this._archiveHotspot1,this._archiveHotspot2,this._archiveHotspot3];
    const dialogues=['archive_door_symbol','archive_floating','archive_reflection'];
    hotspots.forEach((hs,i)=>{
      if (!hs||this._archiveFound[i]) return;
      if (mx>=hs.x&&mx<=hs.x+hs.w&&my>=hs.y&&my<=hs.y+hs.h) {
        this._archiveFound[i]=true;
        this.anomaliesFound.archive++;
        this.stats.totalAnomalies++;
        const sd=SCENES[this.currentScene];
        this.addPerception(sd.perceptionGain/sd.anomaliesTotal);
        this.updateFoundCounter('archive',this.anomaliesFound.archive,sd.anomaliesTotal);
        this.showNotification('Anomaly Detected');
        this.spawnParticlesAt(e.clientX,e.clientY);
        setTimeout(()=>{
          this.startDialogue(DIALOGUES[dialogues[i]],()=>{
            if (this.anomaliesFound.archive>=3) this.onSceneComplete('archive');
          });
        },300);
      }
    });
  },

  // ═══════════════════════════════════════════════════════
  //  SCENE 3: OBSERVATORY
  // ═══════════════════════════════════════════════════════
  initObservatory() {
    this.anomaliesFound.observatory=0;
    this._obsStarsFound=[];
    this._obsStarPositions=[];
    clearInterval(this._starPulseInterval);
    setTimeout(()=>{
      const canvas=document.getElementById('obs-canvas');
      if (!canvas) return;
      canvas.width=canvas.offsetWidth||canvas.clientWidth;
      canvas.height=canvas.offsetHeight||canvas.clientHeight;
      this.drawObservatoryScene(canvas);
    },100);
  },

  drawObservatoryScene(canvas) {
    const ctx=canvas.getContext('2d');
    const W=canvas.width, H=canvas.height;
    const sky=ctx.createRadialGradient(W/2,0,100,W/2,H*.5,W*.8);
    sky.addColorStop(0,'#0d1535'); sky.addColorStop(.5,'#080c1e'); sky.addColorStop(1,'#060910');
    ctx.fillStyle=sky; ctx.fillRect(0,0,W,H);
    // Dome arch
    ctx.strokeStyle='rgba(201,168,76,0.2)'; ctx.lineWidth=2;
    ctx.beginPath(); ctx.arc(W/2,H*.85,W*.55,Math.PI,0); ctx.stroke();
    ctx.strokeStyle='rgba(201,168,76,0.1)'; ctx.lineWidth=1;
    ctx.beginPath(); ctx.arc(W/2,H*.85,W*.48,Math.PI,0); ctx.stroke();
    // Floor
    ctx.fillStyle='#080c18'; ctx.fillRect(0,H*.82,W,H*.18);
    ctx.strokeStyle='rgba(201,168,76,0.15)'; ctx.lineWidth=1;
    ctx.beginPath(); ctx.moveTo(0,H*.82); ctx.lineTo(W,H*.82); ctx.stroke();
    // Telescope
    const tx=W*.55,ty=H*.55;
    ctx.fillStyle='#0d1228'; ctx.fillRect(tx-8,ty,16,H*.27);
    ctx.save(); ctx.translate(tx,ty); ctx.rotate(-0.4);
    ctx.fillStyle='#0f1530'; ctx.fillRect(-8,-60,16,60);
    ctx.fillStyle='#1a2040'; ctx.beginPath(); ctx.arc(0,-60,14,0,Math.PI*2); ctx.fill();
    ctx.restore();
    // Railing
    ctx.strokeStyle='rgba(201,168,76,0.2)'; ctx.lineWidth=1.5;
    ctx.beginPath(); ctx.moveTo(W*.1,H*.82); ctx.lineTo(W*.9,H*.82); ctx.stroke();
    for (let i=0;i<=10;i++) {
      const rx=W*.1+i*(W*.8/10);
      ctx.beginPath(); ctx.moveTo(rx,H*.75); ctx.lineTo(rx,H*.82); ctx.stroke();
    }
    // Background stars
    ctx.fillStyle='rgba(255,255,255,0.6)';
    for (let i=0;i<200;i++) {
      const sx=Math.random()*W, sy=Math.random()*H*.8, sr=Math.random()*1.2;
      ctx.beginPath(); ctx.arc(sx,sy,sr,0,Math.PI*2); ctx.fill();
    }
    // Anomaly stars
    const stars=[{x:W*.28,y:H*.18},{x:W*.65,y:H*.12},{x:W*.48,y:H*.32}];
    this._obsStarPositions=stars;
    stars.forEach(star=>{
      const g=ctx.createRadialGradient(star.x,star.y,2,star.x,star.y,25);
      g.addColorStop(0,'rgba(123,79,255,0.3)'); g.addColorStop(1,'rgba(123,79,255,0)');
      ctx.fillStyle=g; ctx.beginPath(); ctx.arc(star.x,star.y,25,0,Math.PI*2); ctx.fill();
      ctx.fillStyle='rgba(200,180,255,0.9)'; ctx.beginPath(); ctx.arc(star.x,star.y,4,0,Math.PI*2); ctx.fill();
      ctx.fillStyle='white'; ctx.beginPath(); ctx.arc(star.x,star.y,2,0,Math.PI*2); ctx.fill();
    });
    this._startStarPulseAnim(ctx,stars,W,H);
  },

  _startStarPulseAnim(ctx,stars,W,H) {
    let t=0;
    this._starPulseInterval=setInterval(()=>{
      stars.forEach((star,i)=>{
        if (this._obsStarsFound.includes(i)) return;
        const pulse=3+Math.sin(t+i*2.1)*2;
        ctx.clearRect(star.x-32,star.y-32,64,64);
        const sky=ctx.createRadialGradient(W/2,0,100,W/2,H*.5,W*.8);
        sky.addColorStop(0,'#0d1535'); sky.addColorStop(.5,'#080c1e'); sky.addColorStop(1,'#060910');
        ctx.fillStyle=sky; ctx.fillRect(star.x-32,star.y-32,64,64);
        const g=ctx.createRadialGradient(star.x,star.y,1,star.x,star.y,20+pulse*2);
        g.addColorStop(0,'rgba(123,79,255,0.4)'); g.addColorStop(1,'rgba(123,79,255,0)');
        ctx.fillStyle=g; ctx.beginPath(); ctx.arc(star.x,star.y,20+pulse*2,0,Math.PI*2); ctx.fill();
        ctx.fillStyle=`rgba(200,180,255,${0.7+Math.sin(t+i*2.1)*0.3})`;
        ctx.beginPath(); ctx.arc(star.x,star.y,pulse,0,Math.PI*2); ctx.fill();
        ctx.fillStyle='white'; ctx.beginPath(); ctx.arc(star.x,star.y,pulse*.5,0,Math.PI*2); ctx.fill();
      });
      t+=0.04;
    },50);
  },

  handleObservatoryClick(e) {
    if (!this.perceptionActive) { this.showNotification('Activate Perception Mode First'); return; }
    const canvas=document.getElementById('obs-canvas');
    const rect=canvas.getBoundingClientRect();
    const mx=e.clientX-rect.left, my=e.clientY-rect.top;
    const dialogues=['observatory_star1','observatory_star2','observatory_star3'];
    this._obsStarPositions.forEach((star,i)=>{
      if (this._obsStarsFound.includes(i)) return;
      const dist=Math.sqrt((mx-star.x)**2+(my-star.y)**2);
      if (dist<35) {
        this._obsStarsFound.push(i);
        this.anomaliesFound.observatory++;
        this.stats.totalAnomalies++;
        const sd=SCENES[this.currentScene];
        this.addPerception(sd.perceptionGain/sd.anomaliesTotal);
        this.updateFoundCounter('observatory',this.anomaliesFound.observatory,sd.anomaliesTotal);
        this.showNotification('Star Aligned');
        this.spawnParticlesAt(e.clientX,e.clientY);
        const ctx=canvas.getContext('2d');
        ctx.strokeStyle='rgba(201,168,76,0.8)'; ctx.lineWidth=1.5;
        ctx.beginPath(); ctx.arc(star.x,star.y,18,0,Math.PI*2); ctx.stroke();
        if (this._obsStarsFound.length>=2) this._drawConstellationLines(ctx);
        setTimeout(()=>{
          this.startDialogue(DIALOGUES[dialogues[i]],()=>{
            if (this.anomaliesFound.observatory>=3) {
              this._drawFinalConstellation(ctx);
              setTimeout(()=>this.onSceneComplete('observatory'),1500);
            }
          });
        },300);
      }
    });
  },

  _drawConstellationLines(ctx) {
    const found=this._obsStarsFound, stars=this._obsStarPositions;
    ctx.strokeStyle='rgba(123,79,255,0.4)'; ctx.lineWidth=1; ctx.setLineDash([4,4]);
    for (let i=0;i<found.length-1;i++) {
      const a=stars[found[i]],b=stars[found[i+1]];
      ctx.beginPath(); ctx.moveTo(a.x,a.y); ctx.lineTo(b.x,b.y); ctx.stroke();
    }
    ctx.setLineDash([]);
  },

  _drawFinalConstellation(ctx) {
    const stars=this._obsStarPositions;
    ctx.strokeStyle='rgba(201,168,76,0.7)'; ctx.lineWidth=1.5; ctx.setLineDash([]);
    ctx.beginPath(); ctx.moveTo(stars[0].x,stars[0].y); ctx.lineTo(stars[1].x,stars[1].y); ctx.lineTo(stars[2].x,stars[2].y); ctx.closePath(); ctx.stroke();
    const cx=(stars[0].x+stars[1].x+stars[2].x)/3, cy=(stars[0].y+stars[1].y+stars[2].y)/3;
    ctx.strokeStyle='rgba(201,168,76,0.5)'; ctx.lineWidth=1;
    ctx.beginPath(); ctx.arc(cx,cy,20,0,Math.PI*2); ctx.stroke();
    ctx.beginPath(); ctx.arc(cx,cy,12,0,Math.PI*2); ctx.stroke();
    const g=ctx.createRadialGradient(cx,cy,5,cx,cy,80);
    g.addColorStop(0,'rgba(201,168,76,0.15)'); g.addColorStop(1,'rgba(201,168,76,0)');
    ctx.fillStyle=g; ctx.beginPath(); ctx.arc(cx,cy,80,0,Math.PI*2); ctx.fill();
  },

  // ═══════════════════════════════════════════════════════
  //  END SCREEN
  // ═══════════════════════════════════════════════════════
  showEndScreen() {
    clearInterval(this._floatInterval);
    clearInterval(this._starPulseInterval);
    const overlay=document.getElementById('transition-overlay');
    overlay.classList.add('fade-in');
    setTimeout(()=>{
      document.querySelectorAll('.screen').forEach(s=>s.classList.remove('active'));
      const end=document.getElementById('screen-end');
      end.innerHTML=`
        <div class="stars-bg" id="end-stars"></div>
        <div class="end-label">Academy Beyond This World</div>
        <div class="end-title">Part I Complete</div>
        <div class="end-quote">"Reality is not fixed.<br>It responds to how you see it."</div>
        <div class="end-gold">— The Hidden System Activated —</div>
        <div class="end-stats">
          <div class="stat-item"><span class="stat-value">${this.stats.totalAnomalies}</span><span class="stat-label">Anomalies Found</span></div>
          <div class="stat-item"><span class="stat-value">${Math.round(this.perceptionLevel)}%</span><span class="stat-label">Perception</span></div>
          <div class="stat-item"><span class="stat-value">${this.stats.choicesMade}</span><span class="stat-label">Choices Made</span></div>
        </div>
        <button class="btn-replay" onclick="Game.replay()">Play Again</button>
      `;
      end.classList.add('active');
      overlay.classList.remove('fade-in');
      this.buildStars('end-stars',100);
    },900);
  },

  replay() {
    this.currentScene=0; this.perceptionLevel=0; this.perceptionActive=false;
    this.anomaliesFound={corridor:0,archive:0,observatory:0};
    this.stats={totalAnomalies:0,perceptionActivations:0,choicesMade:0};
    this._archiveFound={}; this._obsStarsFound=[];
    document.getElementById('perception-fill').style.width='0%';
    this.showScreen('screen-title');
  }
};