# Maiba Hero Slider Widget Plugin

A custom hero slider widget plugin for the Maiba theme with full admin configuration capabilities.

## Features

- **Fully Customizable Slides**: Each slide can have:
  - Main image
  - Thumbnail image (for vertical navigation)
  - Heading text
  - Tag line (e.g., "GLAMOROUS LIFE")
  - Description text
  - Call-to-action button with custom text and URL
  - Display order
  - Active/Inactive status

- **Figma-Inspired Design**:
  - Vertical thumbnail navigation on the left
  - Large main image area
  - Text content overlay
  - Geometric shape decorations
  - Pagination dots
  - Auto-play with 5-second intervals
  - Pause on hover

- **Admin Interface**:
  - Manage multiple slides
  - Upload images via NopCommerce's picture manager
  - Drag-and-drop reordering (via display order)
  - Enable/disable individual slides

## Installation

1. **Add Plugin to Solution**:
   - The plugin is located in `/Themes/Maiba/Plugins/Nop.Plugin.Widgets.MaibaHeroSlider/`
   - Add the project to your NopCommerce solution:
     ```bash
     dotnet sln add src/Presentation/Nop.Web/Themes/Maiba/Plugins/Nop.Plugin.Widgets.MaibaHeroSlider/Nop.Plugin.Widgets.MaibaHeroSlider.csproj
     ```

2. **Build the Solution**:
   ```bash
   dotnet build
   ```
   Or use the provided script:
   ```bash
   .\scripts\run-nop.ps1
   ```

3. **Restart NopCommerce**:
   - The plugin will be automatically discovered on restart
   - Check logs if it doesn't appear

4. **Install the Plugin**:
   - Go to **Admin → Configuration → Local Plugins**
   - Find "Maiba Hero Slider"
   - Click **"Install"**
   - Wait for the installation to complete (database migration will run)
   - After installation, click **"Configure"**

5. **Activate the Widget**:
   - Go to **Admin → Configuration → Widgets**
   - Find "Maiba Hero Slider" in the list
   - Click **"Edit"**
   - Check **"Is active"**
   - Click **"Update"**

## Usage

### Adding Slides

1. Navigate to **Admin → Configuration → Widgets → Maiba Hero Slider → Configure**
2. Click "Add New Slide"
3. Fill in the slide details:
   - Upload main image (recommended: 1400px wide)
   - Upload thumbnail image (recommended: 150px x 150px)
   - Enter heading, tag, description
   - Set button text and URL
   - Set display order (lower numbers appear first)
   - Check "Is Active" to make the slide visible
4. Click "Save"

### Managing Slides

- Edit slides by clicking the "Edit" button
- Delete slides by clicking the "Delete" button
- Change slide order by adjusting the "Display Order" field
- Enable/disable slides using the "Is Active" checkbox

## Widget Zone

The slider automatically renders in the `PublicWidgetZones.HomepageTop` zone, replacing the default NopCommerce slider.

## Customization

### Styling

The slider uses the existing Maiba theme styles defined in:
- `src/styles/pages/_home.scss`

The CSS classes used are:
- `.p-home__hero` - Main hero section
- `.p-home__hero-sidebar` - Vertical thumbnail navigation
- `.p-home__hero-thumbs` - Thumbnail container
- `.p-home__hero-main` - Main content area
- `.hero-slide` - Individual slide wrapper
- `.hero-slide__image` - Slide image
- `.hero-slide__content` - Slide text content
- `.p-home__hero-pagination` - Pagination dots

### Autoplay Settings

To modify autoplay duration, edit the JavaScript in `Views/PublicInfo.cshtml`:
```javascript
autoplayInterval = setInterval(nextSlide, 5000); // 5000ms = 5 seconds
```

## Database Schema

The plugin creates a `HeroSlide` table with the following fields:
- `Id` (int, primary key)
- `DisplayOrder` (int)
- `PictureId` (int, foreign key to Picture table)
- `ThumbnailPictureId` (int, foreign key to Picture table)
- `Heading` (nvarchar)
- `Tag` (nvarchar)
- `Description` (nvarchar)
- `ButtonText` (nvarchar)
- `ButtonUrl` (nvarchar)
- `IsActive` (bit)

## Troubleshooting

### Slider not showing
1. Ensure the plugin is installed and activated
2. Check that at least one slide is marked as "Active"
3. Verify images are uploaded and valid
4. Clear cache: Admin → System → Maintenance → Clear cache

### Images not displaying
1. Ensure images are uploaded through NopCommerce's picture manager
2. Check file permissions on the `/images` folder
3. Verify PictureId and ThumbnailPictureId are valid

## Version History

- **1.0.0** - Initial release
  - Basic slider functionality
  - Admin configuration
  - Auto-play feature
  - Responsive design

## License

Copyright © Maiba Jewelry Shop

