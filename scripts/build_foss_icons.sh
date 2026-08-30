#!/usr/bin/env bash

set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
art_root="${repo_root}/RaylibUI/FOSSart"
icon_root="${art_root}/Icons"
source_override_root="${art_root}/IconSources"

if ! command -v ffmpeg >/dev/null 2>&1; then
    echo "Error: ffmpeg is required to rebuild the compact FOSS icons." >&2
    exit 1
fi

categories=(Advances Improvements Wonders)

for category in "${categories[@]}"; do
    source_dir="${art_root}/${category}"
    output_dir="${icon_root}/${category}"
    mkdir -p "$output_dir"

    while IFS= read -r -d '' source_path; do
        filename="$(basename "$source_path")"
        stem="${filename%.*}"
        override_path="${source_override_root}/${category}/${stem}.png"
        output_path="${output_dir}/${stem}.png"

        if [[ -f "$override_path" ]]; then
            ffmpeg -hide_banner -loglevel error -y \
                -i "$override_path" \
                -vf "scale=36:20:force_original_aspect_ratio=increase:flags=area,crop=36:20,unsharp=5:5:0.55:5:5:0.0" \
                "$output_path"
        else
            ffmpeg -hide_banner -loglevel error -y \
                -i "$source_path" \
                -vf "scale=72:40:force_original_aspect_ratio=increase:flags=lanczos,crop=72:40,eq=contrast=1.12:brightness=-0.025:saturation=1.28,scale=36:20:flags=area,unsharp=5:5:0.7:5:5:0.0,drawbox=x=0:y=0:w=iw:h=ih:color=0x2b200e:t=1" \
                "$output_path"
        fi
    done < <(find "$source_dir" -maxdepth 1 -type f -iname '*.jpg' -print0 | sort -z)
done

source_count="$(find "${art_root}/Advances" "${art_root}/Improvements" "${art_root}/Wonders" -maxdepth 1 -type f -iname '*.jpg' | wc -l)"
icon_count="$(find "$icon_root" -mindepth 2 -maxdepth 2 -type f -iname '*.png' | wc -l)"

if [[ "$source_count" -ne "$icon_count" ]]; then
    echo "Error: generated ${icon_count} icons from ${source_count} source paintings." >&2
    exit 1
fi

echo "Generated ${icon_count} compact 36x20 FOSS icons."
