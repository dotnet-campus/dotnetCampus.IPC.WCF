using Newtonsoft.Json;

namespace Dreamland.IPC.WCF.Message
{
    /// <summary>
    /// 定义基础消息类型
    /// </summary>
    [JsonObject]
    public interface IMassage
    {
        /// <summary>
        /// 消息id
        /// 备注：推送行为的消息id，用于区分消息（类似于消息接口）
        /// </summary>
        [JsonProperty("id")]
        string Id { get; }

        /// <summary>
        /// 会话Id
        /// </summary>
        [JsonProperty("sessionId")]
        string SessionId { get; }

        /// <summary>
        /// 序列号
        /// </summary>
        [JsonProperty("sequence")]
        long Sequence { get; }

        /// <summary>
        /// 消息目标	
        /// </summary>
        [JsonProperty("destination")]
        string Destination { get; }

        /// <summary>
        /// 消息来源
        /// 标记此消息来自哪个客户端或服务端，需要业务方保证在一个端的实例中，发送的消息中此字段保持不变
        /// </summary>
        [JsonProperty("source")]
        string Source { get; }

        /// <summary>
        /// 从Json字符串中初始化实例
        /// </summary>
        /// <param name="data"></param>
        void FromString(string data);

        /// <summary>
        /// 从字节数组中初始化实例
        /// </summary>
        /// <param name="data"></param>
        void FromBinary(byte[] data);

        /// <summary>
        /// 将实例转为字节数组
        /// </summary>
        /// <returns></returns>
        byte[] ToBinary();

        /// <summary>
        /// 将实例转为Json字符串
        /// </summary>
        /// <returns></returns>
        string ToString();
    }
}