// Uses the native-pixel stroke/bone/polygon helpers from weapons-pixel.js.
// Palette changes are scoped to anatomy; wood and steel keep their own colors.
if(p.referenceWeapon){tones={H:[250,250,250],L:[191,203,214],M:[139,152,170],S:[103,108,134],D:[29,29,49],W:[112,39,7],B:[73,16,9]};}
var steel=tones;
var bonePalette={H:[252,247,215],L:[235,199,117],M:[177,130,79],S:[163,98,56],D:[36,30,23]};
var boneMap={'191,203,214':'L','139,152,170':'M','103,108,134':'S','29,29,49':'D','250,250,250':'H','192,213,220':'L','160,172,197':'M','105,121,158':'S','58,42,61':'D'};
function anatomyPatch(name,x,y,angle){
 var source=patches[name],a=(angle||0)*Math.PI/180,co=Math.cos(a),si=Math.sin(a);
 // Inverse sampling preserves continuous opaque clusters during the death turn.
 var w=name==='head'?11:name==='ribs'?10:10,h=name==='head'?13:name==='ribs'?12:5;
 var by={};for(var k=0;k<source.length;k++)by[source[k][0]+','+source[k][1]]=source[k].slice(2);
 for(var yy=-8;yy<21;yy++)for(var xx=-8;xx<21;xx++){
  var sx=Math.round((xx-w/2)*co+(yy-h/2)*si+w/2);
  var sy=Math.round(-(xx-w/2)*si+(yy-h/2)*co+h/2);
  var rgb=by[sx+','+sy];
  if(rgb){var key=boneMap[rgb.join(',')];px(x+xx,y+yy,p.flash?steel.H:(yellow&&key?bonePalette[key]:rgb));}
 }
}
function torso(){
 var cx=p.chest[0],cy=p.chest[1],dx=p.hip[0]-cx,dy=p.hip[1]-cy;
 var len=Math.sqrt(dx*dx+dy*dy),ux=dx/len,uy=dy/len;
 function at(across,along){return [Math.round(cx+uy*across+ux*along),Math.round(cy-ux*across+uy*along)];}
 bone(at(0,9),[p.hip[0]+1,p.hip[1]],true);
 polygon([at(-4,0),at(4,0),at(4,7),at(2,10),at(-3,10),at(-5,6)],'D');
 bone(at(-4,0),at(4,0),false);
 for(var r=2;r<=8;r+=3){stroke(at(-4,r),at(0,r+1),'L');stroke(at(0,r+1),at(4,r),'H');}
 stroke(at(0,1),at(0,9),'M');
}
if(layer==='vfx'){
 if(p.fx){
  if(p.kind==='axe'){
   for(var step=1;step<p.sweep.length;step++)for(var k=1;k<p.sweep[step].length;k++){
    var quad=[p.sweep[step-1][k-1],p.sweep[step-1][k],p.sweep[step][k],p.sweep[step][k-1]];
    polygon(quad,step<5?'S':step<12?'L':'H');
   }
  }else{
   var tx=p.tip[0],ty=p.tip[1];
   polygon([[tx-24,ty-2],[tx+2,ty],[tx-24,ty+2],[tx-18,ty]],'L');
   stroke([tx-20,ty],[tx+2,ty],'H');
  }
 }
}else{
 if(yellow)tones=bonePalette;
 if(p.flash)tones={H:steel.H,L:steel.H,M:steel.L,S:steel.M,D:steel.S};
 for(var leg=1;leg>=0;leg--){
  var foot=p.feet[leg];
  bone([p.hip[0]+leg*5,p.hip[1]+3],p.knees[leg],!!leg);
  bone(p.knees[leg],foot,!!leg);
  stroke([foot[0]-2,foot[1]+1],[foot[0]+3,foot[1]+1],leg?'S':'L');
  stroke([foot[0]-1,foot[1]],[foot[0]+2,foot[1]],leg?'M':'H');
 }
 var shoulders=p.shoulders||[[p.chest[0]+7,p.chest[1]+2],[p.chest[0]-3,p.chest[1]+2]];
 arm(shoulders[1],p.elbows[1],p.hands[1],true);
 anatomyPatch('pelvis',p.hip[0]-3,p.hip[1],0);
 if(p.torso)torso();else{
  bone([p.chest[0]+1,p.chest[1]+10],[p.hip[0]+1,p.hip[1]],true);
  anatomyPatch('ribs',p.chest[0]-3,p.chest[1],0);
 }
 bone([p.head[0]+4,p.head[1]+11],[p.chest[0]+1,p.chest[1]+1],true);
 anatomyPatch('head',p.head[0],p.head[1],p.headAngle);
 arm(shoulders[0],p.elbows[0],p.hands[0],false);
 tones=steel;
 if(p.referenceWeapon){sourceWeapon();}else{
 stroke(p.butt,p.tip,'B',2);stroke(p.butt,p.tip,'W');
 polygon(p.blade,'M');polygon(p.light,'L');
 for(var j=0;j<p.blade.length;j++)stroke(p.blade[j],p.blade[(j+1)%p.blade.length],p.kind==='axe'?'D':'S');
 for(var j=1;j<p.edge.length;j++)stroke(p.edge[j-1],p.edge[j],'H');
 polygon(p.socket,'M');stroke(p.socket[0],p.socket[1],'L');
 }
 // Only attached fingers cover the wood. Released hands are painted above.
 if(yellow)tones=bonePalette;
 for(var h=0;h<2;h++)if(p.grips[h]!==null){
  var hand=p.hands[h];
  stroke([hand[0]-1,hand[1]],[hand[0]+1,hand[1]],h?'M':'H');
  stroke([hand[0]-1,hand[1]+1],[hand[0]+1,hand[1]+1],h?'S':'L');
 }
 tones=steel;
}

function sourceWeapon(){
 var len=Math.sqrt(Math.pow(p.tip[0]-p.butt[0],2)+Math.pow(p.tip[1]-p.butt[1],2));
 var ux=(p.tip[0]-p.butt[0])/len,uy=(p.tip[1]-p.butt[1])/len;
 var cu=3/Math.sqrt(2410),cv=-49/Math.sqrt(2410),lookup={};
 for(var k=0;k<patches.weapon.length;k++){
  var q=patches.weapon[k];lookup[q[0]+','+q[1]]=q.slice(2);
 }
 // Inverse nearest native sampling; throw if an authored pose loses any pixels.
 for(var y=-20;y<84;y++)for(var x=-20;x<116;x++){
  var dx=x-p.tip[0],dy=y-p.tip[1],a=dx*ux+dy*uy,b=-dx*uy+dy*ux;
  var sx=Math.round(61+a*cu-b*cv),sy=Math.round(14+a*cv+b*cu);
  var rgb=lookup[sx+','+sy];if(rgb)px(x,y,rgb);
 }
}
