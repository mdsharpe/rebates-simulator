using RebatesSimulator.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSignalR(configure =>
{
#if DEBUG
    configure.EnableDetailedErrors = true;
#endif
}).AddMessagePackProtocol();

builder.Services.AddResponseCompression();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHttpsRedirection();
    ////app.UseHsts();
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapControllers();
app.MapHub<GameHub>("/tilehub");
app.MapFallbackToFile("index.html");

if (!app.Environment.IsDevelopment())
{
    app.UseResponseCompression();
}

app.Run();
