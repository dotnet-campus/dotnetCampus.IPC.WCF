using System;

namespace dotnetCampus.IPC.WCF
{
    /// <summary>
    /// 消息处理类型
    /// </summary>
    public enum MessageHandlerType
    {
        /// <summary>
        /// Request
        /// </summary>
        Request,

        /// <summary>
        /// RequestAsync
        /// </summary>
        RequestAsync,

        /// <summary>
        /// Notify
        /// </summary>
        Notify
    }

    /// <summary>
    /// 消息处理
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MessageHandlerAttribute : Attribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="type"></param>
        public MessageHandlerAttribute(string messageId, MessageHandlerType type)
        {
            MessageId = messageId;
            Type = type;
        }

        /// <summary>
        /// 消息Id
        /// </summary>
        public string MessageId { get; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageHandlerType Type { get; }
    }
}