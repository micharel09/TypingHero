// Read-only geometry acceptance in reference units; called while tutorial reading holds combat.
module.exports = function combatHUD(s, check, label) {
  const k=s.height/540, near=(a,b)=>Math.abs(a-b)<1.5;
  const top=r=>r.y+r.height, right=r=>r.x+r.width;
  for(const [r,foot,name] of [[s.playerStatusRect,s.playerFootPoint,'player'],[s.enemyStatusRect,s.enemyFootPoint,'enemy']]){
    check(near(r.width/k,112) && near((foot.y-top(r))/k,12) && near((r.x+r.width/2-foot.x)/k,0),`${label}: ${name} status112px / foot gap12px / centered`);
  }
  const tray=s.trayRect;
  check(near(tray.width/k,672) && near(tray.height/k,56) && near(tray.x+tray.width/2,s.width/2),`${label}: centered672x56 typing tray`);
  check(near((Math.min(s.playerStatusRect.y,s.enemyStatusRect.y)-top(tray))/k,64),`${label}: tray64px below lowest status (8+48+8)`);
  check(tray.y>=0 && right(tray)<=s.width && s.playerStatusRect.x>=0 && right(s.enemyStatusRect)<=s.width,`${label}: status/tray inside render bounds`);
  const score=s.scoreRect, mul=s.multiplierRect, end=mul.width>0?right(mul):right(score);
  check(near((score.x+end)/2,s.width/2) && near((Math.min(s.playerStatusRect.y,s.enemyStatusRect.y)-top(score))/k,8),`${label}: visible score/multiplier centered8px below lowest status`);
  check((top(score)-top(s.wordRects[0]))/k<=80,`${label}: score to word <=80 reference pixels`);
  check(s.wordRects.length===3 && s.wordRects.every(r=>r.width>0 && r.x>=tray.x && right(r)<=right(tray) && r.y>=tray.y && top(r)<=top(tray)),`${label}: all3words fit inside tray`);
  check(Math.abs(s.playerHpFill-s.hp/s.maxHp)<.002 && Math.abs(s.enemyHpFill-s.enemyHp/850)<.002 && Math.abs(s.staminaFill-s.stamina/250)<.002,`${label}: reused HP/stamina bindings match gameplay`);
};
