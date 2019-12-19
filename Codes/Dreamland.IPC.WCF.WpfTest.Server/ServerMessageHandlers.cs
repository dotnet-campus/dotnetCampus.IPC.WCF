using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Dreamland.IPC.WCF.Message;
using Dreamland.IPC.WCF.TestBase;

namespace Dreamland.IPC.WCF.WpfTest.Server
{
    class ServerMessageHandlers
    {
        private readonly ObservableCollection<RequestMessage> _requestMessages;

        public ServerMessageHandlers(ObservableCollection<RequestMessage> requestMessages)
        {
            _requestMessages = requestMessages;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        [MessageHandler(TestCustomText.SendToServerMessageAsync, MessageHandlerType.RequestAsync)]
        public async Task<ResponseMessage> SendMessageAsync(RequestMessage requestMessage)
        {
            _requestMessages.Add(requestMessage);
            return new ResponseMessage(requestMessage)
            {
                Result = new ResponseResult()
                {
                    Success = true
                }
            };
        }
    }
}
