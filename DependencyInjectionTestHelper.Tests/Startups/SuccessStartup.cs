using DependencyInjectionTestHelper.Tests.Interfaces;
using DependencyInjectionTestHelper.Tests.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace DependencyInjectionTestHelper.Tests.Startups
{
    public class SuccessStartup
    {
        private readonly IWebHostEnvironment _env;

        public SuccessStartup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ISuccessService, SuccessService>();
            services.AddSingleton<ISuccessDependentService, SuccessDependentService>();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopment())
            {
                Console.WriteLine("Configuring...");
            }
        }
    }
}