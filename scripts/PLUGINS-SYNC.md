# Plugin Sync Scripts

## Overview

The Maiba theme stores custom plugins in its own `Plugins` folder for version control. However, NopCommerce needs these plugins in the main `src/Plugins/` folder to build them.

These scripts automatically sync plugins between the two locations.

## 📁 Directory Structure

```
maiba-jewelry-shop/
├── src/
│   ├── Plugins/                          # Build location (compile from here)
│   │   ├── Nop.Plugin.Widgets.MaibaHeroSlider/
│   │   └── Nop.Plugin.Security.AdminAccess/
│   └── Presentation/
│       └── Nop.Web/
│           └── Themes/
│               └── Maiba/
│                   └── Plugins/          # Version control (Git)
│                       ├── Nop.Plugin.Widgets.MaibaHeroSlider/
│                       └── Nop.Plugin.Security.AdminAccess/
└── scripts/
    ├── sync-theme-plugins.ps1           # PowerShell sync script
    └── sync-theme-plugins.sh            # Bash sync script
```

## 🔄 Workflows

### After Pulling Theme Submodule

When you pull the Maiba theme (or update it), sync plugins to the build folder:

**PowerShell (Windows):**
```powershell
.\scripts\sync-theme-plugins.ps1
```

**Bash (Linux/Mac):**
```bash
./scripts/sync-theme-plugins.sh
```

### After Editing Plugin Code in Build Folder

If you edited plugins in `src/Plugins/` and want to save to the theme:

**PowerShell:**
```powershell
.\scripts\sync-theme-plugins.ps1 -Reverse
```

**Bash:**
```bash
./scripts/sync-theme-plugins.sh --reverse
```

### Preview Without Changing Files

To see what would be synced without actually copying:

**PowerShell:**
```powershell
.\scripts\sync-theme-plugins.ps1 -DryRun
```

**Bash:**
```bash
./scripts/sync-theme-plugins.sh --dry-run
```

## 📝 Script Options

### PowerShell (`sync-theme-plugins.ps1`)

| Option | Description |
|--------|-------------|
| `-Reverse` | Sync from Build → Theme (save your edits) |
| `-DryRun` | Preview changes without copying files |

### Bash (`sync-theme-plugins.sh`)

| Option | Description |
|--------|-------------|
| `--reverse` | Sync from Build → Theme (save your edits) |
| `--dry-run` | Preview changes without copying files |

## 🎯 Common Scenarios

### Scenario 1: Fresh Clone / Submodule Update

```powershell
# 1. Pull the theme submodule
git submodule update --init --recursive

# 2. Sync plugins to build folder
.\scripts\sync-theme-plugins.ps1

# 3. Build the solution
cd src
dotnet build

# 4. Run NopCommerce
cd ..
.\scripts\run-nop.ps1
```

### Scenario 2: Developing a Plugin

```powershell
# Option A: Edit in Theme folder (recommended for version control)
# 1. Edit files in: Presentation/Nop.Web/Themes/Maiba/Plugins/YourPlugin/
# 2. Sync to build folder
.\scripts\sync-theme-plugins.ps1
# 3. Build
cd src && dotnet build

# Option B: Edit in Build folder (quick iteration)
# 1. Edit files in: src/Plugins/YourPlugin/
# 2. Build immediately
cd src && dotnet build
# 3. When done, sync back to theme for Git
cd ..
.\scripts\sync-theme-plugins.ps1 -Reverse
```

### Scenario 3: Adding a New Plugin

```powershell
# 1. Create plugin in Theme folder
# Create: Presentation/Nop.Web/Themes/Maiba/Plugins/Nop.Plugin.YourName/

# 2. Sync to build folder
.\scripts\sync-theme-plugins.ps1

# 3. Add to solution
cd src
dotnet sln add Plugins/Nop.Plugin.YourName/Nop.Plugin.YourName.csproj

# 4. Build
dotnet build
```

## ⚠️ Important Notes

1. **Always commit from Theme folder**: The `Maiba/Plugins/` folder is the source of truth for Git
2. **Build folder is temporary**: The `src/Plugins/` folder can be regenerated anytime
3. **Don't edit both locations**: Choose one location to edit, then sync
4. **Run sync after submodule updates**: Always sync after pulling theme changes

## 🔍 Troubleshooting

### Plugins Not Building

```powershell
# 1. Ensure plugins are synced
.\scripts\sync-theme-plugins.ps1 -DryRun  # Preview
.\scripts\sync-theme-plugins.ps1          # Sync

# 2. Ensure plugins are in solution
cd src
dotnet sln list  # Check if your plugin is listed

# 3. Rebuild
dotnet clean
dotnet build
```

### Lost Changes

If you edited in the build folder and forgot to sync back:

```powershell
# Recover your changes to the theme
.\scripts\sync-theme-plugins.ps1 -Reverse
git status  # Verify changes in theme folder
```

## 📚 Related Scripts

- `run-nop.ps1` / `run-nop.sh` - Build and run NopCommerce
- `db.ps1` / `db.sh` - Manage database container
- `bootstrap-dev.ps1` / `bootstrap-dev.sh` - Initial setup

## 🤝 Contributing

When contributing theme plugins:

1. Create/edit plugins in `Maiba/Plugins/`
2. Sync to build folder with `sync-theme-plugins.ps1`
3. Build and test
4. Commit only the `Maiba/Plugins/` folder
5. `.gitignore` should ignore `src/Plugins/Nop.Plugin.*` (build artifacts)

