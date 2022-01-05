#!/usr/bin/env bash
#docfx/docfx.exe docs/docfx.json --serve
cd docs/_site
python3 -m http.server 8000
