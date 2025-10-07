using Nop.Data;
using Nop.Plugin.Widgets.MaibaHeroSlider.Domain;

namespace Nop.Plugin.Widgets.MaibaHeroSlider.Services;

/// <summary>
/// Hero slide service
/// </summary>
public partial class HeroSlideService : IHeroSlideService
{
    #region Fields

    private readonly IRepository<HeroSlide> _heroSlideRepository;

    #endregion

    #region Ctor

    public HeroSlideService(IRepository<HeroSlide> heroSlideRepository)
    {
        _heroSlideRepository = heroSlideRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets all hero slides
    /// </summary>
    /// <param name="activeOnly">A value indicating whether to load only active slides</param>
    /// <returns>Hero slides</returns>
    public virtual async Task<IList<HeroSlide>> GetAllSlidesAsync(bool activeOnly = false)
    {
        var query = _heroSlideRepository.Table;

        if (activeOnly)
            query = query.Where(s => s.IsActive);

        query = query.OrderBy(s => s.DisplayOrder).ThenBy(s => s.Id);

        return await query.ToListAsync();
    }

    /// <summary>
    /// Gets a hero slide by identifier
    /// </summary>
    /// <param name="slideId">Hero slide identifier</param>
    /// <returns>Hero slide</returns>
    public virtual async Task<HeroSlide?> GetSlideByIdAsync(int slideId)
    {
        return await _heroSlideRepository.GetByIdAsync(slideId, cache => default);
    }

    /// <summary>
    /// Inserts a hero slide
    /// </summary>
    /// <param name="slide">Hero slide</param>
    public virtual async Task InsertSlideAsync(HeroSlide slide)
    {
        await _heroSlideRepository.InsertAsync(slide);
    }

    /// <summary>
    /// Updates the hero slide
    /// </summary>
    /// <param name="slide">Hero slide</param>
    public virtual async Task UpdateSlideAsync(HeroSlide slide)
    {
        await _heroSlideRepository.UpdateAsync(slide);
    }

    /// <summary>
    /// Deletes a hero slide
    /// </summary>
    /// <param name="slide">Hero slide</param>
    public virtual async Task DeleteSlideAsync(HeroSlide slide)
    {
        await _heroSlideRepository.DeleteAsync(slide);
    }

    #endregion
}

