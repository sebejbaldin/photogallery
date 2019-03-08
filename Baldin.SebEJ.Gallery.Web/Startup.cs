using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Baldin.SebEJ.Gallery.Web.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Baldin.SebEJ.Gallery.Data;
using Baldin.SebEJ.Gallery.ImageStorage;
using Baldin.SebEJ.Gallery.Web.Hubs;
using Microsoft.AspNetCore.HttpOverrides;
using Baldin.SebEJ.Gallery.Caching;
using Baldin.SebEJ.Gallery.Search;
using Baldin.SebEJ.Gallery.Search.Models;

namespace Baldin.SebEJ.Gallery.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
            //options.UseSqlServer(
            //    Configuration.GetConnectionString("SQLServerConn")));
            options.UseNpgsql(
                Configuration.GetConnectionString("PgSQLConnOnline")));
            services.AddDefaultIdentity<IdentityUser>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped<IUserClaimsPrincipalFactory<IdentityUser>, CustomClaimsPrincipalFactory>();

            services.AddTransient<IDataAccess, PgSQLData>();
            //services.AddSingleton<IImageManager>(new LocalUploader(Environment.WebRootPath));
            services.AddSingleton<ICaching>(new StupidRedisCaching(Configuration));
            //services.AddSingleton<ICaching, StupidRedisCaching>();
            services.AddSingleton<ISearch, ElasticSearch>();
            switch (Configuration["Storage"])
            {
                case "Amazon":
                    Configuration["PhotoUrl"] = Configuration["CloudStorage:Amazon:Storage"] + Configuration["CloudStorage:Amazon:Folder"];
                    services.AddTransient<IImageManager, AWSUploaderS3>();
                    break;
                case "Azure":
                    Configuration["PhotoUrl"] = Configuration["CloudStorage:Azure:Storage"] + Configuration["CloudStorage:Azure:Folder"];
                    services.AddTransient<IImageManager, AzureStorageUploader>();
                    break;
                default:
                    Configuration["PhotoUrl"] = "/uploads";
                    services.AddSingleton<IImageManager>(new LocalUploader(Environment.WebRootPath));
                    break;
            }

            services.AddSignalR();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ISearch search, IDataAccess dataAccess, UserManager<IdentityUser> userManager)
        {
            app.UseForwardedHeaders();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var data = dataAccess.GetPictures().GroupBy(x => x.User_Id);
            var esData = new List<ES_DN_Photo>();
            foreach (var group in data)
            {
                var user = userManager.FindByIdAsync(group.Key).Result;

                esData.AddRange(group.Select(e => new ES_DN_Photo {
                    PhotoId = e.Id,
                    User = new ES_DN_User
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        UserName = user.Email
                    },
                    Data = new ES_DN_Data
                    {
                        Name = e.OriginalName ?? e.Name,
                        TotalRating = e.Total_Rating,
                        Votes = e.Votes,
                        Url = e.Url,
                        Thumbnail_Url = e.Thumbnail_Url ?? e.Url
                    }
                }));
            }

            search.InsertPhotosAsync(esData);

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseSignalR(routes =>
            {
                routes.MapHub<GalleryHub>("/Comments");
            });

            app.UseMvc();
        }
    }
}
