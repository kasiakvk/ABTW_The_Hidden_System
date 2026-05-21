const h = require('./helpers');

function buildPart1() {
  const c = [];

  // PART I OPENER
  c.push(h.spacer(600));
  c.push(h.sectionTitle('🟣 PART I'));
  c.push(h.centeredText('NOTICING', 36, true, '5A4A7A'));
  c.push(h.spacer(200));
  c.push(h.epigraph('Some things are always there.'));
  c.push(h.epigraph('You just haven’t learned how to see them yet.'));
  c.push(h.pageBreak());

  // CH 1
  const ch1 = [
    'Astra slowed down—not because she was tired, and not because anything blocked her way, but because something didn’t feel right.',
    'The corridor looked exactly the same as always: pale walls holding the day’s warmth for a little longer, polished stone catching late afternoon light in long, thinning bands, the quiet echo of footsteps moving somewhere beyond sight. Outside the tall windows, the sky held a washed, steady blue, softened in the way it became when the season leaned toward longer evenings.',
    'Everything was where it should be.',
    'And still, just for a moment, Astra stopped.',
    'She didn’t know why at first. There wasn’t a clear thought attached to the movement; only a shift inside her—the part that noticed things before it understood them quietly redirected her attention downward, toward the floor.',
    'The shadow near her feet did not move.',
    'Astra shifted her weight, heel lifting and settling again. The shadow stayed still. She frowned. That wasn’t possible. She took a careful step forward, and the shadow moved again—smoothly, correctly—folding back into place as if the corridor had corrected itself before the mistake had time to exist.',
    'Astra watched it a second longer than necessary, then kept walking.',
    'A little further on, where the corridor opened toward a row of tall windows, it happened again. Not the shadow this time—something else. She passed the glass slowly, her reflection sliding alongside her, matching her pace and posture. For a second—barely a second—she thought she saw it: not movement, but alignment, as if the space behind the glass hesitated and then quietly put itself back together.',
    'She stopped, turned, and looked directly at the window.',
    'Nothing.',
    'Just sky—soft blue, calm, untroubled. She leaned closer. Her reflection leaned with her. Every line matched; every angle held.',
    '“Are you coming?”',
    'The voice broke the stillness behind her. Another student stood a few steps away, already drifting farther down the corridor. Astra nodded. “Yeah.”',
    'She glanced back at the window once more. No flicker. No hesitation.',
    'She told herself it was nothing. People noticed things sometimes—light softened, eyes grew tired during longer afternoons, shadows stretched strangely when the year shifted. There were explanations. There were always explanations.',
    'Still, she found herself walking more slowly now—not because she meant to, but because she was watching.',
    'The corridor widened into a busier stretch of the Academy. Voices layered over one another: lockers closing, fabric brushing fabric, laughter rising and falling in uneven bursts. Everything felt normal again. Perfect. Too perfect.',
    'Someone dropped a pen. It struck the floor once and rolled across the stone in a clean, predictable line. Astra’s eyes followed it without thinking. The pen stopped—perfectly straight, perfectly still.',
    'She looked away.',
    'She didn’t notice when her hand moved, only the moment her fingers closed around the bracelet at her wrist. Cool metal against warm skin. Familiar. Always in the same place.',
    'Near the far end of the corridor, just before a turn that led away from the busiest routes, Astra slowed again. Something on the wall ahead felt wrong. Not obvious. Not cracked or damaged. Nothing she could point to without sounding foolish.',
    'But if she stared long enough—just long enough—something began to form: not fully, not clearly, but as a suggestion of shape, faint and incomplete, as if it hadn’t decided whether to exist in this moment or the one beside it.',
    'A symbol.',
    'Structured. Deliberate. Not glowing, not drawn.',
    'Then it vanished.',
    'Astra blinked. The wall was empty again—flat, solid, real. Her heartbeat picked up, not with fear but with something sharper: a bright, focused alertness.',
    '“Okay…” she whispered.',
    'She sat down and opened her notebook. The page was blank. She held her pencil for a moment, then wrote:',
    'Something is not right.',
    'Below it, after a pause:',
    'And I think I’m the only one who can see it.',
    'She closed the notebook.',
    'Around her, the Academy continued exactly as it should. Perfect. Too perfect.',
    'Far behind her, in the corridor she had just left, the pattern remained—clear, still—waiting with the patience of something that understood time differently.'
  ];
  h.chapterBlock(1, 'Something Is Not Right', '🌙 first blur', ch1).forEach(b => c.push(b));

  // CH 2
  const ch2 = [
    'Astra told herself she wouldn’t look for it again—at least not yet, and not on purpose.',
    'It had already happened once. That should have been enough. She repeated that thought as she walked the corridor the next day, as if routine could smooth out the sharp edge of what she’d seen. The Academy looked ordinary in the way it always did, but ordinary felt different once you had started paying attention.',
    'It was still afternoon—late enough that the light no longer felt bright, only long. The sun slipped in through the windows at a lower angle, turning parts of the stone floor into pale mirrors and others into dull shadow.',
    'The walls were the same. The windows were the same. The light stretched the same way across the corridor.',
    'Nothing looked different. Nothing she could point to.',
    'And that was exactly what made it harder.',
    'If something is clearly wrong, you can see it. You can react. You can name it. But when everything looks normal—when the world keeps behaving as if nothing has changed—there is nothing to push against. There is only the quiet sense that something is misaligned, and that you might be the only one who has noticed.',
    'Astra slowed down.',
    'The shadow.',
    'There.',
    'She stopped.',
    'The shadow wasn’t following her. It didn’t lag this time. It didn’t flicker. It simply stayed, pressed against the floor as if it belonged to an earlier moment and hadn’t been told to move on.',
    'Astra shifted her weight subtly to the left. Her body moved; the light adjusted; the space responded. The shadow didn’t—it remained exactly where it had been.',
    'Astra held still. The corridor hummed with distant movement—voices somewhere down the hall, lockers closing, the soft scrape of shoes—but the space around her felt quieter, as if sound itself had stepped back just enough to make room for something else.',
    'Then, very carefully, she stepped forward.',
    'Now the shadow moved again—perfectly aligned, perfectly normal—sliding into place as if the corridor had corrected itself before anyone could notice the mistake.',
    '“That…” she whispered, then stopped.',
    'There wasn’t a sentence that fit.',
    'She looked around. Students passed her without slowing, talking and moving inside the same light as if it belonged to them entirely. No one looked down. No one paused. No one saw anything.',
    '“It’s not random,” she said quietly.',
    'She didn’t try to explain it. Something inside her settled deeper, firmer—the way something does when it finally finds the right shape.',
    'This wasn’t a mistake. This wasn’t her imagination. It followed something.',
    'She just didn’t know what yet.',
    'Astra kept walking. This time she didn’t try to ignore it or control it; she simply noticed. Behind her, the light stretched across the floor again, softer now, more patient as the day approached its threshold.',
    'Everything returned to balance. Perfect. Stable. Unchanged.',
    'Except for a moment—too small for anyone else to catch—when the shadow hesitated…',
    '…and then caught up.'
  ];
  h.chapterBlock(2, 'The Shadow That Didn’t Move', '🌙 second blur', ch2).forEach(b => c.push(b));

  // CH 3
  const ch3 = [
    'Astra didn’t expect it to happen again so quickly. That should have made it easier to ignore.',
    'It didn’t.',
    'If anything, it made her more careful.',
    'The corridor looked the same as before, but now she noticed how late-day light rested instead of moving on. It stretched across the floor in long, shallow bands, softened by the season, as if the building preferred reflection over motion.',
    'She walked the corridor the same way as before, keeping the same pace and the same path.',
    'At least, she tried to.',
    'Her steps slowed without asking her permission—not enough for anyone else to notice, just enough for her to feel it. The difference wasn’t speed so much as intention, like the corridor had gained a second rhythm and she was testing whether she could hear it.',
    'Don’t watch it.',
    'She told herself that clearly and deliberately. If she wasn’t looking, then whatever it was wouldn’t matter.',
    'She took one step and nothing happened. Another followed—still nothing.',
    'Astra almost relaxed.',
    'Maybe it had been random after all. Maybe the first two times had lined up by chance, shaped by tired eyes and afternoons that seemed to stretch farther than they should.',
    'Maybe—',
    'The light shifted.',
    'Not visibly. Not in a way anyone passing by would notice.',
    'But Astra saw it.',
    'The edge of the light moved too late—no more than a fraction of a second, just enough to break the rhythm.',
    'Astra stopped. Not sharply this time. Not with surprise.',
    'With expectation.',
    '“There,” she whispered.',
    'She tilted her head slightly. The angle changed, and for a split second the surface wasn’t flat. It formed something—nothing clear or complete, but enough.',
    'A pattern.',
    'Astra didn’t move. She didn’t blink or change her breathing if she could help it.',
    'She simply watched.',
    'The pattern flickered, almost gone, then held—longer than before.',
    '“It’s longer,” she whispered. “Longer than yesterday.”',
    'Her hand lifted instinctively, not reaching or touching, just moving closer, as if proximity alone might matter.',
    'The pattern didn’t disappear right away. It reacted—not by moving, but by becoming clearer.',
    '“You’re doing that on purpose,” she said quietly.',
    'The pattern flickered once, then snapped back.',
    'Gone.',
    '“That’s twice,” she said, then paused and shook her head slightly. “No. More than that.”',
    'The shadow, the window, the paper, and the wall—different places and different moments, but the same kind of break and the same kind of correction.',
    '“It repeats.”',
    '“Okay,” she whispered. “Then I’ll find it.”',
    'She turned and walked forward, this time not trying to ignore it and not trying to force it—just watching more carefully.',
    'Behind her, for the briefest moment, the pattern returned, fainter than before and less stable but still there.',
    'Not random. Waiting.'
  ];
  h.chapterBlock(3, 'The Second Time', '🌙 third blur', ch3).forEach(b => c.push(b));

  // CH 4
  const ch4 = [
    'Astra waited—not for the glitch, not exactly, but for a moment when it could happen and when someone else would be there. That part mattered now. It wasn’t enough to see it herself; she needed to know whether anyone else could.',
    'The corridor filled slowly with movement as the afternoon drifted closer to evening. Voices layered over one another, footsteps crossed in uneven patterns, and somewhere nearby someone laughed too loudly before stopping mid-sentence.',
    '“Why are you just standing there?”',
    'Astra turned slightly. Another student had stopped beside her, adjusting the strap of their bag.',
    '“Hey,” Astra said, forcing her voice to stay casual. “Can you just… look at that for a second?”',
    'She gestured lightly toward the wall—not pointing directly, just enough to suggest a direction.',
    'The other student frowned. “At what?”',
    '“Just the wall,” she said.',
    'The student followed her gesture and looked properly.',
    'Nothing happened.',
    'The student shrugged. “It’s a wall.”',
    'Astra blinked. “Yeah,” she said too quickly. “I know. I just thought—” She stopped. “That it looked weird,” she finished instead.',
    '“It looks the same as always.”',
    '“Are you okay?”',
    'Astra nodded immediately—too fast, too sharp. “Yeah. Yeah, I’m fine.”',
    'The student nodded back, uncertain, and then walked on.',
    'Astra stayed where she was. The wall in front of her didn’t change, didn’t react, didn’t even flicker at the edges.',
    'She exhaled slowly—not frustrated, not yet, just thinking.',
    'She returned to her original position.',
    'And then, just for a moment, the shadow lagged.',
    '“There,” she said quickly.',
    'Too late.',
    'The shadow snapped back into place, perfectly aligned.',
    '“Did you see that?” she asked, louder now.',
    'Someone nearby turned. “See what?”',
    'This time Astra pointed clearly and directly. “There. The shadow—it just—”',
    'She stopped.',
    'Everything was normal again.',
    '“I didn’t see anything.”',
    'Astra lowered her hand slowly. “Yeah,” she said quietly. “Me neither.”',
    'Something tightened in her chest—not fear and not panic, but something smaller and heavier.',
    'Doubt.',
    '“You did see it,” she murmured to herself.',
    'She closed her eyes briefly, then opened them again.',
    'So she changed something.',
    'Astra turned away and walked without looking. Three steps. Four. Five.',
    'Then she stopped—suddenly, not because she saw anything, but because she felt it: the same shift, the same quiet pause in the space, like a held breath.',
    'She turned slowly.',
    'There it was.',
    'The edge of the wall didn’t quite hold. The shadow moved a fraction too late. The pattern began to form—faint, incomplete, but unmistakable.',
    'This time she said nothing. She didn’t call anyone, didn’t point, didn’t move; she just watched.',
    'The moment held—long enough, clear enough, real.',
    'Then it snapped back again, perfect, as if nothing had ever happened.',
    '“It’s not that they can’t see it,” she whispered. “They just… don’t.”',
    'The thought stayed. It didn’t dissolve under doubt.',
    'Astra nodded once. “That’s different.”',
    '“Okay,” she said quietly. “Then I’ll watch for both of us.”',
    'She turned and walked on.',
    'Behind her, for just a moment, the shadow hesitated. No one said anything. No one stopped. No one noticed.',
    'Except Astra.'
  ];
  h.chapterBlock(4, 'Did You See That?', '🌙 fourth blur', ch4).forEach(b => c.push(b));

  // CH 5
  const ch5 = [
    'Astra didn’t stop noticing. She tried—at first.',
    'The next day she walked differently. She kept her pace steady and her gaze forward, refusing to look at the floor, the walls, or the edges of light where it had appeared before. She moved with intention, telling herself that if she ignored it, then whatever it was would stop.',
    'It didn’t work.',
    'Because once you notice something, you don’t unnotice it.',
    'The corridor felt the same. Everything still looked right, ordinary in the precise, careful way the Academy always was. Late-afternoon light rested along the stone floor, softer now, thinner and more patient as the season leaned toward evening.',
    'Astra could feel where to look.',
    'Not directly. Never directly. Just slightly before something changed.',
    'She slowed at the usual place—not fully and not enough to draw attention, just enough to loosen the moment. And there it was. Not the shadow this time and not the light itself, but something smaller.',
    'The edge of a locker door didn’t line up. For a moment—just a moment—it sat too far forward, like a decision made half a second too late. Astra blinked, and it snapped back into place, perfect again.',
    'She walked past without stopping. “That’s new,” she whispered—then corrected herself almost immediately. Not new. Different. Another version of the same thing.',
    'She moved farther down the corridor and felt another shift. This time someone’s reflection passed across a glass surface, except that for half a second the reflection didn’t match the movement. It lagged, too slow, detached from the body it should have followed, before aligning perfectly again.',
    'Astra stopped.',
    'Three places now: shadow, light, surface. Different forms, the same behaviour.',
    '“It repeats,” she said again, but this time the words meant something else. Not just that it happened more than once, but that it followed something—a tendency, a rhythm.',
    'Astra leaned lightly against the wall without looking at anything directly, letting her eyes soften and her attention widen instead of narrowing. That’s when she saw it—not on the wall or on the floor, but between things.',
    'The space between one point and another felt uneven, like the world had folded improperly and corrected itself too neatly. She tilted her head just slightly, and the sensation sharpened. For a brief moment, lines appeared: not complete, not stable, but connected.',
    'A pattern.',
    'Astra didn’t move. She didn’t try to hold it or force it into clarity. She let it exist. The pattern clarified—not as something drawn or glowing, not as an image the eye could fully grasp, but as a structure. Too ordered to be random.',
    '“You’re the same,” she said quietly—not to the wall or the space around it, but to the pattern itself. Shadow, light, surface, alignment: different expressions of the same system.',
    'The lines flickered, then disappeared.',
    'Astra pushed away from the wall and straightened. It didn’t matter that it had vanished; that wasn’t the important part anymore. What mattered was that it behaved the same way every time.',
    'At the doorway at the end of the hall, she paused. Not because she needed to, but because she wanted to confirm it one last time.',
    'The corridor stretched behind her, quiet and unchanged. For a moment, nothing happened. Then, very faintly, the shadow near the lockers didn’t move. A reflection lagged. The space between two edges bent.',
    'All at once. In multiple places. With the same behaviour and the same correction.',
    '“It’s not separate,” she whispered. Not one glitch. Not one mistake. One pattern. A system.',
    'And it wasn’t part of the normal world.',
    '“Okay,” she said quietly. “If you don’t belong here…” She paused, then finished the thought. “Then what are you part of?”',
    'No answer came, and this time she didn’t expect one.',
    'This wasn’t random. This wasn’t imagination. This wasn’t breaking.',
    'It was something else.',
    'And whatever it was, it followed rules she hadn’t learned yet.'
  ];
  h.chapterBlock(5, 'A Pattern That Doesn’t Belong', '🌙 fifth blur', ch5).forEach(b => c.push(b));

  return c;
}

module.exports = { buildPart1 };
