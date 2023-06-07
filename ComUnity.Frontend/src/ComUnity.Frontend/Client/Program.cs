using Blazored.SessionStorage;
using ComUnity.Frontend.Api;
using ComUnity.Frontend.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

namespace ComUnity.Frontend;

public class Program
{
    private const string ComUnityApi = "ComUnityApi";

    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddBlazoredSessionStorage();
        builder.Services.AddAuthenticationCore();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();
        builder.Services.AddMudServices();
        builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();
        builder.Services.AddTransient<CookieHandler>();
        builder.Services.AddHttpClient(ComUnityApi, conf => conf.BaseAddress = new Uri("https://localhost:7229/"))
            .AddHttpMessageHandler<CookieHandler>();
        builder.Services.AddScoped<IAzureStorageFileUploader, AzureStorageFileUploader>();
        builder.Services.AddScoped<ErrorHandler>();
        builder.Services.AddScoped<IComUnityApiClient>(sp =>
        {
            var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var client = clientFactory.CreateClient(ComUnityApi);

            return new ComUnityApiClient(client);
        });

        await builder.Build().RunAsync();
    }
}