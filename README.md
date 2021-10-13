# DependencyInjectionTestHelper
Dependency Injection Test Helper for .NET.

Ever see errors like this?

>InvalidOperationException: Unable to resolve service for type

Convert those pesky DI runtime errors into simple test failures, so that you can catch them before they are released into production. 

**Note**: The helper expects that your Startup file inherits from IStartup. Which means you should call UseStartup<> in the Program.cs file, in your CreateWebHostBuilder function.

**Note**: This should work for the vast majority of ASP.NET Core projects, where dependencies are all simply registered in the Startup.cs file upon application startup. But it won't work easily if you are doing anything fancier.

Example usage:

```csharp
    public class DependencyInjectionTests
    {
        private readonly DependencyInjectionTestHelper _helper;

        public DependencyInjectionTests()
        {
            var webHostBuilder = Program.CreateWebHostBuilder(null);

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
```
