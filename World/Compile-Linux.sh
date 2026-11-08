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

# --- Environment setup for GUI / double-click launches ----------------------
# When Dolphin executes this script (double-click) it does NOT read ~/.bashrc,
# so PATH can be minimal and miss Homebrew/Linuxbrew (where mcs / mono / csc
# live). Add the usual locations when they exist and are not already present.
_add_path() {
    [ -d "$1" ] || return 0
    case ":${PATH}:" in
        *":$1:"*) ;;
        *) PATH="$1:${PATH}" ;;
    esac
}
if [ -n "${HOMEBREW_PREFIX:-}" ]; then _add_path "${HOMEBREW_PREFIX}/bin"; fi
_add_path "${HOME}/linuxbrew/.linuxbrew/bin"
_add_path "/home/linuxbrew/.linuxbrew/bin"
_add_path "/home/linuxbrew/.linuxbrew/sbin"
_add_path "/usr/local/bin"
_add_path "/usr/bin"
_add_path "/bin"
export PATH

# --- Re-open in Konsole when launched without a terminal --------------------
# A file-manager "execute" launch has no TTY, so build output would be invisible
# and no window would exist. In that case re-run ourselves inside Konsole so the
# output is visible; the window closes automatically when the build finishes.
# Console runs already have a TTY and skip this.
if [ ! -t 1 ] && [ -z "${MEMENTO_IN_TERMINAL:-}" ] \
   && { [ -n "${WAYLAND_DISPLAY:-}" ] || [ -n "${DISPLAY:-}" ]; } \
   && command -v konsole >/dev/null 2>&1; then
    _self="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" >/dev/null 2>&1 && pwd)/$(basename -- "${BASH_SOURCE[0]}")"
    MEMENTO_IN_TERMINAL=1 exec konsole -e bash "$_self" "$@"
fi

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
