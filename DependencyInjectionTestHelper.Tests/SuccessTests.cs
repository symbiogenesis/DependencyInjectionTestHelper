using System;
using DependencyInjectionTestHelper;
using DependencyInjectionTestHelper.Tests.Startups;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace DependencyInjectionTestHelper.Tests
{
    public class SuccessTests
    {
        private readonly IServiceCollection serviceCollection;

        public SuccessTests()
        {
            var mockConfig = new Mock<IConfiguration>();

            serviceCollection = new ServiceCollection();

            var startup = new SuccessStartup(mockConfig.Object);

            startup.ConfigureServices(serviceCollection);
        }

        [Fact]
        public void TryToResolveAllServices_Succeeds()
        {
            DependencyInjectionTestHelper.TryToResolveAllServices(serviceCollection);
        }

        [Fact]
        public void TryToResolveAllOptions_Succeeds()
        {
            DependencyInjectionTestHelper.TryToResolveAllOptions(serviceCollection);
        }
    }
}
