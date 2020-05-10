using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colipu.Extensions.ACM.Aliyun;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spidernet.BLL.Configs;
using Spidernet.Client.Jobs;
using Spidernet.DAL;

namespace Spidernet.Client {
  public class Startup {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    public Startup(IConfiguration configuration) {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services) {

      services.AddAliyunACM(Configuration, (ex) => {
        System.Console.WriteLine(ex.Message);
      });

      services.Configure<DbConnectionConfig>(Configuration.GetSection("DatabaseSettings"));
      services.Configure<SpidernetClientConfig>(Configuration.GetSection("SpidernetClientConfig"));

      services.AddHostedService<ReadTaskJob>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {

      if (env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
      }

      app.UseRouting();

      app.UseEndpoints(endpoints => {
        endpoints.MapGet("/", async context => {
          await context.Response.WriteAsync("Hello World!");
        });
      });
    }
  }
}
