using Microsoft.Identity.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace NotSoConfidentialClient;

internal class Program
{
    public static async Task Main(string[] args)
    {
        // APPID URL => https://spectologicb2crtm.onmicrosoft.com/5e8b0e5d-24a3-4ca1-8cd7-ed3baab998fd

        HttpClient httpClient = new HttpClient();
        var tenantId = "faa4161a-4092-475d-9a42-7ada6552a0e9";
        var clientId = "5e8b0e5d-24a3-4ca1-8cd7-ed3baab998fd";
        var clientSecret = "<REMOVED FOR SECURITY REASONS>";

        IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(clientId)
            .WithClientSecret(clientSecret)
            .WithTenantId(tenantId)
            .Build();

        Console.ReadLine();

        var response = await app.AcquireTokenForClient(new List<string> { "https://spectologicb2crtm.onmicrosoft.com/5e8b0e5d-24a3-4ca1-8cd7-ed3baab998fd/.default" }).ExecuteAsync();
        var accessToken = response.AccessToken;

        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://localhost:7169/home/api")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var resp = await httpClient.SendAsync(request);
        Console.WriteLine(request.RequestUri);
        Console.WriteLine(resp.StatusCode);
        var content = await resp.Content.ReadAsStringAsync();
        Console.WriteLine(content);
    }
}
