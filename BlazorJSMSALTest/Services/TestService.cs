using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SpawnDev.BlazorJS.WebWorkers;
using System.Diagnostics;

namespace BlazorJSMSALTest
{
    public interface ITestService
    {
        Task<string> ReadAppSettingsValue(string key);
    }

    public class TestService : ITestService
    {
        WebAssemblyHostConfiguration _configuration;
        public TestService(WebAssemblyHostConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<string> ReadAppSettingsValue(string key)
        {
            return Task.FromResult(_configuration[key] ?? "");
        }
    }
}
