using Newtonsoft.Json;

namespace dotnetCampus.IPC.WCF.Message
{
    /// <summary>
    /// 响应结果
    /// </summary>
    [JsonObject]
    public class ResponseResult
    {
        /// <summary>
        /// 默认构造
        /// </summary>
        public ResponseResult() : this(string.Empty)
        {
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="data">请求携带的数据</param>
        public ResponseResult(object data)
        {
            Data = JsonConvert.SerializeObject(data);
        }

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
        /// 消息内容(jsonObject序列化后的数据)
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }

        /// <summary>
        /// 将Data转为指定类型的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T DeserializeData<T>()
        {
            return JsonConvert.DeserializeObject<T>(Data);
        }
    }
}