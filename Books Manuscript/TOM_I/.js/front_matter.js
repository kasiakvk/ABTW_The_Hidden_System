const h = require('./helpers');

function buildFrontMatter() {
  const c = [];

  // HALF-TITLE
  c.push(h.spacer(800));
  c.push(h.centeredText('ACADEMY BEYOND THIS WORLD', 52, true, '2C3E7A'));
  c.push(h.spacer(40));
  c.push(h.centeredText('TOM I', 36, false, '7A6A9A'));
  c.push(h.spacer(40));
  c.push(h.centeredText('The Hidden System', 30, false, '5A4A7A'));
  c.push(h.spacer(600));
  c.push(h.centeredText('Katarzyna Kalina vel Kalinowska', 24, false, '3A3A5A'));
  c.push(h.pageBreak());

  // COPYRIGHT
  c.push(h.spacer(400));
  c.push(h.centeredText('ACADEMY BEYOND THIS WORLD', 28, true, '2C3E7A'));
  c.push(h.centeredText('TOM I: The Hidden System', 24, false, '5A4A7A'));
  c.push(h.spacer(120));
  c.push(h.rule());
  c.push(h.spacer(80));
  c.push(h.bodyPara('Copyright © 2026 by Katarzyna Kalina vel Kalinowska'));
  c.push(h.bodyPara('All rights reserved. No part of this publication may be reproduced, stored in a retrieval system, or transmitted in any form or by any means — electronic, mechanical, photocopying, recording, or otherwise — without prior written permission of the publisher or author, except for brief quotations used in reviews or educational contexts.'));
  c.push(h.spacer(60));
  c.push(h.bodyPara('This is a work of fiction. Names, characters, places, events, and incidents are products of the author’s imagination or used fictitiously. Any resemblance to actual persons, living or dead, or real events is purely coincidental.'));
  c.push(h.spacer(60));
  c.push(h.bodyPara('Published by: Infinicorecipher FutureTech Education'));
  c.push(h.bodyPara('First Edition'));
  c.push(h.bodyPara('Printed in the United Kingdom'));
  c.push(h.bodyPara('ISBN: ___________________________'));
  c.push(h.spacer(60));
  c.push(h.rule());
  c.push(h.spacer(60));
  c.push(h.centeredText('Observe. Understand.', 22, false, '7A6A9A'));
  c.push(h.pageBreak());

  // EPIGRAPH
  c.push(h.spacer(600));
  c.push(h.epigraph('What if nothing around you needs to change…'));
  c.push(h.epigraph('For you to start seeing something completely different?'));
  c.push(h.spacer(200));
  c.push(h.rule());
  c.push(h.spacer(200));
  c.push(h.epigraph('Some worlds wait to be noticed.'));
  c.push(h.pageBreak());

  // BLURB
  c.push(h.spacer(200));
  c.push(h.sectionTitle('About This Book'));
  c.push(h.spacer(80));
  c.push(h.bodyPara('Astra begins to notice things that don’t quite belong.'));
  c.push(h.bodyPara('Shadows that don’t move. Moments that don’t repeat. Patterns that appear—and disappear—before anyone else can see them.'));
  c.push(h.bodyPara('At first, she doubts herself.'));
  c.push(h.bodyPara('But then she meets Lumi, who searches for answers through logic, and Nela, who trusts what she feels before she understands it.'));
  c.push(h.bodyPara('Together, they begin to uncover something hidden within the ordinary world: a system that responds not to force, but to attention, understanding, and connection.'));
  c.push(h.bodyPara('As they learn to see more, they realise something even more important—'));
  c.push(h.bodyPara('They were never outside of it.'));
  c.push(h.pageBreak());

  // TABLE OF CONTENTS
  c.push(h.sectionTitle('Table of Contents'));
  c.push(h.spacer(80));
  c.push(h.tocPart('🟣 PART I — NOTICING'));
  c.push(h.tocEntry('1', 'Something Is Not Right', '7'));
  c.push(h.tocEntry('2', 'The Shadow That Didn’t Move', '11'));
  c.push(h.tocEntry('3', 'The Second Time', '15'));
  c.push(h.tocEntry('4', 'Did You See That?', '19'));
  c.push(h.tocEntry('5', 'A Pattern That Doesn’t Belong', '23'));
  c.push(h.tocPart('🔵 PART II — UNDERSTANDING'));
  c.push(h.tocEntry('6', 'Lumi', '29'));
  c.push(h.tocEntry('7', 'The Notebook That Doesn’t Match', '33'));
  c.push(h.tocEntry('8', 'There Are Rules', '37'));
  c.push(h.tocEntry('9', 'Nela', '41'));
  c.push(h.tocEntry('10', '“Don’t Ask. Just Hold This.”', '45'));
  c.push(h.tocPart('🟡 PART III — CONNECTING'));
  c.push(h.tocEntry('11', 'The First Symbol', '51'));
  c.push(h.tocEntry('12', 'If It Means Something…', '55'));
  c.push(h.tocEntry('13', 'Try It Again', '59'));
  c.push(h.tocEntry('14', 'It Reacted', '63'));
  c.push(h.tocEntry('15', 'The Three of Us', '67'));
  c.push(h.tocPart('🌌 PART IV — ACTIVATING'));
  c.push(h.tocEntry('16', 'The Corridor Changes', '73'));
  c.push(h.tocEntry('17', 'Focus', '77'));
  c.push(h.tocEntry('18', 'Not Everything Is Visible', '81'));
  c.push(h.tocEntry('19', 'The First Gate', '85'));
  c.push(h.tocEntry('20', 'Now', '89'));
  c.push(h.tocPart('🌙 PART V — REALISING'));
  c.push(h.tocEntry('21', 'The Academy', '95'));
  c.push(h.tocEntry('22', 'It Was Always There', '99'));
  c.push(h.tocEntry('23', 'We’re Not Imagining This', '103'));
  c.push(h.tocEntry('24', 'We Choose to See', '107'));
  c.push(h.tocPart('✨ EPILOGUE'));
  c.push(h.tocEntry('', 'Something Else Is Watching', '113'));
  c.push(h.spacer(80));
  c.push(h.rule());
  c.push(h.spacer(40));
  c.push(h.tocPart('BONUS PAGES'));
  c.push(h.tocEntry('', 'Character Cards — The Trio', '117'));
  c.push(h.tocEntry('', 'Character Cards — Supporting Cast', '121'));
  c.push(h.tocEntry('', 'What’s Next', '127'));
  c.push(h.pageBreak());

  return c;
}

module.exports = { buildFrontMatter };
