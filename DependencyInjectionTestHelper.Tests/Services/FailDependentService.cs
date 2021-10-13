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