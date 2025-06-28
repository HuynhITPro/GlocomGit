using NFCWebBlazor.Server.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using NFCWebBlazor;
using Microsoft.Extensions.FileProviders;
using NFCWebBlazor.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<NFCWebBlazor.Model.CallAPI>();
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://192.168.1.5:8001") });

//builder.Services.AddScoped<DashboardConfigurator>((IServiceProvider serviceProvider) => {
//    return DashboardUtils.CreateDashboardConfigurator(configuration, fileProvider);
//});

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
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();


app.UseRouting();



app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();