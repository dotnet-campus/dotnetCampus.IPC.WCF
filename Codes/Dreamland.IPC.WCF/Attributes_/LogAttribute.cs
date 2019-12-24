using System;

namespace Dreamland.IPC.WCF
{
    /// <summary>
    /// 日志标记特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class LogAttribute : Attribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        public LogAttribute()
        {
        }

        /// <summary>
        /// 构造
        /// </summary>
        public LogAttribute(string description)
        {
            Description = description ?? string.Empty;
        }

        /// <summary>
        ///   获取或设置字段或属性描述。
        /// </summary>
        public string Description { get; set; }
    }
}