namespace Blog.Controllers
{
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models.Articles;
    using Services;
    using System.Threading.Tasks;

    public class ArticlesController : Controller
    {
        private readonly IArticleService articles;

        public ArticlesController(IArticleService articles)
            => this.articles = articles;

        public async Task<IActionResult> Index(int page = 1)
            => Ok(await this.articles.All(page));

        [HttpGet]
        [Authorize]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(ArticleFormModel model)
        {
            if (this.ModelState.IsValid)
            {
                var id = await this.articles.Create(
                    model.Title,
                    model.Description,
                    this.User.GetUserId());

                return RedirectToAction(nameof(Details), new { id });
            }

            return this.View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            if (!await this.articles.Exists(id, this.User.GetUserId()))
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int id, ArticleFormModel model)
        {
            if (!await this.articles.Exists(id, this.User.GetUserId()))
            {
                return NotFound();
            }

            if (this.ModelState.IsValid)
            {
                await this.articles.Edit(id, model.Title, model.Description);

                return RedirectToAction(nameof(Details), new { id });
            }

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var article = await this.articles.Details(id);

            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await this.articles.Exists(id, this.User.GetUserId()))
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            if (!await this.articles.Exists(id, this.User.GetUserId()))
            {
                return NotFound();
            }

            await this.articles.Delete(id);

            return Redirect(nameof(Index));
        }
    }
}
