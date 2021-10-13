using System;
using DependencyInjectionTestHelper.Tests.Startups;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace DependencyInjectionTestHelper.Tests
{
    public class FailTests
    {
        private readonly DependencyInjectionTestHelper _helper;

        public FailTests()
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder(null).UseStartup<FailStartup>();

            _helper = new DependencyInjectionTestHelper(webHostBuilder);
        }

        [Fact]
        public void TryToResolveAllServices_Fails()
        {
            Assert.Throws<InvalidOperationException>(() => _helper.TryToResolveAllServices());
        }
    }
}
