using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace McsaMeetsMailer
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
      WebHost
        .CreateDefaultBuilder(args)
        //.UseSetting("detailedErrors", "true")
        //.CaptureStartupErrors(true)
        .UseStartup<Startup>();
  }
}
