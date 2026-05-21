const h = require('./helpers');

function buildEpilogue() {
  const c = [];

  // EPILOGUE OPENER
  c.push(h.pageBreak());
  c.push(h.spacer(600));
  c.push(h.sectionTitle('✨ EPILOGUE'));
  c.push(h.centeredText('Something Else Is Watching', 30, false, '5A4A7A'));
  c.push(h.spacer(200));
  c.push(h.epigraph('Every system has a keeper.'));
  c.push(h.epigraph('Not all of them are visible.'));
  c.push(h.pageBreak());

  const epilogueParas = [
    'The corridor was empty.',
    'Not the way it was empty when students had gone home and the building settled into its evening quiet. This was a different kind of empty—deliberate, held, as if the space itself had exhaled and decided to stay that way.',
    'The light was low. Not dark, but reduced, the way it became in the hours between one thing and the next.',
    'Nothing moved.',
    'And then something did.',
    'Not a shadow. Not a reflection. Not a pattern forming at the edge of perception.',
    'Something else.',
    'It moved through the corridor the way understanding moves through a mind—not visibly, not loudly, but completely. It passed the place where the shadow had first stayed still. It passed the window where the reflection had hesitated. It passed the wall where the symbol had appeared and disappeared and appeared again.',
    'It paused.',
    'Not because it was uncertain.',
    'Because it was paying attention.',
    'The bracelet—left on a desk in an empty classroom, forgotten in the rush of everything that had happened—pulsed once. Soft. Steady. Unmistakable.',
    'Then it went still.',
    'The corridor held its quiet.',
    'Somewhere in the building, three students were walking home, talking about what they had seen and what it meant and what they would do next. They were not afraid. They were not confused.',
    'They were ready.',
    'The thing in the corridor did not follow them.',
    'It didn’t need to.',
    'It had been watching since the beginning—since the first shadow, the first flicker, the first moment a girl had slowed down in a corridor and felt that something wasn’t right.',
    'It had watched them notice.',
    'It had watched them understand.',
    'It had watched them choose.',
    'And now it waited, the way it always waited—patient, present, unhurried—for whatever came next.',
    'Because the Academy was not finished.',
    'It was never finished.',
    'It only ever waited for the next person who was ready to begin.',
    'The light in the corridor shifted—barely, almost imperceptibly.',
    'And somewhere, in a classroom not far away, a different student slowed down.',
    'Looked at a shadow.',
    'Frowned.',
    'And wrote something in a notebook.',
    'Something is not right.',
    'The Academy noticed.',
    'And smiled—if systems can smile—in the only way it knew how.',
    'By waiting.',
    'By being there.',
    'By being exactly what it had always been.',
    'Hidden in plain sight.',
    'Ready to be seen.'
  ];
  epilogueParas.forEach(p => c.push(h.bodyPara(p, true)));
  c.push(h.italicPara('✨ — End of Tome I — ✨'));

  return c;
}

function buildEndMatter() {
  const c = [];

  // SERIES NOTE
  c.push(h.pageBreak());
  c.push(h.sectionTitle('About the Series'));
  c.push(h.spacer(80));
  c.push(h.bodyPara('Academy Beyond This World is a multi-volume series following Astra, Lumi, and Nela as they uncover the hidden system woven into the fabric of their world.'));
  c.push(h.spacer(40));
  c.push(h.bodyPara('Each tome deepens their understanding—and the stakes.'));
  c.push(h.spacer(80));
  c.push(h.rule());
  c.push(h.spacer(80));

  // WHAT'S NEXT
  c.push(h.sectionTitle('What’s Next'));
  c.push(h.spacer(60));
  c.push(h.centeredText('TOME II: The Notebook That Writes', 26, true, '2C3E7A'));
  c.push(h.spacer(80));
  c.push(h.bodyPara('The system has noticed them back.'));
  c.push(h.bodyPara('Now it’s leaving messages.'));
  c.push(h.spacer(40));
  c.push(h.bodyPara('When Lumi’s notebook begins filling with writing she didn’t put there—precise, structured, and impossible—the trio realises the Academy isn’t just something to be understood.'));
  c.push(h.bodyPara('It’s trying to communicate.'));
  c.push(h.spacer(40));
  c.push(h.bodyPara('But not everyone who can see the system wants it understood.'));
  c.push(h.bodyPara('And the first person who tries to stop them… is someone they already know.'));
  c.push(h.spacer(80));
  c.push(h.rule());

  // CHARACTER CARDS — THE TRIO
  c.push(h.pageBreak());
  c.push(h.sectionTitle('Character Cards'));
  c.push(h.centeredText('The Trio', 28, false, '5A4A7A'));
  c.push(h.spacer(80));

  // ASTRA
  c.push(h.cardHeader('⭐', 'ASTRA'));
  c.push(h.rule());
  c.push(h.cardField('Core line', 'She notices what others walk past'));
  c.push(h.cardField('Colours / Icon', 'Deep violet, silver-grey — ⭐ star'));
  c.push(h.cardField('What she notices', 'Gaps, delays, things that don’t quite fit'));
  c.push(h.cardField('What she avoids', 'Drawing attention before she’s certain'));
  c.push(h.cardField('What she carries', 'A bracelet that responds to the system'));
  c.push(h.cardField('Strength', 'Patience and precision of observation'));
  c.push(h.cardField('Soft fear', 'That she’s imagining it all'));
  c.push(h.cardField('Catchphrase', '“Something is not right.”'));
  c.push(h.spacer(60));

  // LUMI
  c.push(h.cardHeader('🧠', 'LUMI'));
  c.push(h.rule());
  c.push(h.cardField('Core line', 'She turns chaos into rules'));
  c.push(h.cardField('Colours / Icon', 'Cobalt blue, white — 🧠 brain'));
  c.push(h.cardField('What she notices', 'Patterns, inconsistencies, logical gaps'));
  c.push(h.cardField('What she avoids', 'Uncertainty she can’t categorise'));
  c.push(h.cardField('What she carries', 'A notebook full of observations (not theories)'));
  c.push(h.cardField('Strength', 'Systematic thinking and calm under pressure'));
  c.push(h.cardField('Soft fear', 'Being wrong about something she was certain of'));
  c.push(h.cardField('Catchphrase', '“That’s not a rule. That’s an observation.”'));
  c.push(h.spacer(60));

  // NELA
  c.push(h.cardHeader('⚡', 'NELA'));
  c.push(h.rule());
  c.push(h.cardField('Core line', 'She moves with the system before she understands it'));
  c.push(h.cardField('Colours / Icon', 'Warm gold, amber — ⚡ lightning'));
  c.push(h.cardField('What she notices', 'Energy, flow, when something wants to move'));
  c.push(h.cardField('What she avoids', 'Standing still when something is happening'));
  c.push(h.cardField('What she carries', 'A prism lens that splits light into roles'));
  c.push(h.cardField('Strength', 'Instinct and the ability to act without overthinking'));
  c.push(h.cardField('Soft fear', 'That her way of knowing isn’t real knowing'));
  c.push(h.cardField('Catchphrase', '“I don’t look for it. I follow it.”'));

  // SUPPORTING CAST
  c.push(h.pageBreak());
  c.push(h.sectionTitle('Character Cards'));
  c.push(h.centeredText('Supporting Cast', 28, false, '5A4A7A'));
  c.push(h.spacer(80));

  // PROFESSOR LYRA QUILL
  c.push(h.cardHeader('🎓', 'PROFESSOR LYRA QUILL'));
  c.push(h.rule());
  c.push(h.cardField('Role', 'Mentor — knows more than she says'));
  c.push(h.cardField('Core line', 'She teaches the rules of the visible world while quietly watching for those who notice the invisible one'));
  c.push(h.cardField('Colours / Icon', 'Forest green, gold — 🎓 graduation cap'));
  c.push(h.cardField('What she notices', 'Which students slow down in corridors'));
  c.push(h.cardField('What she carries', 'A key that doesn’t fit any visible lock'));
  c.push(h.cardField('Strength', 'Patience and the long view'));
  c.push(h.cardField('Soft fear', 'That she waited too long to act'));
  c.push(h.cardField('Catchphrase', '“Some questions answer themselves—if you give them time.”'));
  c.push(h.spacer(60));

  // MARA KEENE
  c.push(h.cardHeader('🧩', 'MARA KEENE'));
  c.push(h.rule());
  c.push(h.cardField('Role', 'The one who almost saw it—and chose not to'));
  c.push(h.cardField('Core line', 'She noticed the system once. She decided it was safer not to look again'));
  c.push(h.cardField('Colours / Icon', 'Muted teal, grey — 🧩 puzzle piece'));
  c.push(h.cardField('What she notices', 'Everything—but filters it carefully'));
  c.push(h.cardField('What she carries', 'A habit of looking away at the right moment'));
  c.push(h.cardField('Strength', 'Adaptability and social intelligence'));
  c.push(h.cardField('Soft fear', 'That she made the wrong choice'));
  c.push(h.cardField('Catchphrase', '“Some things are better left alone.”'));
  c.push(h.spacer(60));

  // MR. VALE
  c.push(h.cardHeader('🔇', 'MR. VALE — Curator of Silence'));
  c.push(h.rule());
  c.push(h.cardField('Role', 'Antagonist — tragic, not simply villainous'));
  c.push(h.cardField('Core line', 'He once understood the system better than anyone. Then he lost something in a portal collapse—and now he wants to close what he once opened'));
  c.push(h.cardField('Colours / Icon', 'Deep charcoal, cold silver — 🔇 muted'));
  c.push(h.cardField('What he notices', 'Every activation, every breach, every student who starts to see'));
  c.push(h.cardField('What he carries', 'The memory of what the system cost him'));
  c.push(h.cardField('Strength', 'Absolute knowledge of the system’s rules'));
  c.push(h.cardField('Soft fear', 'That he’s wrong about what needs to be protected'));
  c.push(h.cardField('Catchphrase', '“Some doors should stay closed.”'));

  // END NOTE
  c.push(h.pageBreak());
  c.push(h.spacer(400));
  c.push(h.rule());
  c.push(h.spacer(120));
  c.push(h.italicPara('These roles may shift. The Academy notices when they do.'));
  c.push(h.spacer(120));
  c.push(h.rule());
  c.push(h.spacer(200));
  c.push(h.centeredText('ACADEMY BEYOND THIS WORLD', 28, true, '2C3E7A'));
  c.push(h.centeredText('Tome I: The Hidden System', 22, false, '5A4A7A'));
  c.push(h.spacer(60));
  c.push(h.centeredText('Katarzyna Kalina vel Kalinowska', 20, false, '3A3A5A'));
  c.push(h.centeredText('Infinicorecipher FutureTech Education', 18, false, '7A7A7A'));
  c.push(h.centeredText('© 2026 — First Edition — United Kingdom', 18, false, '7A7A7A'));

  return c;
}

module.exports = { buildEpilogue, buildEndMatter };
