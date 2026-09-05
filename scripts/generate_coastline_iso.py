import os
import numpy as np
from PIL import Image

W, H = 300, 150          # diamond footprint (2:1 isometric)
SS = 3                   # supersample factor
PAD = 300                # padded canvas size for the 300x300 variant
OUT = "/mnt/user-data/outputs/coastline_iso"
os.makedirs(OUT, exist_ok=True)
os.makedirs(OUT + "/padded_300x300", exist_ok=True)

S = 300.0                # world-space tile size in px (drives all frequencies)
TAU = 2 * np.pi

# ---------------- periodic value noise sampled at arbitrary (u,v) ------------
rng = np.random.default_rng(20240517)

def grid(freq):
    return rng.random((freq, freq))

def nsample(g, u, v):
    f = g.shape[0]
    cu, cv = u * f, v * f
    iu = np.floor(cu).astype(np.int64); fu = cu - iu
    iv = np.floor(cv).astype(np.int64); fv = cv - iv
    iu %= f; iv %= f
    ju, jv = (iu + 1) % f, (iv + 1) % f
    su = fu * fu * fu * (fu * (fu * 6 - 15) + 10)
    sv = fv * fv * fv * (fv * (fv * 6 - 15) + 10)
    top = g[iv, iu] * (1 - su) + g[iv, ju] * su
    bot = g[jv, iu] * (1 - su) + g[jv, ju] * su
    return top * (1 - sv) + bot * sv

class FBM:
    def __init__(self, base, octaves, gain=0.5, lac=2):
        self.layers = []
        amp, f, norm = 1.0, base, 0.0
        for _ in range(octaves):
            self.layers.append((grid(f), amp)); norm += amp; amp *= gain; f *= lac
        self.norm = norm
    def __call__(self, u, v):
        tot = 0.0
        for g, amp in self.layers:
            tot = tot + amp * nsample(g, u, v)
        return tot / self.norm

def smoothstep(e0, e1, x):
    t = np.clip((x - e0) / (e1 - e0), 0, 1)
    return t * t * (3 - 2 * t)

F_coast  = FBM(3, 6)
F_coast2 = FBM(11, 4)
F_grain  = FBM(100, 3)
F_grain2 = FBM(200, 2)
F_blotch = FBM(9, 3)
F_ripple = FBM(26, 2)
F_foam   = FBM(34, 4)
F_foam2  = FBM(70, 3)
F_bed    = FBM(5, 4)
F_caus   = FBM(17, 3)
F_caus2  = FBM(33, 2)
F_glint  = FBM(120, 2)
F_shelf  = FBM(4, 3)
F_swellw = FBM(7, 3)
F_spray  = FBM(150, 2)
F_tone   = FBM(13, 3)
F_blot2  = FBM(21, 2)
G_speck  = grid(300)
G_shell  = grid(150)

# ---------------- colour ramp -------------------------------------------------
STOPS = [
    (-420, (  5,  28,  60)), (-150, (  5,  28,  60)), (-130, (  7,  44,  82)),
    (-108, ( 10,  66, 110)), ( -84, ( 14,  94, 138)), ( -64, ( 19, 126, 164)),
    ( -48, ( 26, 160, 184)), ( -36, ( 42, 192, 196)), ( -26, ( 66, 214, 206)),
    ( -18, (100, 228, 214)), ( -11, (140, 236, 220)), (  -5, (176, 226, 205)),
    (  -1, (166, 146, 116)), (   4, (146, 124,  96)), (  13, (178, 154, 122)),
    (  32, (210, 190, 154)), (  72, (233, 219, 187)), ( 150, (245, 234, 208)),
    ( 420, (245, 234, 208)),
]
_ds = np.array([s[0] for s in STOPS], float)
_cs = np.array([s[1] for s in STOPS], float)

def ramp(d):
    out = np.empty(d.shape + (3,))
    for k in range(3):
        out[..., k] = np.interp(d, _ds, _cs[:, k])
    return out

# ---------------- screen -> world (inverse isometric projection) -------------
# World square corners map to diamond vertices:
#   (u,v)=(0,0) -> N (top)    (1,0) -> E (right)
#   (1,1)       -> S (bottom) (0,1) -> W (left)
sw, sh = W * SS, H * SS
px = (np.arange(sw) + 0.5) / SS
py = (np.arange(sh) + 0.5) / SS
SX = np.tile(px, (sh, 1))
SY = np.tile(py[:, None], (1, sw))
a = SY / (H / 2)
b = (SX - W / 2) / (W / 2)
U_raw = 0.5 * (a + b)
V_raw = 0.5 * (a - b)

EPS = 0.005                      # hair of overlap so neighbours don't hairline
inside = ((U_raw > -EPS) & (U_raw < 1 + EPS) &
          (V_raw > -EPS) & (V_raw < 1 + EPS))
U = np.clip(U_raw, 0, 1)         # clamped for colour: extends art past the edge
V = np.clip(V_raw, 0, 1)         # so downsampling can't pull in a halo

AMP = 0.115

def build(N, E, Sc, Wc):
    """Corners in world order TL,TR,BR,BL == screen N,E,S,W."""
    TL, TR, BR, BL = N, E, Sc, Wc
    f = ((1 - U) * (1 - V) * TL + U * (1 - V) * TR +
         U * V * BR + (1 - U) * V * BL)
    dfu = (1 - V) * (TR - TL) + V * (BR - BL)
    dfv = (1 - U) * (BL - TL) + U * (BR - TR)
    gmag = np.maximum(np.sqrt(dfu ** 2 + dfv ** 2) / S, 0.42 / S)

    warp = AMP * (0.84 * (F_coast(U, V) - 0.5) +
                  0.16 * (F_coast2(U, V) - 0.5)) * 2.0
    d = np.clip((f - 0.5 + warp) / gmag, -150.0, 150.0)

    n_shelf = F_shelf(U, V)
    shelf = ((n_shelf - 0.5) * 55 * smoothstep(-4, -30, d)
             * smoothstep(-148, -100, d))
    d_col = d + shelf

    # Narrow the shelf and trim the beach before the ramp is applied, leaving every
    # texture term below on the distances it was tuned for. Untouched, the ramp did
    # not reach deep ocean until 150 world pixels out - half a tile - so an ocean
    # tile touching land was bright edge to edge, and everything landward of the
    # shoreline was sand, putting the beach half a tile out to sea. Kept in step
    # with scripts/tune_coastline.py, which applies the same move to painted tiles
    # on machines without Pillow.
    SHELF_REACH = 0.38          # sea is at full depth by about 57px rather than 150
    BEACH_TRIM = 46.0           # world pixels of sand trimmed back towards the land
    d_col = np.where(d_col < 0, d_col / SHELF_REACH, d_col - BEACH_TRIM)
    img = ramp(d_col)

    n_grain, n_grain2 = F_grain(U, V), F_grain2(U, V)
    n_speck = nsample(G_speck, U, V)
    n_shell = nsample(G_shell, U, V)
    n_blotch, n_ripple = F_blotch(U, V), F_ripple(U, V)
    n_foam, n_foam2 = F_foam(U, V), F_foam2(U, V)
    n_bed, n_spray = F_bed(U, V), F_spray(U, V)

    # ---- sand ---------------------------------------------------------------
    land = smoothstep(-2, 6, d)
    grain = (n_grain - 0.5) * 20 + (n_grain2 - 0.5) * 14 + (n_speck - 0.5) * 10
    img += (land * grain)[..., None]
    img += land[..., None] * ((n_blotch - 0.5) * 14)[..., None] * np.array([1.0, .93, .80])
    img += land[..., None] * ((F_blot2(U, V) - 0.5) * 13)[..., None] * np.array([1.0, .95, .85])
    img += (smoothstep(0.93, 1.0, n_shell) * smoothstep(10, 40, d) * 26)[..., None]
    dune = (smoothstep(22, 95, d)
            * np.sin(TAU * (17 * U + 7 * V) + (n_ripple - 0.5) * 7.0) * 5.5)
    img += dune[..., None]
    wrack = (smoothstep(20, 27, d) * (1 - smoothstep(31, 46, d))
             * smoothstep(0.45, 0.75, n_foam))
    img -= (wrack * 18)[..., None] * np.array([1.0, 1.05, 1.15])
    wet = (1 - smoothstep(4, 20, d)) * smoothstep(-2, 2, d)
    img -= (wet * 26)[..., None] * np.array([1.0, 1.02, 0.86])
    img += (wet * 14)[..., None] * np.array([0.35, 0.75, 1.0])
    img -= ((1 - smoothstep(16, 34, d)) * smoothstep(2, 12, d) * 9)[..., None]

    # ---- water body ---------------------------------------------------------
    sea = 1 - smoothstep(-4, 2, d)
    patch = smoothstep(0.56, 0.86, n_bed) * sea * smoothstep(-120, -30, d_col)
    img[..., 0] -= patch * 30; img[..., 1] -= patch * 8; img[..., 2] -= patch * 14
    img += ((F_tone(U, V) - 0.5) * 19 * sea)[..., None] * np.array([0.5, 0.9, 1.0])
    img += ((n_bed - 0.5) * 9 * sea)[..., None] * np.array([0.5, 0.9, 1.0])
    sr = (np.sin(TAU * (7 * U + 2 * V) + (n_ripple - 0.5) * 9.0)
          * smoothstep(-70, -16, d_col) * sea * 5)
    img += sr[..., None]

    sww = (F_swellw(U, V) - 0.5) * 5.0
    swell = np.sin(TAU * (3 * U + 5 * V) + sww)
    img += (swell * 2.4 * sea)[..., None] * np.array([0.7, 1.0, 1.0])
    swell2 = np.sin(TAU * (-5 * U + 3 * V) - sww)
    img += (swell2 * 2.0 * sea)[..., None] * np.array([0.7, 1.0, 1.0])
    crest = np.clip(swell, 0, 1) ** 3 * smoothstep(-150, -60, d_col) * sea
    img += (crest * 6)[..., None]

    cw = (F_caus(U, V) * 2 - 1) * 2.4 + (F_caus2(U, V) * 2 - 1) * 1.1
    c = np.clip(np.sin(TAU * 11 * U + cw) * np.sin(TAU * 10 * V - cw), 0, 1) ** 2
    cmask = smoothstep(-85, -22, d_col) * (1 - smoothstep(-15, -7, d_col)) * sea
    img += (c * cmask * 20)[..., None] * np.array([0.8, 1.0, .98])

    sw_ = (smoothstep(0.48, 0.82, n_foam)
           * (1 - smoothstep(-58, -44, d)) * smoothstep(-76, -62, d))
    img += (sw_ * 34)[..., None]

    # ---- surf ---------------------------------------------------------------
    band = smoothstep(-14, -8, d) * (1 - smoothstep(-1, 4, d))
    lace = smoothstep(0.40, 0.74, n_foam * 0.55 + n_foam2 * 0.45)
    foam = band * (0.20 + 0.60 * lace)
    lip = smoothstep(-5.0, -2.6, d) * (1 - smoothstep(-1.8, 1.6, d))
    foam = np.clip(foam + lip * 0.72, 0, 1)
    foam = np.clip(foam + smoothstep(3, 6, d) * (1 - smoothstep(8, 15, d))
                   * smoothstep(0.62, 0.90, n_foam2) * 0.42, 0, 1) * 0.92
    foam = np.clip(foam + smoothstep(0.90, 0.99, n_spray)
                   * smoothstep(-22, -12, d) * (1 - smoothstep(6, 12, d)) * 0.5, 0, 1)
    img = img * (1 - foam[..., None]) + np.array([250.0, 253.0, 252.0]) * foam[..., None]

    gl = smoothstep(0.80, 0.97, F_glint(U, V)) * smoothstep(-150, -60, d_col) * sea
    img += (gl * 22)[..., None]

    rgba = np.zeros((sh, sw, 4), np.uint8)
    rgba[..., :3] = np.clip(img, 0, 255).astype(np.uint8)
    rgba[..., 3] = np.where(inside, 255, 0)
    return Image.fromarray(rgba, "RGBA").resize((W, H), Image.LANCZOS)

# ---------------- 16 tiles ----------------------------------------------------
NAMES = {
    0: "ocean",          1: "corner_land_W",   2: "corner_land_S",
    3: "edge_land_SW",   4: "corner_land_E",   5: "diagonal_E_W",
    6: "edge_land_SE",   7: "inner_water_N",   8: "corner_land_N",
    9: "edge_land_NW",  10: "diagonal_N_S",   11: "inner_water_E",
    12: "edge_land_NE", 13: "inner_water_S",  14: "inner_water_W",
    15: "land",
}
tiles = {}
for mask in range(16):
    n, e, s, w = (mask >> 3) & 1, (mask >> 2) & 1, (mask >> 1) & 1, mask & 1
    im = build(n, e, s, w)
    tiles[mask] = im
    im.save(f"{OUT}/coast_{mask:02d}_{NAMES[mask]}.png")
    pad = Image.new("RGBA", (PAD, PAD), (0, 0, 0, 0))
    pad.paste(im, ((PAD - W) // 2, PAD - H))      # bottom-anchored
    pad.save(f"{OUT}/padded_300x300/coast_{mask:02d}_{NAMES[mask]}.png")

# ---------------- contact sheet ----------------------------------------------
CW, CH = 200, 100
sheet = Image.new("RGBA", (CW * 4 + 40, CH * 4 + 40), (30, 30, 34, 255))
for mask in range(16):
    r, c = divmod(mask, 4)
    sheet.alpha_composite(tiles[mask].resize((CW, CH), Image.LANCZOS),
                          (8 + c * (CW + 8), 8 + r * (CH + 8)))
sheet.convert("RGB").save(f"{OUT}/_contact_sheet.png")

# ---------------- assembled isometric demo -----------------------------------
GW, GH = 10, 10
cw_, ch_ = GW + 1, GH + 1
cy, cx = np.mgrid[0:ch_, 0:cw_]
ncx, ncy = cx / (cw_ - 1) - .5, cy / (ch_ - 1) - .5
r = np.sqrt((ncx * 1.3) ** 2 + (ncy * 1.3) ** 2)
lobe = 0.30 + 0.08 * np.sin(np.arctan2(ncy, ncx) * 3.0 + 1.1)
lg = (r < lobe).astype(int)
lg[2, 8] = 1; lg[8, 2] = 1

TW, TH = 120, 60
ox, oy = GH * TW // 2, 20
demo = Image.new("RGBA", (TW * (GW + GH) // 2 + 40, TH * (GW + GH) // 2 + 80),
                 (16, 22, 40, 255))
small = {m: tiles[m].resize((TW, TH), Image.LANCZOS) for m in tiles}
for ty in range(GH):
    for tx in range(GW):
        m = ((lg[ty, tx] << 3) | (lg[ty, tx + 1] << 2) |
             (lg[ty + 1, tx + 1] << 1) | lg[ty + 1, tx])
        X = ox + (tx - ty) * TW // 2
        Y = oy + (tx + ty) * TH // 2
        demo.alpha_composite(small[m], (X, Y))
demo.convert("RGB").save(f"{OUT}/_demo_island.png")
print("done")
