using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace X.Sharp.Benchmark
{
    public static class DataConvertExtension
    {
        public static DataTable ToDataTable<T>(this List<T> list)
        {
            DataTable dt = new DataTable("tableName");
            foreach (var item in typeof(T).GetProperties())
            {
                dt.Columns.Add(item.Name);
            }

            var func = CreateEntityToRowFunc<T>();

            foreach (var item in list)
            {
                var row = func(item);
                dt.Rows.Add(row);
            }

            return dt;
        }
        public static DataTable ToDataTable2<T>(this List<T> list)
        {
            DataTable dt = new DataTable("tableName");
            foreach (var item in typeof(T).GetProperties())
            {
                dt.Columns.Add(item.Name);
            }
            foreach (var item in list)
            {
                var row = dt.NewRow();
                foreach (var p in typeof(T).GetProperties())
                {
                    row[p.Name] = p.GetValue(item);
                }
                dt.Rows.Add(row);
            }
            return dt;
        }
        //public static List<T> ToList<T>(this DataTable table)
        //{
        //}

        private static Func<T, object[]> CreateEntityToRowFunc<T>()
        {
            var type = typeof(T);
            var parameter = Expression.Parameter(type, "entity");
            var body = Expression.NewArrayInit(
                typeof(object),
                type.GetProperties().Select(p =>
                    Expression.Convert(
                        Expression.Property(parameter, p),
                        typeof(object)
                    )
                )
            );
            return Expression.Lambda<Func<T, object[]>>(body, parameter).Compile();
        }
    }
}