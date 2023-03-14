// Copyright (c) Ax0ne.  All Rights Reserved

namespace X.Sharp
{
    public abstract class Fxx
    {
        public void Foo()
        {

        }
        public abstract void Bar();
        public virtual void Haz()
        {
            throw new NotImplementedException();
        }
    }
    public class Axx : Fxx
    {
        public override void Bar()
        {
            throw new NotImplementedException();
        }
        public override void Haz()
        {
            base.Haz();
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