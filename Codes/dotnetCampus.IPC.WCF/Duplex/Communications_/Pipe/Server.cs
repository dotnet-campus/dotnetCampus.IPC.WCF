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
            ServerMessageHandler.TryAddMessageListener(InnerMessageIds.CheckServerBinding, HasClientBoundServer);
            ServerMessageHandler.TryAddMessageListener(InnerMessageIds.CheckServerBinding, HasClientBoundServerAsync);
            ServerMessageHandler.TryAddMessageListener(InnerMessageIds.BindingServer, ClientBindingServer);
            ServerMessageHandler.TryAddMessageListener(InnerMessageIds.BindingServer, ClientBindingServerAsync);
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

        #region 连接服务

        private ResponseMessage HasClientBoundServer(RequestMessage message)
        {
            return new ResponseMessage(message)
            {
                Result = new ResponseResult()
                {
                    Success = _callbackContracts.ContainsKey(message.Source)
                }
            };
        }

        private Task<ResponseMessage> HasClientBoundServerAsync(RequestMessage message)
        {
            return Task.Run(() => HasClientBoundServer(message));
        }

        private ResponseMessage ClientBindingServer(RequestMessage message)
        {
            var channel = OperationContext.Current.GetCallbackChannel<IDuplexCallbackContract>();
            _callbackContracts.AddOrUpdate(message.Source, channel, (s, contract) => channel);
            return ResponseMessage.SuccessfulResponseMessage(message);
        }

        private Task<ResponseMessage> ClientBindingServerAsync(RequestMessage message)
        {
            return Task.Run(() => ClientBindingServer(message));
        }

        #endregion

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
        /// 向客户端发送通知，在<see cref="RequestMessage"/>的"IsBroadcast"属性为True则忽略"Destination"发送广播，否则则定向发送至"Destination"对应的端
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public NotifyResult Notify(NotifyMessage message)
        {
            var notifyResult = new NotifyResult()
            {
                ExceptionInfos = new Dictionary<string, string>(),
                SentTerminals = new List<string>()
            };

            if (message.IsBroadcast)
            {
                foreach (var duplexCallbackContract in _callbackContracts)
                {
                    try
                    {
                        duplexCallbackContract.Value.CallbackNotify(message);
                        notifyResult.SentTerminals.Add(duplexCallbackContract.Key);
                    }
                    catch (Exception ex)
                    {
                        notifyResult.ExceptionInfos.Add(duplexCallbackContract.Key, ex.Message);
                    }
                }
            }
            else if(message.Destination != null)
            {
                if (_callbackContracts.TryGetValue(message.Destination, out var callbackContract))
                {
                    try
                    {
                        callbackContract.CallbackNotify(message);
                    }
                    catch (Exception ex)
                    {
                        notifyResult.ExceptionInfos.Add(message.Destination, ex.Message);
                    }
                }
                else
                {
                    notifyResult.ExceptionInfos.Add(message.Destination, ErrorCodes.FindClientFailed.GetCustomAttribute<LogAttribute>()
                        ?.Description);
                }
            }

            return notifyResult;
        }

        #endregion
    }
}