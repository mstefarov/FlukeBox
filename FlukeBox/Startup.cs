using FlukeBox.Annotations;
using FlukeBox.Jobs;
using FlukeBox.MusicLibrary;
using FlukeBox.MusicLibrary.Sqlite;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FlukeBox {
    [UsedImplicitly]
    public class Startup {
        private readonly ILoggerFactory _loggerFactory;

        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory) {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            _loggerFactory = loggerFactory;
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        [UsedImplicitly]
        public void ConfigureServices(IServiceCollection services) {
            // Add framework services.
            services.AddMvc();
            services.AddSingleton<IJobRegistry, GoodJob.Registry>();

            var sqliteMetadataRepo = new SqliteMetadataRepo(_loggerFactory.CreateLogger(typeof(IMetadataRepo)));
            sqliteMetadataRepo.PrepareAsync().Wait();
            services.AddSingleton<IMetadataRepo>(sqliteMetadataRepo);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            _loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            _loggerFactory.AddDebug();

            app.UseMvc();

            DefaultFilesOptions dfOptions = new DefaultFilesOptions();
            dfOptions.DefaultFileNames.Clear();
            dfOptions.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(dfOptions);

            app.UseStaticFiles();
        }
    }
}
