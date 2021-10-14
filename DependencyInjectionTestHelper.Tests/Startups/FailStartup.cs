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
    public class FailStartup
    {
        private readonly IWebHostEnvironment _env;

        public FailStartup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IFailDependentService, FailDependentService>();
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