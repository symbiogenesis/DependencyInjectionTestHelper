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
    public class SuccessStartup : IStartup
    {
        private readonly IHostingEnvironment _env;

        public SuccessStartup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ISuccessService, SuccessService>();
            services.AddSingleton<ISuccessDependentService, SuccessDependentService>();
            services.Configure<SuccessSettings>(x => x = new SuccessSettings());

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