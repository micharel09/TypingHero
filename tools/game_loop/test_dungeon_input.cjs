'use strict';
const test = require('node:test');
const assert = require('node:assert/strict');
const vm = require('node:vm');
const { readyRemaining, observedKey, observedClick, selectAllObserved, continueReady, installObservationJSON } = require('./dungeon_input.cjs');
const prompt = (current = 'READY', frame = 1) => ({ scene: 'SampleScene', endless: true,
  finished: true, hp: 35, enemyHp: -1, defeated: 1, endlessSlot: 1, current, frame });
const active = frame => ({ ...prompt('BLADE', frame), finished: false, endlessSlot: 1,
  enemyHp: 200, timeScale: 1 });

test('READY recognizes the full prompt and every remaining suffix only between live duels', () => {
  for (const word of ['READY', 'EADY', 'ADY', 'DY', 'Y', 'rEaDy']) assert.equal(readyRemaining(prompt(word)), word.toUpperCase());
  for (const delta of [{ current: 'READ' }, { current: '' }, { current: 'BLADE' }, { hp: 0 },
    { finished: false }, { defeated: 0 }, { defeated: 6 }, { endless: false }, { scene: 'MainMenu' },
    { introRunning: true }, { dungeonTransitioning: true }, { paused: true }, { encounterReading: true }]) {
    assert.equal(readyRemaining({ ...prompt(), ...delta }), null);
  }
});

function fixture(initial, publications) {
  const keys = [], predicates = [], events = [];
  return { keys, predicates, events, page: { keyboard: {
    down: async key => { keys.push(key); events.push('down:' + key); },
    up: async key => events.push('up:' + key)
  } }, api: {
    state: async () => initial,
    until: async (_, accepts) => {
      predicates.push(accepts);
      while (publications.length) {
        const s = publications.shift();
        if (s instanceof Error) throw s;
        if (accepts(s)) return s;
      }
      throw Error('observation timeout');
    }
  } };
}

test('full READY acknowledges each letter in a fresh publication and waits for active next duel', async () => {
  const f = fixture(prompt(), [prompt(), prompt('EADY', 2), prompt('ADY', 3), prompt('DY', 4), prompt('Y', 5),
    prompt('', 6), { ...active(7), dungeonTransitioning: true }, { ...active(8), introRunning: true },
    { ...active(9), timeScale: 0 }, active(10)]);
  const result = await continueReady(f.api, f.page);
  assert.equal(result.frame, 10); assert.deepEqual(f.keys, Array.from('READY'));
  assert.deepEqual(f.events, Array.from('READY').flatMap(k => ['down:' + k, 'up:' + k]));
});

test('a partially typed READY sends only its remaining letters', async () => {
  const f = fixture(prompt('DY'), [prompt('Y', 2), prompt('', 3), active(4)]);
  await continueReady(f.api, f.page);
  assert.deepEqual(f.keys, ['D', 'Y']);
});

test('stale or unchanged observations never trigger another READY letter', async () => {
  const f = fixture(prompt(), [prompt('EADY'), prompt('READY', 2), Error('stale timeout')]);
  await assert.rejects(continueReady(f.api, f.page), /stale timeout/);
  assert.deepEqual(f.keys, ['R']); assert.deepEqual(f.events, ['down:R', 'up:R']);
});

test('a scene change before acknowledgement stops input and releases the held letter', async () => {
  const f = fixture(prompt(), [{ ...prompt('EADY', 2), scene: 'MainMenu' }, Error('scene timeout')]);
  await assert.rejects(continueReady(f.api, f.page), /scene timeout/);
  assert.deepEqual(f.keys, ['R']); assert.deepEqual(f.events, ['down:R', 'up:R']);
});

test('the last letter can enter a loading scene without leaking input to the next duel', async () => {
  const f = fixture(prompt('Y'), [{ ...prompt('', 2), scene: 'Loading' }, active(3)]);
  assert.equal((await continueReady(f.api, f.page)).frame, 3);
  assert.deepEqual(f.keys, ['Y']);
});

test('terminal victory and player death cannot send READY or Enter', async () => {
  for (const initial of [{ ...prompt(), defeated: 6 }, { ...prompt(), hp: 0 }]) {
    const f = fixture(initial, []);
    await assert.rejects(continueReady(f.api, f.page), /live intermediate-duel/);
    assert.deepEqual(f.keys, []);
  }
});

test('guide Enter is held across observed frames and always released on error', async () => {
  const f = fixture(prompt(), [prompt('READY', 2), Error('guide error')]);
  await assert.rejects(observedKey(f.page, 'Enter', f.api.until, s => s.frame > 2), /guide error/);
  assert.deepEqual(f.events, ['down:Enter', 'up:Enter']);
});

test('keyboard-down failure still attempts key release', async () => {
  const events = [], page = { keyboard: { down: async () => { throw Error('keyboard error'); }, up: async key => events.push(key) } };
  await assert.rejects(observedKey(page, 'R', async () => {}, () => true), /keyboard error/);
  assert.deepEqual(events, ['R']);
});

test('UI mouse press spans observed frames and waits for the release to be observed', async () => {
  const events=[],states=[{frame:1},{frame:2},{frame:3},{frame:3},{frame:4}];
  const page={mouse:{move:async(x,y)=>events.push([x,y]),down:async()=>events.push('down'),up:async()=>events.push('up')}};
  const until=async(_,accepts)=>{while(states.length){const s=states.shift();if(accepts(s))return s;}throw Error('timeout');};
  assert.equal((await observedClick(page,10,20,until,1)).frame,4);
  assert.deepEqual(events,[[10,20],'down','up']);
});

test('UI mouse press is released if observation fails', async () => {
  const events=[],page={mouse:{move:async()=>{},down:async()=>events.push('down'),up:async()=>events.push('up')}};
  await assert.rejects(observedClick(page,10,20,async()=>{throw Error('mouse timeout');},1),/mouse timeout/);
  assert.deepEqual(events,['down','up']);
});

test('select-all holds Control through an observed A and releases both keys on failure', async () => {
  const f=fixture(prompt(),[Error('selection timeout')]);
  await assert.rejects(selectAllObserved(f.api,f.page),/selection timeout/);
  assert.deepEqual(f.events,['down:Control','down:A','up:A','up:Control']);
});

test('observed slot already advances on victory and stays fixed through READY and scene reload', async () => {
  const f = fixture(prompt('Y'), [prompt('', 2), { ...active(3), endlessSlot: 2 }, active(4)]);
  assert.equal((await continueReady(f.api, f.page)).frame, 4);
  assert.deepEqual(f.keys, ['Y']);
});

function observationJSON() {
  const context = vm.createContext({});
  vm.runInContext('(' + installObservationJSON.toString() + ')()', context);
  return text => { context.input = text; return vm.runInContext('JSON.parse(input)', context); };
}

test('non-finite QA coordinates retain their exact numeric values without changing text', () => {
  const parse = observationJSON();
  const value = parse('{"x":Infinity,"y":-Infinity,"z":1.23456789012345,"text":"Infinity -Infinity \\\"Infinity\\\"","Infinity":2}');
  assert.equal(value.x, Infinity); assert.equal(value.y, -Infinity); assert.equal(value.z, 1.23456789012345);
  assert.equal(value.text, 'Infinity -Infinity "Infinity"'); assert.equal(value.Infinity, 2);
});

test('ordinary JSON and revivers retain native behavior; malformed payloads still fail', () => {
  const context = vm.createContext({});
  vm.runInContext('(' + installObservationJSON.toString() + ')()', context);
  assert.equal(vm.runInContext('JSON.parse(\'{"x":4}\', (key,value) => key === "x" ? value*2 : value).x', context), 8);
  assert.equal(vm.runInContext('JSON.parse(\'{"x":Infinity}\', (key,value) => key === "x" ? -value : value).x', context), -Infinity);
  const parse = observationJSON();
  for (const text of ['{"x":NaN}', '{"x":+Infinity}', '{"x":Infinityx}', '{"x":Infinity,}', '{"text":"Infinity",}']) {
    assert.throws(() => parse(text), error => error.name === 'SyntaxError');
  }
});
