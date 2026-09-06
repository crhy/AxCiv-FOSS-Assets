# rhYciv website

Static site for `rhyciv.org` and `rhyciv.com`, deployed as a Cloudflare Worker
with static assets (config in `wrangler.jsonc` at the repo root).

Cloudflare Workers Builds (git integration) settings:
- Production branch: `master`
- Root directory: `/`
- Build command: `bash website/build.sh`
- Deploy command: `npx wrangler deploy`

`wrangler.jsonc` points the `assets` binding at `website/dist`, so the deploy
command needs no extra flags. The build copies current art from
`RaylibUI/FOSSart`, so future art replacements remain easy.

Recommended canonical host: `https://rhyciv.org`. Redirect `www.rhyciv.org`, `rhyciv.com`, and `www.rhyciv.com` to the canonical host while preserving paths and query strings.
