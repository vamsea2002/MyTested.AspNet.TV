namespace Blog
{
    using AutoMapper;
    using Controllers;
    using Data;
    using Infrastructure;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Services;

    public class Startup
    {
        public Startup(IConfiguration configuration) 
            => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDbContext<BlogDbContext>(options => options
                    .UseSqlServer(Configuration.GetDefaultConnectionString()));
            
            services
                .AddDefaultIdentity<IdentityUser>(options => options
                    .SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<BlogDbContext>();

            services.AddConventionalServices();

            services.AddAutoMapper(this.GetType());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandling(env);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints();

            app.SeedData();
        }
    }
}
