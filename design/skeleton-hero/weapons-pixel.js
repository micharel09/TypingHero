// LibreSprite 1.3: redraw each cel at native resolution, never rotate a bitmap.
// p, patches and layer are injected by prepare_weapons.py.
var img=app.activeSprite.layer(0).cel(0).image, color=app.pixelColor;
var tones={H:[250,250,250],L:[192,213,220],M:[160,172,197],S:[105,121,158],D:[58,42,61],W:[122,70,46],B:[76,43,38]};
function px(x,y,t){
 if(x<0||x>=96||y<0||y>=64)throw new Error('Clipped pixel '+x+','+y);
 var c=typeof t==='string'?tones[t]:t;
 img.putPixel(x,y,color.rgba(c[0],c[1],c[2],255));
}
function stroke(a,b,t,width){
 var x=a[0],y=a[1],dx=Math.abs(b[0]-x),dy=-Math.abs(b[1]-y),sx=x<b[0]?1:-1,sy=y<b[1]?1:-1,err=dx+dy;
 while(true){for(var i=0;i<(width||1);i++)px(x+i,y,t);if(x===b[0]&&y===b[1])break;var e=2*err;if(e>=dy){err+=dy;x+=sx;}if(e<=dx){err+=dx;y+=sy;}}
}
function bone(a,b,far){
 stroke([a[0]+1,a[1]], [b[0]+1,b[1]],far?'S':'M',2);
 stroke(a,b,far?'M':'L',2);
 if(!far)stroke(a,b,'H');
}
function polygon(points,t){
 var minY=63,maxY=0;
 for(var j=0;j<points.length;j++){minY=Math.min(minY,points[j][1]);maxY=Math.max(maxY,points[j][1]);}
 for(var y=minY;y<=maxY;y++)for(var x=0;x<96;x++){
  var inside=false;
  for(var i=0,j=points.length-1;i<points.length;j=i++){
   var a=points[i],b=points[j];
   if((a[1]>y)!==(b[1]>y)&&x<(b[0]-a[0])*(y-a[1])/(b[1]-a[1])+a[0])inside=!inside;
  }
  if(inside)px(x,y,t);
 }
 for(var i=0;i<points.length;i++)stroke(points[i],points[(i+1)%points.length],t);
}
function patch(name,x,y,lean){
 var source=patches[name];
 for(var j=0;j<source.length;j++){
  var q=source[j],offset=lean?lean[q[1]]||0:0;px(x+q[0]+offset,y+q[1],q.slice(2));
 }
}
function arm(shoulder,elbow,hand,far){bone(shoulder,elbow,far);bone(elbow,hand,far);}
if(layer==='vfx'){
 // Only the burst cel carries a trail. It never substitutes for the weapon.
 if(p.fx){
  if(p.kind==='axe'){
   polygon([[52,5],[65,11],[78,24],[85,42],[81,56],[74,63],[70,62],[78,54],[80,42],[72,25],[61,13]],'L');
   stroke([65,12],[76,24],'H',2);stroke([76,24],[83,41],'H',2);
   stroke([83,41],[80,53],'H');stroke([80,53],[73,62],'H');
  }else{
   polygon([[68,33],[91,35],[69,37],[75,35]],'L');
   stroke([74,35],[91,35],'H');stroke([62,40],[73,40],'M');
  }
 }
}else{
 // Fixed foot contacts; knees and the pelvis carry the intentional weight shift.
 bone([p.hip[0]+5,p.hip[1]+3],p.knees[1],true);bone(p.knees[1],[53,62],true);
 stroke([51,63],[56,63],'S');stroke([52,62],[55,62],'M');
 bone([p.hip[0],p.hip[1]+3],p.knees[0],false);bone(p.knees[0],[36,62],false);
 stroke([34,63],[39,63],'L');stroke([35,62],[38,62],'H');
 // Far arm is partly occluded by the thorax, with both wrists authored per cel.
 arm([p.chest[0]-3,p.chest[1]+2],p.elbows[1],p.hands[1],true);
 patch('pelvis',p.hip[0]-3,p.hip[1]);
 bone([p.chest[0]+1,p.chest[1]+10],[p.hip[0]+1,p.hip[1]],true);
 patch('ribs',p.chest[0]-3,p.chest[1],p.lean);
 bone([p.head[0]+4,p.head[1]+11],[p.chest[0]+1,p.chest[1]+1],true);
 patch('head',p.head[0],p.head[1]);
 arm([p.chest[0]+7,p.chest[1]+2],p.elbows[0],p.hands[0],false);
 // Shaft geometry is independently rasterized for this pose. No image transform.
 stroke(p.butt,p.tip,'B',2);stroke(p.butt,p.tip,'W');
 polygon(p.blade,'M');polygon(p.light,'L');
 var rim=p.kind==='spear'?'S':'D';
 for(var j=0;j<p.blade.length;j++)stroke(p.blade[j],p.blade[(j+1)%p.blade.length],rim);
 for(var j=1;j<p.edge.length;j++)stroke(p.edge[j-1],p.edge[j],'H');
 polygon(p.socket,'M');
 if(p.kind==='axe')for(var j=0;j<p.socket.length;j++)stroke(p.socket[j],p.socket[(j+1)%p.socket.length],'D');
 stroke(p.socket[0],p.socket[1],'L');
 // The shaft's two-pixel shadow must not protrude past the fine spear point.
 if(p.kind==='spear')img.putPixel(p.tip[0]+1,p.tip[1],color.rgba(0,0,0,0));
 // Fingers occlude the shaft; visible dark gaps separate the two grips.
 for(var h=0;h<2;h++){
  var hand=p.hands[h];stroke([hand[0]-1,hand[1]],[hand[0]+1,hand[1]],h?'M':'H',1);
  stroke([hand[0]-1,hand[1]+1],[hand[0]+1,hand[1]+1],h?'S':'L');
 }
}
