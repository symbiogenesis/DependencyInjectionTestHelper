using System;
using DependencyInjectionTestHelper;
using DependencyInjectionTestHelper.Tests.Startups;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace DependencyInjectionTestHelper.Tests
{
    public class FailTests
    {
        private readonly FailStartup startup;
        private readonly IServiceCollection serviceCollection;

        public FailTests()
        {
            var mockConfig = new Mock<IConfiguration>();

            serviceCollection = new ServiceCollection();

            startup = new FailStartup(mockConfig.Object);

            startup.ConfigureServices(serviceCollection);
        }

        [Fact]
        public void TryToResolveAllServices_Fails()
        {
            Assert.Throws<InvalidOperationException>(() => DependencyInjectionTestHelper.TryToResolveAllServices(serviceCollection));
        }

        [Fact]
        public void TryToResolveAllOptions_Fails()
        {
            Assert.Throws<InvalidOperationException>(() => DependencyInjectionTestHelper.TryToResolveAllOptions(serviceCollection));
        }
    }
}
