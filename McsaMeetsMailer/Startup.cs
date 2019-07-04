using McsaMeetsMailer.BusinessLogic.EmailAddressSheet;
using McsaMeetsMailer.BusinessLogic.MeetsSheet;
using McsaMeetsMailer.Services;
using McsaMeetsMailer.Utils.Logging;
using McsaMeetsMailer.Utils.RestRequest;
using McsaMeetsMailer.Utils.Settings;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace McsaMeetsMailer
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
      services.Configure<CookiePolicyOptions>(options =>
      {
        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.None;
      });

      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

      var logger = new ConsoleLogger();
      var requestMaker = new WebRestRequestMaker();
      var settings = new EnvironmentVariableSettings();
      var meetSheetFactory = new MeetsGoogleSheetFactory();
      var emailAddressSheetFactory = new EmailAddressGoogleSheetFactory();

      var meetsService = new MeetsService(
        settings,
        requestMaker,
        meetSheetFactory,
        logger);

      var emailAddressService = new EmailAddressService(
        settings,
        requestMaker,
        emailAddressSheetFactory,
        logger);

      var emailSenderService = new EmailSenderService(settings);

      services.AddSingleton<ILogger>(logger);
      services.AddSingleton<ISettings>(settings);
      services.AddSingleton<IMeetsService>(meetsService);
      services.AddSingleton<IEmailAddressService>(emailAddressService);
      services.AddSingleton<IEmailSenderService>(emailSenderService);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseCookiePolicy();

      app.UseMvc();
    }
  }
}
