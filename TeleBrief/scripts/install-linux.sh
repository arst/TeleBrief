#!/usr/bin/env bash
# Install the 'brief' command on Linux
set -euo pipefail
SCRIPT_DIR="$(cd "$(dirname "$0")/.." && pwd)"
WRAPPER_PATH="/usr/local/bin/brief"

echo "#!/usr/bin/env bash" | sudo tee "$WRAPPER_PATH" > /dev/null
# pass all arguments to dotnet run
printf 'dotnet run --project "%s" -- "$@"\n' "$SCRIPT_DIR" | sudo tee -a "$WRAPPER_PATH" > /dev/null
sudo chmod +x "$WRAPPER_PATH"

echo "Installed 'brief' command. Run 'brief news' or 'brief beat'."
