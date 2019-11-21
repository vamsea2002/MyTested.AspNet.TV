namespace Blog.Services.Implementations
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Models.Articles;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ArticleService : IArticleService
    {
        private const int ArticlePageSize = 10;

        private readonly BlogDbContext data;
        private readonly IMapper mapper;

        public ArticleService(BlogDbContext data, IMapper mapper)
        {
            this.data = data;
            this.mapper = mapper;
        }

        public async Task<int> Create(string title, string description, string authorId)
        {
            var article = new Article
            {
                Title = title,
                Description = description,
                AuthorId = authorId
            };

            this.data.Add(article);

            await this.data.SaveChangesAsync();

            return article.Id;
        }

        public async Task<IEnumerable<ArticleListingServiceModel>> All(int page)
            => await this.data
                .Articles
                .Skip((page - 1) * ArticlePageSize)
                .Take(ArticlePageSize)
                .ProjectTo<ArticleListingServiceModel>(this.mapper.ConfigurationProvider)
                .ToListAsync();

        public async Task<bool> Edit(int id, string title, string description)
        {
            var article = await this.data.Articles.FindAsync(id);

            if (article == null)
            {
                return false;
            }

            article.Title = title;
            article.Description = description;

            await this.data.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Exists(int id, string authorId)
            => await this.data.Articles
                .AnyAsync(a => a.Id == id && a.AuthorId == authorId);

        public async Task<ArticleDetailsServiceModel> Details(int id)
            => await this.data.Articles
                .Where(a => a.Id == id)
                .ProjectTo<ArticleDetailsServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

        public async Task<bool> Delete(int id)
        {
            var article = await this.data.Articles.FindAsync(id);

            if (article == null)
            {
                return false;
            }

            this.data.Remove(article);

            await this.data.SaveChangesAsync();

            return true;
        }
    }
}
