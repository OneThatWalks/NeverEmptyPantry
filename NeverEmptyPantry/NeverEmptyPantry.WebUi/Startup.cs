using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NeverEmptyPantry.Application.Services;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Models.Account;
using NeverEmptyPantry.Common.Models.Identity;
using NeverEmptyPantry.Common.Models.List;
using NeverEmptyPantry.Common.Models.Product;
using NeverEmptyPantry.Repository.Entity;
using NeverEmptyPantry.Repository.Services;
using NeverEmptyPantry.WebUi.Models;

namespace NeverEmptyPantry.WebUi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // ===== Configure cookie policy
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // ===== Add DbContext
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // ===== Add Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager<ApplicationSignInManager>()
                .AddDefaultTokenProviders();

            // ===== Add Repositories
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IListRepository, ListRepository>();
            services.AddScoped<IListProductRepository, ListProductRepository>();
            services.AddScoped<IUserVoteRepository, UserVoteRepository>();

            // ===== Add Services
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IListService, ListService>();
            services.AddScoped<IListProductService, ListProductService>();
            services.AddScoped<IUserVoteService, UserVoteService>();

            // ===== Add MVC
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationDbContext dbContext)
        {
            // ===== Set error page
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // ===== Turn on app service settings
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            // ===== Setup routes
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // ===== Create database
            dbContext.Database.EnsureCreated();

            // ===== Automapper Mappings
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ProfileDto, ProfileViewModel>();
                cfg.CreateMap<RegisterViewModel, RegisterDto>();
                cfg.CreateMap<LoginDto, LoginViewModel>();
                cfg.CreateMap<ApplicationUser, ProfileViewModel>();
                cfg.CreateMap<ProductViewModel, ProductDto>();
                cfg.CreateMap<ProductDto, ProductViewModel>();
                cfg.CreateMap<ListViewModel, ListDto>();
                cfg.CreateMap<ListDto, ListViewModel>();
            });
        }
    }
}
