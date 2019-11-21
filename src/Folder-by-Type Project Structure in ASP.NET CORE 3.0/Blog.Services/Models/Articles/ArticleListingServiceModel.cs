namespace Blog.Services.Models.Articles
{
    using AutoMapper;
    using Blog.Common.Mapping;
    using Data.Models;

    public class ArticleListingServiceModel : IMapFrom<Article>, IMapExplicitly
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public void RegisterMappings(IProfileExpression profile)
        {
            profile
                .CreateMap<Article, ArticleListingServiceModel>()
                .ForMember(m => m.Author, cfg => cfg.MapFrom(a => a.Author.UserName));
        }
    }
}
