using System;
using System.Threading.Tasks;

namespace Dreamland.IPC.WCF.Message
{
    /// <summary>
    /// 消息处理者
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// 增加请求消息监听
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="requestFunc"></param>
        bool TryAddMessageListener(string messageId, Func<RequestMessage, ResponseMessage> requestFunc);

        /// <summary>
        /// 获取请求消息监听
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="requestFunc"></param>
        bool TryGetMessageListener(string messageId, out Func<RequestMessage, ResponseMessage> requestFunc);

        /// <summary>
        /// 增加请求(异步)消息监听
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="requestAsyncFunc"></param>
        bool TryAddMessageListener(string messageId, Func<RequestMessage, Task<ResponseMessage>> requestAsyncFunc);

        /// <summary>
        /// 获取请求(异步)消息监听
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="requestAsyncFunc"></param>
        bool TryGetMessageListener(string messageId, out Func<RequestMessage, Task<ResponseMessage>> requestAsyncFunc);

        /// <summary>
        /// 增加通知消息监听
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="notifyAction"></param>
        bool TryAddMessageListener(string messageId, Action<NotifyMessage> notifyAction);

        /// <summary>
        /// 获取通知消息监听
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="notifyAction"></param>
        bool TryGetMessageListener(string messageId, out Action<NotifyMessage> notifyAction);

        /// <summary>
        /// 移除指定消息监听
        /// </summary>
        /// <param name="messageId"></param>
        bool TryRemoveMessageListener(string messageId);

        /// <summary>
        /// 移除所有消息监听
        /// </summary>
        void RemoveAllMessageListener();
    }
}