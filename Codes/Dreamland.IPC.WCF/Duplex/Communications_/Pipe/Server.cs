using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Threading.Tasks;
using Dreamland.IPC.WCF.Message;

namespace Dreamland.IPC.WCF.Duplex.Pipe
{
    /// <summary>
    /// 服务端
    /// </summary>
    [ServiceContract]
    public class Server : IDisposable
    {
        private readonly ServiceHost _service;

        private readonly ConcurrentDictionary<string, IDuplexCallbackContract> _callbackContracts = new ConcurrentDictionary<string, IDuplexCallbackContract>();

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="address"></param>
        public Server(Uri address)
        {
            _service = new ServiceHost(typeof(DuplexServerContract), address);
            _service.AddServiceEndpoint(typeof(IDuplexContract), new NetNamedPipeBinding(), address);

            //监听客户端初始化事件
            ServerMessageHandler.TryAddMessageListener("Initialize", ClientInitialize);
            //在服务池中：注册此服务对应的消息处理
            DuplexServicePool.AddOrUpdateServiceHost(_service, ServerMessageHandler);
        }

        private ResponseMessage ClientInitialize(RequestMessage message)
        {
            var channel = OperationContext.Current.GetCallbackChannel<IDuplexCallbackContract>();
            _callbackContracts.AddOrUpdate(message.Data.ToString(), channel, (s, contract) => channel);
            return ResponseMessage.SuccessfulResponseMessage(message);
        }

        /// <summary>
        /// 消息处理器服务
        /// 通过在此处理器服务中注册消息对应的委托，可以针对消息进行处理
        /// </summary>
        public IMessageHandler ServerMessageHandler { get; } = new MessageHandler();

        /// <summary>
        /// 开启服务
        /// </summary>
        public void Open()
        {
            _service.Open();
        }


        #region 调用客户端方法

        /// <summary>
        /// 向客户端发送请求
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ResponseMessage Request(RequestMessage message)
        {
            try
            {
                if (!_callbackContracts.TryGetValue(message.Destination, out var callbackContract))
                {
                    return ResponseMessage.GetResponseMessageFromErrorCode(message, ErrorCodes.FindClientFailed);
                }
                
                return callbackContract.CallbackRequest(message);
            }
            catch (Exception e)
            {
                return ResponseMessage.ExceptionResponseMessage(message, e);
            }
        }

        /// <summary>
        /// 向客户端发送请求
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> RequestAsync(RequestMessage message)
        {
            try
            {
                if (!_callbackContracts.TryGetValue(message.Destination, out var callbackContract))
                {
                    return ResponseMessage.GetResponseMessageFromErrorCode(message, ErrorCodes.FindClientFailed);
                }

                return await callbackContract.CallbackRequestAsync(message).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                return ResponseMessage.ExceptionResponseMessage(message, e);
            }
        }

        /// <summary>
        /// 向客户端发送通知
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [OperationContract(IsOneWay = true)]
        public void Notify(NotifyMessage message)
        {
            try
            {
                if (_callbackContracts.TryGetValue(message.Destination, out var callbackContract))
                {
                    callbackContract.CallbackNotify(message);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        #endregion

        /// <summary>
        /// 清理资源
        /// </summary>
        public void Dispose()
        {
            ((IDisposable) _service)?.Dispose();
        }
    }
}
