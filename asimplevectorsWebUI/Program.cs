using asimplevectorsWebUI;
using asimplevectorsWebUI.Components;
using asimplevectors.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.ResponseCompression;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddSignalR();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});

builder.Services.AddSingleton(sp =>
{
    // var baseUrl = builder.Configuration.GetValue<string>("Asimplevectors:BaseUrl") ?? "http://127.0.0.1:21001";
    var baseUrl = "http://192.168.75.114:21001";
    var logger = sp.GetService<ILogger<asimplevectors.Services.asimplevectorsClient>>();
    return new asimplevectors.Services.asimplevectorsClient(baseUrl, logger);
});

var certificate = new X509Certificate2("/app/certificate.pfx", "billionvectors");

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"/keys"))
    .ProtectKeysWithCertificate(certificate);

var app = builder.Build();

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