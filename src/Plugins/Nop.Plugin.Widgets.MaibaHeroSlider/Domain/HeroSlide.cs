using Nop.Core;

namespace Nop.Plugin.Widgets.MaibaHeroSlider.Domain;

/// <summary>
/// Represents a hero slider slide
/// </summary>
public partial class HeroSlide : BaseEntity
{
    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the picture ID
    /// </summary>
    public int PictureId { get; set; }

    /// <summary>
    /// Gets or sets the heading
    /// </summary>
    public string Heading { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tag (e.g., "GLAMOROUS LIFE")
    /// </summary>
    public string Tag { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the button text
    /// </summary>
    public string ButtonText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the button URL
    /// </summary>
    public string ButtonUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the slide is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the thumbnail picture ID (for vertical navigation)
    /// </summary>
    public int ThumbnailPictureId { get; set; }
}

