using System;
using System.Collections.ObjectModel;
using System.Windows;
using Dreamland.IPC.WCF.Message;
using Dreamland.IPC.WCF.TestBase;

namespace Dreamland.IPC.WCF.WpfTest.Server
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Duplex.Pipe.Server _server;

        private readonly ObservableCollection<RequestMessage> _receivedMessages = new ObservableCollection<RequestMessage>();

        public MainWindow()
        {
            InitializeComponent();
            ReceivedDataGrid.ItemsSource = _receivedMessages;

            _server = new Duplex.Pipe.Server(new Uri(TestCustomText.Address));
            _server.ServerMessageHandler.TryAddMessageListener(TestCustomText.SendMessage, ListenClientSentMessageRequest);
        }

        /// <summary>
        /// 监听来自客户端的"SentMessage"请求
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private ResponseMessage ListenClientSentMessageRequest(RequestMessage arg)
        {
            _receivedMessages.Add(arg);
            return new ResponseMessage(arg)
            {
                Result = new ResponseResult()
                {
                    Success = true
                }
            };
        }

        private void SendToClientButton_OnClick(object sender, RoutedEventArgs e)
        {
            SentRequestMessage(new RequestMessage()
            {
                Sequence = TestCustomText.Sequence,
                Id = TestCustomText.SendMessage,
                Destination = ClientIdComboBox.Text,
                Data = $"【{DateTime.Now.ToLongTimeString()}】" + ServerSendMessageText.Text,
            });
        }

        private void SentRequestMessage(RequestMessage requestMessage)
        {
            RecordControl.AddOrUpdate(requestMessage);
            var response = _server.Request(requestMessage);
            RecordControl.AddOrUpdate(response);
        }

        private void ClientIdComboBox_OnDropDownOpened(object sender, EventArgs e)
        {
            ClientIdComboBox.ItemsSource = _server.ClientIdList;
        }
    }
}
