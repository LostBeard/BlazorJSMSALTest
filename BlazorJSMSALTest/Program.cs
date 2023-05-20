using BlazorJSMSALTest;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SpawnDev.BlazorJS;
using SpawnDev.BlazorJS.WebWorkers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazorJSRuntime();
builder.Services.AddWebWorkerService();

// MSAL stuff
builder.Services.AddHttpClient("{PROJECT NAME}.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)).AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("{PROJECT NAME}.ServerAPI"));

// Add configuration singleton so it can be read in the Test service
builder.Services.AddSingleton(builder.Configuration);
// Test service
builder.Services.AddSingleton<ITestService, TestService>();

Console.WriteLine($"appsettings test in context {BlazorJSRuntime.JS.GlobalThisTypeName}: " + builder.Configuration["Message"]);

#if DEBUG || true

var host = builder.Build();

BlazorJSRuntime.JS.Set("_testWorker", new ActionCallback<bool>(async (verbose) => {
    var webWorkerService = host.Services.GetRequiredService<WebWorkerService>();
    var worker = await webWorkerService.GetWebWorker(verbose);
    var math = worker.GetService<ITestService>();
    var ret = await math.ReadAppSettingsValue("Message");
    Console.WriteLine(ret);
}));
BlazorJSRuntime.JS.Set("_testWorkerAndDispose", new ActionCallback<bool>(async (verbose) => {
    var webWorkerService = host.Services.GetRequiredService<WebWorkerService>();
    using var worker = await webWorkerService.GetWebWorker(verbose);
    var math = worker.GetService<ITestService>();
    var ret = await math.ReadAppSettingsValue("Message");
    Console.WriteLine(ret);
}));

await host.BlazorJSRunAsync();

#else

// build and Init using BlazorJSRunAsync (instead of RunAsync)
await builder.Build().BlazorJSRunAsync();

#endif


