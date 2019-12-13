using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Dreamland.IPC.WCF.Message;

namespace Dreamland.IPC.WCF.Duplex
{
    /// <summary>
    /// 服务端协议
    /// </summary>
    internal class DuplexServerContract : IDuplexContract
    {
        private ServiceHostBase ServiceHost => OperationContext.Current.Host;

        public ResponseMessage Request(RequestMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.Id))
            {
                return ResponseMessage.GetResponseMessageFromErrorCode(message, ErrorCodes.MessageIdNullOrWhiteSpace);
            }

            if (DuplexServicePool.TryGetMessageHandler(ServiceHost, out var messageHandler))
            {
                if (messageHandler.TryGetMessageListener(message.Id,
                    out Func<RequestMessage, ResponseMessage> requestFunc))
                {
                    return requestFunc.Invoke(message);
                }

                return ResponseMessage.GetResponseMessageFromErrorCode(message, ErrorCodes.GetRequestMessageListenerFailed);
            }

            return ResponseMessage.GetResponseMessageFromErrorCode(message, ErrorCodes.GetMessageHandlerFailed);
        }

        public async Task<ResponseMessage> RequestAsync(RequestMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.Id))
            {
                return ResponseMessage.GetResponseMessageFromErrorCode(message, ErrorCodes.MessageIdNullOrWhiteSpace);
            }

            if (DuplexServicePool.TryGetMessageHandler(ServiceHost, out var messageHandler))
            {
                if (messageHandler.TryGetMessageListener(message.Id,
                    out Func<RequestMessage, Task<ResponseMessage>> requestAsyncFunc))
                {
                    return await requestAsyncFunc.Invoke(message);
                }

                return ResponseMessage.GetResponseMessageFromErrorCode(message, ErrorCodes.GetRequestMessageListenerFailed);
            }

            return ResponseMessage.GetResponseMessageFromErrorCode(message, ErrorCodes.GetMessageHandlerFailed);
        }

        public void Notify(NotifyMessage message)
        {
            if (DuplexServicePool.TryGetMessageHandler(ServiceHost, out var messageHandler))
            {
                if (messageHandler.TryGetMessageListener(message.Id,
                    out Action<NotifyMessage> notifyAction))
                {
                    notifyAction.Invoke(message);
                }
            }
        }
    }

    /// <summary>
    /// 客户端协议
    /// </summary>
    internal class DuplexClientContract : DuplexClientBase<IDuplexContract>, IDuplexContract
    {
        public DuplexClientContract(InstanceContext callbackInstance, Binding binding,
            EndpointAddress serverAddress) : base(callbackInstance, binding, serverAddress)
        {

        }

        public ResponseMessage Request(RequestMessage message)
        {
            return Channel.Request(message);
        }

        public async Task<ResponseMessage> RequestAsync(RequestMessage message)
        {
            return await Channel.RequestAsync(message);
        }

        public void Notify(NotifyMessage message)
        {
            Channel.Notify(message);
        }
    }

    internal class DuplexCallbackContract : IDuplexCallbackContract
    {
        /// <summary>
        /// 向客户端异步发送请求(回调)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ResponseMessage CallbackRequest(RequestMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.Id))
            {
                return ResponseMessage.GetResponseMessageFromErrorCode(message, ErrorCodes.MessageIdNullOrWhiteSpace);
            }

            if (DuplexServicePool.TryGetMessageHandler(this, out var messageHandler))
            {
                if (messageHandler.TryGetMessageListener(message.Id,
                    out Func<RequestMessage, ResponseMessage> requestFunc))
                {
                    return requestFunc.Invoke(message);
                }

                return ResponseMessage.GetResponseMessageFromErrorCode(message, ErrorCodes.GetRequestMessageListenerFailed);
            }

            return ResponseMessage.GetResponseMessageFromErrorCode(message, ErrorCodes.GetMessageHandlerFailed);
        }

        /// <summary>
        /// 向客户端异步发送请求(回调)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> CallbackRequestAsync(RequestMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.Id))
            {
                return ResponseMessage.GetResponseMessageFromErrorCode(message, ErrorCodes.MessageIdNullOrWhiteSpace);
            }

            if (DuplexServicePool.TryGetMessageHandler(this, out var messageHandler))
            {
                if (messageHandler.TryGetMessageListener(message.Id,
                    out Func<RequestMessage, Task<ResponseMessage>> requestAsyncFunc))
                {
                    return await requestAsyncFunc.Invoke(message);
                }

                return ResponseMessage.GetResponseMessageFromErrorCode(message, ErrorCodes.GetRequestMessageListenerFailed);
            }

            return ResponseMessage.GetResponseMessageFromErrorCode(message, ErrorCodes.GetMessageHandlerFailed);
        }

        /// <summary>
        /// 向客户端发送通知的回调
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public void CallbackNotify(NotifyMessage message)
        {
            if (DuplexServicePool.TryGetMessageHandler(this, out var messageHandler))
            {
                if (messageHandler.TryGetMessageListener(message.Id,
                    out Action<NotifyMessage> notifyAction))
                {
                    notifyAction.Invoke(message);
                }
            }
        }
    }
}
