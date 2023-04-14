using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace X.Sharp.Benchmark
{
    [MemoryDiagnoser]
    public class TestDataConvert
    {
        [Params(10000,100000)]
        public int Count;

        private List<TestUser> GetUsers()
        {
            var users = new List<TestUser>(Count);
            for (var i = 0; i < Count; i++)
            {
                users.Add(new TestUser
                {
                    Age = i + 1,
                    CreateTime = DateTime.Now,
                    IsEnable = i % 2 == 0,
                    Name = "名称：" + i
                });
            }
            return users;
        }
        [Benchmark]
        public void ConvertTableByExpression()
        {
            var dt = GetUsers().ToDataTable();
        }
        [Benchmark]
        public void ConvertTableByReflect()
        {
            var dt = GetUsers().ToDataTable2();
        }

        public class TestUser
        {
            public int Age { get; set; }
            public string Name { get; set; }
            public bool IsEnable { get; set; }
            public DateTime CreateTime { get; set; }
        }
    }
}
