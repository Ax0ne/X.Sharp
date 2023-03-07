using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Sharp.Redis
{
    public interface ISerializer
    {
        /// <summary>
        /// 序列化指定对象
        /// </summary>
        /// <param name="item">被序列化的对象</param>
        /// <returns>序列化后的数据</returns>
        byte[] Serialize<T>(T? item);

        /// <summary>
        /// 反序列化为指定对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="serializedObject">已序列化数据</param>
        /// <returns>指定对象类型</returns>
        T Deserialize<T>(byte[] serializedObject);
    }
}
