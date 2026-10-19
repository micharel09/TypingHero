// Read-only geometry acceptance in reference units; called while tutorial reading holds combat.
module.exports = function combatHUD(s, check, label) {
  const k=s.height/540, near=(a,b)=>Math.abs(a-b)<1.5;
  const top=r=>r.y+r.height, right=r=>r.x+r.width;
  for(const [r,foot,name] of [[s.playerStatusRect,s.playerFootPoint,'player'],[s.enemyStatusRect,s.enemyFootPoint,'enemy']]){
    check(near(r.width/k,112) && near((foot.y-top(r))/k,12) && near((r.x+r.width/2-foot.x)/k,0),`${label}: ${name} status112px / foot gap12px / centered`);
  }
  check(!s.playerNameVisible && !s.enemyNameVisible,`${label}: both actor names hidden`);
  check(Math.abs(s.staminaColor.r-220/255)<.005 && Math.abs(s.staminaColor.g-196/255)<.005 && Math.abs(s.staminaColor.b-124/255)<.005,`${label}: Gold stamina`);
  check(near((s.popupPoint.x-s.enemyHeadPoint.x)/k,24) && near((s.popupPoint.y-s.enemyHeadPoint.y)/k,12),`${label}: enemy head popup offset24/12`);
  const tray=s.trayRect;
  check(near(tray.width/k,672) && near(tray.height/k,56) && near(tray.x+tray.width/2,s.width/2),`${label}: centered672x56 typing tray`);
  check(near((Math.min(s.playerStatusRect.y,s.enemyStatusRect.y)-top(tray))/k,12),`${label}: tray12px below lowest status`);
  check(tray.y>=0 && right(tray)<=s.width && s.playerStatusRect.x>=0 && right(s.enemyStatusRect)<=s.width,`${label}: status/tray inside render bounds`);
  const score=s.scoreRect;
  check(near((s.width-right(score))/k,24) && near((s.height-top(score))/k,24),`${label}: score fixed24px from top/right`);
  check(s.wordRects.length===3 && s.wordRects.every(r=>r.width>0 && r.x>=tray.x && right(r)<=right(tray) && r.y>=tray.y && top(r)<=top(tray)),`${label}: all3words fit inside tray`);
  check(Math.abs(s.playerHpFill-s.hp/s.maxHp)<.002 && Math.abs(s.enemyHpFill-s.enemyHp/850)<.002 && Math.abs(s.staminaFill-s.stamina/250)<.002,`${label}: reused HP/stamina bindings match gameplay`);
};
