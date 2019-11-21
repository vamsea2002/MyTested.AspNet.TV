namespace Blog.Services
{
    using Common;
    using Models.Articles;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IArticleService : IService
    {
        public Task<IEnumerable<ArticleListingServiceModel>> All(int page);

        public Task<ArticleDetailsServiceModel> Details(int id);

        Task<int> Create(string title, string description, string authorId);

        Task<bool> Edit(int id, string title, string description);

        Task<bool> Exists(int id, string authorId);

        Task<bool> Delete(int id);
    }
}
