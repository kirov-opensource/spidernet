using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spidernet.WebAPI.Tests {
  public class TestWithServer {

    protected TestServer testServer;

    public TestWithServer(bool useTestServer) {
      var webHostBuilder = WebHost.CreateDefaultBuilder(new string[] { }).UseStartup<Startup>();
      TestServer testServer = new TestServer(webHostBuilder);
      this.testServer = testServer;
    }

    public TestWithServer() : this(true) {

    }
  }
}
