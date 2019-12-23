namespace Dreamland.IPC.WCF.Message
{
    /// <summary>
    /// 错误码
    /// </summary>
    public enum ErrorCodes
    {
        /// <summary>
        /// 成功
        /// </summary>
        [Log("成功")]
        Success = 0,

        /// <summary>
        /// 服务异常
        /// </summary>
        [Log("通信异常")]
        CommunicationException = 10000,

        /// <summary>
        /// 业务异常
        /// </summary>
        [Log("业务异常")]
        BusinessException = 10001,

        /// <summary>
        /// 消息Id异常
        /// </summary>
        [Log("消息Id异常（NullOrWhiteSpace）")]
        MessageIdNullOrWhiteSpace = 10002,

        /// <summary>
        /// 查找对应的消息处理器失败
        /// </summary>
        [Log("响应端查找对应的消息处理器失败，可能原因：1.未注册该消息的处理服务；2.请求端方法签名和响应端委托方法签名不一致（如：一端调用异步方法，另一端使用异步委托）")]
        GetMessageHandlerFailed = 10100,

        /// <summary>
        /// 消息处理查找消息对应的处理方法失败
        /// </summary>
        [Log("响应端消息处理查找消息Id（'RequestMessage.Id'）对应的处理方法失败，可能原因：1.未注册该消息的处理服务；2.请求端请求方法签名和响应端委托方法签名不一致（如：一端调用异步方法，另一端使用异步委托）")]
        GetRequestMessageListenerFailed = 10101,

        /// <summary>
        /// 服务端与客户端发送消息失败，未找到对应的客户端
        /// </summary>
        [Log("服务端与客户端发送消息失败，未找到消息目标（'RequestMessage.Destination'）对应的客户端")]
        FindClientFailed = 10102,

        /// <summary>
        /// 客户端连接未初始化
        /// </summary>
        [Log("客户端连接未初始化")]
        ClientUninitialized = 10200,
    }
}