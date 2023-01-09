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

    public DependencyInjectionTestHelper(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
    {
        this.serviceCollection = serviceCollection;
        this.serviceProvider = serviceProvider;
    }

    public DependencyInjectionTestHelper(IWebHostBuilder webHostBuilder)
    {
        IServiceCollection? serviceCollection = null;

        _ = webHostBuilder.ConfigureServices(sc => serviceCollection = sc).Build();

        if (serviceCollection == null)
        {
            throw new InvalidOperationException("Service Collection cannot be null");
        }

        this.serviceCollection = serviceCollection;

        var startup = serviceCollection.BuildServiceProvider().GetRequiredService<IStartup>();

        if (startup == null)
        {
            throw new InvalidOperationException("Startup class cannot be resolved.");
        }

        this.serviceProvider = startup.ConfigureServices(this.serviceCollection);

        CheckThatReadonlyMembersAreInitialized(startup, startup.GetType());
    }

    public void TryToResolveAllServices()
    {
        foreach (var descriptor in this.serviceCollection.Where(IsNotFromSystemAssembly))
        {
            if (descriptor == null)
            {
                continue;
            }

            if (descriptor.ServiceType == null)
            {
                throw new InvalidOperationException("Service Type missing");
            }

            var serviceType = descriptor.ServiceType;

            object service;

            if (descriptor.Lifetime == ServiceLifetime.Singleton)
            {
                service = this.serviceProvider.GetRequiredService(serviceType);
            }
            else
            {
                using var scope = this.serviceProvider.CreateScope();
                service = scope.ServiceProvider.GetRequiredService(serviceType);
            }

            if (service == null)
            {
                throw new InvalidOperationException($"The service {serviceType.Name} was not resolved");
            }

            CheckThatReadonlyMembersAreInitialized(service, serviceType);
        }
    }

    private static bool IsNotFromSystemAssembly(ServiceDescriptor d) => IsNotFromSystemAssembly(d?.ServiceType);

    private static bool IsNotFromSystemAssembly(Type? type)
    {
        var fullName = type?.FullName;

        if (fullName == null)
        {
            return false;
        }

        if (fullName.StartsWith(nameof(Microsoft)))
        {
            return false;
        }

        if (fullName.StartsWith(nameof(System)))
        {
            return false;
        }

        return true;
    }

    private static void CheckThatReadonlyMembersAreInitialized(object service, Type type)
    {
        if (type == null)
        {
            return;
        }

        foreach (var member in GetReadonlyMembers(type))
        {
            string memberType;
            bool failed;

            try
            {
                switch (member)
                {
                    case FieldInfo field:
                        memberType = nameof(field);
                        failed = field.GetValue(service) == null;
                        break;
                    case PropertyInfo property:
                        memberType = nameof(property);
                        failed = property.GetMethod?.IsVirtual != true && property.GetValue(service) == null;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            catch
            {
                memberType = "unknown member";
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

        var readonlyFields = type.GetFields().Where(f => f.IsInitOnly && f.DeclaringType == type);

        members.AddRange(readonlyFields);

        foreach (var property in type.GetProperties().Where(p => p.DeclaringType == type))
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
            {
                return true;
            }
        }

        if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
        {
            return true;
        }

        var baseType = givenType.BaseType;

        if (baseType == null)
        {
            return false;
        }

        return IsAssignableFromGenericType(baseType, genericType);
    }
}
