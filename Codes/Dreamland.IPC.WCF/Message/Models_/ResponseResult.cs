using Newtonsoft.Json;

namespace Dreamland.IPC.WCF.Message
{
    /// <summary>
    /// 响应结果
    /// </summary>
    [JsonObject]
    public class ResponseResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [JsonProperty("errorMessage")]
        public string Message { get; set; }

        /// <summary>
        /// 错误码
        /// </summary>
        [JsonProperty("errorCode")]
        public int ErrorCode { get; set; }

        /// <summary>
        /// 消息内容(jsonObject)
        /// </summary>
        [JsonProperty("data")]
        public object Data { get; set; }
    }
}
