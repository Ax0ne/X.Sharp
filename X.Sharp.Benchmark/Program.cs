using System.Data;
using BenchmarkDotNet.Running;
using static X.Sharp.Benchmark.TestDataConvert;

namespace X.Sharp.Benchmark
{
    internal class Program
    {
        // dotnet run -p X.Sharp.Benchmark.csproj -c release
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<TestDataConvert>();
            //var users = new List<TestUser>(11);
            //for (var i = 0; i < 11; i++)
            //{
            //    users.Add(new TestUser
            //    {
            //        Age = i + 1,
            //        CreateTime = DateTime.Now,
            //        IsEnable = i % 2 == 0,
            //        Name = "名称：" + i
            //    });
            //}
            //var dt = users.ToDataTable2();
            //foreach (DataRow row in dt.Rows)
            //{
            //    var age = row["Age"];
            //    var ctime = row["CreateTime"];
            //    var isEnable = row["IsEnable"];
            //    var name = row["Name"];
            //}
            Console.ReadLine();
        }

    }
}