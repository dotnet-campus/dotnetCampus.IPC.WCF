using System.ServiceModel;
using System.Threading.Tasks;
using Dreamland.IPC.WCF.Message;

namespace Dreamland.IPC.WCF.Duplex
{
    /// <summary>
    /// 双工服务服务端协议
    /// </summary>
    [DeliveryRequirements(RequireOrderedDelivery = true)]
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IDuplexCallbackContract))]
    internal interface IDuplexContract
    {
        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [OperationContract(Name = "request")]
        ResponseMessage Request(RequestMessage message);

        /// <summary>
        /// 异步发送请求
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [OperationContract(Name = "requestAsync")]
        Task<ResponseMessage> RequestAsync(RequestMessage message);

        /// <summary>
        /// 发送通知
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [OperationContract(Name = "notify", IsOneWay = true)]
        void Notify(NotifyMessage message);
    }

    /// <summary>
    /// 双工协议(回调)
    /// </summary>
    [ServiceContract]
    public interface IDuplexCallbackContract
    {
        /// <summary>
        /// 异步发送请求(回调)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [OperationContract(Name = "callbackRequest")]
        ResponseMessage CallbackRequest(RequestMessage message);

        /// <summary>
        /// 异步发送请求(回调)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [OperationContract(Name = "callbackRequestAsync")]
        Task<ResponseMessage> CallbackRequestAsync(RequestMessage message);

        /// <summary>
        /// 发送通知(回调)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [OperationContract(Name = "callbackNotify", IsOneWay = true)]
        void CallbackNotify(NotifyMessage message);
    }
}