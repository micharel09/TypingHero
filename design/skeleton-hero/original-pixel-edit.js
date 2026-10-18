// Edit original cels in LibreSprite; preserve geometry and retouch clusters.
// Preserve alpha/pose; revise interior light and shadow clusters below.
var img=app.activeSprite.layer(0).cel(0).image;
var c=app.pixelColor;
var mapping={
 '29,29,49':[58,42,61],
 '103,108,134':[105,121,158],
 '139,152,170':[160,172,197],
 '191,203,214':[192,213,220]
};
for(var y=0;y<64;y++) for(var x=0;x<96;x++) {
 var p=img.getPixel(x,y);
 if(c.rgbaA(p)===0) continue;
 var next=mapping[[c.rgbaR(p),c.rgbaG(p),c.rgbaB(p)].join(',')];
 if(next) img.putPixel(x,y,c.rgba(next[0],next[1],next[2],255));
}
// Selective interior retouch, never replacement anatomy. Original alpha clips
// every brush span. Per-pose coordinates follow the visible source bones.
var tones={H:[250,250,250],L:[192,213,220],M:[160,172,197],S:[105,121,158]};
// Crown/temple spans [y,x1,x2,tone]; slots [y,x1,x2]; hip [x,y];
// far leg [y,x1,x2]. Only the visible portions of occluded ribs are painted.
var poses={
 'idle-0':{head:[[19,40,42,'L'],[20,39,43,'L'],[21,39,42,'M'],[25,39,42,'L'],[26,40,42,'L'],[28,42,44,'M'],[29,42,43,'S']],ribs:[[32,44,48],[34,45,48],[36,45,48],[38,44,48]],hip:[40,44],leg:[[49,48,49],[50,49,49],[51,49,49],[52,50,50],[53,50,50],[54,50,50],[55,51,51],[56,51,51],[57,51,51]]},
 'attack-0':{head:[[19,40,42,'L'],[20,39,43,'L'],[21,39,42,'M'],[25,39,42,'L'],[26,40,42,'L'],[28,42,44,'M'],[29,42,43,'S']],ribs:[[32,44,48],[34,45,48],[36,45,48],[38,44,48]],hip:[40,44],leg:[[49,48,49],[50,49,49],[51,49,49],[52,50,50],[53,50,50],[54,50,50],[55,51,51],[56,51,51],[57,51,51]]},
 'attack-1':{head:[[19,39,41,'L'],[20,38,42,'L'],[21,38,41,'M'],[25,38,41,'L'],[26,39,41,'L'],[28,41,43,'M'],[29,41,42,'S']],ribs:[[32,43,47],[34,44,47],[36,44,47],[38,43,47]],hip:[39,44],leg:[[49,47,48],[50,48,48],[51,48,48],[52,49,49],[53,49,49],[54,50,50],[55,50,50],[56,50,50],[57,51,51]]},
 'attack-2':{head:[[20,36,38,'L'],[21,35,39,'L'],[22,35,38,'M'],[26,35,38,'L'],[27,36,38,'L'],[29,38,40,'M'],[30,38,39,'S']],ribs:[[37,43,46],[39,43,47]],hip:[38,45],leg:[[50,46,48],[51,48,48],[52,48,48],[53,49,49],[54,49,49],[55,50,50],[56,50,50],[57,51,51]]},
 'attack-3':{head:[[21,35,37,'L'],[22,34,38,'L'],[23,34,37,'M'],[27,34,37,'L'],[28,35,37,'L'],[30,37,39,'M'],[31,37,38,'S']],ribs:[[38,42,45],[40,42,46]],hip:[37,46],leg:[[51,46,48],[52,48,49],[53,49,49],[54,49,49],[55,49,49],[56,50,50],[57,50,50],[58,51,51]]},
 'attack-4':{head:[[22,35,37,'L'],[23,34,38,'L'],[24,34,37,'M'],[28,34,37,'L'],[29,35,37,'L'],[31,37,39,'M'],[32,37,38,'S']],ribs:[[39,42,45],[41,42,46]],hip:[37,47],leg:[[52,46,48],[53,48,49],[54,49,49],[55,49,49],[56,49,50],[57,50,50],[58,51,51]]},
 'attack-5':{head:[[19,49,51,'L'],[20,48,52,'L'],[21,48,51,'M'],[25,48,51,'L'],[26,49,51,'L'],[28,51,53,'M'],[29,51,52,'S']],ribs:[[32,53,57],[34,54,57]],hip:[47,44],leg:[[49,52,53],[50,52,53],[51,54,54],[52,53,53],[53,53,53],[54,52,52],[55,52,52],[56,51,51],[57,51,51]]},
 'attack-6':{head:[[17,48,50,'L'],[18,47,51,'L'],[19,47,50,'M'],[23,47,50,'L'],[24,48,50,'L'],[26,50,52,'M'],[27,50,51,'S']],ribs:[[30,52,56],[32,53,56],[34,53,56],[36,52,56]],hip:[47,42],leg:[[47,52,54],[48,52,54],[49,54,55],[50,55,55],[51,54,54],[52,53,54],[53,53,53],[54,53,53],[55,52,52],[56,51,51]]},
 'attack-7':{head:[[18,46,48,'L'],[19,45,49,'L'],[20,45,48,'M'],[24,45,48,'L'],[25,46,48,'L'],[27,48,50,'M'],[28,48,49,'S']],ribs:[[31,50,54],[33,51,54],[35,51,54],[37,50,54]],hip:[45,43],leg:[[48,50,52],[49,51,53],[50,53,54],[51,54,54],[52,54,54],[53,53,54],[54,53,53],[55,52,53],[56,51,51]]},
 'attack-8':{head:[[18,42,44,'L'],[19,41,45,'L'],[20,41,44,'M'],[24,41,44,'L'],[25,42,44,'L'],[27,44,46,'M'],[28,44,45,'S']],ribs:[[31,46,50],[33,47,50],[35,47,50],[37,46,50]],hip:[41,43],leg:[[48,49,50],[49,49,50],[50,50,50],[51,50,51],[52,50,51],[53,51,51],[54,51,51],[55,51,51],[56,51,51],[57,51,51]]}
};
// Recovery is identical to Attack0 in the originals, so it must match its paint.
if(typeof remainingPoses!=='undefined')for(var key in remainingPoses)poses[key]=remainingPoses[key];
var edit=poses[pose==='attack-9'?'attack-0':pose];
function span(y,x1,x2,tone){
 var rgb=tones[tone];
 for(var x=x1;x<=x2;x++) if(c.rgbaA(img.getPixel(x,y))) img.putPixel(x,y,c.rgba(rgb[0],rgb[1],rgb[2],255));
}
for(var j=0;j<edit.head.length;j++){var b=edit.head[j];span(b[0],b[1],b[2],b[3]);}
for(var j=0;j<edit.ribs.length;j++){
 var b=edit.ribs[j];span(b[0],b[1],b[2],'S');
 span(b[0]+1,b[2]-2,b[2],'L');
}
if(edit.hip){
 span(edit.hip[1],edit.hip[0]+1,edit.hip[0]+5,'L');
 span(edit.hip[1]+1,edit.hip[0],edit.hip[0]+1,'M');
 span(edit.hip[1]+3,edit.hip[0]+4,edit.hip[0]+6,'M');
}
for(var j=0;j<edit.leg.length;j++){var b=edit.leg[j];span(b[0],b[1],b[2],'M');}
if(edit.strokes)for(var j=0;j<edit.strokes.length;j++){var b=edit.strokes[j];span(b[0],b[1],b[2],b[3]);}
// Restore full weapon/VFX envelopes, including transparent pixels.
for(var i=0;i<protectedPixels.length;i++) {
 var q=protectedPixels[i];img.putPixel(q[0],q[1],c.rgba(q[2],q[3],q[4],q[5]));
}
