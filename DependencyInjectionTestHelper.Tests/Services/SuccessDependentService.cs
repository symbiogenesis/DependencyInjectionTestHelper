using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using DependencyInjectionTestHelper.Tests.Interfaces;

namespace DependencyInjectionTestHelper.Tests.Services
{
    public class SuccessDependentService : ISuccessDependentService
    {
        private readonly ISuccessService successService;
        
        public SuccessDependentService(ISuccessService successService)
        {
            this.successService = successService;
        }
    }
}