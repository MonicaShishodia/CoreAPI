using DataAccess;
using Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Service;
using System;

namespace CoreAPI
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
            services.AddDbContext<CoreApidbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("DB_Conn")));
            services.AddScoped<IImage, ImageService>();
            services.AddScoped<IGeoPoint, GeoPointService>();
            services.AddScoped<IQuest, QuestService>();
            services.AddControllers().AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
            
            services.AddSwaggerGen(x => x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Core API", Version = "1", Description = "API to upload images" }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, CoreApidbContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                dbContext.Database.EnsureCreated();
            }
            //app.UseHttpsRedirection();


            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(Configuration["MyConfig:NetworkPath"]),
                RequestPath = Configuration["MyConfig:RequestPath"],
                EnableDefaultFiles = true
            });
            app.UseRouting();
            //app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller}/{action}/{parameter?}");
            });
        }
    }
}
