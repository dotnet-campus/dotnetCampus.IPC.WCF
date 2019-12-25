using System.ComponentModel;
using System.Runtime.CompilerServices;
using dotnetCampus.IPC.WCF.Message;
using dotnetCampus.IPC.WCF.WpfTest.Client.TestBase.Annotations;

namespace dotnetCampus.IPC.WCF.TestBase.ViewModels
{
    /// <summary>
    /// 发送的请求消息记录
    /// </summary>
    public class SentRequestMessageRecord : INotifyPropertyChanged
    {
        private RequestStatus _status;
        private string _id;
        private string _sessionId;
        private long _sequence;
        private string _destination;
        private string _source;
        private object _requestData;
        private bool _success;
        private string _message;
        private int _errorCode;
        private object _responseData;

        /// <summary>
        /// 属性变更事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 属性变更
        /// </summary>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="requestMessage"></param>
        public SentRequestMessageRecord(RequestMessage requestMessage)
        {
            Status = RequestStatus.Sending;
            Id = requestMessage.Id;
            Sequence = requestMessage.Sequence;
            SessionId = requestMessage.SessionId;
            Destination = requestMessage.Destination;
            Source = requestMessage.Source;
            RequestData = requestMessage.Data;
        }

        /// <summary>
        /// 更新发送详情
        /// </summary>
        /// <param name="responseResult"></param>
        public void Update(ResponseResult responseResult)
        {
            Status = RequestStatus.Completed;
            Success = responseResult.Success;
            ResponseData = responseResult.Data;
            ErrorCode = responseResult.ErrorCode;
            Message = responseResult.Message;
        }

        /// <summary>
        /// 发送状态
        /// </summary>
        public RequestStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 消息id
        /// 备注：推送行为的消息id，用于区分消息（类似于消息接口）
        /// </summary>
        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 序列号
        /// </summary>
        public long Sequence
        {
            get => _sequence;
            set
            {
                _sequence = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 会话Id
        /// </summary>
        public string SessionId
        {
            get => _sessionId;
            set
            {
                _sessionId = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 消息目标	
        /// </summary>
        public string Destination
        {
            get => _destination;
            set
            {
                _destination = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 消息来源
        /// </summary>
        public string Source
        {
            get => _source;
            set
            {
                _source = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 消息内容(jsonObject)
        /// </summary>
        public object RequestData
        {
            get => _requestData;
            set
            {
                _requestData = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success
        {
            get => _success;
            set
            {
                _success = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 错误码
        /// </summary>
        public int ErrorCode
        {
            get => _errorCode;
            set
            {
                _errorCode = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 消息内容(jsonObject)
        /// </summary>
        public object ResponseData
        {
            get => _responseData;
            set
            {
                _responseData = value;
                OnPropertyChanged();
            }
        }
    }
}
