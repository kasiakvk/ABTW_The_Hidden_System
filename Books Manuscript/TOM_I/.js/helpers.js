// ─── HELPERS ────────────────────────────────────────────────────────────────
const docx = require('/usr/lib/node_modules/docx');
const {
  Paragraph, TextRun, AlignmentType, PageBreak, BorderStyle
} = docx;

function pageBreak() {
  return new Paragraph({ children: [new PageBreak()] });
}

function spacer(pts = 120) {
  return new Paragraph({ spacing: { before: pts, after: pts }, children: [] });
}

function rule() {
  return new Paragraph({
    border: { bottom: { color: 'C0A96E', size: 6, style: BorderStyle.SINGLE } },
    spacing: { before: 80, after: 80 },
    children: []
  });
}

function centeredText(text, size = 24, bold = false, color = '1A1A2E') {
  return new Paragraph({
    alignment: AlignmentType.CENTER,
    spacing: { before: 60, after: 60 },
    children: [new TextRun({ text, size, bold, color, font: 'Georgia' })]
  });
}

function sectionTitle(text) {
  return new Paragraph({
    alignment: AlignmentType.CENTER,
    spacing: { before: 240, after: 120 },
    children: [new TextRun({ text, size: 32, bold: true, color: '2C3E7A', font: 'Georgia', allCaps: true })]
  });
}

function chapterTitle(text) {
  return new Paragraph({
    spacing: { before: 200, after: 80 },
    children: [new TextRun({ text, size: 28, bold: true, color: '2C3E7A', font: 'Georgia' })]
  });
}

function chapterSubtitle(text) {
  return new Paragraph({
    spacing: { before: 40, after: 120 },
    children: [new TextRun({ text, size: 20, italics: true, color: '7A6A9A', font: 'Georgia' })]
  });
}

function bodyPara(text, indent = false) {
  return new Paragraph({
    spacing: { before: 60, after: 60, line: 360, lineRule: 'auto' },
    indent: indent ? { firstLine: 360 } : {},
    children: [new TextRun({ text, size: 22, font: 'Georgia', color: '1A1A2E' })]
  });
}

function italicPara(text) {
  return new Paragraph({
    spacing: { before: 60, after: 60, line: 360, lineRule: 'auto' },
    alignment: AlignmentType.CENTER,
    children: [new TextRun({ text, size: 22, italics: true, font: 'Georgia', color: '5A4A7A' })]
  });
}

function epigraph(text) {
  return new Paragraph({
    spacing: { before: 120, after: 120, line: 360, lineRule: 'auto' },
    indent: { left: 720, right: 720 },
    alignment: AlignmentType.CENTER,
    children: [new TextRun({ text, size: 22, italics: true, font: 'Georgia', color: '5A4A7A' })]
  });
}

function cardHeader(emoji, name) {
  return new Paragraph({
    spacing: { before: 160, after: 60 },
    children: [new TextRun({ text: emoji + ' CHARACTER CARD — ' + name, size: 26, bold: true, color: '2C3E7A', font: 'Georgia', allCaps: true })]
  });
}

function cardField(label, value) {
  return new Paragraph({
    spacing: { before: 40, after: 40 },
    children: [
      new TextRun({ text: label + ': ', size: 21, bold: true, font: 'Georgia', color: '2C3E7A' }),
      new TextRun({ text: value, size: 21, font: 'Georgia', color: '1A1A2E' })
    ]
  });
}

function tocEntry(num, title, page) {
  return new Paragraph({
    spacing: { before: 40, after: 40 },
    children: [
      new TextRun({ text: num ? `${num}. ${title}` : title, size: 21, font: 'Georgia', color: '1A1A2E' }),
      new TextRun({ text: `  ....  ${page}`, size: 21, font: 'Georgia', color: '7A7A7A' })
    ]
  });
}

function tocPart(label) {
  return new Paragraph({
    spacing: { before: 120, after: 40 },
    children: [new TextRun({ text: label, size: 23, bold: true, font: 'Georgia', color: '2C3E7A' })]
  });
}

function bullet(text) {
  return new Paragraph({
    spacing: { before: 40, after: 40 },
    indent: { left: 360 },
    children: [
      new TextRun({ text: '• ', size: 21, bold: true, font: 'Georgia', color: 'C0A96E' }),
      new TextRun({ text, size: 21, font: 'Georgia', color: '1A1A2E' })
    ]
  });
}

function chapterBlock(num, title, subtitle, paragraphs) {
  const blocks = [
    pageBreak(),
    chapterTitle(`Chapter ${num} — ${title}`),
    chapterSubtitle(subtitle),
    rule()
  ];
  paragraphs.forEach(p => blocks.push(bodyPara(p, true)));
  blocks.push(italicPara('✦'));
  return blocks;
}

module.exports = {
  pageBreak, spacer, rule, centeredText, sectionTitle,
  chapterTitle, chapterSubtitle, bodyPara, italicPara, epigraph,
  cardHeader, cardField, tocEntry, tocPart, bullet, chapterBlock
};

