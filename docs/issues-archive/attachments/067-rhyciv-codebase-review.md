# Codebase review: structure, build hygiene, and a proposed audit plan

## Scope and method — read this first

This review was assembled from what is publicly readable on `master` without a
clone: the repository root listing, `README.md`, `docs/ARCHITECTURE.md`,
`CONTRIBUTING.md`, and `Directory.Build.props`. Directory trees below the root
and individual `.cs` / `.csproj` files were not reachable, so **no C# source was
read**.

Findings are therefore split into two kinds, and they are labelled throughout:

- **[Verified]** — observed directly in a file listed above.
- **[Check]** — a concrete hypothesis worth confirming locally. Each one comes
  with the command or search that confirms or kills it. Do not treat these as
  established defects.

The final section is a repeatable local audit that closes the gap.

---

## 1. Summary

The project is in better shape than most hobby forks: there is a real
architecture document, a written contribution policy, an asset provenance
manifest, a `quality_gate.sh` that gates releases, a dedicated test project, and
a Flatpak packaging path. The clean-room asset discipline in particular is
unusually rigorous.

The weak points cluster in three places:

1. **Identity debt from the fork.** The product is `rhYciv`; the solution,
   several project names, and at least one on-disk data path still carry the
   upstream Civ2-clone identity. This is cheap to fix now and expensive later.
2. **No enforced compiler or style contract.** `Directory.Build.props` sets a
   version and nothing else, so quality rules live in prose in `CONTRIBUTING.md`
   rather than in the build.
3. **A test project scoped to the engine, with the renderer untested.** The
   documented architecture puts a lot of complexity in `RaylibUI` (zoom
   -7..16, atlas loading, lazy GPU cache, shaders) and none of it is covered by
   `Core.Tests` as described.

---

## 2. Fork and naming debt

- **[Verified]** The solution file is `Civ2clone.sln` while the product,
  Flatpak ID (`io.github.crhy.rhYciv`), and README all say rhYciv.
- **[Verified]** Project names `Civ2`, `Civ2Gold`, `Civ2TOT` describe upstream
  compatibility layouts, not rhYciv concepts. `docs/ARCHITECTURE.md` has to
  spend a paragraph explaining that `Civ2Gold` is "the primary compact desktop
  interface implementation used by standalone rhYciv" — the name is actively
  costing documentation.
- **[Verified]** Saves are written to `AxxCiv/Saves` under the platform data
  directory. A user's save directory should not be named after the upstream
  project.
- **[Verified]** The repo is still a GitHub fork of `axx0/Civ2-clone`, so the
  Issues/PR/Insights UI and default PR base behave as a fork.

### Suggested actions

- [ ] Rename `Civ2clone.sln` → `rhYciv.sln`.
- [ ] Rename `Civ2Gold` → something describing what it *is* (e.g.
      `UI.Compact` / `UI.Standalone`), and `Civ2TOT` → `UI.CompatAlternate`.
      Keep `Civ2` if it genuinely means "classic-layout adapters", but say so in
      the project description.
- [ ] Migrate the save directory to `rhYciv/Saves`, with a one-time move of
      `AxxCiv/Saves` on first launch and a fallback read path for one release.
      Document the migration in `ARCHITECTURE.md`.
- [ ] Decide explicitly whether to detach the fork. Given 1,208 commits and a
      divergent product scope, detaching (GitHub Support can do this) makes the
      repo standalone, gives it its own network graph, and stops accidental
      upstream-targeted PRs. If you intend to keep pulling upstream fixes,
      document that instead in `CONTRIBUTING.md` and keep the fork link.
- [ ] `NOTICE.md` should state clearly which parts are upstream-derived and
      which are rhYciv-original. This matters more once names diverge.

---

## 3. Build configuration and compiler hygiene

- **[Verified]** `Directory.Build.props` is 11 lines and sets only
  `VersionPrefix` / `VersionSuffix`. Everything else — target framework,
  nullable, warning level, analyzers, determinism — is either per-project or
  absent.
- **[Verified]** `CONTRIBUTING.md` asks contributors to "target .NET 9 and
  preserve nullable annotations". That is a policy with no enforcement
  mechanism; a PR that adds `#nullable disable` or a new project on `net8.0`
  passes.

### Suggested actions

- [ ] Move shared settings into `Directory.Build.props`:

```xml
<PropertyGroup>
  <TargetFramework>net9.0</TargetFramework>
  <LangVersion>latest</LangVersion>
  <Nullable>enable</Nullable>
  <ImplicitUsings>enable</ImplicitUsings>
  <AnalysisLevel>latest</AnalysisLevel>
  <EnableNETAnalyzers>true</EnableNETAnalyzers>
  <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  <Deterministic>true</Deterministic>
  <ContinuousIntegrationBuild
    Condition="'$(GITHUB_ACTIONS)' == 'true'">true</ContinuousIntegrationBuild>
</PropertyGroup>
```

- [ ] Turning on `TreatWarningsAsErrors` across a 1,200-commit codebase will
      produce a wall of warnings. Do it in two steps: land the property with
      `<WarningsNotAsErrors>` listing the current offenders, then burn that list
      down one rule per PR. Capture the starting count so progress is visible:
      `dotnet build -warnaserror- 2>&1 | grep -c "warning "`.
- [ ] Add `Directory.Packages.props` with
      `<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>` so
      Raylib-cs, the Lua binding, and the test packages can't drift to different
      versions across projects.
- [ ] Add a root `.editorconfig` (`dotnet new editorconfig` is a good start) and
      a `dotnet format --verify-no-changes` step. Style arguments in review are
      pure waste once a formatter owns the question.
- [ ] Add `global.json` pinning the SDK band, so "requires the .NET 9 SDK" in
      the README is enforced rather than hoped for.
- [ ] The version lives in two places (`Directory.Build.props` and
      `packaging/flatpak/io.github.crhy.rhYciv.metainfo.xml`) and the comment in
      the props file admits it. Add a `quality_gate.sh` assertion that the two
      agree — a release built with a mismatched AppStream version is a silent
      packaging bug.

---

## 4. Testing

- **[Verified]** `Core.Tests` is the only test project, described as
  "Engine/unit tests and deterministic clean-room save-fixture generation".
- **[Verified]** `CleanRoomGameFactory` parses bundled rules, generates a seeded
  map, creates civs, writes a save in memory, and reloads a fresh copy per test.
  This is a genuinely good design and should be the model for the gaps below.
- **[Check]** Nothing in `RaylibUI`, `RaylibUtils`, `Civ2`, `Civ2Gold`, or
  `Civ2TOT` appears to be under test.
  Confirm: `dotnet sln list` and check for any `*.Tests` project besides
  `Core.Tests`.

### Suggested actions

- [ ] Add a coverage baseline before adding tests, so the effort is measurable:
      `dotnet test --collect:"XPlat Code Coverage"` with `coverlet.collector`,
      then `reportgenerator` for the HTML. Record the starting number in this
      issue.
- [ ] **Save round-trip property tests.** `ARCHITECTURE.md` says many arrays are
      index-addressed and that dormant spaceship/throne/advisor fields exist for
      backward compatibility. That is exactly the shape of bug that silently
      corrupts saves. Add: generate → save → load → assert deep equality, over
      several seeds.
- [ ] **Index-contract guard tests.** Assert the counts and the ordering that
      `ARCHITECTURE.md` calls contracts (51 unit slots, 88 advances, 11 terrain
      types, governments, leaders). A failing test is a much better warning than
      a doc paragraph when someone reorders a table.
- [ ] **Rules-parser tests against malformed input.** `RULES.txt`, `CITY.txt`,
      and `Game.txt` are user-editable for mods; a truncated or reordered file
      should produce a clear error, not an `IndexOutOfRangeException` at turn 40.
- [ ] **Pure-logic extraction for the UI.** Zoom level math (-7..16), tile→screen
      and screen→tile transforms, and visible-tile culling are pure functions.
      Pull them out of the render path into `RaylibUtils` and unit test them.
      This is the highest-value UI testing available without a headless GPU.
- [ ] **Add a headless smoke test** that boots the game, advances N turns with a
      fixed seed, and asserts no exception — run it in CI. Catches the entire
      class of "startup broke on Linux" regressions.
- [ ] **[Check]** Are the Python scripts in `scripts/` tested at all? They
      generate shipped assets and the manifest, so a bug there ships. At minimum
      add `pytest` smoke tests for `generate_asset_manifest.py` and
      `build_standalone_sheets.py`, plus `ruff` in CI.

---

## 5. CI and the quality gate

- **[Verified]** `.github/workflows` exists and `scripts/quality_gate.sh`
  verifies manifest, restore, build, and test.
- **[Check]** The workflow contents were not readable. Confirm the following:

### Suggested actions

- [ ] Does CI run `quality_gate.sh` itself, or a separate duplicated set of
      steps? It should run the same script the README tells humans to run —
      otherwise the two drift and "works locally" becomes real.
- [ ] Is the build matrixed over Linux / Windows / macOS? The architecture doc
      names save paths for all three, so all three are supported claims and
      should be tested claims.
- [ ] Are Actions pinned to commit SHAs rather than floating tags
      (`actions/checkout@v4` → `actions/checkout@<sha>`)? For a project this
      careful about supply chain in *assets*, the CI supply chain deserves the
      same.
- [ ] Add `permissions: contents: read` at the workflow level.
- [ ] Enable Dependabot for NuGet and GitHub Actions.
- [ ] Add CodeQL for C#. Free for public repos, and it will catch a class of
      resource and null issues that a renderer-heavy codebase accumulates.
- [ ] Add a `concurrency` group so superseded PR runs cancel.
- [ ] Have CI upload the Flatpak build artifact on tags so a release never
      depends on one machine's local state.

---

## 6. Rendering, resources, and performance

`ARCHITECTURE.md` describes a renderer with real complexity: 1920x1080 logical
layout with native high-DPI drawing, 128x64 terrain tiles, 300x300 unit and
citizen sources, zoom from -7 to 16, per-unit shadows, naval wakes and waterline
splashes, and lazily decoded images cached and disposed on interface or ruleset
change. `CONTRIBUTING.md` adds: dispose CPU images and GPU textures explicitly,
and do not decode whole art sets during startup.

That is a policy stated twice in prose and enforced nowhere.

- [ ] **[Check]** Audit every `Image` / `Texture2D` acquisition for a matching
      unload. Search: `LoadTexture`, `LoadImage`, `LoadImageFromMemory`,
      `LoadRenderTexture` vs `UnloadTexture`, `UnloadImage`,
      `UnloadRenderTexture`. Raylib-cs types are not GC-managed; a missed unload
      is a straight VRAM leak.
- [ ] Wrap Raylib handles in `IDisposable` owner types (e.g. a
      `TextureHandle : IDisposable`) so lifetime is expressed in the type system
      rather than in review discipline. A `TextureCache` that owns every handle
      and exposes only borrowed references is the version of this that actually
      holds.
- [ ] Add a debug HUD (toggle key) showing loaded texture count, estimated VRAM,
      draw calls, and frame time. Leak regressions become visible in seconds
      instead of after a 3-hour game.
- [ ] **[Check]** Is the map draw loop culling to the visible viewport, or
      iterating all tiles and relying on Raylib to reject off-screen draws? At
      zoom -7 on a large map the difference is enormous. Confirm by profiling a
      big map at min zoom.
- [ ] **[Check]** Are per-tile draws batched by source atlas? Texture rebinds
      per tile are the classic 2D perf cliff.
- [ ] Consider caching rendered map chunks to a `RenderTexture2D` and
      invalidating per-tile on change, rather than redrawing every tile every
      frame. Panning becomes a blit.
- [ ] **[Check]** Look for allocation in the frame loop — LINQ, `string.Format`,
      lambda captures, and `List<T>` construction inside `Draw`. Confirm with a
      GC allocation profile over 60 seconds of idle map view; anything above a
      trickle is worth fixing.
- [ ] Add a benchmark or timing assertion for map generation and for a full turn
      at large map size, so performance regressions land as failures rather than
      as vibes.

---

## 7. Save format and versioning

- **[Verified]** Saves are JSON `.sav`. The model retains dormant spaceship,
  throne, advisor, and classic-save fields to keep older saves readable.
- **[Verified]** `ARCHITECTURE.md` requires "explicit mapping or a save-version
  migration before changing an index" — which implies a migration mechanism.

### Suggested actions

- [ ] **[Check]** Does a save actually carry a schema version number, and is
      there a migration chain? If the rule is documented but the field doesn't
      exist, add it now while the user base is small.
- [ ] Write a `docs/SAVE-FORMAT.md` capturing the versioning rule, the current
      version, and the migration steps that exist.
- [ ] Keep small committed sample saves for old versions and test that each
      still loads. These are code-generated fixtures, so they don't conflict
      with the no-binary-fixtures rule in `CONTRIBUTING.md`.
- [ ] The dormant fields deserve an audit and a decision: keep and document
      each, or drop it with a migration. Undocumented dead fields in a
      serialized model are the thing that makes format evolution scary later.
- [ ] Consider `System.Text.Json` source generation for the save model if it
      isn't already — faster, allocation-light, and AOT-friendly.

---

## 8. Extensibility surfaces: interface discovery and Lua

- **[Verified]** `RaylibUI` discovers `IUserInterface` implementations from
  built assemblies, and `Game.txt` routes the bundled `rhYciv Standalone`
  ruleset to the Gold-layout adapter.
- **[Verified]** The engine runs Lua behavior scripts from `Engine/Scripts`.

### Suggested actions

- [ ] **[Check]** Assembly scanning for `IUserInterface` — what happens when
      zero or two implementations match? Reflection-based discovery fails at
      runtime, in the dark, on a user's machine. Either make it fail loudly with
      a listing of what was scanned and what matched, or replace it with an
      explicit registration list (a static array of types) and keep scanning
      only for genuine third-party plugins.
- [ ] **[Check]** Trimming and single-file publish will break assembly scanning.
      If the Flatpak ever moves to `PublishTrimmed`, this is where it breaks
      first. Note it in `ARCHITECTURE.md` if nothing else.
- [ ] **[Check]** What is the error path when a Lua script throws mid-turn? A
      scripting error should surface as a named, catchable diagnostic with the
      script and line, not an unhandled exception that loses the turn.
- [ ] Define and document the Lua API surface the scripts may call, and version
      it. Right now the contract between `Engine/Scripts` and the engine is
      implicit in the code.
- [ ] Consider sandboxing what scripts can reach if scenario Lua may ever be
      distributed by third parties — at minimum, no filesystem or process
      access.

---

## 9. Error handling and diagnostics

- [ ] **[Check]** Search for `catch { }` and `catch (Exception) { }` — silent
      swallowing is the usual reason a rendering or asset bug takes a week to
      find.
- [ ] Add structured logging with levels to a file in the platform data
      directory, and mention its location in `docs/`. A bug report from a
      Flatpak user is nearly unactionable without one.
- [ ] Add a global unhandled-exception handler that writes a crash log including
      version, OS, GPU/driver string, and active ruleset, then shows a dialog
      pointing at the file.
- [ ] Asset and rules loading failures should name the file and the expected
      location. This is the single most common failure mode for a game that
      supports external rules directories.

---

## 10. Documentation and repository ergonomics

The docs are a strength — `ARCHITECTURE.md`, `GAMEPLAY.md`,
`CLEAN-ROOM-STATUS.md`, `ASSET-PROVENANCE.md`, a docs index, and a manifest with
per-asset SHA-256 and SPDX rows is more than most projects this size have.

- [ ] `README.md` currently notes the published Flatpak beta predates the
      clean-room font and fixture conversion and tells users to build `master`
      instead. **[Verified]** `Directory.Build.props` says `0.7.2-beta.1` while
      the README points at the `v0.3.0-beta.1` release. Cut a fresh release; a
      README that says "don't use our release" is a strong signal to a
      first-time visitor.
- [ ] Add `.github/ISSUE_TEMPLATE/` (bug / feature) and a PR template with the
      manifest-regeneration and quality-gate checklist from `CONTRIBUTING.md`.
      The rules exist; put them where they get read.
- [ ] **[Verified]** 37 open issues and 1 open PR. Add labels and a milestone so
      the backlog communicates priority — right now an outside contributor has
      no way to find a good first task. Consider a `good first issue` sweep.
- [ ] Add `SECURITY.md` (even if it just says "open an issue") and a
      `CODE_OF_CONDUCT.md`.
- [ ] `ARCHITECTURE.md` would benefit from one diagram of the runtime data flow
      it describes in five numbered steps.
- [ ] Add an `AGENTS.md` or `CLAUDE.md` at the root capturing the invariants
      that already exist in prose — index contracts, disposal rules, no binary
      fixtures, manifest regeneration. This project is developed with AI tooling;
      those constraints should be in the file that tooling reads.

---

## 11. Cross-platform and packaging

- [ ] **[Check]** Path handling — search for hardcoded `/` or `\` and any
      `string` concatenation of paths instead of `Path.Combine`. Also check for
      case-sensitivity assumptions in asset filenames; `FOSSart/Advances/...`
      resolves differently on Linux than on Windows if any lookup differs in
      case.
- [ ] **[Check]** Is file matching using `StringComparison.Ordinal` explicitly
      where it matters? Culture-sensitive comparison on a Turkish locale is the
      classic sleeper bug for rules parsing (`"CITY".ToLower()`).
- [ ] Flatpak: confirm the manifest pins source revisions rather than tracking
      branches, so a build is reproducible.
- [ ] **[Check]** `quality_gate.sh` — does it `set -euo pipefail`? A gate script
      that continues past a failed step is worse than no gate.

---

## 12. A local audit that closes the gaps above

Run this in a clone to turn every **[Check]** into a fact. It takes a few
minutes and produces the numbers this review is missing.

```bash
# Size and shape
find . -name '*.cs' -not -path '*/obj/*' -not -path '*/bin/*' \
  | xargs wc -l | sort -n | tail -30        # 30 largest source files
dotnet sln list                              # every project in the solution
grep -rn "TargetFramework\|Nullable" --include=*.csproj .

# Warning baseline
dotnet build -warnaserror- 2>&1 | grep "warning " | \
  sed -E 's/.*(warning [A-Z]+[0-9]+).*/\1/' | sort | uniq -c | sort -rn

# Raylib resource lifetime
grep -rn "LoadTexture\|LoadImage\|LoadRenderTexture" --include=*.cs . | wc -l
grep -rn "UnloadTexture\|UnloadImage\|UnloadRenderTexture" --include=*.cs . | wc -l

# Smells
grep -rn "catch\s*{\s*}\|catch (Exception)" --include=*.cs .
grep -rn "TODO\|HACK\|FIXME\|XXX" --include=*.cs . | wc -l
grep -rn "Thread.Sleep\|\.Result\b\|\.Wait()" --include=*.cs .
grep -rn "static .*\bnew\b" --include=*.cs . | grep -i "cache\|manager\|instance"

# Coverage baseline
dotnet test --collect:"XPlat Code Coverage"
```

Anything over ~800 lines in that first list is worth a decomposition issue of
its own. If the largest files are in `RaylibUI` and mix input handling, layout
math, and drawing, splitting those three concerns is probably the single
highest-leverage refactor available.

---

## 13. Suggested ordering

**Now (cheap, unblocks everything else)**

1. Shared `Directory.Build.props` + `.editorconfig` + `global.json`.
2. Warning baseline recorded; `TreatWarningsAsErrors` landed with an exclusion
   list.
3. CI runs `quality_gate.sh`, matrixed over three OSes, Actions pinned.
4. Coverage baseline recorded.
5. Cut a current release so the README stops disowning the published build.

**Next (correctness)**

6. Save schema version + migration chain + round-trip tests.
7. Index-contract guard tests.
8. Raylib handle ownership types + leak HUD.
9. Loud failure for `IUserInterface` discovery and Lua script errors.

**Then (structure)**

10. Rename solution/projects; migrate the save directory; decide on detaching
    the fork.
11. Extract pure zoom/tile math into `RaylibUtils` and test it.
12. Decompose the largest `RaylibUI` files along input / layout / draw lines.
13. Crash logs and structured logging.

---

*This review was produced from public repository metadata and documentation
only. Every item marked **[Check]** needs local verification before it is
treated as a real defect.*
