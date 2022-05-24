using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
	var settings = config.Build();
	if (hostingContext.HostingEnvironment.IsDevelopment())
	{
		var connection = settings.GetConnectionString("AppConfig");
		config.AddAzureAppConfiguration(options =>
			options
				.Connect(connection)
				.Select(KeyFilter.Any, LabelFilter.Null)
				.Select(KeyFilter.Any, hostingContext.HostingEnvironment.EnvironmentName)
		);
	}
	else
	{
		ManagedIdentityCredential credentials = new ManagedIdentityCredential();
		var appConfigUrl = settings.GetConnectionString("AppConfigUrl");
		config.AddAzureAppConfiguration(options =>
			options
				.Connect(new Uri(appConfigUrl), credentials)
				.Select(KeyFilter.Any, LabelFilter.Null)
				.Select(KeyFilter.Any, hostingContext.HostingEnvironment.EnvironmentName)
		);
	}
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
