namespace Blog.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;

    public class User : IdentityUser
    {
        public ICollection<Article> Articles { get; set; } = new List<Article>();
    }
}
