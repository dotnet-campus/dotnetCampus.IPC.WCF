using Newtonsoft.Json;

namespace dotnetCampus.IPC.WCF.Message
{
    /// <summary>
    /// 消息（请求类型消息）
    /// </summary>
    [JsonObject]
    public class RequestMessage : MessageBase
    {
        /// <summary>
        /// 默认构造
        /// </summary>
        public RequestMessage() : this(string.Empty)
        {
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="data">请求携带的数据</param>
        public RequestMessage(object data)
        {
            Data = JsonConvert.SerializeObject(data);
        }

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