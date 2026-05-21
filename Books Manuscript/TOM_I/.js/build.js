const docx = require('/usr/lib/node_modules/docx');
const {
  Document, Packer, Paragraph, TextRun, AlignmentType,
  Header, Footer, PageNumber, NumberFormat, SectionType,
  TabStopType, TabStopPosition
} = docx;
const fs = require('fs');
const path = require('path');

const { buildFrontMatter } = require('./front_matter');
const { buildPart1 } = require('./chapters_1_5');
const { buildPart2 } = require('./chapters_6_10');
const { buildPart3 } = require('./chapters_11_15');
const { buildPart4, buildPart5 } = require('./chapters_16_24');
const { buildEpilogue, buildEndMatter } = require('./epilogue_and_endmatter');

// ─── ASSEMBLE ALL CONTENT ────────────────────────────────────────────────────
const allChildren = [
  ...buildFrontMatter(),
  ...buildPart1(),
  ...buildPart2(),
  ...buildPart3(),
  ...buildPart4(),
  ...buildPart5(),
  ...buildEpilogue(),
  ...buildEndMatter()
];

// ─── HEADER / FOOTER ─────────────────────────────────────────────────────────
const runningHeader = new Header({
  children: [
    new Paragraph({
      alignment: AlignmentType.CENTER,
      border: { bottom: { color: 'C0A96E', size: 4, style: 'single' } },
      spacing: { after: 60 },
      children: [
        new TextRun({
          text: 'ACADEMY BEYOND THIS WORLD  —  The Hidden System',
          size: 18,
          italics: true,
          color: '7A6A9A',
          font: 'Georgia'
        })
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
        new TextRun({
          children: [PageNumber.CURRENT],
          size: 18,
          color: '7A6A9A',
          font: 'Georgia'
        })
      ]
    })
  ]
});

// ─── BUILD DOCUMENT ──────────────────────────────────────────────────────────
const doc = new Document({
  numbering: { config: [] },
  sections: [
    {
      properties: {
        page: {
          margin: { top: 1440, bottom: 1440, left: 1260, right: 1260 },
          pageNumbers: { start: 1, formatType: NumberFormat.DECIMAL }
        }
      },
      headers: { default: runningHeader },
      footers: { default: pageFooter },
      children: allChildren
    }
  ]
});

// ─── WRITE FILE ───────────────────────────────────────────────────────────────
const outPath = path.join(__dirname, 'document.docx');
Packer.toBuffer(doc).then(buffer => {
  fs.writeFileSync(outPath, buffer);
  const kb = Math.round(buffer.length / 1024);
  console.log(`SUCCESS: document.docx written (${kb} KB)`);
}).catch(err => {
  console.error('ERROR:', err.message);
  process.exit(1);
});
