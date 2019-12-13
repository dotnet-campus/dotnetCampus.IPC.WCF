using Newtonsoft.Json;

namespace Dreamland.IPC.WCF.Message
{
    /// <summary>
    /// 消息（通知类型消息[单向]）
    /// </summary>
    [JsonObject]
    public class NotifyMessage : MessageBase
    {
        /// <summary>
        /// 消息内容(jsonObject)
        /// </summary>
        [JsonProperty("data")]
        public object Data { get; set; }
    }
}
