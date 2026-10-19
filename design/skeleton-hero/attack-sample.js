// LibreSprite 1.3 pixel script. Inputs are injected by prepare_samples.py.
// Coordinates are native 96x64 pixels; no scale/transform is used.
var img = app.activeSprite.layer(0).cel(0).image;
var col = app.pixelColor;
var palette = {
  '.': [0,0,0,0],
  'D': [76,38,65,255], // hero's existing outline
  'S': [105,121,158,255], // hero's armor shadow
  'M': [160,172,197,255],
  'L': [192,213,220,255],
  'H': [250,250,250,255]
};
function pixel(x,y,c) {
  if(x>=0 && x<96 && y>=0 && y<64) {
    var p=palette[c]; img.putPixel(x,y,col.rgba(p[0],p[1],p[2],p[3]));
  }
}
function stamp(x,y,rows) {
  for(var j=0;j<rows.length;j++) for(var i=0;i<rows[j].length;i++)
    if(rows[j][i]!=='.') pixel(x+i,y+j,rows[j][i]);
}
// Articulated bones, with a dark silhouette and a two-pixel light plane.
function line(x0,y0,x1,y1,c,r) {
  var dx=Math.abs(x1-x0),sx=x0<x1?1:-1,dy=-Math.abs(y1-y0),sy=y0<y1?1:-1,err=dx+dy;
  while(true) {
    for(var oy=-r;oy<=r;oy++) for(var ox=-r;ox<=r;ox++) pixel(x0+ox,y0+oy,c);
    if(x0===x1 && y0===y1) break;
    var e=2*err;if(e>=dy){err+=dy;x0+=sx;}if(e<=dx){err+=dx;y0+=sy;}
  }
}
function bone(a,b,lit) {
  line(a[0],a[1],b[0],b[1],'D',1);
  line(a[0],a[1],b[0],b[1],lit?'L':'S',0);
  line(a[0]-1,a[1],b[0]-1,b[1],lit?'H':'M',0);
}
for(var y=0;y<64;y++) for(var x=0;x<96;x++) pixel(x,y,'.');
var cx=bodyX;
// All frames share planted feet; weight shifts through knees and pelvis.
bone([cx,49],[41,55],true); bone([41,55],[38,62],true);
bone([cx+5,49],[50,55],false); bone([50,55],[51,62],false);
stamp(37,61,['.HL..........SS','LHL..........SSS','LHHL.........SSSS']);
// Far arm first, near arm terminates at the actual sword grip.
bone([cx-4,37],[cx-10,42],false); bone([cx-10,42],[cx-9,46],false);
bone([cx+6,37],elbow,true); bone(elbow,grip,true);
// Open ribcage: sternum, separate ribs, hollow abdomen. No armor/clothing.
stamp(cx-5,35,[
  '..DDDDDDDDDD..',
  '.DLHHHHHLMSDD.',
  'DLHDDHDDSMMMD.',
  'DLHHHHHHHLMSD.',
  'DLHDDHDDHMMMD.',
  '.LHHHHHHHLMSD.',
  '.DHDDHDDHMMD..',
  '..LHHHHHLMMD..',
  '...DDHDDMSD...',
  '.....H.SD.....',
  '....DHSD......',
  '...DDMSDD.....'
]);
stamp(cx-5,46,[
  '..DD.....DD...',
  '.DHLDDDDMLSD..',
  '.DHHHLLLMLSD..',
  '..DHHHLLMSD...',
  '...DHLMSDD....',
  '....DDD.......'
]);
stamp(cx,33,['DMSD','DHSD','DMSD']);
// Shared 14x13 skull, broad cranium with a clear eye and jaw at native size.
stamp(cx-5,headY,[
  '....DDDDDD....',
  '..DDHHHHHLDD..',
  '.DMLHHHHHHHLD.',
  'DMMMLLLLLHHHLD',
  'DMMMMLLLLLLMLD',
  'DSMMDDDDLLLDDD',
  'DSMMDDDDDMLDDD',
  'DSMMDDDDDMLDHD',
  '.DSMLLLLMDLLHD',
  '.DSMLLLDDDLHD.',
  '..DSMMLLLLLD..',
  '...DDMLDMLD...',
  '.....DDDDD....'
]);
// Sword is connected to the hand in every pose; geometry is newly drawn.
bone(grip,tip,true);
var vx=tip[0]-grip[0],vy=tip[1]-grip[1];
var length=Math.sqrt(vx*vx+vy*vy),nx=-vy/length,ny=vx/length;
line(Math.round(grip[0]-nx*3),Math.round(grip[1]-ny*3),Math.round(grip[0]+nx*3),Math.round(grip[1]+ny*3),'D',1);
line(Math.round(grip[0]-nx*3),Math.round(grip[1]-ny*3),Math.round(grip[0]+nx*3),Math.round(grip[1]+ny*3),'M',0);
stamp(grip[0]-1,grip[1]-1,['HL','LM']);
// A narrow outward crescent leaves skull and ribcage unobscured.
if(slash) {
 var arc=slash===2?[[77,14],[84,17],[90,23],[93,30],[91,37],[85,42],[76,44]]:[[80,28],[87,34],[88,40],[83,46],[74,49],[64,49]];
 for(var a=1;a<arc.length;a++) {line(arc[a-1][0],arc[a-1][1],arc[a][0],arc[a][1],'L',1);line(arc[a-1][0],arc[a-1][1],arc[a][0],arc[a][1],'H',0);}
}
