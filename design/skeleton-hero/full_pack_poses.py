"""Review-only poses: extracted axe Idle and authored whole-body animations.

Public GIFs do not expose every axe clip, nor a spear. Those poses are authored
reconstructions, not extracted premium cels. Coordinates are native pixels.
"""
from copy import deepcopy
import math
from prepare_weapons import spear_poses
from axe_reference import SHAFT_LENGTH

def head_at(kind, butt, tip):
    length = math.dist(butt, tip)
    ux, uy = [(tip[j] - butt[j]) / length for j in range(2)]
    local = [[[-14, 0], [-11, -2], [-8, -3], [-4, -2], [0, 0], [-4, 2], [-8, 3], [-11, 2]], [[-12, 0], [-8, -2], [-4, -1], [0, 0], [-7, 0]], [[-13, 0], [0, 0]], [[-17, -1], [-14, -1], [-13, 0], [-14, 1], [-17, 1]]]
    return {name: [[round(tip[0] + a * ux - b * uy), round(tip[1] + a * uy + b * ux)] for a, b in shape] for name, shape in zip(['blade', 'light', 'edge', 'socket'], local)}

def weapon(p, butt, tip, grips=None):
    target = 58
    length = math.dist(butt, tip)
    tip = [round(butt[j] + (tip[j] - butt[j]) * target / length) for j in range(2)]
    shape = head_at(p['kind'], butt, tip)
    points = [butt, tip] + [q for polygon in shape.values() for q in polygon]
    shift = [max(0, 1 - min((q[j] for q in points))) - max(0, max((q[j] for q in points)) - (94 if j == 0 else 63)) for j in range(2)]
    butt = [butt[j] + shift[j] for j in range(2)]
    tip = [tip[j] + shift[j] for j in range(2)]
    p['butt'], p['tip'] = (butt, tip)
    p.update(head_at(p['kind'], butt, tip))
    if grips is None:
        grips = [17, 31]
    p['grips'] = grips
    length = math.dist(butt, tip)
    for h, d in enumerate(grips):
        if d is not None:
            p['hands'][h] = [round(butt[j] + (tip[j] - butt[j]) * d / length) for j in range(2)]
    return p

def idle(kind):
    p = deepcopy(spear_poses()[0])
    p.update(feet=[[36, 62], [53, 62]], headAngle=0, fx=False, torso=False)
    weapon(p, p['butt'], p['tip'])
    return p

def breathe(kind):
    poses = []
    for dy in [0, 0, -1, -1, 0, 0, 1, 1]:
        p = idle(kind)
        for name in ['head', 'chest']:
            p[name][1] += dy
        for q in p['elbows']:
            q[1] += dy
        weapon(p, [23, 41 + dy], [80, 33 + dy])
        poses.append(p)
    return poses

def walk(kind):
    poses = []
    stride = [-9, -6, -2, 3, 7, 9, 6, 2, -3, -7]
    lift = [0, 0, 0, 0, 0, 0, -3, -5, -4, -2]
    for i in range(10):
        p = idle(kind)
        dy = [0, -1, -1, 0, 1, 0, -1, -1, 0, 1][i]
        p['feet'] = [[44 + stride[i], 62 + lift[i]], [44 + stride[(i + 5) % 10], 62 + lift[(i + 5) % 10]]]
        p['knees'] = [[42 + round(stride[i] * 0.45), 54 + min(0, lift[i])], [46 + round(stride[(i + 5) % 10] * 0.45), 54 + min(0, lift[(i + 5) % 10])]]
        for name in ['head', 'chest', 'hip']:
            p[name][1] += dy
        for q in p['elbows']:
            q[1] += dy
        weapon(p, [23, 41 + dy], [80, 33 + dy])
        poses.append(p)
    return poses

def attack1(kind):
    poses = deepcopy(spear_poses() + [spear_poses()[0]])
    for i, p in enumerate(poses):
        p.update(feet=[[36, 62], [53, 62]], headAngle=0)
        weapon(p, p['butt'], p['tip'])
    poses[0] = idle(kind)
    poses[-1] = idle(kind)
    return poses

def attack2(kind):
    source = spear_poses()
    poses = []
    for i, index in enumerate([0, 1, 2, 3, 5, 6, 7, 8, 0]):
        p = deepcopy(source[index])
        p.update(feet=[[36, 62], [53, 62]], headAngle=0, fx=i == 4, torso=True)
        drop = [0, 2, 4, 5, 6, 6, 4, 2, 0][i]
        p['head'][1] += drop // 2
        p['chest'][1] += drop // 2
        p['hip'][1] += drop // 3
        weapon(p, [p['butt'][0], p['butt'][1] + drop], [p['tip'][0], p['tip'][1] + drop])
        for e in p['elbows']:
            e[1] += drop
        poses.append(p)
    poses[0] = idle(kind)
    poses[-1] = idle(kind)
    return poses

def hurt(kind):
    poses = []
    for i, (dx, dy, tilt) in enumerate([(0, 0, 0), (-4, -2, -7), (-5, 0, -9), (-2, 1, -4), (0, 0, 0)]):
        p = idle(kind)
        for name in ['head', 'chest']:
            p[name][0] += dx
            p[name][1] += dy
        p['hip'][0] += dx // 2
        for e in p['elbows']:
            e[0] += dx
            e[1] += dy
        weapon(p, [23 + dx, 41 + dy], [80 + dx, 33 + dy + tilt // 2])
        p['flash'] = i == 1
        poses.append(p)
    return poses

def die(kind):
    poses = []
    values = [([40, 18], [43, 31], [43, 44], 0), ([36, 16], [39, 29], [42, 43], -8), ([32, 19], [36, 32], [41, 45], -18), ([27, 24], [32, 36], [39, 48], -30), ([22, 32], [29, 42], [38, 51], -45), ([17, 41], [27, 49], [38, 55], -60), ([14, 49], [26, 55], [39, 58], -78), ([13, 52], [26, 57], [40, 59], -88), ([13, 53], [26, 58], [41, 59], -90), ([12, 54], [26, 58], [41, 59], -90), ([12, 54], [26, 58], [41, 59], -90), ([12, 54], [26, 58], [41, 59], -90), ([12, 54], [26, 58], [41, 59], -90)]
    for i, (head, chest, hip, angle) in enumerate(values):
        head = [head[0], min(head[1], 51)]
        p = idle(kind)
        p.update(head=head, chest=chest, hip=hip, headAngle=angle, torso=True)
        if i >= 3:
            p['knees'] = [[42, min(60, 55 + i)], [51, min(60, 54 + i)]]
            p['feet'] = [[51, min(62, 57 + i)], [60, min(62, 57 + i)]]
        butt = [23, min(59, 41 + i * 3)]
        tip = [80, min(59, 33 + i * 4)]
        weapon(p, butt, tip)
        if i >= 5:
            p['grips'] = [None, None]
            p['hands'] = [[max(20, 43 - i * 2), min(61, 48 + i * 2)], [max(18, 39 - i * 2), min(59, 45 + i * 2)]]
        p['elbows'] = [[chest[0] + 9, min(60, chest[1] + 8)], [chest[0] - 3, min(58, chest[1] + 8)]]
        poses.append(p)
    poses[0] = idle(kind)
    return poses


def clips(kind):
    if kind == "axe":
        return axe_clips()
    return dict(Idle=breathe(kind), Walk=walk(kind), Attack1=attack1(kind), Attack2=attack2(kind), Hurt=hurt(kind), Die=die(kind))


def axe_clips():
    """Whole-body poses for the source staff, with no independent bounds shifts."""
    def pose(chest, hip, skull, butt, angle, grips=(22,32), bend=1):
        p = dict(kind='axe',lean=None)
        radians = math.radians(angle)
        tip = [round(butt[0]+SHAFT_LENGTH*math.cos(radians)),
               round(butt[1]+SHAFT_LENGTH*math.sin(radians))]
        p.update(chest=list(chest),hip=list(hip),head=list(skull),butt=list(butt),tip=tip,
                 grips=list(grips),feet=[[39,62],[53,62]],knees=[[40,54],[51,54]],
                 headAngle=0,fx=False,torso=True,referenceWeapon=True)
        # Head landmarks are the source cutting rim, transformed with the staff.
        length=math.dist(butt,tip)
        ux,uy=[(tip[j]-butt[j])/length for j in range(2)]
        # Canonical source axis has a slight lean; preserve it in every pose.
        cu,cv=3/math.hypot(3,49),-49/math.hypot(3,49)
        def point(x,y):
            dx,dy=x-61,y-14
            a,b=dx*cu+dy*cv,-dx*cv+dy*cu
            return [round(tip[0]+a*ux-b*uy),round(tip[1]+a*uy+b*ux)]
        p['edge']=[point(x,y) for x,y in [(66,15),(67,17),(67,23),(66,26),(64,29)]]
        p['blade']=p['edge'];p['socket']=[point(60,21),point(62,24)]
        p['light']=p['edge']
        p['shoulders']=[[chest[0]+7,chest[1]+2],[chest[0]-3,chest[1]+2]]
        p['hands']=[];p['elbows']=[]
        for h,d in enumerate(grips):
            hand=([round(butt[j]+(tip[j]-butt[j])*d/length) for j in range(2)]
                  if d is not None else [chest[0]-8,chest[1]+16])
            shoulder=p['shoulders'][h]
            # Projected elbow follows the shoulder and wrist, with a readable bend.
            elbow=[round((shoulder[0]+hand[0])/2)+(3 if h==0 else -4)*bend,
                   round((shoulder[1]+hand[1])/2)+4]
            p['hands'].append(hand);p['elbows'].append(elbow)
        return p

    base=pose((44,31),(44,44),(40,18),(58,63),-86.5,(22,None))
    base.update(butt=[58,63],tip=[61,14],referenceIdle=0)
    idle=[]
    for i,dy in enumerate([0,0,1,2,2,1,0,0,0]):
        p=deepcopy(base);p['referenceIdle']=i
        for name in ['head','chest','tip']:
            p[name][1]+=dy
        for name in ['hands','elbows','shoulders','edge','socket']:
            p[name]=[[x,y+dy] for x,y in p[name]]
        idle.append(p)
    walk=[]
    for i in range(10):
        dy=[0,-1,-1,0,1,0,-1,-1,0,1][i]
        p=pose((44,31+dy),(44,44+dy),(40,18+dy),(58,62+min(dy,0)),
               [-86,-85,-84,-85,-86,-87,-88,-89,-88,-87][i],(22,None))
        stride=[-8,-5,-2,3,7,8,5,2,-3,-7];lift=[0,0,0,0,0,0,-3,-5,-4,-2]
        p['feet']=[[45+stride[i],62+lift[i]],[45+stride[(i+5)%10],62+lift[(i+5)%10]]]
        p['knees']=[[42+round(stride[i]*.4),54+lift[i]],[46+round(stride[(i+5)%10]*.4),54+lift[(i+5)%10]]]
        walk.append(p)
    # Anticipation: catch the upper shaft, then draw it behind the shoulder.
    values=[
        ((43,32),(43,45),(39,19),(62,62),-86,(22,30)),
        ((42,36),(43,48),(38,23),(65,19),-170,(22,33)),
        ((40,37),(42,49),(35,24),(65,20),-173,(22,33)),
        ((48,37),(45,48),(47,24),(35,42),-8,(22,33)),
    ]
    attack=[deepcopy(base)]+[pose(*v) for v in values]
    # Source head raster reaches y63 at this orientation (steel cutting rim).
    for i in range(6):
        attack.append(pose((50+(i==1),40+(i in (1,2))),
                           (45,48+(i in (1,2))),(50,27+(i in (1,2))),
                           (35,56),0,(22,33)))
    attack.extend([
        pose((48,38),(44,47),(47,25),(34,43),0,(22,33)),
        pose((46,35),(44,46),(43,22),(35,51),-30,(22,33)),
        pose((45,33),(44,45),(41,20),(43,59),-60,(22,31)),
        pose((44,31),(44,44),(40,18),(55,62),-82,(22,None)),
        deepcopy(base)])
    attack[4]['fx']=True
    attack[4]['sweep']=axe_sweep(attack[3],attack[4],pose)
    lateral=[deepcopy(base)]
    for v in [
        ((42,35),(43,47),(38,22),(65,18),-170,(22,33)),
        ((40,37),(42,49),(35,24),(65,20),-173,(22,33)),
        ((41,37),(42,49),(36,24),(65,20),-173,(22,33)),
        ((49,35),(46,47),(49,22),(36,37),0,(22,33)),
        ((50,37),(47,48),(50,24),(35,40),12,(22,33)),
        ((47,36),(45,47),(46,23),(35,43),0,(22,33)),
        ((45,33),(44,45),(41,20),(46,59),-65,(22,None)),
    ]:lateral.append(pose(*v))
    lateral.append(deepcopy(base));lateral[4]['fx']=True
    lateral[4]['sweep']=axe_sweep(lateral[3],lateral[4],pose)
    hurt=[deepcopy(base)]
    for dx,dy in [(-4,-2),(-5,0),(-2,1)]:
        hurt.append(pose((44+dx,31+dy),(44+dx//2,44),(40+dx,18+dy),(64+dx,61),-86,(22,None)))
    hurt.append(deepcopy(base))
    death=[]
    for i in range(13):
        if i==0:death.append(deepcopy(base));continue
        chest=(max(26,44-i*3),min(57,31+i*4))
        p=pose(chest,(max(39,44-i),min(59,44+i*2)),
               (max(12,40-i*5),min(51,18+i*5)),
               (min(78,58+i*3),min(59,61-i)),
               [-87,-97,-115,-135,-155,-170,-180][min(i,6)],
               (22,None) if i<4 else (None,None))
        if i>=4:
            butt,angle=[((40,55),-35),((36,53),-15),((37,54),-5),((38,56),0)][min(i-4,3)]
            p=pose(chest,(max(39,44-i),min(59,44+i*2)),
                   (max(12,40-i*5),min(51,18+i*5)),butt,angle,(None,None))
        p['headAngle']=-min(90,i*15)
        if i>=3:
            p['knees']=[[42,min(60,55+i)],[51,min(60,54+i)]]
            p['feet']=[[51,min(62,57+i)],[60,min(62,57+i)]]
        if i>=4:
            p['hands']=[[max(20,chest[0]+7),min(61,chest[1]+7)],
                        [max(18,chest[0]-4),min(59,chest[1]+5)]]
            p['elbows']=[[chest[0]+9,min(60,chest[1]+3)],
                         [chest[0]-4,min(58,chest[1]+3)]]
        death.append(p)
    return dict(Idle=idle,Walk=walk,Attack1=attack,Attack2=lateral,Hurt=hurt,Die=death)


def axe_sweep(a,b,make_pose):
    aa=math.degrees(math.atan2(a['tip'][1]-a['butt'][1],a['tip'][0]-a['butt'][0]))
    ba=math.degrees(math.atan2(b['tip'][1]-b['butt'][1],b['tip'][0]-b['butt'][0]))
    result=[]
    for i in range(17):
        t=i/16
        butt=[round(a['butt'][j]*(1-t)+b['butt'][j]*t) for j in range(2)]
        # Hands dip on the transition, so the long head clears the top edge.
        butt[1]+=round(30*math.sin(math.pi*t))
        result.append(make_pose((44,34),(44,46),(40,21),butt,aa+(ba-aa)*t)['edge'])
    result[0]=a['edge'];result[-1]=b['edge']
    return result
