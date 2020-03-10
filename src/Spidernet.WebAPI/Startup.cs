using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Spidernet.WebAPI.Extensions;
using System.IO;

namespace Spidernet.WebAPI {
  public class Startup {
    public Startup(IConfiguration configuration) {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services.AddControllers().AddNewtonsoftJson(options => {
        options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
        options.SerializerSettings.ContractResolver = new DefaultContractResolver() { NamingStrategy = new DefaultNamingStrategy() };
        options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
      });
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
      services.AddSwaggerGenNewtonsoftSupport();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {

      app.UseMiddleware<ExceptionHandler>();
      if (env.IsStaging() || env.IsDevelopment()) {
        app.UseSwagger();
        app.UseSwaggerUI(c => {
          c.SwaggerEndpoint("/swagger/v1/swagger.json", "Spidernet.WebAPI v1");
        });
      }
      app.UseRouting();
      //app.UseAuthorization();
      app.UseEndpoints(endpoints => {
        endpoints.MapControllers();
      });
    }
  }
}
