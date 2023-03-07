namespace X.Sharp.Web.Example
{
    public interface IFoo
    {
        void Print(string msg);
    }

    [Inject(typeof(IFoo))]
    public class Foo : IFoo
    {
        readonly ILogger<IFoo> _log;

        public Foo(ILogger<IFoo> log)
        {
            _log = log;
        }

        public void Print(string msg)
        {
            _log.LogInformation($"【Information】{msg}");
        }
    }
}