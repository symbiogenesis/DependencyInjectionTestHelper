using DependencyInjectionTestHelper.Tests.Startups;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace DependencyInjectionTestHelper.Tests
{
    public class SuccessTests
    {
        private readonly DependencyInjectionTestHelper _helper;

        public SuccessTests()
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder(null).UseStartup<SuccessStartup>();

            _helper = new DependencyInjectionTestHelper(webHostBuilder);
        }

        [Fact]
        public void TryToResolveAllServices_Succeeds()
        {
            _helper.TryToResolveAllServices();
        }

        [Fact]
        public void TryToResolveAllOptions_Succeeds()
        {
            _helper.TryToResolveAllOptions();
        }
    }
}
