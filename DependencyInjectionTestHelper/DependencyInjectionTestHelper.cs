using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DependencyInjectionTestHelper
{
    public static class DependencyInjectionTestHelper
    {
        public static void TryToResolveAllServices(IServiceCollection serviceCollection)
        {
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var descriptors = serviceCollection.Where(d => !d.ServiceType.FullName.StartsWith(nameof(Microsoft)));

            foreach (var descriptor in descriptors)
            {
                var service = serviceProvider.GetRequiredService(descriptor.ServiceType);

                if (service == null)
                    throw new InvalidOperationException($"The service {descriptor.ImplementationType?.Name} was not resolved");

                CheckThatReadonlyFieldsAreInitialized(service, descriptor.ImplementationType);
            }
        }

        private static void CheckThatReadonlyFieldsAreInitialized(object service, Type type)
        {
            if (type == null)
                return;

            var readonlyFields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(f => f.IsInitOnly);

            foreach(var field in readonlyFields)
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

        public static void TryToResolveAllOptions(IServiceCollection serviceCollection)
        {
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var settingsTypes = GetSettingsTypes(serviceCollection);

            var failed = false;

            foreach (var settingsType in settingsTypes)
            {
                var optionsType = typeof(IOptions<>).MakeGenericType(new Type[] { settingsType });

                var options = serviceProvider.GetRequiredService(optionsType);

                if (options == null)
                    failed = true;

                var value = optionsType.GetProperty("Value").GetValue(options);

                if (value == null)
                    failed = true;

                if (failed)
                    throw new InvalidOperationException($"The setting {settingsType.GetGenericArguments()[0].Name} was not resolved");
            }
        }

        private static IEnumerable<Type> GetSettingsTypes(IServiceCollection serviceCollection)
        {
            foreach (var service in serviceCollection)
            {
                var serviceType = service.ServiceType;

                if (!serviceType.IsGenericType)
                    continue;

                if (!serviceType.Name.StartsWith("IOptions"))
                    continue;

                if (IsAssignableFromGenericType(serviceType, typeof(IConfigureOptions<>)))
                    continue;

                var settingsType = service.ServiceType.GetGenericArguments()[0];

                if (settingsType.FullName == null)
                    continue;

                yield return settingsType;
            }
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
