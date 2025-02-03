using asimplevectorsWebUI;
using asimplevectorsWebUI.Components;
using asimplevectorsWebUI.Services;
using asimplevectors.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.ResponseCompression;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddSignalR();
builder.Services.AddSingleton<ConfigService>();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});

builder.Services.AddSingleton(sp =>
{
    var configService = sp.GetRequiredService<ConfigService>();
    var baseUrl = configService.ServerUrl;
    var logger = sp.GetService<ILogger<asimplevectors.Services.asimplevectorsClient>>();
    return new asimplevectors.Services.asimplevectorsClient(baseUrl, logger);
});

var certificate = new X509Certificate2("/app/certificate.pfx", "billionvectors");

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"/data/keys"))
    .ProtectKeysWithCertificate(certificate);

var app = builder.Build();

// Add this line to set the ServiceProvider
Program.ServiceProvider = app.Services;

app.UseResponseCompression();

if (app.Environment.IsDevelopment())
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
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

public static partial class Program
{
    public static IServiceProvider ServiceProvider { get; set; }
}