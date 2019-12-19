using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Dreamland.IPC.WCF.Message;

namespace Dreamland.IPC.WCF.Duplex.Pipe
{
    /// <summary>
    /// 客户端
    /// </summary>
    [ServiceContract]
    public class Client : IDisposable
    {
        private DuplexClientContract _clientContract;
        private readonly EndpointAddress _endpointAddress;
        private readonly InstanceContext _instanceContext;

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
        /// 客户端Id
        /// </summary>
        public string ClientId { get; }

        /// <summary>
        /// 消息处理器服务
        /// 通过在此处理器服务中注册消息对应的委托，可以针对消息进行处理
        /// </summary>
        public IMessageHandler ClientMessageHandler { get; } = new MessageHandler();

        #region 连接服务

        /// <summary>
        /// 是否已经绑定至Server
        /// </summary>
        public bool HasBindingServer { get; private set; }

        /// <summary>
        /// 绑定服务端
        /// 绑定服务端成功即可与服务端进行双工通信
        /// </summary>
        public ResponseResult BindingServer()
        {
            if (HasBindingServer)
            {
                return new ResponseResult()
                {
                    Success = true
                };
            }

            var response = Request(new RequestMessage()
            {
                Id = "@@Inner_Binding_Server_From_Modification",
                Data = ClientId,
            });

            HasBindingServer = response.Result?.Success ?? false;
            return response.Result;
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
                _clientContract = new DuplexClientContract(_instanceContext, new NetNamedPipeBinding(), _endpointAddress);
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

        /// <summary>
        /// 注销资源
        /// </summary>
        public void Dispose()
        {
            _clientContract.Abort();
            ((IDisposable) _clientContract)?.Dispose();
        }
    }
}
