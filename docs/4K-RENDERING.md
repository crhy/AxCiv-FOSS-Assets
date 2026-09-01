# 4K rendering

rhYciv keeps a compact classic-strategy interface geometry while rendering at modern display density.

## How scaling works

The desktop client uses a 1920x1080 reference canvas. At larger window sizes it selects a display scale in quarter-step increments, up to 2.5x. A 3840x2160 window therefore uses a 2x scale and exposes a 1920x1080 logical layout to menus, dialogs, map hit testing, and mouse input.

This is native rendering, not a final-frame upscale:

- the bundled OFL-licensed Liberation fonts are rasterized at high resolution;
- lines, rectangles, labels, and other UI primitives are drawn directly at display resolution;
- large FOSS textures use bilinear downsampling into their logical footprint;
- the CPU-composed map background is created at the active display density and drawn back at a reciprocal logical scale;
- changing display density invalidates the map view so the backing texture is rebuilt at the new resolution.

Press `F11` to toggle a borderless window at the desktop's current resolution. Normal resizable-window operation remains available.

## Terrain detail

The compatibility grid uses 64x32 logical terrain tiles. rhYciv retains the bundled 1024x1024 project art at a 128x64 working tile size and composes coasts, rivers, resources, improvements, huts, fog dithering, and grid overlays into that higher-resolution target. On a 4K display at 2x UI scale, the normal-zoom map backing reaches the screen without an intermediate 64x32 downsample.

## Remaining art work

Rendering is 4K-aware throughout the Raylib client, but a native-resolution pipeline cannot invent detail absent from a source bitmap. All runtime overlays and interface adapters are now project-original or generated from attributed inputs. Several procedural resource markers and animation frames remain visually simple and can be replaced with richer original art without changing the renderer. See `ASSET-PROVENANCE.md` before adding or replacing media.
