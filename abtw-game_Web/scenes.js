// ═══════════════════════════════════════════════════════════
//  ABTW — THE HIDDEN SYSTEM: PART I
//  Scene Definitions & Dialogue Data
// ═══════════════════════════════════════════════════════════

const SCENES = [
  {
    id: 'corridor',
    number: 'Scene I',
    title: 'The Corridor',
    desc: 'Something is not right.\nThe light is the same. The walls are the same.\nBut the shadow… moved the wrong way.',
    sceneName: 'The Corridor — East Wing',
    anomaliesTotal: 3,
    perceptionGain: 33
  },
  {
    id: 'archive',
    number: 'Scene II',
    title: 'The Archive Door',
    desc: 'The door has always been here.\nBut today, standing perfectly still,\nyou notice something carved into the stone.',
    sceneName: 'The Archive — Lower Level',
    anomaliesTotal: 3,
    perceptionGain: 33
  },
  {
    id: 'observatory',
    number: 'Scene III',
    title: 'The Observatory',
    desc: 'The stars are in the wrong positions.\nNot wrong enough for anyone else to notice.\nBut you notice.',
    sceneName: 'The Observatory — Upper Tower',
    anomaliesTotal: 3,
    perceptionGain: 34
  }
];

const DIALOGUES = {
  corridor_intro: [
    { speaker: 'Astra', text: 'The East Wing corridor. I walk it every morning. Nothing ever changes here.' },
    { speaker: 'Astra', text: 'Except… today something feels different. Like the air is holding its breath.' },
    { speaker: null, text: 'Press the eye button to activate Perception Mode. Look for what doesn\'t belong.' }
  ],
  corridor_shadow: [
    { speaker: 'Astra', text: 'Wait.' },
    { speaker: 'Astra', text: 'That shadow. It\'s pointing the wrong direction. The light source is on the left, but the shadow falls… right.' },
    { speaker: 'Astra', text: 'I blink. I look again. It\'s still wrong.' },
    { speaker: 'Astra', text: 'Is this real… or is it just me?' }
  ],
  corridor_symbol: [
    { speaker: 'Astra', text: 'There\'s something on the wall. A symbol. Circular. Layered.' },
    { speaker: 'Astra', text: 'I\'ve walked past this wall a hundred times. This was never here.' },
    { speaker: 'Astra', text: 'Or maybe it was always here. And I just never looked closely enough.' }
  ],
  corridor_light: [
    { speaker: 'Astra', text: 'The sconce light is flickering. But not like a broken bulb.' },
    { speaker: 'Astra', text: 'It\'s flickering in a pattern. Almost like… a signal.' },
    { speaker: 'Astra', text: 'Three pulses. Pause. Three pulses again.' }
  ],
  corridor_complete: [
    { speaker: 'Astra', text: 'Three things. Three things that shouldn\'t be here.' },
    { speaker: 'Astra', text: 'I could tell someone. But who would believe me?' },
    { speaker: 'Astra', text: 'Lumi would want data. Nela would want to draw it. I just want to understand.' },
    { speaker: null, text: 'Perception increased. The Academy is beginning to respond.' }
  ],
  corridor_choice: [
    {
      text: 'What do you do next?',
      choices: [
        { label: 'Follow the symbol deeper into the corridor', next: 'corridor_follow', perception: 5 },
        { label: 'Write everything down in your notebook', next: 'corridor_write', perception: 3 },
        { label: 'Stand completely still and wait', next: 'corridor_wait', perception: 8 }
      ]
    }
  ],
  corridor_follow: [
    { speaker: 'Astra', text: 'I walk toward the symbol. As I get closer, the air feels… thicker. More present.' },
    { speaker: 'Astra', text: 'The symbol doesn\'t disappear. It stays. Waiting.' }
  ],
  corridor_write: [
    { speaker: 'Astra', text: 'I pull out my notebook. Sketch the symbol. Note the shadow direction. Mark the light pattern.' },
    { speaker: 'Astra', text: 'Evidence. If I\'m going to understand this, I need evidence.' }
  ],
  corridor_wait: [
    { speaker: 'Astra', text: 'I stop moving. Completely still.' },
    { speaker: 'Astra', text: 'The corridor breathes.' },
    { speaker: 'Astra', text: 'I\'m not imagining it. The walls are subtly shifting — not moving, but… adjusting. Like they know I\'m watching.' }
  ],
  archive_intro: [
    { speaker: 'Astra', text: 'The Archive. Lower Level. Students aren\'t supposed to be here after hours.' },
    { speaker: 'Astra', text: 'But the door was open. And something about the corridor led me here.' },
    { speaker: null, text: 'Activate Perception Mode. The Archive holds old things. Some of them remember.' }
  ],
  archive_door_symbol: [
    { speaker: 'Astra', text: 'The door frame. There\'s a symbol carved into the stone.' },
    { speaker: 'Astra', text: 'It\'s the same symbol from the corridor. Exactly the same.' },
    { speaker: 'Astra', text: 'This isn\'t a coincidence. Patterns don\'t repeat by accident.' }
  ],
  archive_floating: [
    { speaker: 'Astra', text: 'One of the books is… hovering. Just slightly. A centimetre off the shelf.' },
    { speaker: 'Astra', text: 'I reach for it. It settles back into place before I can touch it.' },
    { speaker: 'Astra', text: 'Like it was waiting for me to notice. Not to take it. Just to notice.' }
  ],
  archive_reflection: [
    { speaker: 'Astra', text: 'The window reflection. I can see myself. But there\'s a half-second delay.' },
    { speaker: 'Astra', text: 'I raise my hand. My reflection raises its hand… just after.' },
    { speaker: 'Astra', text: 'The Academy is not broken. It\'s showing me something.' }
  ],
  archive_complete: [
    { speaker: 'Astra', text: 'The same symbol. Three times now. Corridor, door frame, and…' },
    { speaker: 'Astra', text: 'I need to find Lumi. She\'ll say I\'m pattern-matching. She\'ll want proof.' },
    { speaker: 'Astra', text: 'Good. Because I have it.' }
  ],
  archive_choice: [
    {
      text: 'The symbol appears again. What do you feel?',
      choices: [
        { label: 'Curiosity — I need to understand the pattern', next: 'archive_curious', perception: 8 },
        { label: 'Caution — this could be dangerous', next: 'archive_cautious', perception: 3 },
        { label: 'Recognition — like I\'ve seen this before', next: 'archive_recognize', perception: 10 }
      ]
    }
  ],
  archive_curious: [
    { speaker: 'Astra', text: 'I lean closer. The symbol has layers — an outer ring, an inner ring, and lines that cross at precise angles.' },
    { speaker: 'Astra', text: 'It\'s not decorative. It\'s a system. A language, maybe.' }
  ],
  archive_cautious: [
    { speaker: 'Astra', text: 'I step back. Whatever this is, it\'s been here a long time.' },
    { speaker: 'Astra', text: 'Old things in the Academy have their own rules. I shouldn\'t rush.' }
  ],
  archive_recognize: [
    { speaker: 'Astra', text: 'I\'ve seen this before. Not here. Not in the Academy.' },
    { speaker: 'Astra', text: 'In a dream. Three months ago. I drew it in my sketchbook and forgot about it.' },
    { speaker: 'Astra', text: 'My hands are shaking.' }
  ],
  observatory_intro: [
    { speaker: 'Astra', text: 'The Observatory. The highest point in the Academy.' },
    { speaker: 'Astra', text: 'I come here when I need to think. The stars are always the same.' },
    { speaker: 'Astra', text: 'Except tonight, they\'re not.' },
    { speaker: null, text: 'Activate Perception Mode. Find the three stars that pulse differently.' }
  ],
  observatory_star1: [
    { speaker: 'Astra', text: 'That star. It\'s brighter than it should be. And it\'s pulsing.' },
    { speaker: 'Astra', text: 'Not like a variable star. Like a signal.' }
  ],
  observatory_star2: [
    { speaker: 'Astra', text: 'Another one. Same pulse frequency. They\'re… synchronized.' },
    { speaker: 'Astra', text: 'Lumi would say I\'m projecting patterns onto random data. But Lumi isn\'t here.' }
  ],
  observatory_star3: [
    { speaker: 'Astra', text: 'Three stars. Forming a triangle. The same proportions as the symbol.' },
    { speaker: 'Astra', text: 'The symbol from the corridor. The symbol from the Archive door.' },
    { speaker: 'Astra', text: 'It\'s everywhere. It\'s been everywhere. I just couldn\'t see it.' }
  ],
  observatory_complete: [
    { speaker: 'Astra', text: 'The constellation connects. Lines of light appear between the stars — just for a moment.' },
    { speaker: 'Astra', text: 'Then they fade. Like the Academy is saying: yes. You\'re seeing correctly.' },
    { speaker: 'Astra', text: 'Reality is not fixed. It responds to how you see it.' },
    { speaker: null, text: 'The Hidden System has been activated. Perception: FULL.' }
  ],
  observatory_final_choice: [
    {
      text: 'You understand now. The Academy has been waiting. What do you do?',
      choices: [
        { label: 'Tell Lumi and Nela everything — tonight', next: 'ending_together', perception: 0 },
        { label: 'Go back to the Archive — alone', next: 'ending_alone', perception: 0 },
        { label: 'Stand here and let the Academy show you more', next: 'ending_open', perception: 0 }
      ]
    }
  ],
  ending_together: [
    { speaker: 'Astra', text: 'I run. Down the tower stairs, through the corridor, past the Archive door.' },
    { speaker: 'Astra', text: 'Lumi\'s light is still on. Nela\'s sketchbook is open on the table.' },
    { speaker: 'Astra', text: '"I need to show you something," I say. "Both of you. Right now."' },
    { speaker: 'Astra', text: 'They look at me. And somehow, I think they\'ve been waiting for this too.' }
  ],
  ending_alone: [
    { speaker: 'Astra', text: 'I go back down. The Archive door is still open.' },
    { speaker: 'Astra', text: 'I sit on the floor in front of it. I open my notebook.' },
    { speaker: 'Astra', text: 'I start drawing everything I remember. Every symbol. Every anomaly. Every moment.' },
    { speaker: 'Astra', text: 'The Academy watches. And for the first time, I feel like it approves.' }
  ],
  ending_open: [
    { speaker: 'Astra', text: 'I stay still. Completely still.' },
    { speaker: 'Astra', text: 'The stars pulse. The symbol glows faintly on the observatory floor.' },
    { speaker: 'Astra', text: 'The Academy breathes.' },
    { speaker: 'Astra', text: 'And I breathe with it.' },
    { speaker: 'Astra', text: 'I don\'t know what this is yet. But I know one thing:' },
    { speaker: 'Astra', text: 'Reality is listening.' }
  ]
};