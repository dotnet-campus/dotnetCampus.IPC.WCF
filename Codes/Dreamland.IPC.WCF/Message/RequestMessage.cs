using Newtonsoft.Json;

namespace Dreamland.IPC.WCF.Message
{
    /// <summary>
    /// 消息（请求类型消息）
    /// </summary>
    [JsonObject]
    public class RequestMessage : MessageBase
    {
        /// <summary>
        /// 消息内容(jsonObject)
        /// </summary>
        [JsonProperty("data")]
        public object Data { get; set; }
    }
}
