
// ═══════════════════════════════════════════════════════════
//  ABTW — THE HIDDEN SYSTEM: PART I  |  Scenes & Dialogues
// ═══════════════════════════════════════════════════════════

const SCENES = [
  {
    id: 'corridor',
    number: 'Scene I',
    title: 'The Corridor',
    sceneName: 'Scene I — The Corridor',
    desc: 'Something is not right. The shadows are not moving the way they should.',
    anomaliesTotal: 3,
    perceptionGain: 33,
    bg: 'img/corridor.jpg'
  },
  {
    id: 'archive',
    number: 'Scene II',
    title: 'The Archive Door',
    sceneName: 'Scene II — The Archive Door',
    desc: 'The Archive holds more than books. Some things here remember being seen.',
    anomaliesTotal: 3,
    perceptionGain: 33,
    bg: 'img/archive.jpg'
  },
  {
    id: 'observatory',
    number: 'Scene III',
    title: 'The Observatory',
    sceneName: 'Scene III — The Observatory',
    desc: 'Under the dome, the stars are not where they should be.',
    anomaliesTotal: 3,
    perceptionGain: 34,
    bg: 'img/observatory.jpg'
  }
];

// ── DIALOGUES ─────────────────────────────────────────────
const DIALOGUES = {

  // ── SCENE 1: CORRIDOR INTRO ──
  corridor_intro: [
    { speaker: 'Narrator', text: 'The Academy corridor stretches ahead — warm stone, golden light, the faint smell of old paper and something else. Something that doesn\'t quite belong.' },
    { speaker: 'Astra', text: 'I\'ve walked this corridor a hundred times. But today... something feels different. Like the air is holding its breath.' },
    { speaker: 'Narrator', text: 'Activate Perception Mode to see what others cannot. Look carefully — the Academy responds to those who notice.' }
  ],

  // ── CORRIDOR ANOMALIES ──
  corridor_shadow: [
    { speaker: 'Astra', text: 'Wait.' },
    { speaker: 'Astra', text: 'That shadow. It\'s not mine. There\'s no one else here — but the shadow is moving on its own. Slightly. Just slightly.' },
    { speaker: 'Astra', text: 'It\'s not frightening. It\'s... like it\'s trying to show me something.' }
  ],
  corridor_symbol: [
    { speaker: 'Astra', text: 'There — on the wall. A symbol. I\'ve never noticed it before, but it looks... old. Like it was always there, waiting to be seen.' },
    { speaker: 'Astra', text: 'Lumi would want to record this. She\'d say: "If it appears once, it\'s a coincidence. If it appears twice, it\'s a pattern."' },
    { speaker: 'Astra', text: 'I\'m going to remember this.' }
  ],
  corridor_light: [
    { speaker: 'Astra', text: 'The light above me just... shifted. Not flickered — shifted. Like it changed colour for a fraction of a second.' },
    { speaker: 'Astra', text: 'Violet. It was violet. That\'s not how Academy lights work.' },
    { speaker: 'Narrator', text: 'The Hidden System is responding. You are beginning to notice.' }
  ],
  corridor_complete: [
    { speaker: 'Narrator', text: 'Three anomalies. Three moments where reality showed a different layer.' },
    { speaker: 'Astra', text: 'This isn\'t random. These things are connected. The shadow, the symbol, the light — they\'re all part of something.' },
    { speaker: 'Astra', text: 'I need to go deeper. The Archive. That\'s where answers live.' }
  ],
  corridor_final_choice: [
    {
      text: 'How does Astra respond to what she has seen?',
      choices: [
        {
          label: 'Record everything carefully — Lumi\'s method.',
          next: 'choice_record',
          perception: 8
        },
        {
          label: 'Follow instinct — Nela\'s way. Move forward.',
          next: 'choice_instinct',
          perception: 5
        },
        {
          label: 'Stay still. Observe. Let the Academy respond.',
          next: 'choice_observe',
          perception: 12
        }
      ]
    }
  ],

  choice_record: [
    { speaker: 'Astra', text: 'I pull out my notebook. Three anomalies. Time, location, description. Lumi always says: the pattern only becomes visible when you write it down.' },
    { speaker: 'Astra', text: 'Shadow — delayed. Symbol — wall, left side. Light — violet shift, ceiling. All within twenty metres of each other.' },
    { speaker: 'Astra', text: 'That\'s not a coincidence.' }
  ],
  choice_instinct: [
    { speaker: 'Astra', text: 'Nela would say: don\'t overthink it. If something is pulling you forward, follow it.' },
    { speaker: 'Astra', text: 'I walk. The corridor feels different now — like it knows I\'ve noticed. Like it\'s watching back.' }
  ],
  choice_observe: [
    { speaker: 'Astra', text: 'I stop. I breathe. I don\'t move.' },
    { speaker: 'Astra', text: 'And then — very quietly — the symbol on the wall pulses once. Just once. Like a heartbeat.' },
    { speaker: 'Narrator', text: 'The Academy does not reveal itself. It responds to perception.' }
  ],

  // ── SCENE 2: ARCHIVE INTRO ──
  archive_intro: [
    { speaker: 'Narrator', text: 'The Archive. Thousands of volumes, centuries of knowledge — and something else. A presence that predates the books.' },
    { speaker: 'Astra', text: 'The door is different today. I can feel it before I even touch the handle. Something on the other side is aware.' },
    { speaker: 'Narrator', text: 'Use Perception Mode. The Archive reveals itself only to those who are ready to see.' }
  ],

  // ── ARCHIVE ANOMALIES ──
  archive_door_symbol: [
    { speaker: 'Astra', text: 'The symbol on the door. It\'s the same one from the corridor — but here it\'s complete. In the corridor it was partial, like a fragment.' },
    { speaker: 'Astra', text: 'It\'s a circle with eight lines. Like a compass. Or a clock. Or something that measures something I don\'t have a word for yet.' },
    { speaker: 'Astra', text: 'I touch it. The stone is warm.' }
  ],
  archive_floating: [
    { speaker: 'Astra', text: 'That book. It\'s floating. Not dramatically — just slightly, barely off the shelf. Like it\'s been waiting for someone to notice.' },
    { speaker: 'Astra', text: 'The title is in a language I don\'t recognise. But the symbol on the cover — I\'ve seen it twice now.' },
    { speaker: 'Astra', text: 'I reach for it. It settles back onto the shelf. Like it was testing whether I was paying attention.' }
  ],
  archive_reflection: [
    { speaker: 'Astra', text: 'The window. The reflection in the glass — it\'s showing the room as it was, not as it is. The books are in different positions. The candle is unlit.' },
    { speaker: 'Astra', text: 'It\'s like looking at a memory. The Archive\'s memory.' },
    { speaker: 'Narrator', text: 'Archives are not just storage. They are memory spaces. The past leaves traces for those who know how to look.' }
  ],
  archive_complete: [
    { speaker: 'Narrator', text: 'The Archive has shown you three layers of itself. Symbol. Object. Memory.' },
    { speaker: 'Astra', text: 'The system isn\'t random. It\'s layered. Each anomaly is a different kind of signal — visual, physical, temporal.' },
    { speaker: 'Astra', text: 'I need to tell Lumi and Nela. But first — the Observatory. If the system is everywhere, it will be there too.' }
  ],
  archive_final_choice: [
    {
      text: 'Before leaving the Archive, Astra must decide:',
      choices: [
        {
          label: 'Take the floating book. It chose to be noticed.',
          next: 'choice_take_book',
          perception: 10
        },
        {
          label: 'Leave everything as it is. Observe without disturbing.',
          next: 'choice_leave_archive',
          perception: 15
        },
        {
          label: 'Sketch the symbol. Record before moving on.',
          next: 'choice_sketch',
          perception: 8
        }
      ]
    }
  ],

  choice_take_book: [
    { speaker: 'Astra', text: 'I reach for the book. It feels heavier than it looks — like it\'s full of something denser than words.' },
    { speaker: 'Astra', text: 'The symbol on the cover glows faintly as I hold it. Then stops. Like it was confirming something.' }
  ],
  choice_leave_archive: [
    { speaker: 'Astra', text: 'I don\'t touch anything. I just look. And the longer I look, the more I see.' },
    { speaker: 'Astra', text: 'Three more symbols appear on the spines of books I hadn\'t noticed before. The Archive is responding to my attention.' },
    { speaker: 'Narrator', text: 'The Academy responds to observation. Not force. Not demand. Attention.' }
  ],
  choice_sketch: [
    { speaker: 'Astra', text: 'I draw the symbol carefully. Eight lines. A circle. A smaller circle inside.' },
    { speaker: 'Astra', text: 'As I finish the last line, the candle on the table lights itself.' },
    { speaker: 'Astra', text: 'I don\'t run. I just... note it down.' }
  ],

  // ── SCENE 3: OBSERVATORY INTRO ──
  observatory_intro: [
    { speaker: 'Narrator', text: 'The Observatory dome opens to the night sky. Cool air, the smell of old brass and starlight. And above — the stars.' },
    { speaker: 'Astra', text: 'I\'ve been here before. But tonight the stars look... different. Not wrong. Just — more. Like there are layers I\'ve never seen.' },
    { speaker: 'Narrator', text: 'Activate Perception Mode. Some stars are not stars. Find the ones that are waiting to be found.' }
  ],

  // ── OBSERVATORY ANOMALIES ──
  observatory_star1: [
    { speaker: 'Astra', text: 'That star. It\'s pulsing. Not twinkling — pulsing. Rhythmically. Like a signal.' },
    { speaker: 'Astra', text: 'The same rhythm as the symbol in the Archive. I\'m sure of it.' }
  ],
  observatory_star2: [
    { speaker: 'Astra', text: 'Another one. This one is brighter than it should be — and it\'s in the wrong position. I know this constellation. That star doesn\'t belong there.' },
    { speaker: 'Astra', text: 'Unless the constellation itself is the anomaly. Unless the pattern I learned is incomplete.' }
  ],
  observatory_star3: [
    { speaker: 'Astra', text: 'Three. Three anomalous stars. And they form a triangle.' },
    { speaker: 'Astra', text: 'The same triangle as the symbol. The same proportions. The same angles.' },
    { speaker: 'Astra', text: 'The symbol isn\'t just on walls and books. It\'s in the sky. It\'s everywhere. It\'s the system itself.' }
  ],
  observatory_complete: [
    { speaker: 'Narrator', text: 'The constellation aligns. Three points of light that were always there — waiting for someone to connect them.' },
    { speaker: 'Astra', text: 'The Hidden System isn\'t hidden because it\'s secret. It\'s hidden because most people don\'t look.' },
    { speaker: 'Astra', text: 'I\'ve been looking. And now I can\'t stop seeing it.' }
  ],
  observatory_final_choice: [
    {
      text: 'Astra stands under the aligned constellation. What does she understand?',
      choices: [
        {
          label: '"The system is a map. I need to learn to read it."',
          next: 'ending_map',
          perception: 15
        },
        {
          label: '"The system is alive. It has been watching me too."',
          next: 'ending_alive',
          perception: 10
        },
        {
          label: '"I\'m not ready to understand it yet. But I will be."',
          next: 'ending_patient',
          perception: 20
        }
      ]
    }
  ],

  // ── ENDINGS ──
  ending_map: [
    { speaker: 'Astra', text: 'A map. That\'s what it is. The symbols, the anomalies, the stars — they\'re coordinates. Not in space. In understanding.' },
    { speaker: 'Astra', text: 'I need Lumi. She\'ll see the structure. She\'ll find the logic.' },
    { speaker: 'Narrator', text: 'The Hidden System has been activated. Part I complete.' },
    { speaker: 'Narrator', text: 'Reality is not fixed. It responds to how you see it.' }
  ],
  ending_alive: [
    { speaker: 'Astra', text: 'It\'s been watching me. Every time I noticed something, it noticed me noticing.' },
    { speaker: 'Astra', text: 'That\'s not frightening. That\'s... extraordinary. The Academy isn\'t just a place. It\'s a presence.' },
    { speaker: 'Narrator', text: 'The Hidden System has been activated. Part I complete.' },
    { speaker: 'Narrator', text: 'The Academy does not reveal itself. It responds to perception.' }
  ],
  ending_patient: [
    { speaker: 'Astra', text: 'I don\'t understand it yet. And that\'s okay. Understanding takes time. Noticing comes first.' },
    { speaker: 'Astra', text: 'I\'ve noticed. That\'s enough for today.' },
    { speaker: 'Narrator', text: 'The Hidden System has been activated. Part I complete.' },
    { speaker: 'Narrator', text: 'Some worlds are hidden. Others... are waiting to be noticed.' }
  ]
};
