using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using System.Reflection;
using Battleship.API.Model;

namespace Battleship.API
{
    public class Startup
    {

        public IHostingEnvironment HostingEnvironment { get; private set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(env.ContentRootPath))
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            HostingEnvironment = env;
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            // Register the IConfiguration instance which Info binds against.
            if (HostingEnvironment.IsDevelopment())
            {
                services.AddCors(options => options.AddPolicy("AllowDevelopment",
                    p => p.WithOrigins("http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader()));
            }

            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                var assembly = Assembly.GetAssembly(typeof(Startup));
                var assemblyName = assembly.GetName();
                var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                var fullSemVersion = assemblyName.Version.ToString();
                if (fileVersionInfo.ProductVersion.Contains("+"))
                {
                    fullSemVersion = fileVersionInfo.ProductVersion.Split("+".ToCharArray())[0];
                }
                c.SwaggerDoc($"v{fullSemVersion}", new Info { Title = $"{assembly.GetName().Name}", Version = $"v{fullSemVersion}" });
                c.IncludeXmlComments(Path.ChangeExtension(Assembly.GetEntryAssembly().Location, "xml"));
            });

            services.AddSingleton<IGameBoardListContainer, GameBoardListContainer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (HostingEnvironment.IsDevelopment())
            {
                app.UseCors("AllowDevelopment");
            }

            var options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("index.html");

            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value))
                {
                    context.Request.Path = "/index.html";
                }
            });

            app.MapWhen(context => context.Request.Path.Value.StartsWith("/api"), builder =>
            {
                builder.UseMvc();
            });

            app.UseDefaultFiles(options);
            app.UseStaticFiles();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                var assembly = Assembly.GetAssembly(typeof(Startup));
                var assemblyName = assembly.GetName();
                var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                var fullSemVersion = assemblyName.Version.ToString();
                if (fileVersionInfo.ProductVersion.Contains("+"))
                {
                    fullSemVersion = fileVersionInfo.ProductVersion.Split("+".ToCharArray())[0];
                }
                c.SwaggerEndpoint($"/swagger/v{fullSemVersion}/swagger.json", $"{assembly.GetName().Name} v{fullSemVersion}");
            });
        }

    }
}
