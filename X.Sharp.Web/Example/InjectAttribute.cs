// Copyright (c) Ax0ne.  All Rights Reserved

namespace X.Sharp.Web
{
    /// <summary>
    /// 依赖注入属性标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class InjectAttribute : Attribute
    {
        /// <summary>
        /// 继承的对象类型
        /// </summary>
        public Type? BaseType { get; set; }

        /// <summary>
        /// 依赖注入的生命周期
        /// </summary>
        public ServiceLifetime ServiceLifetime { get; set; }

        /// <summary>
        /// 实例化注入属性标记
        /// </summary>
        /// <param name="baseType">继承的对象类型</param>
        /// <param name="serviceLifetime">依赖注入的生命周期</param>
        public InjectAttribute(Type? baseType, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            BaseType = baseType;
            ServiceLifetime = serviceLifetime;
        }
    }
}