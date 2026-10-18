"""Review-only poses: observed axe Idle and the approved design attack contracts.

Public GIFs do not expose every axe clip, nor a spear. Those poses are authored
reconstructions, not extracted premium cels. Coordinates are native pixels.
"""
from copy import deepcopy
import math

from prepare_weapons import axe_poses, spear_poses


def head_at(kind, butt, tip):
    length = math.dist(butt, tip)
    ux, uy = [(tip[j]-butt[j])/length for j in range(2)]
    if kind == 'axe':
        # Traced bearded head/counterpoint/socket from the public overview GIF.
        shapes = [
            [[61,24],[59,25],[57,24],[57,21],[56,21],[56,27],
             [58,29],[57,31],[57,33],[60,31],[62,31],[63,34],
             [64,33],[66,30],[67,26],[67,21],[66,21],[66,24],[64,25],[64,24]],
            [[64,25],[66,24],[66,28],[65,31],[63,33],[63,29],[60,29]],
            [[67,21],[67,26],[66,30],[64,33],[63,34]],
            [[61,23],[63,23],[63,27],[61,27]],
        ]
        local = [[(24-y, x-62) for x,y in shape] for shape in shapes]
    else:
        local = [
            [[-14,0],[-11,-2],[-8,-3],[-4,-2],[0,0],[-4,2],[-8,3],[-11,2]],
            [[-12,0],[-8,-2],[-4,-1],[0,0],[-7,0]],
            [[-13,0],[0,0]],
            [[-17,-1],[-14,-1],[-13,0],[-14,1],[-17,1]],
        ]
    return {name: [[round(tip[0]+a*ux-b*uy), round(tip[1]+a*uy+b*ux)]
                   for a,b in shape]
            for name,shape in zip(['blade','light','edge','socket'],local)}


def weapon(p, butt, tip, grips=None):
    target=32 if p['kind']=='axe' else 58
    length=math.dist(butt,tip)
    tip=[round(butt[j]+(tip[j]-butt[j])*target/length) for j in range(2)]
    shape=head_at(p['kind'],butt,tip)
    points=[butt,tip]+[q for polygon in shape.values() for q in polygon]
    # A falling weapon is allowed to change contact, never to leave the cel.
    shift=[max(0,1-min(q[j] for q in points))-max(0,max(q[j] for q in points)-(94 if j==0 else 63)) for j in range(2)]
    butt=[butt[j]+shift[j] for j in range(2)]
    tip=[tip[j]+shift[j] for j in range(2)]
    p['butt'], p['tip'] = butt, tip
    p.update(head_at(p['kind'],butt,tip))
    if grips is None:
        grips = [7,15] if p['kind']=='axe' else [17,31]
    p['grips'] = grips
    length = math.dist(butt,tip)
    for h,d in enumerate(grips):
        if d is not None:
            p['hands'][h] = [round(butt[j]+(tip[j]-butt[j])*d/length) for j in range(2)]
    return p


def idle(kind):
    p = deepcopy((axe_poses() if kind=='axe' else spear_poses())[0])
    p.update(feet=[[36,62],[53,62]], headAngle=0, fx=False, torso=False)
    if kind=='axe':
        p['hands'][1] = [36,48]
        p['elbows'] = [[54,41],[37,40]]
        weapon(p,[61,56],[61,24],[15,None])
    else:
        weapon(p,p['butt'],p['tip'])
    return p


def breathe(kind):
    poses=[]
    for dy in [0,0,-1,-1,0,0,1,1]:
        p=idle(kind)
        for name in ['head','chest']:
            p[name][1]+=dy
        for q in p['elbows']: q[1]+=dy
        if kind=='axe':
            weapon(p,[61,56+dy],[61,24+dy],[15,None])
            p['hands'][1][1]+=dy
        else:
            weapon(p,[23,41+dy],[80,33+dy])
        poses.append(p)
    return poses


def walk(kind):
    poses=[]
    # Two opposed support phases; planted foot stays at y63 while it travels back.
    stride=[-9,-6,-2,3,7,9,6,2,-3,-7]
    lift=[0,0,0,0,0,0,-3,-5,-4,-2]
    for i in range(10):
        p=idle(kind)
        dy=[0,-1,-1,0,1,0,-1,-1,0,1][i]
        p['feet']=[[44+stride[i],62+lift[i]],
                   [44+stride[(i+5)%10],62+lift[(i+5)%10]]]
        p['knees']=[[42+round(stride[i]*.45),54+min(0,lift[i])],
                    [46+round(stride[(i+5)%10]*.45),54+min(0,lift[(i+5)%10])]]
        for name in ['head','chest','hip']: p[name][1]+=dy
        for q in p['elbows']: q[1]+=dy
        if kind=='axe':
            tilt=[0,0,1,1,0,0,-1,-1,0,0][i]
            weapon(p,[61,56+dy],[61+tilt,24+dy],[15,None])
            p['hands'][1]=[36-stride[i]//4,48+dy]
        else:
            weapon(p,[23,41+dy],[80,33+dy])
        poses.append(p)
    return poses


def sweep_between(a,b,steps=16):
    aa=math.atan2(a['tip'][1]-a['butt'][1],a['tip'][0]-a['butt'][0])
    ba=math.atan2(b['tip'][1]-b['butt'][1],b['tip'][0]-b['butt'][0])
    rims=[]
    for i in range(steps+1):
        t=i/steps
        angle=aa+(ba-aa)*t
        butt=[a['butt'][j]+(b['butt'][j]-a['butt'][j])*t for j in range(2)]
        butt[1]+=17*math.sin(math.pi*t)
        tip=[butt[0]+32*math.cos(angle),butt[1]+32*math.sin(angle)]
        rims.append(head_at('axe',butt,tip)['edge'])
    return rims


def attack1(kind):
    poses=deepcopy(axe_poses() if kind=='axe' else spear_poses()+[spear_poses()[0]])
    for i,p in enumerate(poses):
        p.update(feet=[[36,62],[53,62]],headAngle=0)
        if kind=='axe':
            butt,tip=p['butt'][:],p['tip'][:]
            if 5<=i<=10:
                butt[1]=tip[1]=58
            weapon(p,butt,tip)
        else:
            weapon(p,p['butt'],p['tip'])
    poses[0]=idle(kind)
    poses[-1]=idle(kind)
    if kind=='axe':
        poses[4]['sweep']=sweep_between(poses[3],poses[4])
        # Let the support hand come away visibly before returning to one-hand Idle.
        poses[14]['hands'][1]=[38,43]
        poses[14]['elbows'][1]=[38,36]
        poses[14]['grips'][1]=None
        weapon(poses[14],[59,55],[62,23],[14,None])
    return poses


def attack2(kind):
    if kind=='spear':
        source=spear_poses()
        poses=[]
        for i,index in enumerate([0,1,2,3,5,6,7,8,0]):
            p=deepcopy(source[index])
            p.update(feet=[[36,62],[53,62]],headAngle=0,fx=i==4,torso=True)
            drop=[0,2,4,5,6,6,4,2,0][i]
            p['head'][1]+=drop//2
            p['chest'][1]+=drop//2
            p['hip'][1]+=drop//3
            weapon(p,[p['butt'][0],p['butt'][1]+drop],
                   [p['tip'][0],p['tip'][1]+drop])
            for e in p['elbows']: e[1]+=drop
            poses.append(p)
        poses[0]=idle(kind);poses[-1]=idle(kind)
        return poses
    values=[
        ([43,31],[43,44],[61,56],[61,24],[[54,41],[37,40]]),
        ([42,32],[42,45],[53,44],[27,25],[[48,42],[39,35]]),
        ([40,33],[41,46],[50,43],[20,32],[[45,42],[34,36]]),
        ([41,33],[42,46],[51,42],[21,31],[[46,42],[35,36]]),
        ([49,33],[46,46],[54,39],[86,39],[[55,43],[58,34]]),
        ([50,35],[47,47],[52,41],[80,56],[[53,47],[58,39]]),
        ([47,35],[45,46],[51,44],[81,55],[[54,47],[54,38]]),
        ([44,32],[44,45],[56,52],[72,24],[[55,45],[43,35]]),
        ([43,31],[43,44],[61,56],[61,24],[[54,41],[37,40]]),
    ]
    poses=[]
    for i,(chest,hip,butt,tip,elbows) in enumerate(values):
        p=idle(kind)
        p.update(chest=chest,hip=hip,head=[chest[0]-3,chest[1]-13],
                 elbows=elbows,torso=True,fx=i==4)
        length=math.dist(butt,tip)
        tip=[round(butt[j]+(tip[j]-butt[j])*32/length) for j in range(2)]
        weapon(p,butt,tip)
        poses.append(p)
    poses[0]=idle(kind);poses[-1]=idle(kind)
    # A short lateral cutting trail; no persistent effects during recovery.
    poses[4]['sweep']=sweep_between(poses[3],poses[4])
    return poses


def hurt(kind):
    poses=[]
    for i,(dx,dy,tilt) in enumerate([(0,0,0),(-4,-2,-7),(-5,0,-9),(-2,1,-4),(0,0,0)]):
        p=idle(kind)
        for name in ['head','chest']: p[name][0]+=dx;p[name][1]+=dy
        p['hip'][0]+=dx//2
        for e in p['elbows']: e[0]+=dx;e[1]+=dy
        if kind=='axe':
            weapon(p,[61+dx,56+dy],[61+dx+tilt,24+dy],[15,None])
            p['hands'][1]=[35+dx,47+dy]
        else:
            weapon(p,[23+dx,41+dy],[80+dx,33+dy+tilt//2])
        p['flash']=i==1
        poses.append(p)
    return poses


def die(kind):
    poses=[]
    # Recoil, buckling, fall, separate weapon landing, then fragment settling.
    values=[
        ([40,18],[43,31],[43,44],0),
        ([36,16],[39,29],[42,43],-8),
        ([32,19],[36,32],[41,45],-18),
        ([27,24],[32,36],[39,48],-30),
        ([22,32],[29,42],[38,51],-45),
        ([17,41],[27,49],[38,55],-60),
        ([14,49],[26,55],[39,58],-78),
        ([13,52],[26,57],[40,59],-88),
        ([13,53],[26,58],[41,59],-90),
        ([12,54],[26,58],[41,59],-90),
        ([12,54],[26,58],[41,59],-90),
        ([12,54],[26,58],[41,59],-90),
        ([12,54],[26,58],[41,59],-90),
    ]
    for i,(head,chest,hip,angle) in enumerate(values):
        head=[head[0],min(head[1],51)]
        p=idle(kind)
        p.update(head=head,chest=chest,hip=hip,headAngle=angle,torso=True)
        if i>=3:
            p['knees']=[[42,min(60,55+i)],[51,min(60,54+i)]]
            p['feet']=[[51,min(62,57+i)],[60,min(62,57+i)]]
        if kind=='axe':
            a=[0,-.2,-.45,-.7,-1.0,-1.25,-1.5,-1.57][min(i,7)]
            butt=[61-i//2,min(61,56+i)]
            tip=[round(butt[0]+32*math.sin(a)),round(butt[1]-32*math.cos(a))]
            if i>=7: butt,tip=[62,58],[30,58]
        else:
            butt=[23,min(59,41+i*3)]
            tip=[80,min(59,33+i*4)]
        weapon(p,butt,tip)
        if i>=5:
            p['grips']=[None,None]
            p['hands']=[[max(20,43-i*2),min(61,48+i*2)],
                        [max(18,39-i*2),min(59,45+i*2)]]
        p['elbows']=[[chest[0]+9,min(60,chest[1]+8)],
                     [chest[0]-3,min(58,chest[1]+8)]]
        poses.append(p)
    poses[0]=idle(kind)
    return poses


def clips(kind):
    return dict(Idle=breathe(kind),Walk=walk(kind),Attack1=attack1(kind),
                Attack2=attack2(kind),Hurt=hurt(kind),Die=die(kind))
