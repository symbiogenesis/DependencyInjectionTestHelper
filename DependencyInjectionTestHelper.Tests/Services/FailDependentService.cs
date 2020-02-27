using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using DependencyInjectionTestHelper.Tests.Interfaces;

namespace DependencyInjectionTestHelper.Tests.Services
{
    public class FailDependentService : IFailDependentService
    {
        private readonly IFailService failService;
        
        public FailDependentService(IFailService failService)
        {
            this.failService = failService;
        }
    }
}