#!/usr/bin/env python3
"""Build teacher PDFs in uploads/ from uploads/_pdf_source/*.md (strips YAML front matter). Requires fpdf2."""
from __future__ import annotations

import re
import sys
import unicodedata
from pathlib import Path


def strip_front_matter(md: str) -> str:
    md = md.lstrip("\ufeff")
    if not md.startswith("---"):
        return md
    parts = md.split("---", 2)
    return parts[2].lstrip("\n") if len(parts) >= 3 else md


def md_to_html(body: str) -> str:
    lines = body.splitlines()
    parts: list[str] = [
        "<meta charset='utf-8'/>",
        "<style>"
        "body{font-family:Helvetica,Arial,sans-serif;font-size:11pt;line-height:1.4;margin:24px;color:#222;}"
        "pre{font-size:9pt;background:#f5f5f5;padding:8px;border:1px solid #ddd;white-space:pre-wrap;}"
        "table{border-collapse:collapse;width:100%;margin:10px 0;}"
        "td,th{border:1px solid #ccc;padding:6px;text-align:left;vertical-align:top;}"
        "th{background:#eef;}"
        "hr{border:0;border-top:1px solid #ccc;margin:14px 0;}"
        "h1{font-size:18pt;color:#334;} h2{font-size:14pt;margin-top:16px;color:#446;}"
        "h3{font-size:12pt;margin-top:12px;}"
        "p{margin:8px 0;} code{font-size:9pt;background:#f0f0f0;padding:1px 3px;}"
        "</style>",
        "<body>",
    ]
    i = 0
    n = len(lines)
    while i < n:
        line = lines[i]
        stripped = line.strip()
        if stripped.startswith("```"):
            lang = stripped[3:].strip()
            i += 1
            chunks: list[str] = []
            while i < n and not lines[i].strip().startswith("```"):
                chunks.append(lines[i])
                i += 1
            if i < n:
                i += 1
            escaped = (
                "\n".join(chunks)
                .replace("&", "&amp;")
                .replace("<", "&lt;")
                .replace(">", "&gt;")
            )
            parts.append(f"<pre>{escaped}</pre>")
            continue

        if "|" in line and stripped.startswith("|") and stripped.count("|") >= 2:
            rows_html: list[str] = ["<table>"]
            header_row_pending = True
            while i < n and "|" in lines[i] and lines[i].strip().startswith("|"):
                row_text = lines[i].strip()
                if re.match(r"^\|\s*:?-+", row_text):  # | --- | :--- | separator
                    header_row_pending = False
                    i += 1
                    continue
                cells = [c.strip() for c in row_text.strip("|").split("|")]
                tag = "th" if header_row_pending else "td"
                # Avoid inline markdown in tables (nested <b> etc. breaks fpdf HTML)
                rows_html.append(
                    "<tr>"
                    + "".join(f"<{tag}>{_esc(c)}</{tag}>" for c in cells)
                    + "</tr>"
                )
                if header_row_pending:
                    header_row_pending = False
                i += 1
            rows_html.append("</table>")
            parts.extend(rows_html)
            continue

        if stripped == "---":
            parts.append("<hr/>")
            i += 1
            continue
        if stripped.startswith("####"):
            parts.append(f"<h4>{_inline(_esc(stripped[4:].strip()))}</h4>")
        elif stripped.startswith("###"):
            parts.append(f"<h3>{_inline(_esc(stripped[4:].strip()))}</h3>")
        elif stripped.startswith("##"):
            parts.append(f"<h2>{_inline(_esc(stripped[3:].strip()))}</h2>")
        elif stripped.startswith("#"):
            parts.append(f"<h1>{_inline(_esc(stripped[1:].strip()))}</h1>")
        elif stripped == "":
            parts.append("<br/>")
        else:
            parts.append(f"<p>{_inline(_esc(line))}</p>")
        i += 1
    parts.append("</body>")
    return "\n".join(parts)


def _esc(t: str) -> str:
    return t.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;")


def _inline(t: str) -> str:
    t = re.sub(r"\*\*(.+?)\*\*", r"<b>\1</b>", t)
    t = re.sub(r"`([^`]+)`", r"<code>\1</code>", t)
    return t


def _ascii_for_core_fonts(text: str) -> str:
    """fpdf core fonts are latin-1; fold to ASCII-ish for reliable PDF output."""
    text = (
        text.replace("\u2014", "--")
        .replace("\u2013", "-")
        .replace("\u2192", "->")
        .replace("\u2190", "<-")
        .replace("\u2022", "*")
        .replace("\u2605", "*")
        .replace("\u2020", "+")
        .replace("\u2019", "'")
        .replace("\u201c", '"')
        .replace("\u201d", '"')
    )
    text = unicodedata.normalize("NFKD", text)
    return "".join(ch for ch in text if ord(ch) < 128)


def md_file_to_pdf(src: Path, dest: Path) -> None:
    from fpdf import FPDF

    raw = _ascii_for_core_fonts(src.read_text(encoding="utf-8"))
    html = md_to_html(strip_front_matter(raw))
    pdf = FPDF()
    pdf.add_page(format="Letter")
    pdf.write_html(html)
    dest.parent.mkdir(parents=True, exist_ok=True)
    pdf.output(dest.as_posix())


def main() -> int:
    root = Path(__file__).resolve().parent.parent
    src_dir = root / "uploads" / "_pdf_source"
    if not src_dir.is_dir():
        print("Missing", src_dir, file=sys.stderr)
        return 1
    for md_path in sorted(src_dir.glob("*.md")):
        dest = root / "uploads" / f"{md_path.stem}.pdf"
        md_file_to_pdf(md_path, dest)
        print(f"Wrote {dest.relative_to(root)}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
