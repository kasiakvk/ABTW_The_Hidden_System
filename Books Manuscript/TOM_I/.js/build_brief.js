const docx = require('/usr/lib/node_modules/docx');
const {
  Document, Packer, Paragraph, TextRun, AlignmentType,
  Header, Footer, PageNumber, BorderStyle, Table, TableRow,
  TableCell, WidthType, VerticalAlign, ShadingType
} = docx;
const fs = require('fs');
const path = require('path');

// ─── HELPERS ─────────────────────────────────────────────────────────────────
function h1(text) {
  return new Paragraph({
    alignment: AlignmentType.CENTER,
    spacing: { before: 240, after: 120 },
    children: [new TextRun({ text, size: 36, bold: true, color: '2C3E7A', font: 'Georgia', allCaps: true })]
  });
}
function h2(text) {
  return new Paragraph({
    spacing: { before: 200, after: 80 },
    children: [new TextRun({ text, size: 28, bold: true, color: '2C3E7A', font: 'Georgia' })]
  });
}
function h3(text) {
  return new Paragraph({
    spacing: { before: 140, after: 60 },
    children: [new TextRun({ text, size: 24, bold: true, color: '5A4A7A', font: 'Georgia' })]
  });
}
function body(text) {
  return new Paragraph({
    spacing: { before: 40, after: 40, line: 340, lineRule: 'auto' },
    children: [new TextRun({ text, size: 21, font: 'Georgia', color: '1A1A2E' })]
  });
}
function italic(text) {
  return new Paragraph({
    spacing: { before: 40, after: 40 },
    children: [new TextRun({ text, size: 21, italics: true, font: 'Georgia', color: '5A4A7A' })]
  });
}
function rule() {
  return new Paragraph({
    border: { bottom: { color: 'C0A96E', size: 6, style: BorderStyle.SINGLE } },
    spacing: { before: 80, after: 80 },
    children: []
  });
}
function spacer(pts = 80) {
  return new Paragraph({ spacing: { before: pts, after: pts }, children: [] });
}
function pageBreak() {
  const { PageBreak } = docx;
  return new Paragraph({ children: [new PageBreak()] });
}
function bullet(text, color = '1A1A2E') {
  return new Paragraph({
    spacing: { before: 30, after: 30 },
    indent: { left: 360 },
    children: [
      new TextRun({ text: '• ', size: 21, bold: true, font: 'Georgia', color: 'C0A96E' }),
      new TextRun({ text, size: 21, font: 'Georgia', color })
    ]
  });
}

// ─── TABLE HELPERS ────────────────────────────────────────────────────────────
function cell(text, bold = false, bg = 'FFFFFF', color = '1A1A2E', width = 20) {
  return new TableCell({
    width: { size: width, type: WidthType.PERCENTAGE },
    verticalAlign: VerticalAlign.TOP,
    shading: { fill: bg, type: ShadingType.CLEAR },
    margins: { top: 80, bottom: 80, left: 120, right: 120 },
    children: [
      new Paragraph({
        children: [new TextRun({ text, size: 20, bold, font: 'Georgia', color })]
      })
    ]
  });
}

function pageRow(pageNum, section, type, typeColor, illustrationNotes, visualMood) {
  const typeBg = typeColor === 'TEXT_ONLY' ? 'EAF4EA'
    : typeColor === 'TEXT_ILLUS' ? 'EAF0FA'
    : 'FFF8E7';

  const typeLabel = typeColor === 'TEXT_ONLY' ? 'TEXT ONLY'
    : typeColor === 'TEXT_ILLUS' ? 'TEXT + ILLUSTRATION'
    : 'ILLUSTRATION ONLY';

  const typeFg = typeColor === 'TEXT_ONLY' ? '2A6A2A'
    : typeColor === 'TEXT_ILLUS' ? '1A3A7A'
    : '7A5A00';

  return new TableRow({
    children: [
      cell(String(pageNum), true, 'F0EEF8', '2C3E7A', 8),
      cell(section, false, 'FAFAFA', '1A1A2E', 22),
      cell(typeLabel, true, typeBg, typeFg, 18),
      cell(illustrationNotes, false, 'FFFFFF', '3A3A5A', 30),
      cell(visualMood, false, 'FDFAF5', '5A4A7A', 22)
    ]
  });
}

function tableHeader() {
  return new TableRow({
    tableHeader: true,
    children: [
      cell('PAGE', true, '2C3E7A', 'FFFFFF', 8),
      cell('SECTION / CHAPTER', true, '2C3E7A', 'FFFFFF', 22),
      cell('TYPE', true, '2C3E7A', 'FFFFFF', 18),
      cell('ILLUSTRATION NOTES', true, '2C3E7A', 'FFFFFF', 30),
      cell('VISUAL MOOD', true, '2C3E7A', 'FFFFFF', 22)
    ]
  });
}

// ─── PAGE DATA ────────────────────────────────────────────────────────────────
// Types: TEXT_ONLY | TEXT_ILLUS | ILLUS_ONLY
const pages = [
  // FRONT MATTER
  [1,  'Half-Title Page', 'ILLUS_ONLY', 'Atmospheric full-page spread: Academy silhouette at dusk, stars emerging, violet-indigo sky. Title text overlaid.', 'Mysterious, grand, inviting'],
  [2,  'Copyright Page', 'TEXT_ONLY', 'No illustration needed. Clean typographic layout.', 'Minimal, professional'],
  [3,  'Epigraph Page', 'TEXT_ILLUS', 'Subtle background: faint constellation or geometric pattern behind epigraph text. Not distracting.', 'Quiet, contemplative'],
  [4,  'Back Cover Blurb / About', 'TEXT_ILLUS', 'Small vignette: Astra\'s silhouette in a corridor, light bending oddly around her. Right-aligned or bottom.', 'Intriguing, slightly uncanny'],
  [5,  'Table of Contents', 'TEXT_ONLY', 'No illustration. Clean layout with part colour indicators (purple, blue, yellow, teal, moon).', 'Organised, elegant'],

  // PART I
  [6,  'Part I Opener — NOTICING', 'ILLUS_ONLY', 'Full-page: corridor perspective, shadow on floor that doesn\'t match the figure. Violet tones. Faint symbol suggestion on wall.', 'Eerie, beautiful, still'],
  [7,  'Ch 1 — Something Is Not Right', 'TEXT_ILLUS', 'Chapter opener vignette: Astra\'s hand near bracelet, corridor behind her slightly blurred. Shadow on floor misaligned.', 'Unsettled, alert'],
  [8,  'Ch 1 continued', 'TEXT_ONLY', '', ''],
  [9,  'Ch 1 continued', 'TEXT_ONLY', '', ''],
  [10, 'Ch 1 continued', 'TEXT_ILLUS', 'Small inset: close-up of notebook page with the words "Something is not right." Pencil resting beside it.', 'Personal, quiet revelation'],
  [11, 'Ch 2 — The Shadow That Didn\'t Move', 'TEXT_ILLUS', 'Chapter opener: floor-level view of corridor, one shadow frozen while others move. Late afternoon light.', 'Uncanny, precise'],
  [12, 'Ch 2 continued', 'TEXT_ONLY', '', ''],
  [13, 'Ch 2 continued', 'TEXT_ONLY', '', ''],
  [14, 'Ch 2 continued', 'TEXT_ONLY', '', ''],
  [15, 'Ch 3 — The Second Time', 'TEXT_ILLUS', 'Chapter opener vignette: light edge on wall, slightly too late. Astra\'s profile watching it.', 'Focused, expectant'],
  [16, 'Ch 3 continued', 'TEXT_ONLY', '', ''],
  [17, 'Ch 3 continued', 'TEXT_ONLY', '', ''],
  [18, 'Ch 3 continued', 'TEXT_ONLY', '', ''],
  [19, 'Ch 4 — Did You See That?', 'TEXT_ILLUS', 'Chapter opener: two students in corridor, one pointing, one looking confused. Shadow misaligned in background.', 'Social tension, isolation'],
  [20, 'Ch 4 continued', 'TEXT_ONLY', '', ''],
  [21, 'Ch 4 continued', 'TEXT_ONLY', '', ''],
  [22, 'Ch 4 continued', 'TEXT_ONLY', '', ''],
  [23, 'Ch 5 — A Pattern That Doesn\'t Belong', 'TEXT_ILLUS', 'Chapter opener: multiple glitch points visible simultaneously — shadow, reflection, locker edge. Astra at centre, calm.', 'Revelation, clarity emerging'],
  [24, 'Ch 5 continued', 'TEXT_ONLY', '', ''],
  [25, 'Ch 5 continued', 'TEXT_ONLY', '', ''],
  [26, 'Ch 5 continued', 'TEXT_ONLY', '', ''],
  [27, 'Ch 5 continued', 'TEXT_ONLY', '', ''],

  // PART II
  [28, 'Part II Opener — UNDERSTANDING', 'ILLUS_ONLY', 'Full-page: library or classroom, two figures at desks, light forming subtle geometric shapes between them. Blue tones.', 'Intellectual, warm, connected'],
  [29, 'Ch 6 — Lumi', 'TEXT_ILLUS', 'Chapter opener: Lumi\'s open notebook with precise diagrams and symbols. Her hand holding pen, mid-thought.', 'Precise, curious, structured'],
  [30, 'Ch 6 continued', 'TEXT_ONLY', '', ''],
  [31, 'Ch 6 continued', 'TEXT_ONLY', '', ''],
  [32, 'Ch 6 continued', 'TEXT_ONLY', '', ''],
  [33, 'Ch 7 — The Notebook That Doesn\'t Match', 'TEXT_ILLUS', 'Chapter opener: notebook page with faint lines visible only at an angle. Bracelet glowing softly nearby.', 'Mysterious, layered'],
  [34, 'Ch 7 continued', 'TEXT_ONLY', '', ''],
  [35, 'Ch 7 continued', 'TEXT_ONLY', '', ''],
  [36, 'Ch 7 continued', 'TEXT_ONLY', '', ''],
  [37, 'Ch 8 — There Are Rules', 'TEXT_ILLUS', 'Chapter opener: library setting, Lumi\'s notebook open showing "Rule 4: The system is not independent of the observer."', 'Scholarly, revelatory'],
  [38, 'Ch 8 continued', 'TEXT_ONLY', '', ''],
  [39, 'Ch 8 continued', 'TEXT_ONLY', '', ''],
  [40, 'Ch 8 continued', 'TEXT_ONLY', '', ''],
  [41, 'Ch 9 — Nela', 'TEXT_ILLUS', 'Chapter opener: Nela mid-step through light, the space around her slightly compressed/warped. Energy and motion.', 'Dynamic, warm, disruptive'],
  [42, 'Ch 9 continued', 'TEXT_ONLY', '', ''],
  [43, 'Ch 9 continued', 'TEXT_ONLY', '', ''],
  [44, 'Ch 9 continued', 'TEXT_ONLY', '', ''],
  [45, 'Ch 10 — "Don\'t Ask. Just Hold This."', 'TEXT_ILLUS', 'Chapter opener: all three girls in triangle formation, pattern forming between them. Violet, blue, gold tones.', 'Unified, powerful, still'],
  [46, 'Ch 10 continued', 'TEXT_ONLY', '', ''],
  [47, 'Ch 10 continued', 'TEXT_ONLY', '', ''],
  [48, 'Ch 10 continued', 'TEXT_ONLY', '', ''],

  // PART III
  [49, 'Part III Opener — CONNECTING', 'ILLUS_ONLY', 'Full-page: the first complete symbol, glowing faintly, three figures positioned around it. Gold-violet tones.', 'Magical, earned, significant'],
  [50, 'Ch 11 — The First Symbol', 'TEXT_ILLUS', 'Chapter opener: the symbol forming in air, incomplete but recognisable. Three pairs of eyes reflected in it.', 'Wonder, precision, connection'],
  [51, 'Ch 11 continued', 'TEXT_ONLY', '', ''],
  [52, 'Ch 11 continued', 'TEXT_ONLY', '', ''],
  [53, 'Ch 11 continued', 'TEXT_ONLY', '', ''],
  [54, 'Ch 12 — If It Means Something…', 'TEXT_ILLUS', 'Chapter opener: symbol with segments lighting up in sequence. Astra\'s hand near it, not touching.', 'Anticipation, meaning'],
  [55, 'Ch 12 continued', 'TEXT_ONLY', '', ''],
  [56, 'Ch 12 continued', 'TEXT_ONLY', '', ''],
  [57, 'Ch 12 continued', 'TEXT_ONLY', '', ''],
  [58, 'Ch 13 — Try It Again', 'TEXT_ILLUS', 'Chapter opener: symbol collapsed into fragments, three girls looking at each other — not defeated, adjusting.', 'Resilience, teamwork'],
  [59, 'Ch 13 continued', 'TEXT_ONLY', '', ''],
  [60, 'Ch 13 continued', 'TEXT_ONLY', '', ''],
  [61, 'Ch 13 continued', 'TEXT_ONLY', '', ''],
  [62, 'Ch 14 — It Reacted', 'TEXT_ILLUS', 'Chapter opener: prism splitting light into three coloured beams — violet, blue, gold — each touching one girl.', 'Revelation, identity, roles'],
  [63, 'Ch 14 continued', 'TEXT_ONLY', '', ''],
  [64, 'Ch 14 continued', 'TEXT_ONLY', '', ''],
  [65, 'Ch 14 continued', 'TEXT_ONLY', '', ''],
  [66, 'Ch 15 — The Three of Us', 'TEXT_ILLUS', 'Chapter opener: three silhouettes walking in alignment, corridor behind them, light ahead different from light behind.', 'Belonging, forward motion'],
  [67, 'Ch 15 continued', 'TEXT_ONLY', '', ''],
  [68, 'Ch 15 continued', 'TEXT_ONLY', '', ''],
  [69, 'Ch 15 continued', 'TEXT_ONLY', '', ''],

  // PART IV
  [70, 'Part IV Opener — ACTIVATING', 'ILLUS_ONLY', 'Full-page: Portal Corridor, seam of light visible along wall, three figures approaching. Deep teal and indigo.', 'Threshold, anticipation, depth'],
  [71, 'Ch 16 — The Corridor Changes', 'TEXT_ILLUS', 'Chapter opener: corridor with visible seam of light, space slightly expanded. Astra\'s bracelet pulsing.', 'Spatial wonder, active system'],
  [72, 'Ch 16 continued', 'TEXT_ONLY', '', ''],
  [73, 'Ch 16 continued', 'TEXT_ONLY', '', ''],
  [74, 'Ch 16 continued', 'TEXT_ONLY', '', ''],
  [75, 'Ch 17 — Focus', 'TEXT_ILLUS', 'Chapter opener: two lines of light, one stable, one flickering. Three girls holding position, intense concentration.', 'Tension, discipline, shared effort'],
  [76, 'Ch 17 continued', 'TEXT_ONLY', '', ''],
  [77, 'Ch 17 continued', 'TEXT_ONLY', '', ''],
  [78, 'Ch 17 continued', 'TEXT_ONLY', '', ''],
  [79, 'Ch 18 — Not Everything Is Visible', 'TEXT_ILLUS', 'Chapter opener: corridor with structure visible only in peripheral areas — edges, corners, between things. Soft layered light.', 'Peripheral vision, expanded awareness'],
  [80, 'Ch 18 continued', 'TEXT_ONLY', '', ''],
  [81, 'Ch 18 continued', 'TEXT_ONLY', '', ''],
  [82, 'Ch 18 continued', 'TEXT_ONLY', '', ''],
  [83, 'Ch 19 — The First Gate', 'TEXT_ILLUS', 'Chapter opener: boundary/gate visible as outline in corridor — not a door, a fold in reality. Three girls facing it.', 'Threshold, choice, awe'],
  [84, 'Ch 19 continued', 'TEXT_ONLY', '', ''],
  [85, 'Ch 19 continued', 'TEXT_ONLY', '', ''],
  [86, 'Ch 19 continued', 'TEXT_ONLY', '', ''],
  [87, 'Ch 20 — Now', 'TEXT_ILLUS', 'Chapter opener: threads of light connecting floating points, three girls stepping forward together, threads completing.', 'Decision, unity, activation'],
  [88, 'Ch 20 continued', 'TEXT_ONLY', '', ''],
  [89, 'Ch 20 continued', 'TEXT_ONLY', '', ''],
  [90, 'Ch 20 continued', 'TEXT_ONLY', '', ''],

  // PART V
  [91, 'Part V Opener — REALISING', 'ILLUS_ONLY', 'Full-page: the Academy corridor seen with full awareness — layers of structure visible everywhere, beautiful and complex. Moon tones.', 'Revelation, belonging, wonder'],
  [92, 'Ch 21 — The Academy', 'TEXT_ILLUS', 'Chapter opener: same corridor as Ch 1, but now the hidden structure is fully visible — same place, transformed perception.', 'Full circle, understanding'],
  [93, 'Ch 21 continued', 'TEXT_ONLY', '', ''],
  [94, 'Ch 21 continued', 'TEXT_ONLY', '', ''],
  [95, 'Ch 21 continued', 'TEXT_ONLY', '', ''],
  [96, 'Ch 22 — It Was Always There', 'TEXT_ILLUS', 'Chapter opener: bracelet resting still on wrist, warm light. Background shows corridor with layered structure visible.', 'Peace, completion, knowing'],
  [97, 'Ch 22 continued', 'TEXT_ONLY', '', ''],
  [98, 'Ch 22 continued', 'TEXT_ONLY', '', ''],
  [99, 'Ch 22 continued', 'TEXT_ONLY', '', ''],
  [100, 'Ch 23 — We\'re Not Imagining This', 'TEXT_ILLUS', 'Chapter opener: Lumi\'s notebook open, "Rule 4" visible, but now with additional notes in different handwriting — all three girls\' contributions.', 'Shared truth, validation'],
  [101, 'Ch 23 continued', 'TEXT_ONLY', '', ''],
  [102, 'Ch 23 continued', 'TEXT_ONLY', '', ''],
  [103, 'Ch 23 continued', 'TEXT_ONLY', '', ''],
  [104, 'Ch 24 — We Choose to See', 'TEXT_ILLUS', 'Chapter opener: the original corridor from Ch 1, but now Astra sees the symbol clearly on the wall — complete, stable, real.', 'Resolution, choice, beginning'],
  [105, 'Ch 24 continued', 'TEXT_ONLY', '', ''],
  [106, 'Ch 24 continued', 'TEXT_ONLY', '', ''],
  [107, 'Ch 24 continued', 'TEXT_ONLY', '', ''],

  // EPILOGUE
  [108, 'Epilogue Opener', 'ILLUS_ONLY', 'Full-page: empty corridor at night, something moving through it — not visible, only suggested by light bending. Bracelet on desk glowing.', 'Mysterious, watching, open-ended'],
  [109, 'Epilogue — Something Else Is Watching', 'TEXT_ILLUS', 'Inset: a different student in a different corridor, slowing down, looking at a shadow. Notebook open. First line written.', 'Cycle beginning again, hope'],
  [110, 'Epilogue continued', 'TEXT_ONLY', '', ''],
  [111, 'Epilogue continued', 'TEXT_ONLY', '', ''],
  [112, 'End of Tome I marker', 'TEXT_ILLUS', 'Small decorative element: the symbol from the story, complete and stable, centred on page. "End of Tome I" below it.', 'Closure, satisfaction'],

  // BONUS / END MATTER
  [113, 'About the Series / What\'s Next', 'TEXT_ILLUS', 'Small vignette: Lumi\'s notebook with new writing appearing on its own — teaser for Tome II.', 'Anticipation, continuation'],
  [114, 'Character Cards — Astra', 'TEXT_ILLUS', 'Portrait illustration: Astra in corridor, bracelet visible, expression alert and thoughtful. Violet tones.', 'Character identity, violet palette'],
  [115, 'Character Cards — Lumi', 'TEXT_ILLUS', 'Portrait illustration: Lumi at desk with open notebook, pen in hand, focused expression. Blue tones.', 'Character identity, blue palette'],
  [116, 'Character Cards — Nela', 'TEXT_ILLUS', 'Portrait illustration: Nela mid-movement, prism lens in hand, light splitting around her. Gold tones.', 'Character identity, gold palette'],
  [117, 'Character Cards — Prof. Quill & Mara', 'TEXT_ILLUS', 'Two smaller portraits: Prof. Quill with key, Mara looking away from something. Green and teal tones.', 'Supporting cast, mystery'],
  [118, 'Character Cards — Mr. Vale & Orion', 'TEXT_ILLUS', 'Two smaller portraits: Mr. Vale in shadow, Orion with competitive expression. Dark charcoal and amber tones.', 'Antagonist/rival energy'],
  [119, 'End Note / Colophon', 'TEXT_ONLY', 'No illustration. Clean typographic close.', 'Minimal, professional']
];

// ─── BUILD DOCUMENT ───────────────────────────────────────────────────────────
const children = [];

// TITLE PAGE
children.push(spacer(600));
children.push(h1('Illustration Brief'));
children.push(new Paragraph({
  alignment: AlignmentType.CENTER,
  spacing: { before: 60, after: 60 },
  children: [new TextRun({ text: 'Academy Beyond This World — Tome I: The Hidden System', size: 26, italics: true, color: '5A4A7A', font: 'Georgia' })]
}));
children.push(spacer(60));
children.push(rule());
children.push(spacer(60));
children.push(new Paragraph({
  alignment: AlignmentType.CENTER,
  spacing: { before: 40, after: 40 },
  children: [new TextRun({ text: 'Author: Katarzyna Kalina vel Kalinowska', size: 22, font: 'Georgia', color: '3A3A5A' })]
}));
children.push(new Paragraph({
  alignment: AlignmentType.CENTER,
  spacing: { before: 40, after: 40 },
  children: [new TextRun({ text: 'Publisher: Infinicorecipher FutureTech Education', size: 22, font: 'Georgia', color: '3A3A5A' })]
}));
children.push(new Paragraph({
  alignment: AlignmentType.CENTER,
  spacing: { before: 40, after: 40 },
  children: [new TextRun({ text: 'Document Purpose: Page-by-page illustration planning guide', size: 22, font: 'Georgia', color: '7A7A7A' })]
}));
children.push(pageBreak());

// HOW TO USE
children.push(h1('How to Use This Brief'));
children.push(spacer(40));
children.push(body('This document provides a page-by-page breakdown of the full manuscript for Tome I. Each page is classified into one of three types:'));
children.push(spacer(40));
children.push(bullet('TEXT ONLY — No illustration required. Pure typographic layout.', '2A6A2A'));
children.push(bullet('TEXT + ILLUSTRATION — Text page with an accompanying illustration (chapter opener vignette, inset, or background element).', '1A3A7A'));
children.push(bullet('ILLUSTRATION ONLY — Full-page or near-full-page illustration with minimal or no body text.', '7A5A00'));
children.push(spacer(60));
children.push(body('For TEXT + ILLUSTRATION pages, the "Illustration Notes" column describes the recommended visual content and placement. The "Visual Mood" column provides tonal guidance for the illustrator.'));
children.push(spacer(40));
children.push(body('Page numbers are approximate and based on standard trade paperback formatting (5.5" × 8.5", 11pt Georgia, 1" margins). Final pagination may shift during layout.'));
children.push(spacer(80));
children.push(rule());

// COLOUR PALETTE
children.push(spacer(60));
children.push(h2('Colour Palette Reference'));
children.push(spacer(40));
children.push(body('The following colour associations are used throughout the book and should guide illustration choices:'));
children.push(spacer(40));
children.push(bullet('Astra — Deep violet, silver-grey. Bracelet: soft silver glow.'));
children.push(bullet('Lumi — Cobalt blue, white. Notebook: clean white pages, precise blue ink.'));
children.push(bullet('Nela — Warm gold, amber. Prism lens: splits light into violet/blue/gold.'));
children.push(bullet('The System / Academy — Layered indigo-violet, with gold accents for activated states.'));
children.push(bullet('The Portal Corridor — Teal-indigo, clean light, slightly too organised.'));
children.push(bullet('Mr. Vale — Deep charcoal, cold silver. Shadows that don\'t quite behave.'));
children.push(bullet('Professor Quill — Forest green, warm gold. Quiet authority.'));
children.push(spacer(80));
children.push(rule());

// ILLUSTRATION STYLE NOTES
children.push(spacer(60));
children.push(h2('Illustration Style Notes'));
children.push(spacer(40));
children.push(body('The visual style should feel:'));
children.push(spacer(20));
children.push(bullet('Grounded and architectural — the Academy is a real place, not a fantasy realm.'));
children.push(bullet('Subtly uncanny — glitches and system effects should feel almost-normal, not overtly magical.'));
children.push(bullet('Character-focused — when characters appear, their expressions and body language carry the emotional weight.'));
children.push(bullet('Light-aware — light is a key story element. Illustrations should treat light as active, not passive.'));
children.push(bullet('Age-appropriate — the trio are young adults (approx. 14–16). Illustrations should reflect this.'));
children.push(spacer(40));
children.push(body('Chapter opener vignettes should be approximately 1/4 page width, positioned at the top-right or bottom of the opening page. Full-page illustrations (Part openers, Epilogue) should bleed to the edge.'));
children.push(spacer(80));
children.push(rule());

// SUMMARY STATISTICS
children.push(spacer(60));
children.push(h2('Summary Statistics'));
children.push(spacer(40));

const textOnly = pages.filter(p => p[2] === 'TEXT_ONLY').length;
const textIllus = pages.filter(p => p[2] === 'TEXT_ILLUS').length;
const illustOnly = pages.filter(p => p[2] === 'ILLUS_ONLY').length;
const total = pages.length;

children.push(body(`Total pages: ${total}`));
children.push(bullet(`Text Only: ${textOnly} pages (${Math.round(textOnly/total*100)}%)`));
children.push(bullet(`Text + Illustration: ${textIllus} pages (${Math.round(textIllus/total*100)}%)`));
children.push(bullet(`Illustration Only: ${illustOnly} pages (${Math.round(illustOnly/total*100)}%)`));
children.push(spacer(40));
children.push(body(`Total illustrations required: ${textIllus + illustOnly} (${textIllus} vignettes/insets + ${illustOnly} full-page)`));
children.push(pageBreak());

// PAGE-BY-PAGE TABLE
children.push(h1('Page-by-Page Breakdown'));
children.push(spacer(60));

// Build table
const tableRows = [tableHeader()];
pages.forEach(p => {
  if (p[3] !== '' || p[2] !== 'TEXT_ONLY') {
    tableRows.push(pageRow(p[0], p[1], p[2], p[2], p[3], p[4]));
  } else {
    tableRows.push(pageRow(p[0], p[1], p[2], p[2], '—', '—'));
  }
});

children.push(new Table({
  width: { size: 100, type: WidthType.PERCENTAGE },
  rows: tableRows
}));

children.push(spacer(120));
children.push(rule());
children.push(spacer(80));
children.push(new Paragraph({
  alignment: AlignmentType.CENTER,
  children: [new TextRun({ text: 'End of Illustration Brief — Academy Beyond This World, Tome I', size: 20, italics: true, color: '7A6A9A', font: 'Georgia' })]
}));

// ─── HEADER / FOOTER ─────────────────────────────────────────────────────────
const runningHeader = new Header({
  children: [
    new Paragraph({
      alignment: AlignmentType.CENTER,
      border: { bottom: { color: 'C0A96E', size: 4, style: 'single' } },
      spacing: { after: 60 },
      children: [
        new TextRun({ text: 'ILLUSTRATION BRIEF  —  Academy Beyond This World, Tome I', size: 18, italics: true, color: '7A6A9A', font: 'Georgia' })
      ]
    })
  ]
});

const pageFooter = new Footer({
  children: [
    new Paragraph({
      alignment: AlignmentType.CENTER,
      border: { top: { color: 'C0A96E', size: 4, style: 'single' } },
      spacing: { before: 60 },
      children: [
        new TextRun({ children: [PageNumber.CURRENT], size: 18, color: '7A6A9A', font: 'Georgia' })
      ]
    })
  ]
});

const doc = new Document({
  sections: [{
    properties: {
      page: {
        margin: { top: 1080, bottom: 1080, left: 1080, right: 1080 }
      }
    },
    headers: { default: runningHeader },
    footers: { default: pageFooter },
    children
  }]
});

const outPath = path.join(__dirname, 'document.docx');
Packer.toBuffer(doc).then(buffer => {
  fs.writeFileSync(outPath, buffer);
  const kb = Math.round(buffer.length / 1024);
  console.log(`SUCCESS: illustration brief written (${kb} KB)`);
}).catch(err => {
  console.error('ERROR:', err.message);
  process.exit(1);
});