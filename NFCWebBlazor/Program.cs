using NFCWebBlazor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.SessionStorage;

using System.Globalization;
using Blazored.Modal;
using NFCWebBlazor.App_ClassDefine;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.Model;
using Microsoft.AspNetCore.Components.WebAssembly;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddBlazorBootstrap();

builder.Services.AddBlazoredSessionStorage();
builder.Services.AddBlazoredModal();



builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<BrowserService>(); // scoped service lấy kích thước màn hình

builder.Services.AddScoped<NFCWebBlazor.Model.CallAPI>();

builder.Services.AddSingleton<NFCWebBlazor.Model.UserGlobal>();
builder.Services.AddSingleton<PhanQuyenAccess>();
builder.Services.AddSingleton<NFCWebBlazor.Model.ThemeColor>();
builder.Services.AddSingleton(sp => new SignalRConnect());
builder.Services.AddDevExpressBlazor();
builder.Services.AddScoped<PopupService>();
//Sử dụng Skia để xuất report 

builder.Services.AddDevExpressWebAssemblyBlazorReportViewer();
//
builder.Services.AddDevExpressServerSideBlazorPdfViewer();

var culture = new CultureInfo(CultureInfo.CurrentCulture.Name);
Console.WriteLine(culture.Name);
culture.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

await builder.Build().RunAsync();