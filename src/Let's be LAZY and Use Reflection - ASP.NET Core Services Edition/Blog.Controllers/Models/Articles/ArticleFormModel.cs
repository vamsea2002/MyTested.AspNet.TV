namespace Blog.Controllers.Models.Articles
{
    using System.ComponentModel.DataAnnotations;

    using static Data.DataValidation.Article;

    public class ArticleFormModel
    {
        [Required]
        [MaxLength(MaxTitleLength)]
        public string Title { get; set; }

        [MaxLength(MaxDescriptionLength)]
        public string Description { get; set; }
    }
}
