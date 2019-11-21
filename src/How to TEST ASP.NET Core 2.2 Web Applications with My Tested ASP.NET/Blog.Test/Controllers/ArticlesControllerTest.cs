namespace Blog.Test.Controllers
{
    using Blog.Controllers;
    using Blog.Controllers.Models;
    using Blog.Data.Models;
    using Shouldly;
    using System;
    using System.Linq;
    using MyTested.AspNetCore.Mvc;
    using Xunit;

    public class ArticlesControllerTest
    {
        [Fact]
        public void AllShouldReturnCorrectArticles()
            => MyController<ArticlesController>
                .Instance()
                .WithData(new Article
                {
                    Id = 1,
                    Title = "Test Article",
                    Content = "Test Content",
                    IsPublic = true,
                    PublishedOn = new DateTime(2013, 12, 12),
                    User = new User
                    {
                        Id = TestUser.Identifier,
                        UserName = TestUser.Username
                    }
                })
                .Calling(c => c.All(1))
                .ShouldReturn()
                .View(result => result
                    .WithModelOfType<ArticleListingViewModel>()
                    .Passing(model =>
                    {
                        model.Page.ShouldBe(1);
                        model.Articles.Count().ShouldBe(1);
                        model.Articles.FirstOrDefault(article => article.Id == 1).ShouldNotBeNull();
                    }));

        [Fact]
        public void PostCreateShouldBeAllowedOnlyForPostRequestAndAuthorizedUsers()
            => MyController<ArticlesController>
                .Instance()
                .Calling(c => c.Create(With.Default<ArticleFormModel>()))
                .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests()
                    .RestrictingForHttpMethod(HttpMethod.Post));

        [Fact]
        public void PostCreateShouldReturnViewWithTheSameModelWhenModelStateIsInvalid()
            => MyController<ArticlesController>
                .Instance()
                .Calling(c => c.Create(With.Default<ArticleFormModel>()))
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .View(result => result
                    .WithModelOfType<ArticleFormModel>()
                    .Passing(article => article.Title.ShouldBeNull()));

        [Theory]
        [InlineData("Test Title", "Test Content")]
        public void PostCreateShouldReturnRedirectWithTempDataMessageAndShouldSaveArticleWithValidArticle(
            string title,
            string content)
            => MyController<ArticlesController>
                .Instance()
                .WithUser()
                .Calling(c => c.Create(new ArticleFormModel
                {
                    Title = title,
                    Content = content
                }))
                .ShouldHave()
                .Data(data => data
                    .WithSet<Article>(set =>
                    {
                        set.ShouldNotBeNull();
                        set.FirstOrDefault(article => article.Title == title).ShouldNotBeNull();
                    }))
                .AndAlso()
                .ShouldHave()
                .TempData(tempData => tempData
                    .ContainingEntryWithKey(ControllerConstants.SuccessMessage))
                .AndAlso()
                .ShouldReturn()
                .Redirect(result => result
                    .To<ArticlesController>(c => c.Mine()));
    }
}
