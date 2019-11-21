namespace EFCoreOptimizations
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class Program
    {
        public static void Main()
        {
            // SeedData();
            // TooManyQueries();
            // LazyLoadingTooManyQueries();
            // SelectSpecificColumns();
            // IncorrectDataLoading();
            // DeleteOptimized();
            // ColdAndWarmQueries();
            // CompiledQueries();
            // ChangeTracking();

            MiscellaneousOptimizations();
        }

        private static void MiscellaneousOptimizations()
        {
            // Evaluating Slow Queries
            using (var db = new CatsDbContext())
            {
                db.Cats
                    .Where(c =>
                        c.BirthDate.Year == 2019 &&
                        c.Color.Contains("B") &&
                        c.Owner.Cats.Any(c => c.Age < 3) &&
                        c.Owner.Cats.Count(c => c.Name.Length > 3) > 3)
                    .OrderBy(c => c.Owner.Cats.Average(c => c.BirthDate.Year))
                    .Select(c => new CatFamilyResult
                    {
                        Name = c.Name,
                        Cats = c.Owner
                            .Cats
                            .Count(c =>
                                c.Age < 3 &&
                                c.Name.StartsWith("A"))
                    })
                    .ToList();
            }

            // Indexing
            using (var db = new CatsDbContext())
            {
                var cat = db.Cats
                    .Where(c => c.Age > 10)
                    .ToList();
            }

            // Client Functions
            using (var db = new CatsDbContext())
            {
                var cat = db.Cats
                    .Where(c => c.Owner.Name.Length > 3)
                    .Select(c => new
                    {
                        c.Name,
                        Owner = c.Owner.Name
                    })
                    .ToList();

                Console.WriteLine($"{db.ChangeTracker.Entries().Count()} Tracked Entities.");
            }

            // Use Async/Await
            // Dispose The DbContext Frequently
            // Use Caching Techniques
            // Enable Multiple Result Sets
            // Try Entity Framework Plus
            // Entity Framework May Not Be The Answer
            // -- Try Dapper
            // -- Use Native SQL
        }

        private static string SelectOwnerName(Owner owner)
            => owner.Name;

        private static bool FilterOwner(Owner owner)
            => owner.Name.Length > 3;

        private static void ChangeTracking()
        {
            // Warm-up
            using (var db = new CatsDbContext())
            {
                var cat = db.Cats
                    .Where(c => c.Id % 10 == 0)
                    .ToList();
            }

            var stopWatch = Stopwatch.StartNew();

            // No tracking on a DbContext instance level
            using (var db = new CatsDbContext())
            {
                db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                var cat = db.Cats
                    .Where(c => c.Id % 10 == 0)
                    .ToList();

                Console.WriteLine($"{stopWatch.Elapsed} - No tracking per context");

                Console.WriteLine($"{db.ChangeTracker.Entries().Count()} Tracked Entities.");
            }

            Console.WriteLine();

            stopWatch = Stopwatch.StartNew();

            // No tracking per single query
            using (var db = new CatsDbContext())
            {
                var cat = db.Cats
                    .AsNoTracking()
                    .Where(c => c.Id % 10 == 0)
                    .ToList();

                Console.WriteLine($"{stopWatch.Elapsed} - No tracking per query");

                Console.WriteLine($"{db.ChangeTracker.Entries().Count()} Tracked Entities.");
            }

            Console.WriteLine();

            stopWatch = Stopwatch.StartNew();

            // Default tracking
            using (var db = new CatsDbContext())
            {
                var cat = db.Cats
                    .Where(c => c.Id % 10 == 0)
                    .ToList();

                Console.WriteLine($"{stopWatch.Elapsed} - Tracking");

                Console.WriteLine($"{db.ChangeTracker.Entries().Count()} Tracked Entities.");
            }

            Console.WriteLine();

            stopWatch = Stopwatch.StartNew();

            // Tracking in projection without a data model
            using (var db = new CatsDbContext())
            {
                var cat = db.Cats
                    .Where(c => c.Id % 10 == 0)
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        Owner = c.Owner.Name
                    })
                    .ToList();

                Console.WriteLine($"{stopWatch.Elapsed} - Tracking in projection without a data model");

                Console.WriteLine($"{db.ChangeTracker.Entries().Count()} Tracked Entities.");
            }

            Console.WriteLine();

            stopWatch = Stopwatch.StartNew();

            // Tracking in projection with a data model
            using (var db = new CatsDbContext())
            {
                var cat = db.Cats
                    .Where(c => c.Id % 10 == 0)
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Owner
                    })
                    .ToList();

                Console.WriteLine($"{stopWatch.Elapsed} - Tracking in projection with a data model");

                Console.WriteLine($"{db.ChangeTracker.Entries().Count()} Tracked Entities.");
            }

            Console.WriteLine();

            stopWatch = Stopwatch.StartNew();

            // No tracking in projection with a data model
            using (var db = new CatsDbContext())
            {
                var cat = db.Cats
                    .AsNoTracking()
                    .Where(c => c.Id % 10 == 0)
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Owner
                    })
                    .ToList();

                Console.WriteLine($"{stopWatch.Elapsed} - No tracking in projection with a data model");

                Console.WriteLine($"{db.ChangeTracker.Entries().Count()} Tracked Entities.");
            }
        }

        private static void CompiledQueries()
        {
            var stopWatch = Stopwatch.StartNew();

            // First query will be slow
            using (var db = new CatsDbContext())
            {
                CatQuery(db, 5, "C");
            }

            Console.WriteLine($"{stopWatch.Elapsed} - Cold");

            stopWatch = Stopwatch.StartNew();

            // Second query will be faster because of EF internal cache
            using (var db = new CatsDbContext())
            {
                CatQuery(db, 5, "C");
            }

            Console.WriteLine($"{stopWatch.Elapsed} - Warm");

            stopWatch = Stopwatch.StartNew();

            // Third query will use our compiled query for the first time
            using (var db = new CatsDbContext())
            {
                CatQueries.CatQuery(db, 5, "C");
            }

            Console.WriteLine($"{stopWatch.Elapsed} - Compiled Cold");

            stopWatch = Stopwatch.StartNew();

            // Fourth query will use our compiled query after it is processed
            using (var db = new CatsDbContext())
            {
                CatQueries.CatQuery(db, 5, "C");
            }

            Console.WriteLine($"{stopWatch.Elapsed} - Compiled Warm");
        }

        private static void CatQuery(CatsDbContext db, int age, string nameStart)
        {
            var cat = db.Cats
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
                })
                .ToList();
        }

        private static void ColdAndWarmQueries()
        {
            var stopWatch = Stopwatch.StartNew();

            // First query will be slow
            using (var db = new CatsDbContext())
            {
                SameQuery(db);
            }

            Console.WriteLine(stopWatch.Elapsed);

            stopWatch = Stopwatch.StartNew();

            // Second query will be fast
            using (var db = new CatsDbContext())
            {
                SameQuery(db);
            }

            Console.WriteLine(stopWatch.Elapsed);
        }

        private static void SameQuery(CatsDbContext db)
        {
            var cat = db.Cats
                .Where(c =>
                    c.BirthDate.Year == 2019 &&
                    c.Color.Contains("B") &&
                    c.Owner.Cats.Any(c => c.Age < 5) &&
                    c.Owner.Cats.Count(c => c.Name.Length > 3) > 3)
                .Select(c => new
                {
                    c.Name,
                    Cats = c.Owner
                        .Cats
                        .Count(c =>
                            c.Age < 5 &&
                            c.Name.StartsWith("C"))
                })
                .ToList();
        }

        private static void DeleteOptimized()
        {
            // Makes two queries - one read and one delete
            using (var db = new CatsDbContext())
            {
                var cat = db.Cats.Find(15);

                db.Remove(cat);

                db.SaveChanges();
            }

            // Makes one query - only delete
            using (var db = new CatsDbContext())
            {
                var cat = new Cat { Id = 16 };

                db.Remove(cat);

                db.SaveChanges();
            }

            // Delete multiple rows - slow
            using (var db = new CatsDbContext())
            {
                var catsToDelete = db.Cats
                    .Where(c => c.Age == 9)
                    .Select(c => c.Id);

                db.RemoveRange(catsToDelete.Select(id => new Cat { Id = id }));

                db.SaveChanges();
            }

            // Delete multiple rows - fast with SQL
            using (var db = new CatsDbContext())
            {
                db.Database
                    .ExecuteSqlInterpolated($"DELETE FROM Cats WHERE Age = {9}");
            }
        }

        private static void IncorrectDataLoading()
        {
            // Methods which load data from the database
            // .ToList(), .ToArray(), .ToDictionary(),
            // .Any(), .All(), Count(), .Contains()
            // .First...(), .Last...(), .Single...()
            // .Max(), Min(), Average(), Sum()
            // All Async versions and more

            // No Database Call
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .Where(c => c.Name.Contains("1"))
                    .OrderBy(c => c.Name)
                    .Select(c => c.Name);
            }

            var stopWatch = Stopwatch.StartNew();

            // Getting All Rows
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .ToList()
                    .Where(c => c.Name.Contains("1"));
            }

            Console.WriteLine(stopWatch.Elapsed);

            stopWatch = Stopwatch.StartNew();

            // Getting Only The Rows We Need
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .Where(c => c.Name.Contains("1"))
                    .ToList();
            }

            Console.WriteLine(stopWatch.Elapsed);

            stopWatch = Stopwatch.StartNew();

            // Inner Queries Work OK Depending On EF Version
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .Where(c => c.Name.Contains("1"))
                    .Select(c => new
                    {
                        c.Name,
                        OtherCats = c.Owner.Cats.ToList()
                    })
                    .ToList();
            }

            Console.WriteLine(stopWatch.Elapsed);
        }

        private static void SelectSpecificColumns()
        {
            var stopWatch = Stopwatch.StartNew();

            // Getting All Columns
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .Where(c => c.Name.Contains("1"))
                    .ToDictionary(c => c.Name, c => c.Age);
            }

            Console.WriteLine(stopWatch.Elapsed);

            stopWatch = Stopwatch.StartNew();

            // Getting Only The Columns We Need
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .Where(c => c.Name.Contains("1"))
                    .Select(c => new CatResult
                    {
                        Name = c.Name,
                        Age = c.Age
                    })
                    .ToDictionary(c => c.Name, c => c.Age);
            }

            Console.WriteLine(stopWatch.Elapsed);
        }

        private static void LazyLoadingTooManyQueries()
        {
            var stopWatch = Stopwatch.StartNew();

            // N + 1 With Lazy Loading
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .Where(c => c.Name.Contains("1"))
                    .ToList();

                var ownerNames = new List<string>();

                foreach (var cat in cats)
                {
                    var ownerName = cat.Owner.Name;
                    ownerNames.Add(ownerName);
                }
            }

            Console.WriteLine(stopWatch.Elapsed);

            stopWatch = Stopwatch.StartNew();

            // Using Select
            using (var db = new CatsDbContext())
            {
                var ownerNames = db.Cats
                    .Where(c => c.Name.Contains("1"))
                    .Select(c => c.Owner.Name)
                    .ToList();
            }

            Console.WriteLine(stopWatch.Elapsed);
        }

        private static void TooManyQueries()
        {
            var stopWatch = Stopwatch.StartNew();

            // N + 1
            using (var db = new CatsDbContext())
            {
                var owners = db.Owners
                    .Where(o => o.Name.Contains("1"))
                    .ToList();

                foreach (var owner in owners)
                {
                    var cats = db.Cats
                        .Where(c => c.OwnerId == owner.Id && c.Name.Contains("1"))
                        .ToList();
                }
            }

            Console.WriteLine(stopWatch.Elapsed);

            stopWatch = Stopwatch.StartNew();

            // Using Include
            using (var db = new CatsDbContext())
            {
                var owners = db.Owners
                    .Where(o => o.Name.Contains("1"))
                    .Include(o => o.Cats)
                    .ToList();

                foreach (var owner in owners)
                {
                    var cats = owner.Cats
                        .Where(c => c.Name.Contains("1"))
                        .ToList();
                }
            }

            Console.WriteLine(stopWatch.Elapsed);

            stopWatch = Stopwatch.StartNew();

            // Using Select
            using (var db = new CatsDbContext())
            {
                var owners = db.Owners
                    .Where(o => o.Name.Contains("1"))
                    .Select(o => new
                    {
                        Cats = o.Cats
                            .Where(c => c.Name.Contains("1"))
                    })
                    .ToList();
            }

            Console.WriteLine(stopWatch.Elapsed);
        }

        private static void SeedData()
        {
            using (var db = new CatsDbContext())
            {
                db.Database.Migrate();

                db.ChangeTracker.AutoDetectChangesEnabled = true;

                for (int i = 1; i <= 10000; i++)
                {
                    var owner = new Owner
                    {
                        Name = $"Owner {i}"
                    };

                    for (int j = 1; j <= 10; j++)
                    {
                        owner.Cats.Add(new Cat
                        {
                            Name = $"Cat {i} {j}",
                            Color = j % 2 == 0 ? "Black" : "White",
                            BirthDate = DateTime.Now.AddDays(-j),
                            Age = j
                        });
                    }

                    db.Owners.Add(owner);

                    if (i % 200 == 0)
                    {
                        db.SaveChanges();
                        Console.Write(".");
                    }
                }
            }
        }
    }
}
