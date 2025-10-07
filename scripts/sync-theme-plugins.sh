#!/bin/bash
# Sync Maiba Theme Plugins to Build Folder
# Run this after pulling the Maiba theme submodule or when you update plugin code

set -e

# Parse arguments
REVERSE=0
DRY_RUN=0

while [[ $# -gt 0 ]]; do
    case $1 in
        --reverse)
            REVERSE=1
            shift
            ;;
        --dry-run)
            DRY_RUN=1
            shift
            ;;
        *)
            echo "Unknown option: $1"
            echo "Usage: $0 [--reverse] [--dry-run]"
            exit 1
            ;;
    esac
done

# Paths
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
THEME_PLUGINS_PATH="$SCRIPT_DIR/../src/Presentation/Nop.Web/Themes/Maiba/Plugins"
BUILD_PLUGINS_PATH="$SCRIPT_DIR/../src/Plugins"

# Colors
INFO='\033[0;36m'
SUCCESS='\033[0;32m'
WARNING='\033[0;33m'
ERROR='\033[0;31m'
GRAY='\033[0;90m'
NC='\033[0m' # No Color

function write_info() {
    echo -e "${INFO}ℹ️  $1${NC}"
}

function write_success() {
    echo -e "${SUCCESS}✅ $1${NC}"
}

function write_warning() {
    echo -e "${WARNING}⚠️  $1${NC}"
}

function write_error() {
    echo -e "${ERROR}❌ $1${NC}"
}

# Check if directories exist
if [ ! -d "$THEME_PLUGINS_PATH" ]; then
    write_error "Theme plugins folder not found: $THEME_PLUGINS_PATH"
    exit 1
fi

if [ ! -d "$BUILD_PLUGINS_PATH" ]; then
    write_error "Build plugins folder not found: $BUILD_PLUGINS_PATH"
    exit 1
fi

# Find all plugin directories
PLUGIN_COUNT=0
for plugin_dir in "$THEME_PLUGINS_PATH"/Nop.Plugin.*; do
    if [ -d "$plugin_dir" ]; then
        ((PLUGIN_COUNT++))
    fi
done

if [ $PLUGIN_COUNT -eq 0 ]; then
    write_warning "No plugins found in theme folder"
    exit 0
fi

write_info "Found $PLUGIN_COUNT plugin(s) in Maiba theme"
echo ""

# Sync each plugin
for plugin_dir in "$THEME_PLUGINS_PATH"/Nop.Plugin.*; do
    if [ ! -d "$plugin_dir" ]; then
        continue
    fi
    
    PLUGIN_NAME=$(basename "$plugin_dir")
    SOURCE_PATH="$plugin_dir"
    TARGET_PATH="$BUILD_PLUGINS_PATH/$PLUGIN_NAME"
    
    if [ $REVERSE -eq 1 ]; then
        # Reverse sync: Build → Theme
        SOURCE_PATH="$TARGET_PATH"
        TARGET_PATH="$plugin_dir"
        DIRECTION="Build → Theme"
    else
        # Normal sync: Theme → Build
        DIRECTION="Theme → Build"
    fi
    
    write_info "Processing: $PLUGIN_NAME ($DIRECTION)"
    
    if [ $DRY_RUN -eq 1 ]; then
        echo -e "${WARNING}  [DRY RUN] Would copy from:${NC}"
        echo -e "${GRAY}    $SOURCE_PATH${NC}"
        echo -e "${WARNING}  [DRY RUN] To:${NC}"
        echo -e "${GRAY}    $TARGET_PATH${NC}"
    else
        # Create target directory if it doesn't exist
        mkdir -p "$TARGET_PATH"
        
        # Copy all files and folders
        rsync -a --delete "$SOURCE_PATH/" "$TARGET_PATH/"
        
        write_success "  Synced: $PLUGIN_NAME"
    fi
    
    echo ""
done

if [ $DRY_RUN -eq 1 ]; then
    write_warning "DRY RUN completed. No files were actually copied."
    echo "Run without --dry-run to perform the actual sync."
else
    write_success "Plugin sync completed!"
    
    if [ $REVERSE -eq 0 ]; then
        echo ""
        write_info "Next steps:"
        echo -e "${GRAY}  1. Build the solution: dotnet build${NC}"
        echo -e "${GRAY}  2. Run NopCommerce: ./scripts/run-nop.sh${NC}"
    fi
fi

