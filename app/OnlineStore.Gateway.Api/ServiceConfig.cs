namespace OnlineStore.Gateway.Api;

public class ServiceConfig
{
    public string Name { get; set; }
    public string Uri { get; set; }
    public bool IgnoreRootTypes { get; set; } = false;
    public string SchemaFileName { get; set; }
}
