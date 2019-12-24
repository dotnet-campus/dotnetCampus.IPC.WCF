using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Dreamland.IPC.WCF.Message
{
    /// <summary>
    /// 消息处理者
    /// </summary>
    internal class MessageHandler : IMessageHandler
    {
        /// <summary>
        /// 增加请求消息监听
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="requestFunc"></param>
        public bool TryAddMessageListener(string messageId, Func<RequestMessage, ResponseMessage> requestFunc)
        {
            return _requestFuncDictionary.TryAdd(messageId, requestFunc);
        }

        /// <summary>
        /// 获取请求消息监听
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="requestFunc"></param>
        public bool TryGetMessageListener(string messageId, out Func<RequestMessage, ResponseMessage> requestFunc)
        {
            return _requestFuncDictionary.TryGetValue(messageId, out requestFunc);
        }

        /// <summary>
        /// 增加请求(异步)消息监听
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="requestAsyncFunc"></param>
        public bool TryAddMessageListener(string messageId,
            Func<RequestMessage, Task<ResponseMessage>> requestAsyncFunc)
        {
            return _requestAsyncFuncDictionary.TryAdd(messageId, requestAsyncFunc);
        }

        /// <summary>
        /// 获取请求(异步)消息监听
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="requestAsyncFunc"></param>
        public bool TryGetMessageListener(string messageId,
            out Func<RequestMessage, Task<ResponseMessage>> requestAsyncFunc)
        {
            return _requestAsyncFuncDictionary.TryGetValue(messageId, out requestAsyncFunc);
        }

        /// <summary>
        /// 增加通知消息监听
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="notifyAction"></param>
        public bool TryAddMessageListener(string messageId, Action<NotifyMessage> notifyAction)
        {
            return _notifyActionDictionary.TryAdd(messageId, notifyAction);
        }

        /// <summary>
        /// 获取通知消息监听
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="notifyAction"></param>
        public bool TryGetMessageListener(string messageId, out Action<NotifyMessage> notifyAction)
        {
            return _notifyActionDictionary.TryGetValue(messageId, out notifyAction);
        }

        /// <summary>
        /// 移除指定消息监听
        /// </summary>
        /// <param name="messageId"></param>
        public bool TryRemoveMessageListener(string messageId)
        {
            if (_requestFuncDictionary.ContainsKey(messageId))
            {
                return _requestFuncDictionary.TryRemove(messageId, out _);
            }

            if (_requestAsyncFuncDictionary.ContainsKey(messageId))
            {
                return _requestAsyncFuncDictionary.TryRemove(messageId, out _);
            }

            if (_notifyActionDictionary.ContainsKey(messageId))
            {
                return _notifyActionDictionary.TryRemove(messageId, out _);
            }

            return false;
        }

        /// <summary>
        /// 移除所有消息监听
        /// </summary>
        public void RemoveAllMessageListener()
        {
            _requestFuncDictionary.Clear();
            _requestAsyncFuncDictionary.Clear();
            _notifyActionDictionary.Clear();
        }

        /// <summary>
        /// 通知消息监听接口字典
        /// </summary>
        private readonly ConcurrentDictionary<string, Action<NotifyMessage>> _notifyActionDictionary =
            new ConcurrentDictionary<string, Action<NotifyMessage>>();

        /// <summary>
        /// 请求(异步)消息监听接口字典
        /// </summary>
        private readonly ConcurrentDictionary<string, Func<RequestMessage, Task<ResponseMessage>>>
            _requestAsyncFuncDictionary =
                new ConcurrentDictionary<string, Func<RequestMessage, Task<ResponseMessage>>>();

        /// <summary>
        /// 请求消息监听接口字典
        /// </summary>
        private readonly ConcurrentDictionary<string, Func<RequestMessage, ResponseMessage>> _requestFuncDictionary =
            new ConcurrentDictionary<string, Func<RequestMessage, ResponseMessage>>();
    }
}