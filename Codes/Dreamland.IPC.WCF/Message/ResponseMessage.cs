using System;
using Newtonsoft.Json;

namespace Dreamland.IPC.WCF.Message
{
    /// <summary>
    /// 消息（响应类型消息）
    /// </summary>
    [JsonObject]
    public class ResponseMessage : MessageBase
    {
        /// <summary>
        /// 默认构造
        /// </summary>
        public ResponseMessage()
        {
        }

        /// <summary>
        /// 通过消息请求创建消息响应
        /// </summary>
        /// <param name="requestMessage"></param>
        public ResponseMessage(RequestMessage requestMessage)
        {
            Id = requestMessage.Id;
            SessionId = requestMessage.SessionId;
            Destination = requestMessage.Source;
            Source = requestMessage.Destination;
            Sequence = requestMessage.Sequence;
        }

        /// <summary>
        /// 响应结果
        /// </summary>
        [JsonProperty("result")]
        public ResponseResult Result { get; set; }

        #region 获取指定响应

        /// <summary>
        /// 成功的响应消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResponseMessage SuccessfulResponseMessage(RequestMessage message)
        {
            return new ResponseMessage(message)
            {
                Result = new ResponseResult
                {
                    Success = true,
                    ErrorCode = (int) ErrorCodes.Success,
                    Message = ErrorCodes.Success.GetCustomAttribute<LogAttribute>()
                        ?.Description
                }
            };
        }

        /// <summary>
        /// 通过错误码获取指定异常响应
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public static ResponseMessage GetResponseMessageFromErrorCode(RequestMessage message, ErrorCodes errorCode)
        {
            return new ResponseMessage(message)
            {
                Result = new ResponseResult
                {
                    Success = false,
                    ErrorCode = (int) errorCode,
                    Message = errorCode.GetCustomAttribute<LogAttribute>()
                        ?.Description
                }
            };
        }

        /// <summary>
        /// 异常消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="isBusinessException"></param>
        /// <returns></returns>
        public static ResponseMessage ExceptionResponseMessage(RequestMessage message, Exception ex,
            bool isBusinessException = true)
        {
            return new ResponseMessage(message)
            {
                Result = new ResponseResult
                {
                    Success = false,
                    ErrorCode = isBusinessException
                        ? (int) ErrorCodes.BusinessException
                        : (int) ErrorCodes.CommunicationException,
                    Message = ex.Message
                }
            };
        }

        #endregion
    }
}