using ComUnity.Frontend.Api;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace ComUnity.Frontend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped<IComUnityApiClient>(sp =>
            {
                var httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7229/") };
                var client = new ComUnityApiClient(httpClient);

                return client;
            });

            await builder.Build().RunAsync();
        }
    }
}