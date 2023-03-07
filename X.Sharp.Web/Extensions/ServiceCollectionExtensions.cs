using System.Reflection;

namespace X.Sharp.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加注入服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddInjectServices<T>(this IServiceCollection services)
        {
            var serviceTypes = typeof(T).Assembly.GetTypes()
                .Where(p => p is { IsClass: true, IsAbstract: false });
            foreach (var serviceType in serviceTypes)
            {
                var attribute = serviceType.GetCustomAttribute<InjectAttribute>();
                if (attribute == null) continue;
                if (attribute.BaseType != null)
                {
                    services.Add(new ServiceDescriptor(attribute.BaseType, serviceType,
                        attribute.ServiceLifetime));
                }
                else
                {
                    services.Add(new ServiceDescriptor(serviceType, serviceType, attribute.ServiceLifetime));
                }
            }

            return services;
        }
    }
}
