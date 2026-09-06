#!/usr/bin/env python3
"""Export every issue and pull request, with comments, into the repository.

Why this exists: leaving GitHub's fork network permanently deletes a
repository's issues, pull requests, wikis, stars, watchers and comments. Git
history survives; none of that does. rhYciv is still a fork of axx0/Civ2-clone,
so if it is ever detached, this file is the only thing that keeps the backlog.

It is also useful without detaching -- it puts the backlog under version control,
so it is diffable, greppable offline, and not solely GitHub's copy.

What this cannot preserve is **issue numbers**. Re-imported issues are
renumbered from 1, so references like "#67" in commit messages, docs and release
notes would then point somewhere else. The archive records the original number
for every entry so a mapping can be rebuilt by hand, but nothing can make the old
references resolve again.

Attachments *are* copied. Bodies and comments referencing
`github.com/user-attachments/...` point at files GitHub serves on the repository's
behalf, and those links die with the metadata. Each one is downloaded into
`attachments/` beside the archive so the screenshots and documents survive.

Usage:
    python3 scripts/export_issues.py [--repo crhy/rhYciv]

Requires the `gh` CLI, authenticated.
"""

from __future__ import annotations

import argparse
import json
import re
import shutil
import subprocess
import sys
from datetime import datetime, timezone
from pathlib import Path

REPOSITORY = Path(__file__).resolve().parents[1]
ARCHIVE = REPOSITORY / "docs" / "issues-archive"


def gh_api(path: str) -> list[dict]:
    """Fetch a paginated GitHub API collection through the gh CLI."""
    result = subprocess.run(
        ["gh", "api", "--paginate", path],
        capture_output=True, text=True,
    )
    if result.returncode != 0:
        raise SystemExit(f"gh api {path} failed:\n{result.stderr.strip()}")

    # --paginate concatenates one JSON array per page; gh emits them back to back.
    decoder, items, index = json.JSONDecoder(), [], 0
    text = result.stdout.strip()
    while index < len(text):
        page, offset = decoder.raw_decode(text, index)
        items.extend(page)
        index = offset
        while index < len(text) and text[index] in " \t\r\n":
            index += 1
    return items


def slim_user(user: dict | None) -> str | None:
    return user.get("login") if user else None


# Markdown image syntax and raw HTML both appear in issue bodies, so stop at
# whitespace, a closing paren or bracket, or the quote that ends an HTML attribute.
ATTACHMENT = re.compile(r'https://github\.com/user-attachments/[^\s)\]"\'<>]+')


def download_attachments(entries: list[dict], target: Path) -> int:
    """Copy every GitHub-hosted attachment into the archive.

    These are served by GitHub for the repository and disappear with its
    metadata, so an archive that only records their URLs preserves dead links.
    """
    urls: dict[str, int] = {}
    for entry in entries:
        for text in [entry["body"]] + [c["body"] for c in entry["comments"]]:
            for url in ATTACHMENT.findall(text or ""):
                urls.setdefault(url, entry["number"])

    if not urls:
        return 0

    target.mkdir(parents=True, exist_ok=True)
    saved = 0
    for url, number in sorted(urls.items(), key=lambda pair: pair[1]):
        stem = url.rsplit("/", 1)[-1]
        # Asset URLs end in a bare UUID; give those a suffix once we know the type.
        destination = target / f"{number:03d}-{stem}"
        result = subprocess.run(
            ["curl", "-fsSL", "--max-time", "60", "-o", str(destination), url],
            capture_output=True, text=True)
        if result.returncode != 0:
            print(f"  WARNING: could not fetch attachment for #{number}: {url}")
            destination.unlink(missing_ok=True)
            continue
        if destination.suffix == "":
            head = destination.read_bytes()[:8]
            suffix = (".png" if head.startswith(b"\x89PNG") else
                      ".jpg" if head.startswith(b"\xff\xd8") else
                      ".gif" if head.startswith(b"GIF8") else ".bin")
            destination = destination.rename(destination.with_suffix(suffix))
        saved += 1
        print(f"  #{number:<4} attachment -> {destination.name}")
    return saved


def export(repo: str) -> dict:
    print(f"Fetching issues and pull requests from {repo}...")
    raw = gh_api(f"repos/{repo}/issues?state=all&per_page=100")

    entries = []
    for item in sorted(raw, key=lambda i: i["number"]):
        is_pr = "pull_request" in item
        comments = []
        if item["comments"]:
            comments = [
                {
                    "author": slim_user(comment.get("user")),
                    "created_at": comment["created_at"],
                    "body": comment.get("body") or "",
                }
                for comment in gh_api(
                    f"repos/{repo}/issues/{item['number']}/comments?per_page=100")
            ]

        entries.append({
            "number": item["number"],
            "type": "pull_request" if is_pr else "issue",
            "title": item["title"],
            "state": item["state"],
            "author": slim_user(item.get("user")),
            "created_at": item["created_at"],
            "closed_at": item.get("closed_at"),
            "labels": [label["name"] for label in item.get("labels", [])],
            "milestone": (item.get("milestone") or {}).get("title"),
            "url": item["html_url"],
            "body": item.get("body") or "",
            "comments": comments,
        })
        print(f"  #{item['number']:<4} {item['state']:<6} {item['title'][:64]}")

    return {
        "repository": repo,
        "exported_at": datetime.now(timezone.utc).isoformat(timespec="seconds"),
        "counts": {
            "total": len(entries),
            "issues": sum(1 for e in entries if e["type"] == "issue"),
            "pull_requests": sum(1 for e in entries if e["type"] == "pull_request"),
            "open": sum(1 for e in entries if e["state"] == "open"),
            "closed": sum(1 for e in entries if e["state"] == "closed"),
        },
        "entries": entries,
    }


def write_markdown(archive: dict, path: Path) -> None:
    """A readable companion to the JSON, so the backlog can be browsed in the repo."""
    counts = archive["counts"]
    lines = [
        "# Issue archive",
        "",
        f"Exported from `{archive['repository']}` on {archive['exported_at'][:10]}.",
        "",
        f"{counts['total']} entries: {counts['issues']} issues and "
        f"{counts['pull_requests']} pull requests, {counts['open']} open and "
        f"{counts['closed']} closed.",
        "",
        f"{archive.get('attachments_saved', 0)} screenshots and documents attached to "
        "these issues are copied into `attachments/`, named by issue number. GitHub "
        "serves those files on the repository's behalf and they die with its metadata, "
        "so the URLs in the issue text are not enough on their own.",
        "",
        "`issues.json` beside this file holds the full text and every comment.",
        "This exists because leaving GitHub's fork network deletes all of it;",
        "see the header of `scripts/export_issues.py`.",
        "",
        "Issue numbers cannot survive a re-import. The originals are recorded here",
        "so a mapping can be rebuilt, but references like `#67` in commit messages",
        "and release notes would no longer resolve.",
        "",
        "## Open",
        "",
    ]
    for state, heading in (("open", "## Open"), ("closed", "## Closed")):
        if heading != "## Open":
            lines += ["", heading, ""]
        for entry in archive["entries"]:
            if entry["state"] != state:
                continue
            labels = f" — `{'`, `'.join(entry['labels'])}`" if entry["labels"] else ""
            kind = "PR" if entry["type"] == "pull_request" else "issue"
            lines.append(f"- **#{entry['number']}** ({kind}) {entry['title']}{labels}")
    path.write_text("\n".join(lines) + "\n", encoding="utf-8")


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--repo", default="crhy/rhYciv")
    args = parser.parse_args()

    if shutil.which("gh") is None:
        print("ERROR: the gh CLI is required and was not found on PATH.", file=sys.stderr)
        return 1

    archive = export(args.repo)
    ARCHIVE.mkdir(parents=True, exist_ok=True)

    print("\nDownloading attachments...")
    archive["attachments_saved"] = download_attachments(
        archive["entries"], ARCHIVE / "attachments")

    (ARCHIVE / "issues.json").write_text(
        json.dumps(archive, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    write_markdown(archive, ARCHIVE / "README.md")

    counts = archive["counts"]
    print(f"\nArchived {counts['total']} entries "
          f"({counts['issues']} issues, {counts['pull_requests']} pull requests) "
          f"to {ARCHIVE.relative_to(REPOSITORY)}/")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
