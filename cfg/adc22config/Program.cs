using adc22config.Models;
using adc22config.Services;
using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

IConfigurationRefresher? _refresher = null;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    var settings = config.Build();
    if (hostingContext.HostingEnvironment.IsDevelopment())
    {
        var connection = settings.GetConnectionString("AppConfig");
        config.AddAzureAppConfiguration(options =>
        {
            options
                .Connect(connection)
                .ConfigureRefresh(refresh =>
                {
                    refresh
                        .Register("TestApp:Settings:Sentinel",
                            /* hostingContext.HostingEnvironment.EnvironmentName, */
                            true)
                        .SetCacheExpiration(TimeSpan.FromDays(30)); // Important: Reduce poll frequency
                })
                .Select(KeyFilter.Any, LabelFilter.Null)
                .Select(KeyFilter.Any, hostingContext.HostingEnvironment.EnvironmentName);
            _refresher = options.GetRefresher();
        });
    }
    else
    {
        ManagedIdentityCredential credentials = new ManagedIdentityCredential();
        var appConfigUrl = settings.GetConnectionString("AppConfigUrl");
        config.AddAzureAppConfiguration(options =>
            options
                .Connect(new Uri(appConfigUrl), credentials)
                .ConfigureKeyVault(kv =>
                {
                    kv.SetCredential(credentials);
                })
                .ConfigureRefresh(refresh =>
                {
                    refresh.Register("TestApp:Settings:Sentinel",
                        /* hostingContext.HostingEnvironment.EnvironmentName, */
                        true)
                        .SetCacheExpiration(TimeSpan.FromSeconds(30));
                })
                .Select(KeyFilter.Any, LabelFilter.Null)
                .Select(KeyFilter.Any, hostingContext.HostingEnvironment.EnvironmentName)
        );
    }
});

// Add services to the container.
builder.Services.Configure<Settings>(builder.Configuration.GetSection("TestApp:Settings"));
builder.Services.AddControllersWithViews();
builder.Services.AddAzureAppConfiguration();
builder.Services.AddHostedService<ConfigurationUpdateService>();
builder.Services.AddSingleton<IConfigurationUpdater, ConfigurationUpdater>();
if (_refresher != null) builder.Services.AddSingleton<IConfigurationRefresher>(_refresher);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseAzureAppConfiguration();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
