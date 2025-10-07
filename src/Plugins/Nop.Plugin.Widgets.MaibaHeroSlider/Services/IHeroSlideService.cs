using Nop.Plugin.Widgets.MaibaHeroSlider.Domain;

namespace Nop.Plugin.Widgets.MaibaHeroSlider.Services;

/// <summary>
/// Hero slide service interface
/// </summary>
public partial interface IHeroSlideService
{
    /// <summary>
    /// Gets all hero slides
    /// </summary>
    /// <param name="activeOnly">A value indicating whether to load only active slides</param>
    /// <returns>Hero slides</returns>
    Task<IList<HeroSlide>> GetAllSlidesAsync(bool activeOnly = false);

    /// <summary>
    /// Gets a hero slide by identifier
    /// </summary>
    /// <param name="slideId">Hero slide identifier</param>
    /// <returns>Hero slide</returns>
    Task<HeroSlide?> GetSlideByIdAsync(int slideId);

    /// <summary>
    /// Inserts a hero slide
    /// </summary>
    /// <param name="slide">Hero slide</param>
    Task InsertSlideAsync(HeroSlide slide);

    /// <summary>
    /// Updates the hero slide
    /// </summary>
    /// <param name="slide">Hero slide</param>
    Task UpdateSlideAsync(HeroSlide slide);

    /// <summary>
    /// Deletes a hero slide
    /// </summary>
    /// <param name="slide">Hero slide</param>
    Task DeleteSlideAsync(HeroSlide slide);
}

