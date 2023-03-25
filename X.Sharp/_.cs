// Copyright (c) Ax0ne.  All Rights Reserved

using System.Text.Json;

namespace X.Sharp
{
    public class CacheHelper
    {
        private readonly string _cacheKey;
        private static readonly Lazy<Dictionary<string, byte[]>> Cache = new(() => new Dictionary<string, byte[]>());

        public CacheHelper(string cacheKey)
        {
            _cacheKey = cacheKey;
        }

        public static T? GetCache<T>(string key)
        {
            Cache.Value.TryGetValue(key, out var value);
            return JsonSerializer.Deserialize<T>(value);
        }

        public static bool SetCache<T>(string key, T value)
        {
            return Cache.Value.TryAdd(key, JsonSerializer.SerializeToUtf8Bytes(value));
        }
    }

    public enum ServiceLifetime
    {
        Transient,
        Scoped,
        Singleton
    }

    public class ServiceDescriptor
    {
        public Type? ServiceType { get; set; }
        public ServiceLifetime ServiceLifetime { get; set; }
        public ServiceDescriptor(Type serviceType, ServiceLifetime lifetime)
        {
            ServiceType = serviceType;
            ServiceLifetime = lifetime;
        }
    }

    public interface IWebApplication
    {
    }

    public class WebApplication : IWebApplication
    {
        public WebApplication(List<ServiceDescriptor> services)
        {
            Services = services;
        }

        public List<ServiceDescriptor> Services { get; }
    }

    public interface IWebApplicationBuilder
    {
        void Add(ServiceDescriptor serviceDescriptor);
        IWebApplication Builder();
    }

    public class WebApplicationBuilder : IWebApplicationBuilder
    {
        private readonly List<ServiceDescriptor> _services = new();

        public void Add(ServiceDescriptor serviceDescriptor)
        {
            _services.Add(serviceDescriptor);
        }

        public IWebApplication Builder()
        {
            return new WebApplication(_services);
        }
    }

    public static class WebApplicationBuilderExtensions
    {
        public static void AddTransient<T>(this IWebApplicationBuilder builder)
        {
            builder.Add(new ServiceDescriptor(typeof(T), ServiceLifetime.Transient));
        }

        public static void AddScoped<T>(this IWebApplicationBuilder builder)
        {
            builder.Add(new ServiceDescriptor(typeof(T), ServiceLifetime.Scoped));
        }

        public static void AddSingleton<T>(this IWebApplicationBuilder builder)
        {
            builder.Add(new ServiceDescriptor(typeof(T), ServiceLifetime.Singleton));
        }
    }

    public class TestClass
    {
        public void Run()
        {
            IWebApplicationBuilder builder = new WebApplicationBuilder();
            builder.AddScoped<TestClass>();
            builder.AddSingleton<TestClass>();
            builder.AddTransient<TestClass>();
            builder.Builder();
        }
    }



    public interface ILogger
    {
        void Info(string message);
    }

    public interface ILoggerProvider
    {
        public ILogger CreateLogger();
    }

    public class ConsoleLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger()
        {
            return new ConsoleLogger();
        }

        public class ConsoleLogger : ILogger
        {
            public void Info(string message)
            {
                Console.WriteLine($"[ConsoleLogger] {message}");
            }
        }
    }

    public class FileLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger()
        {
            return new FileLogger();
        }

        public class FileLogger : ILogger
        {
            public void Info(string message)
            {
                Console.WriteLine($"[FileLogger] {message}");
            }
        }
    }

    public class LoggerFactory
    {
        private readonly IEnumerable<ILoggerProvider> _providers;

        public LoggerFactory(IEnumerable<ILoggerProvider> providers)
        {
            _providers = providers;
        }

        public ILogger Create()
        {
            return new LoggerCollection(_providers.Select(s => s.CreateLogger()));
        }

        class LoggerCollection : ILogger
        {
            private readonly IEnumerable<ILogger> _loggers;

            public LoggerCollection(IEnumerable<ILogger> loggers)
            {
                _loggers = loggers;
            }

            public void Info(string message)
            {
                foreach (var logger in _loggers)
                {
                    logger.Info(message);
                }
            }
        }
    }

    public class TestClass2
    {
        public void Run()
        {
            LoggerFactory loggerFactory = new LoggerFactory(new List<ILoggerProvider>
            {
                new ConsoleLoggerProvider() ,
                new FileLoggerProvider()
            });
            var log = loggerFactory.Create();
            log.Info("test log msg");
        }
    }
}