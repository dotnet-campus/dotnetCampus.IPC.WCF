using System.Collections.Generic;
using Newtonsoft.Json;

namespace dotnetCampus.IPC.WCF.Message
{
    /// <summary>
    /// 发送结果
    /// </summary>
    [JsonObject]
    public class NotifyResult
    {
        /// <summary>
        /// 已向其发送通知的终端
        /// </summary>
        [JsonProperty("sentTerminals")]
        public List<string> SentTerminals { get; set; }

        /// <summary>
        /// 发送通知失败的终端，以及对应失败的愿因
        /// </summary>
        [JsonProperty("exceptionInfos")]
        public Dictionary<string, string> ExceptionInfos { get; set; }
    }
}
