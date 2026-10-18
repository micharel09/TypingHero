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
var cx=pose==='contact'?53:43;
// Feet terminate at the original ground pixels. Pelvis/leg joints are redrawn.
if(pose==='idle') {
  bone([43,49],[41,55],true); bone([41,55],[38,62],true);
  bone([48,49],[50,55],false); bone([50,55],[51,62],false);
  bone([39,36],[35,42],true); bone([35,42],[35,47],true);
  bone([51,36],[53,41],false); bone([53,41],[57,43],true);
} else if(pose==='anticipation') {
  bone([41,49],[41,55],true); bone([41,55],[37,62],true);
  bone([46,49],[50,55],false); bone([50,55],[51,62],false);
  bone([39,37],[31,39],true); bone([31,39],[35,34],true);
  bone([49,37],[51,35],false); bone([51,35],[41,35],true);
} else {
  bone([51,49],[46,55],true); bone([46,55],[38,62],true);
  bone([57,49],[54,55],false); bone([54,55],[51,62],false);
  bone([49,35],[44,38],true); bone([44,38],[35,40],true);
  bone([59,35],[63,32],false); bone([63,32],[66,30],true);
}
// Keep the original exact footprint; body can become broader above it.
if(pose==='anticipation') {
  stamp(36,61,['.HL...........SS','LHL...........SSS','LHHL..........SSSS']);
} else if(pose==='contact') {
  stamp(37,61,['.HL..........SS','LHL..........SSS','LHL..........SSSS']);
} else {
  stamp(37,61,['.HL..........SS','LHL..........SSS','LHHL.........SSSS']);
}
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
stamp(cx-5,21+(pose==='anticipation'?2:0),[
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
// Original weapon, grip and visible slash are restored AFTER body drawing.
// The geometric weapon envelope also protects empty pixels around the blade.
for(var n=0;n<protectedPixels.length;n++) {
  var p=protectedPixels[n];img.putPixel(p[0],p[1],col.rgba(p[2],p[3],p[4],p[5]));
}
