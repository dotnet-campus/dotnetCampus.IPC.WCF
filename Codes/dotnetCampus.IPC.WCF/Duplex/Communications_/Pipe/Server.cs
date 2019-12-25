using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using dotnetCampus.IPC.WCF.Message;

namespace dotnetCampus.IPC.WCF.Duplex.Pipe
{
    /// <summary>
    /// 服务端
    /// </summary>
    [ServiceContract]
    public class Server : IDisposable
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="address"></param>
        public Server(Uri address) : this(address, Guid.NewGuid().ToString())
        {
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="address"></param>
        /// <param name="serverId"></param>
        public Server(Uri address, string serverId)
        {
            ServerId = serverId;
            _service = new ServiceHost(typeof(DuplexServerContract), address);
            _service.AddServiceEndpoint(typeof(IDuplexContract), new NetNamedPipeBinding(), address);

            //监听客户端初始化事件
            ServerMessageHandler.TryAddMessageListener("@@Inner_Binding_Server_From_Modification", ClientBindingServer);
            //在服务池中：注册此服务对应的消息处理
            DuplexServicePool.AddOrUpdateServiceHost(_service, ServerMessageHandler);
        }

        /// <summary>
        /// 服务端Id
        /// </summary>
        public string ServerId { get; }

        /// <summary>
        /// 消息处理器服务
        /// 通过在此处理器服务中注册消息对应的委托，可以针对消息进行处理
        /// </summary>
        public IMessageHandler ServerMessageHandler { get; } = new MessageHandler();

        /// <summary>
        /// 当前的服务端连接状态
        /// </summary>
        public CommunicationState State => _service.State;

        /// <summary>
        /// 获取连接到此服务的客户端Id
        /// </summary>
        public List<string> ClientIdList => _callbackContracts.Keys.ToList();

        /// <summary>
        /// 初始化通信
        /// </summary>
        /// <param name="timeout">指定在超时前必须完成打开操作的时间（毫秒）</param>
        public void Initialize(double timeout = 10000)
        {
            if (State != CommunicationState.Created)
            {
                return;
            }

            //启动服务
            _service.Open(TimeSpan.FromMilliseconds(timeout));
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public void Dispose()
        {
            _service.Abort();
            ((IDisposable) _service)?.Dispose();
        }

        private readonly ConcurrentDictionary<string, IDuplexCallbackContract> _callbackContracts =
            new ConcurrentDictionary<string, IDuplexCallbackContract>();

        private readonly ServiceHost _service;

        private ResponseMessage ClientBindingServer(RequestMessage message)
        {
            var channel = OperationContext.Current.GetCallbackChannel<IDuplexCallbackContract>();
            _callbackContracts.AddOrUpdate(message.Data, channel, (s, contract) => channel);
            return ResponseMessage.SuccessfulResponseMessage(message);
        }

        #region 调用客户端方法

        /// <summary>
        /// 向客户端发送请求，必须在<see cref="RequestMessage"/>的"Destination"属性中指定要发送的客户端目标
        /// </summary>
        /// <param name="message">消息请求</param>
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
                return ResponseMessage.ExceptionResponseMessage(message, e, false);
            }
        }

        /// <summary>
        /// 向客户端发送请求，必须在<see cref="RequestMessage"/>的"Destination"属性中指定要发送的客户端目标
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
                return ResponseMessage.ExceptionResponseMessage(message, e, false);
            }
        }

        /// <summary>
        /// 向客户端发送通知，必须在<see cref="RequestMessage"/>的"Destination"属性中指定要发送的客户端目标，否则发送广播
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
                else
                {
                    foreach (var duplexCallbackContract in _callbackContracts)
                    {
                        duplexCallbackContract.Value.CallbackNotify(message);
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        #endregion
    }
}