using DependencyInjectionTestHelper.Tests.Interfaces;
using DependencyInjectionTestHelper.Tests.Services;
using DependencyInjectionTestHelper.Tests.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;

namespace DependencyInjectionTestHelper.Tests.Startups
{
    public class FailStartup : IStartup
    {
        private readonly IHostingEnvironment _env;

        public FailStartup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IFailDependentService, FailDependentService>();
            services.Configure<FailSettings>(o => o = null);

            return services.BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
        }
    }
}