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
    public class Client
    {
        private readonly DuplexClientContract _clientContract;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="address"></param>
        /// <param name="clientId"></param>
        public Client(Uri address, string clientId)
        {
            ClientId = clientId;
            var callbackContract = new DuplexCallbackContract();
            var content = new InstanceContext(callbackContract);
            _clientContract = new DuplexClientContract(content, new NetNamedPipeBinding(), new EndpointAddress(address));

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

        #region 初始化

        /// <summary>
        /// 是否已经初始化
        /// </summary>
        public bool HasInitialized { get; private set; }

        /// <summary>
        /// 初始化（注册连接）
        /// </summary>
        public bool Initialize()
        {
            if (HasInitialized)
            {
                return true;
            }

            var success = Request(new RequestMessage()
            {
                Id = "Initialize",
                Data = ClientId,
            }).Result?.Success ?? false;

            if (success)
            {
                HasInitialized = true;
            }

            return success;
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
            return _clientContract.Request(message);
        }

        /// <summary>
        /// 向服务端发送请求
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> RequestAsync(RequestMessage message)
        {
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
            _clientContract.Notify(message);
        }

        #endregion
    }
}
