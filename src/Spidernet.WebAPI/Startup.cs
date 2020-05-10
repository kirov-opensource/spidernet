using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Snowflake.Core;
using Spidernet.WebAPI.Extensions;
using System.IO;

namespace Spidernet.WebAPI {
  /// <summary>
  /// 
  /// </summary>
  public class Startup {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    public Startup(IConfiguration configuration) {
      Configuration = configuration;
    }
    /// <summary>
    /// 
    /// </summary>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// This method gets called by the runtime. Use this method to add services to the container.
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services) {
      // DB������
      services.Configure<DAL.DbConnectionConfig>(Configuration.GetSection("DatabaseSettings"));
      services.AddTransient<DAL.DbConnectionFactory>();

      // ID������
      services.AddSingleton(item => new IdWorker(1, Configuration.GetValue<int>("ServerNodeNo")));

      // ���л�����
      services.AddControllers().AddNewtonsoftJson(options => {
        options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
        options.SerializerSettings.ContractResolver = new DefaultContractResolver() { NamingStrategy = new DefaultNamingStrategy() };
        options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
      });

      // ���SwaggerUI
      services.AddSwaggerGen(options => {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Colipu TMS WebAPI", Version = "v1" });
        var basePath = PlatformServices.Default.Application.ApplicationBasePath;
        var webAppXmlPath = Path.Combine(basePath, "Spidernet.WebAPI.xml");
        var dalXmlPath = Path.Combine(basePath, "Spidernet.DAL.xml");
        var coreXmlPath = Path.Combine(basePath, "Spidernet.BLL.xml");
        options.IncludeXmlComments(webAppXmlPath);
        options.IncludeXmlComments(dalXmlPath);
        options.IncludeXmlComments(coreXmlPath);
      });

      // Swagger JSON֧��
      services.AddSwaggerGenNewtonsoftSupport();

      // ����ע��
      this.ConfigureDependencyInjection(services);
    }
    private void ConfigureDependencyInjection(IServiceCollection services) {

      services.AddScoped<Filters.AuthorizationFilter>();
      services.AddScoped<BLL.Session>();
      services.AddScoped<BLL.Services.UserService>();
      services.AddScoped<BLL.Services.TemplateService>();

      services.AddTransient<DAL.Repositories.UserRepository>();
      services.AddTransient<DAL.Repositories.TemplateRepository>();
      services.AddTransient<DAL.Repositories.TaskRepository>();
    }
    /// <summary>
    /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="env"></param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {

      app.UseMiddleware<ExceptionHandler>();

      if (!env.IsProduction()) {
        app.UseSwagger();
        app.UseSwaggerUI(c => {
          c.SwaggerEndpoint("/swagger/v1/swagger.json", "Spidernet.WebAPI v1");
        });
      }


      app.UseRouting();
      app.UseAuthorization();
      app.UseEndpoints(endpoints => {
        endpoints.MapControllers();
      });
    }
  }
}
