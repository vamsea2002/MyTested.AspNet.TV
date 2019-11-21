namespace EFCoreOptimizations
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CatQueries
    {
        public static Func<CatsDbContext, int, string, IEnumerable<CatFamilyResult>> CatQuery
            => EF.CompileQuery((CatsDbContext db, int age, string nameStart) =>
                db.Cats
                    .Where(c =>
                        c.BirthDate.Year == 2019 &&
                        c.Color.Contains("B") &&
                        c.Owner.Cats.Any(c => c.Age < age) &&
                        c.Owner.Cats.Count(c => c.Name.Length > 3) > 3)
                    .Select(c => new CatFamilyResult
                    {
                        Name = c.Name,
                        Cats = c.Owner
                            .Cats
                            .Count(c =>
                                c.Age < age &&
                                c.Name.StartsWith(nameStart))
                    }));
    }
}
