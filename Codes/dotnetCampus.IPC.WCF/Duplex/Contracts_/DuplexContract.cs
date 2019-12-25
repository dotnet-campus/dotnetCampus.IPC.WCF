using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using dotnetCampus.IPC.WCF.Message;

namespace dotnetCampus.IPC.WCF.Duplex
{
    /// <summary>
    /// 服务端协议
    /// </summary>
    internal class DuplexServerContract : IDuplexContract
    {
        /// <summary>
        /// 同步请求处理
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ResponseMessage Request(RequestMessage message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(message.Id))
                {
                    return ResponseMessage.GetResponseMessageFromErrorCode(message,
                        ErrorCodes.MessageIdNullOrWhiteSpace);
                }

                //获取服务对应的消息处理器服务
                if (DuplexServicePool.TryGetMessageHandler(ServiceHost, out var messageHandler))
                {
                    //获取对应此消息Id的请求处理
                    if (messageHandler.TryGetMessageListener(message.Id,
                        out Func<RequestMessage, ResponseMessage> requestFunc))
                    {
                        return requestFunc.Invoke(message);
                    }

                    return ResponseMessage.GetResponseMessageFromErrorCode(message,
                        ErrorCodes.GetRequestMessageListenerFailed);
                }

                return ResponseMessage.GetResponseMessageFromErrorCode(message, ErrorCodes.GetMessageHandlerFailed);
            }
            catch (Exception e)
            {
                return ResponseMessage.ExceptionResponseMessage(message, e, false);
            }
        }

        /// <summary>
        /// 异步请求处理
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> RequestAsync(RequestMessage message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(message.Id))
                {
                    return ResponseMessage.GetResponseMessageFromErrorCode(message,
                        ErrorCodes.MessageIdNullOrWhiteSpace);
                }

                //获取服务对应的消息处理器服务
                if (DuplexServicePool.TryGetMessageHandler(ServiceHost, out var messageHandler))
                {
                    //获取对应此消息Id的请求处理
                    if (messageHandler.TryGetMessageListener(message.Id,
                        out Func<RequestMessage, Task<ResponseMessage>> requestAsyncFunc))
                    {
                        return await requestAsyncFunc.Invoke(message);
                    }

                    return ResponseMessage.GetResponseMessageFromErrorCode(message,
                        ErrorCodes.GetRequestMessageListenerFailed);
                }

                return ResponseMessage.GetResponseMessageFromErrorCode(message, ErrorCodes.GetMessageHandlerFailed);
            }
            catch (Exception e)
            {
                return ResponseMessage.ExceptionResponseMessage(message, e, false);
            }
        }

        /// <summary>
        /// 通知处理
        /// </summary>
        /// <param name="message"></param>
        public void Notify(NotifyMessage message)
        {
            try
            {
                //获取服务对应的消息处理器服务
                if (DuplexServicePool.TryGetMessageHandler(ServiceHost, out var messageHandler))
                {
                    //获取对应此消息Id的通知处理
                    if (messageHandler.TryGetMessageListener(message.Id,
                        out Action<NotifyMessage> notifyAction))
                    {
                        notifyAction.Invoke(message);
                    }
                }
            }
            catch (Exception)
            {
                // 忽略
            }
        }

        private ServiceHostBase ServiceHost => OperationContext.Current.Host;
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
            try
            {
                return Channel.Request(message);
            }
            catch (Exception e)
            {
                return ResponseMessage.ExceptionResponseMessage(message, e, false);
            }
        }

        public async Task<ResponseMessage> RequestAsync(RequestMessage message)
        {
            try
            {
                return await Channel.RequestAsync(message);
            }
            catch (Exception e)
            {
                return ResponseMessage.ExceptionResponseMessage(message, e, false);
            }
        }

        public void Notify(NotifyMessage message)
        {
            try
            {
                Channel.Notify(message);
            }
            catch (Exception)
            {
                // ignored
            }
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
            try
            {
                if (string.IsNullOrWhiteSpace(message.Id))
                {
                    return ResponseMessage.GetResponseMessageFromErrorCode(message,
                        ErrorCodes.MessageIdNullOrWhiteSpace);
                }

                //获取服务对应的消息处理器服务
                if (DuplexServicePool.TryGetMessageHandler(this, out var messageHandler))
                {
                    //获取对应此消息Id的请求处理
                    if (messageHandler.TryGetMessageListener(message.Id,
                        out Func<RequestMessage, ResponseMessage> requestFunc))
                    {
                        return requestFunc.Invoke(message);
                    }

                    return ResponseMessage.GetResponseMessageFromErrorCode(message,
                        ErrorCodes.GetRequestMessageListenerFailed);
                }

                return ResponseMessage.GetResponseMessageFromErrorCode(message, ErrorCodes.GetMessageHandlerFailed);
            }
            catch (Exception e)
            {
                return ResponseMessage.ExceptionResponseMessage(message, e, false);
            }
        }

        /// <summary>
        /// 向客户端异步发送请求(回调)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> CallbackRequestAsync(RequestMessage message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(message.Id))
                {
                    return ResponseMessage.GetResponseMessageFromErrorCode(message,
                        ErrorCodes.MessageIdNullOrWhiteSpace);
                }

                //获取服务对应的消息处理器服务
                if (DuplexServicePool.TryGetMessageHandler(this, out var messageHandler))
                {
                    //获取对应此消息Id的请求处理
                    if (messageHandler.TryGetMessageListener(message.Id,
                        out Func<RequestMessage, Task<ResponseMessage>> requestAsyncFunc))
                    {
                        return await requestAsyncFunc.Invoke(message);
                    }

                    return ResponseMessage.GetResponseMessageFromErrorCode(message,
                        ErrorCodes.GetRequestMessageListenerFailed);
                }

                return ResponseMessage.GetResponseMessageFromErrorCode(message, ErrorCodes.GetMessageHandlerFailed);
            }
            catch (Exception e)
            {
                return ResponseMessage.ExceptionResponseMessage(message, e, false);
            }
        }

        /// <summary>
        /// 向客户端发送通知的回调
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public void CallbackNotify(NotifyMessage message)
        {
            try
            {
                //获取服务对应的消息处理器服务
                if (DuplexServicePool.TryGetMessageHandler(this, out var messageHandler))
                {
                    //获取对应此消息Id的通知处理
                    if (messageHandler.TryGetMessageListener(message.Id,
                        out Action<NotifyMessage> notifyAction))
                    {
                        notifyAction.Invoke(message);
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}