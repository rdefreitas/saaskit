﻿using Microsoft.AspNetCore.Http;
using SaasKit.Multitenancy;
using SaasKit.Multitenancy.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
	using Extensions;
	using System.Reflection;

	public static class MultitenancyServiceCollectionExtensions
    {
        public static IServiceCollection AddMultitenancy<TTenant, TResolver>(this IServiceCollection services) 
            where TResolver : class, ITenantResolver<TTenant>
            where TTenant : class
        {
            Ensure.Argument.NotNull(services, nameof(services));

            services.AddScoped<ITenantResolver<TTenant>, TResolver>();

	    services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

	    // Make Tenant and TenantContext injectable
	    services.AddScoped(prov => 
                prov.GetService<IHttpContextAccessor>()?.HttpContext?.GetTenant<TTenant>());

            services.AddScoped(prov =>
                prov.GetService<IHttpContextAccessor>()?.HttpContext?.GetTenantContext<TTenant>());

            // Ensure caching is available for caching resolvers
	        var resolverType = typeof(TResolver);
	        if (typeof(MemoryCacheTenantResolver<TTenant>).IsAssignableFrom(resolverType))
			{
				services.AddMemoryCache();
			}

            return services;
        }
    }
}
