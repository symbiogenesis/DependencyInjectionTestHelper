# DependencyInjectionTestHelper
Dependency Injection Test Helper for .NET.

Ever see errors like this?

>InvalidOperationException: Unable to resolve service for type

Convert those pesky DI runtime errors into simple test failures, so that you can catch them before they are released into production. 

**Note**: This should work for the vast majority of ASP.NET Core projects, where dependencies are all simply registered in the Startup.cs file. But it won't work easily if you are doing anything fancier.

Example usage:

```csharp
    public class DependencyInjectionTests
    {
        private readonly IServiceCollection serviceCollection;

        public DependencyInjectionTests()
        {
            var config = GetConfiguration();

            serviceCollection = new ServiceCollection();

            var startup = new Startup(config);

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
```
