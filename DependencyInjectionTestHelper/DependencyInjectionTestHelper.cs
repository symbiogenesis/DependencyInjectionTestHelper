using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionTestHelper
{
    public class DependencyInjectionTestHelper
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly IServiceProvider _serviceProvider;

        public DependencyInjectionTestHelper(IWebHostBuilder webHostBuilder)
        {
            IServiceCollection? serviceCollection = null;

            webHostBuilder.ConfigureServices(sc => serviceCollection = sc).Build();

            if (serviceCollection == null)
                throw new InvalidOperationException("Service Collection cannot be null");

            _serviceCollection = serviceCollection;

            var startup = _serviceCollection.BuildServiceProvider().GetRequiredService<IStartup>();

            if (startup == null)
                throw new InvalidOperationException("Startup class cannot be resolved.");

            _serviceProvider = startup.ConfigureServices(_serviceCollection);

            CheckThatReadonlyFieldsAreInitialized(startup, startup.GetType());
        }

        public void TryToResolveAllServices()
        {
            foreach (var descriptor in _serviceCollection.Where(IsNotFromSystemAssembly))
            {
                if (descriptor == null)
                    continue;

                if (descriptor.ServiceType == null)
                    throw new InvalidOperationException("Service Type missing");

                object service;

                if (descriptor.Lifetime == ServiceLifetime.Singleton)
                {
                    service = _serviceProvider.GetRequiredService(descriptor.ServiceType);
                }
                else
                {
                    using var scope = _serviceProvider.CreateScope();
                    service = scope.ServiceProvider.GetRequiredService(descriptor.ServiceType);
                }

                if (service == null)
                    throw new InvalidOperationException($"The service {descriptor.ServiceType?.Name} was not resolved");

                CheckThatReadonlyFieldsAreInitialized(service, descriptor.ServiceType);
            }
        }

        private static bool IsNotFromSystemAssembly(ServiceDescriptor d)
        {
            return IsNotFromSystemAssembly(d?.ServiceType);
        }

        private static bool IsNotFromSystemAssembly(Type? type)
        {
            var fullName = type?.FullName;

            if (fullName == null)
                return false;

            if (fullName.StartsWith(nameof(Microsoft)))
                return false;

            if (fullName.StartsWith(nameof(System)))
                return false;

            return true;
        }

        private static void CheckThatReadonlyFieldsAreInitialized(object service, Type type)
        {
            if (type == null)
                return;

            foreach (var field in GetReadonlyFields(type))
            {
                bool failed;

                try
                {
                    var fieldValue = field.GetValue(service);
                    failed = fieldValue == null;
                }
                catch
                {
                    failed = true;
                }

                if (failed)
                {
                    var errorMessage = $"The field {field.Name} for service {type.Name} was not resolved";
                    throw new InvalidOperationException(errorMessage);
                }
            }
        }

        private static IEnumerable<FieldInfo> GetReadonlyFields(Type type)
        {
            return type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(f => f.IsInitOnly);
        }

        public static bool IsAssignableFromGenericType(Type givenType, Type genericType)
        {
            foreach (var it in givenType.GetInterfaces())
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            var baseType = givenType.BaseType;

            if (baseType == null)
                return false;

            return IsAssignableFromGenericType(baseType, genericType);
        }
    }
}