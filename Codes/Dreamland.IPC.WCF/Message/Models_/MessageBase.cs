using System.Linq;
using Newtonsoft.Json;

namespace Dreamland.IPC.WCF.Message
{
    /// <summary>
    /// 消息抽象类
    /// </summary>
    public abstract class MessageBase : IMassage
    {
        /// <summary>
        /// 消息id
        /// 备注：推送行为的消息id，用于区分消息（类似于消息接口）
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 会话Id
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// 序列号
        /// </summary>
        public long Sequence { get; set; }

        /// <summary>
        /// 消息目标	
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// 消息来源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 从Json字符串中初始化实例
        /// </summary>
        /// <param name="data"></param>
        public void FromString(string data)
        {
            var obj = JsonConvert.DeserializeObject<MessageBase>(data);
            GetType().GetProperties().ToList().ForEach(p =>
            {
                var properties = obj.GetType().GetProperties();
                foreach (var propertyInfo in properties.Where(prop => p.Name == prop.Name))
                    p.SetValue(this, propertyInfo.GetValue(obj, null), null);
            });
        }

        /// <summary>
        /// 从字节数组中初始化实例
        /// </summary>
        /// <param name="data"></param>
        public void FromBinary(byte[] data)
        {
            var str = System.Text.Encoding.Default.GetString(data);
            FromString(str);
        }

        /// <summary>
        /// 将实例转为字节数组
        /// </summary>
        /// <returns></returns>
        public byte[] ToBinary()
        {
           return System.Text.Encoding.Default.GetBytes(ToString());
        }

        /// <summary>
        /// 将实例转为Json字符串
        /// </summary>
        /// <returns></returns>
        public new string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
