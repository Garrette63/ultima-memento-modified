#!/usr/bin/env bash
#
# Compile-Linux.sh  --  Linux "shortcut" for the Memento shard.
#
# Running this file (double-click / execute / `bash Compile-Linux.sh`) will:
#   1. Run `git pull` from the repository base folder (the parent of this World folder).
#   2. Run the Linux compiler in Source/Tools (compile-world-linux.sh, which uses Mono's `mcs`).
#
# Both compilers use paths relative to World/Source/Tools, so we cd there before
# invoking them. If `git pull` fails we stop before compiling so we never build
# stale/conflicting code.
set -euo pipefail

# Folder that contains this shortcut == the World folder.
WORLD_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" >/dev/null 2>&1 && pwd)"
# Repository base folder == parent of the World folder.
BASE_DIR="$(cd -- "${WORLD_DIR}/.." >/dev/null 2>&1 && pwd)"
# Folder that holds the actual compiler.
TOOLS_DIR="${WORLD_DIR}/Source/Tools"

echo "==> git pull  (base folder: ${BASE_DIR})"
if ! git -C "${BASE_DIR}" pull; then
    echo "!! git pull failed - aborting so we don't compile stale code." >&2
    exit 1
fi

echo "==> Compiling with the Linux compiler  (${TOOLS_DIR}/compile-world-linux.sh)"
cd "${TOOLS_DIR}"
bash ./compile-world-linux.sh

echo "==> Build finished."
