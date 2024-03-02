using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace OnlineStore.Gateway.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddHttpContextAccessor();

        var grapqlSrv = builder.Services.AddGraphQLServer();
        var services = GetServiceConfigs();

        foreach (var item in services)
        {
            builder.Services.AddHttpClient(item.Name, (sp, client) =>
            {
                var context = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;

                if (context.Request.Headers.ContainsKey("Authorization"))
                {
                    client.DefaultRequestHeaders.Authorization =
                        AuthenticationHeaderValue.Parse(
                            context.Request.Headers["Authorization"]
                                .ToString());
                }

                client.BaseAddress = new Uri(item.Uri);
            });

            grapqlSrv.AddRemoteSchema(item.Name, item.IgnoreRootTypes);

            if (!string.IsNullOrEmpty(item.SchemaFileName))
            {
                grapqlSrv.AddTypeExtensionsFromFile(item.SchemaFileName);
            }
        }

        var app = builder.Build();
        app.UseWebSockets();
        app.MapGraphQL("/gateway/graphql");
        app.Run();
    }

    private static List<ServiceConfig> GetServiceConfigs()
    {
        var services = new List<ServiceConfig>();
        using (StreamReader r = new StreamReader("./service-configs.json"))
        {
            var json = r.ReadToEnd();
            services = JsonConvert.DeserializeObject<List<ServiceConfig>>(json);
        }

        return services;
    }
}
