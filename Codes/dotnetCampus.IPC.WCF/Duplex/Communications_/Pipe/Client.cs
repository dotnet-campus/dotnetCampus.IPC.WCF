using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Timers;
using dotnetCampus.IPC.WCF.Extensions;
using dotnetCampus.IPC.WCF.Message;

namespace dotnetCampus.IPC.WCF.Duplex.Pipe
{
    /// <summary>
    /// 客户端
    /// </summary>
    [ServiceContract]
    public class Client : IDisposable
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="address"></param>
        /// <param name="clientId"></param>
        public Client(Uri address, string clientId)
        {
            ClientId = clientId;
            var callbackContract = new DuplexCallbackContract();
            _instanceContext = new InstanceContext(callbackContract);
            _endpointAddress = new EndpointAddress(address);
            _clientContract = new DuplexClientContract(_instanceContext, new NetNamedPipeBinding(), _endpointAddress);

            //在服务池中：注册此客户端对应的消息处理
            DuplexServicePool.AddOrUpdateServiceHost(callbackContract, ClientMessageHandler);

            _checkBindingTimer.Elapsed += CheckBindingTimerOnElapsed;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="address"></param>
        public Client(Uri address) : this(address, Guid.NewGuid().ToString())
        {

        }

        /// <summary>
        /// 客户端Id
        /// </summary>
        public string ClientId { get; }

        /// <summary>
        /// 消息处理器服务
        /// 通过在此处理器服务中注册消息对应的委托，可以针对消息进行处理
        /// </summary>
        public IMessageHandler ClientMessageHandler { get; } = new MessageHandler();

        /// <summary>
        /// 注销资源
        /// </summary>
        public void Dispose()
        {
            _clientContract.Abort();
            ((IDisposable) _clientContract)?.Dispose();
        }

        private readonly EndpointAddress _endpointAddress;
        private readonly InstanceContext _instanceContext;
        private DuplexClientContract _clientContract;
        private bool _isNeedBindingServer;

        #region 连接服务

        /// <summary>
        /// 绑定中断
        /// </summary>
        public event EventHandler<BoundBreakEventArgs> BoundBreak;

        /// <summary>
        /// 当前的连接状态
        /// </summary>
        public CommunicationState State => _clientContract.State;

        /// <summary>
        /// 是否连接异常
        /// </summary>
        public bool IsCommunicationStateAbnormal => State == CommunicationState.Closed ||
                                                    State == CommunicationState.Closing ||
                                                    State == CommunicationState.Faulted;

        /// <summary>
        /// 检测是否已绑定服务器
        /// </summary>
        /// <returns></returns>
        public bool IsServerBindingOpen()
        {
            return CheckServerBinding().Success;
        }

        /// <summary>
        /// 检测是否已绑定服务器（异步）
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsServerBindingOpenAsync()
        {
            return Task.Run(IsServerBindingOpen);
        }

        /// <summary>
        /// 绑定服务端
        /// 绑定服务端成功即可与服务端进行双工通信
        /// </summary>
        public ResponseResult BindingServer()
        {
            _isNeedBindingServer = true;

            var result = InnerRequest(new RequestMessage
            {
                Id = InnerMessageIds.BindingServer,
                Source = ClientId,
            }).Result;

            if (result.Success)
            {
                lock (_locker)
                {
                    _checkBindingTimer.Enabled = true;
                }
            }

            return result;
        }

        /// <summary>
        /// 绑定服务端（异步）
        /// 绑定服务端成功即可与服务端进行双工通信
        /// </summary>
        public Task<ResponseResult> BindingServerAsync()
        {
            return Task.Run(BindingServer);
        }

        /// <summary>
        /// 重连服务
        /// </summary>
        /// <returns></returns>
        public bool TryReconnectServer()
        {
            try
            {
                switch (_clientContract.State)
                {
                    case CommunicationState.Faulted:
                    case CommunicationState.Closed:
                    case CommunicationState.Closing:
                        _clientContract = new DuplexClientContract(_instanceContext, new NetNamedPipeBinding(), _endpointAddress);
                        break;
                    default:
                        _clientContract.Open();
                        break;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 重连服务
        /// </summary>
        /// <returns></returns>
        public Task<bool> ReconnectServerAsync()
        {
            return Task.Run(TryReconnectServer);
        }

        /// <summary>
        /// 检测客户端绑定服务端状态
        /// </summary>
        /// <returns></returns>
        private ResponseResult CheckServerBinding()
        {
            return InnerRequest(new RequestMessage
            {
                Id = InnerMessageIds.CheckServerBinding,
                Source = ClientId,
            }).Result;
        }

        private readonly object _locker = new object();

        private void CheckBindingTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            lock (_locker)
            {
                var request = CheckServerBinding();

                if (!request.Success && _checkBindingTimer.Enabled)
                {
                    BoundBreak?.Invoke(this, new BoundBreakEventArgs(request.Message));
                    _checkBindingTimer.Enabled = false;
                }
            }
        }

        private readonly Timer _checkBindingTimer = new Timer(1000)
        {
            AutoReset = true
        };

        private bool InnerRepairConnection()
        {
            if (!IsCommunicationStateAbnormal)
            {
                return true;
            }

            if (!TryReconnectServer())
            {
                return false;
            }

            return !_isNeedBindingServer || BindingServer().Success;
        }

        #endregion

        #region 调用服务端方法

        /// <summary>
        /// 向服务端发送请求
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private ResponseMessage InnerRequest(RequestMessage message)
        {
            //设置消息源
            message.Source = ClientId;

            return _clientContract.Request(message);
        }

        /// <summary>
        /// 向服务端发送请求
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ResponseMessage Request(RequestMessage message)
        {
            return InnerRepairConnection()
                ? InnerRequest(message)
                : ResponseMessage.GetResponseMessageFromErrorCode(message, ErrorCodes.FindServerFailed);
        }

        /// <summary>
        /// 向服务端发送请求
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> RequestAsync(RequestMessage message)
        {
            //设置消息源
            message.Source = ClientId;

            if (!InnerRepairConnection())
            {
                return ResponseMessage.GetResponseMessageFromErrorCode(message, ErrorCodes.FindServerFailed);
            }
            return await _clientContract.RequestAsync(message).ConfigureAwait(false);
        }

        /// <summary>
        /// 向服务端发送通知
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool Notify(NotifyMessage message, out string errorMessage)
        {
            //设置消息源
            message.Source = ClientId;

            if (!InnerRepairConnection())
            {
                errorMessage = ErrorCodes.FindServerFailed.GetCustomAttribute<LogAttribute>()
                    ?.Description;
                return false;
            }
            try
            {
                errorMessage = "success";
                _clientContract.Notify(message);
                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
        }

        #endregion
    }
}