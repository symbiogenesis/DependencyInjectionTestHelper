namespace DependencyInjectionTestHelper;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

public class DependencyInjectionTestHelper
{
    private readonly IServiceCollection serviceCollection;
    private readonly IServiceProvider serviceProvider;

    public DependencyInjectionTestHelper(IWebHostBuilder webHostBuilder)
    {
        IServiceCollection? serviceCollection = null;

        webHostBuilder.ConfigureServices(sc => serviceCollection = sc).Build();

        if (serviceCollection == null)
            throw new InvalidOperationException("Service Collection cannot be null");

        this.serviceCollection = serviceCollection;

        var startup = serviceCollection.BuildServiceProvider().GetRequiredService<IStartup>();

        if (startup == null)
            throw new InvalidOperationException("Startup class cannot be resolved.");

        this.serviceProvider = startup.ConfigureServices(this.serviceCollection);

        CheckThatReadonlyMembersAreInitialized(startup, startup.GetType());
    }

    public void TryToResolveAllServices()
    {
        foreach (var descriptor in serviceCollection.Where(IsNotFromSystemAssembly))
        {
            if (descriptor == null)
                continue;

            if (descriptor.ServiceType == null)
                throw new InvalidOperationException("Service Type missing");

            var serviceType = descriptor.ServiceType;

            object service;

            if (descriptor.Lifetime == ServiceLifetime.Singleton)
            {
                service = serviceProvider.GetRequiredService(serviceType);
            }
            else
            {
                using var scope = serviceProvider.CreateScope();
                service = scope.ServiceProvider.GetRequiredService(serviceType);
            }

            if (service == null)
                throw new InvalidOperationException($"The service {serviceType.Name} was not resolved");

            CheckThatReadonlyMembersAreInitialized(service, serviceType);
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

    private static void CheckThatReadonlyMembersAreInitialized(object service, Type type)
    {
        if (type == null)
            return;

        foreach (var member in GetReadonlyMembers(type))
        {
            bool failed;

            var memberType = "unknown member";

            try
            {
                switch (member)
                {
                    case FieldInfo field:
                        var fieldValue = field.GetValue(service);
                        failed = fieldValue == null;
                        memberType = nameof(field);
                        break;
                    case PropertyInfo property:
                        var propertyValue = property.GetValue(service);
                        failed = propertyValue == null;
                        memberType = nameof(property);
                        break;
                    default:
                        failed = true;
                        break;
                }
            }
            catch
            {
                failed = true;
            }

            if (failed)
            {
                var errorMessage = $"The {memberType} {member.Name} for service {type.Name} was not resolved";
                throw new InvalidOperationException(errorMessage);
            }
        }
    }

    private static IEnumerable<MemberInfo> GetReadonlyMembers(Type type)
    {
        var members = new List<MemberInfo>();

        var readonlyFields = type.GetFields().Where(f => f.IsInitOnly);

        members.AddRange(readonlyFields);

        foreach (var property in type.GetProperties())
        {
            var accessors = property.GetAccessors();
            if (accessors.Length < 2)
            {
                members.Add(property);
            }
        }

        return members;
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
