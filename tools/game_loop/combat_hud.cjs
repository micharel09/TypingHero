// Fixed approved r7 geometry. State bridge only reads live bindings and rendered rectangles.
module.exports = function combatHUD(s, check, label) {
  const k=Math.min(s.width/960,s.height/540), px=k*3;
  const ox=(s.width-960*k)/2, oy=(s.height-540*k)/2;
  const near=(a,b)=>Math.abs(a-b)<1.5;
  const rect=(r,x,y,w,h,name)=>check(near((r.x-ox)/px,x)&&near((s.height-oy-r.y-r.height)/px,y)&&near(r.width/px,w)&&near(r.height/px,h),`${label}: ${name} approved fixed geometry`);
  rect(s.playerStatusRect,3,11,102,21,'player status');
  rect(s.enemyStatusRect,215,11,102,36,'enemy status');
  rect(s.trayRect,61,146,198,30,'typing row');
  check(s.playerNameVisible && s.enemyNameVisible,`${label}: live names visible`);
  check(s.scoreFont===36 && s.comboFont===24,`${label}: score/combo fonts12/8 native`);
  check(near((s.popupPoint.x-s.enemyHeadPoint.x)/k,24)&&near((s.popupPoint.y-s.enemyHeadPoint.y)/k,12),`${label}: popup follows real enemy head`);
  check(s.wordRects.length===3 && s.wordRects.every((r,i)=>near((r.x+r.width/2-ox)/px,[94,160,226][i]) && r.width/px<=58),`${label}: fixed centered cells fit actual pool`);
  check(Math.abs(s.playerHpFill-s.hp/s.maxHp)<.002 && Math.abs(s.enemyHpFill-s.enemyHp/850)<.002 && Math.abs(s.staminaFill-s.stamina/250)<.002,`${label}: tutorial HP/stamina live bindings`);
};
