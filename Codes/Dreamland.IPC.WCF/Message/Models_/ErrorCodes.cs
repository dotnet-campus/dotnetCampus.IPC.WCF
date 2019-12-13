using System;

namespace Dreamland.IPC.WCF.Message
{
    /// <summary>
    /// 错误码
    /// </summary>
    public enum ErrorCodes
    {
        /// <summary>
        /// 成功
        /// </summary>
        [Log("成功")]
        Success = 0,

        /// <summary>
        /// 异常
        /// </summary>
        [Log("异常")]
        Exception = 1,

        /// <summary>
        /// 消息Id异常
        /// </summary>
        [Log("消息Id异常（NullOrWhiteSpace）")]
        MessageIdNullOrWhiteSpace = 10,

        /// <summary>
        /// 客户端连接未初始化
        /// </summary>
        [Log("客户端连接未初始化")]
        ClientUninitialized = 100,

        /// <summary>
        /// 服务端查找对应的消息处理器失败
        /// </summary>
        [Log("服务端查找对应的消息处理器失败")]
        GetMessageHandlerFailed = 200,

        /// <summary>
        /// 服务端消息处理查找消息对应的处理方法失败
        /// </summary>
        [Log("服务端消息处理查找消息对应的处理方法失败")]
        GetRequestMessageListenerFailed = 201,

        /// <summary>
        /// 服务端与客户端发送消息失败，未找到对应的客户端
        /// </summary>
        [Log("服务端与客户端发送消息失败，未找到对应的客户端")]
        FindClientFailed = 202,
    }
}
