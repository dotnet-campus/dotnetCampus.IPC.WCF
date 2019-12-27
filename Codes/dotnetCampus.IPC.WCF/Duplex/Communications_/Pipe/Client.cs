using System;
using System.ServiceModel;
using System.Threading.Tasks;
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

        #region 连接服务

        /// <summary>
        /// 当前的连接状态
        /// </summary>
        public CommunicationState State => _clientContract.State;

        /// <summary>
        /// 检测是否已绑定服务器
        /// </summary>
        /// <returns></returns>
        public bool HasBoundServer()
        {
            return Request(new RequestMessage
            {
                Id = InnerMessageIds.CheckHasBoundServer,
                Source = ClientId,
            }).Result.Success;
        }

        /// <summary>
        /// 检测是否已绑定服务器（异步）
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsBoundServerAsync()
        {
            return (await RequestAsync(new RequestMessage
            {
                Id = InnerMessageIds.CheckHasBoundServer,
                Source = ClientId,
            })).Result.Success;
        }

        /// <summary>
        /// 绑定服务端
        /// 绑定服务端成功即可与服务端进行双工通信
        /// </summary>
        public ResponseResult BindingServer()
        {
            return Request(new RequestMessage
            {
                Id = InnerMessageIds.BindingServer,
                Source = ClientId,
            }).Result;
        }

        /// <summary>
        /// 绑定服务端（异步）
        /// 绑定服务端成功即可与服务端进行双工通信
        /// </summary>
        public async Task<ResponseResult> BindingServerAsync()
        {
            return (await RequestAsync(new RequestMessage
            {
                Id = InnerMessageIds.BindingServer,
                Source = ClientId,
            })).Result;
        }

        /// <summary>
        /// 重连服务
        /// </summary>
        /// <returns></returns>
        public CommunicationState ReconnectServer()
        {
            try
            {
                _clientContract.Abort();
                _clientContract =
                    new DuplexClientContract(_instanceContext, new NetNamedPipeBinding(), _endpointAddress);
            }
            catch (Exception)
            {
                return CommunicationState.Faulted;
            }

            return _clientContract.State;
        }

        /// <summary>
        /// 尝试修复连接
        /// </summary>
        private void TryRepairConnection()
        {
            try
            {
                switch (_clientContract.State)
                {
                    case CommunicationState.Closed:
                    case CommunicationState.Closing:
                    case CommunicationState.Faulted:
                        ReconnectServer();
                        break;
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        #endregion

        #region 调用服务端方法

        /// <summary>
        /// 向服务端发送请求
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ResponseMessage Request(RequestMessage message)
        {
            TryRepairConnection();
            return _clientContract.Request(message);
        }

        /// <summary>
        /// 向服务端发送请求
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> RequestAsync(RequestMessage message)
        {
            TryRepairConnection();
            return await _clientContract.RequestAsync(message).ConfigureAwait(false);
        }

        /// <summary>
        /// 向服务端发送通知
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [OperationContract(IsOneWay = true)]
        public void Notify(NotifyMessage message)
        {
            TryRepairConnection();
            _clientContract.Notify(message);
        }

        #endregion
    }
}