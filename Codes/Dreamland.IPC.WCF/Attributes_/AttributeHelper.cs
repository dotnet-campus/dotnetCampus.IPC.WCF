using System;
using System.Linq;
using System.Reflection;

namespace Dreamland.IPC.WCF
{
    /// <summary>
    /// 特性帮助类
    /// </summary>
    internal static class AttributeHelper
    {
        /// <summary>
        /// 获取对象上的指定特性
        /// </summary>
        /// <param name="classType"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static TResult GetCustomAttribute<TResult>(this Type classType, string fieldName) where TResult : Attribute
        {
            foreach (var fieldInfo in classType.GetFields())
            {
                if (fieldInfo.Name == fieldName)
                {
                    return fieldInfo.GetCustomAttribute(typeof(TResult), false) as TResult;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取枚举值上的指定特性
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static TResult GetCustomAttribute<TResult>(this Enum enumValue) where TResult : Attribute
        {
            var type = enumValue.GetType();
            var fieldName = Enum.GetName(type, enumValue);
            var attributes = type.GetField(fieldName).GetCustomAttributes(false);
            return attributes.FirstOrDefault(p => p.GetType() == typeof(TResult)) as TResult;
        }
    }
}
