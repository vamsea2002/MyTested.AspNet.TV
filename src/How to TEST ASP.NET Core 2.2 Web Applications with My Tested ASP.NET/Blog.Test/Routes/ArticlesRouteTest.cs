namespace Blog.Test.Routes
{
    using Blog.Controllers;
    using Blog.Controllers.Models;
    using MyTested.AspNetCore.Mvc;
    using Xunit;

    public class ArticlesRouteTest
    {
        [Fact]
        public void PostCreateShouldBeRoutedCorrectly()
            => MyRouting
                .Configuration()
                .ShouldMap(request => request
                    .WithLocation("/Articles/Create")
                    .WithMethod(HttpMethod.Post)
                    .WithUser()
                    .WithAntiForgeryToken())
                .To<ArticlesController>(c => c.Create(With.Any<ArticleFormModel>()));

        [Fact]
        public void GetEditShouldBeRoutedCorrectly()
            => MyRouting
                .Configuration()
                .ShouldMap("/Articles/Details/1")
                .To<ArticlesController>(c => c.Details(1));
    }
}
