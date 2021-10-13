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