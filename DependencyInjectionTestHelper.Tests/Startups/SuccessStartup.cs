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
        private readonly IServiceProvider _serviceProvider;

        public SuccessStartup(IConfiguration configuration, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            Configuration = configuration;
            _env = env;
            _serviceProvider = serviceProvider;
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ISuccessService, SuccessService>();
            services.AddSingleton<ISuccessDependentService, SuccessDependentService>();
            services.AddSingleton<IOptions<SuccessSettings>>(x => Options.Create(new SuccessSettings()));

            return _serviceProvider;
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