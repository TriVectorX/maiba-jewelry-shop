﻿using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.News.Public.Models;

/// <summary>
/// Represents the news item model
/// </summary>
public record NewsItemModel : BaseNopEntityModel
{
    #region Properties

    public string MetaKeywords { get; set; }
    public string MetaDescription { get; set; }
    public string MetaTitle { get; set; }
    public string SeName { get; set; }
    public string Title { get; set; }
    public string Short { get; set; }
    public string Full { get; set; }
    public bool AllowComments { get; set; }
    public bool PreventNotRegisteredUsersToLeaveComments { get; set; }
    public DateTime CreatedOn { get; set; }
    public List<NewsCommentModel> Comments { get; set; } = [];
    public AddNewsCommentModel AddNewComment { get; set; } = new();

    #endregion
}