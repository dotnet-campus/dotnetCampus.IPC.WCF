using System;
using System.Collections.ObjectModel;
using System.Windows;
using Dreamland.IPC.WCF.Message;
using Dreamland.IPC.WCF.TestBase;

namespace Dreamland.IPC.WCF.WpfTest.Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Duplex.Pipe.Client _client;

        private readonly ObservableCollection<RequestMessage> _receivedMessages = new ObservableCollection<RequestMessage>();

        public MainWindow()
        {
            InitializeComponent();
            ReceivedDataGrid.ItemsSource = _receivedMessages;

            _client = new Duplex.Pipe.Client(new Uri(TestCustomText.Address), TestCustomText.TestClientAppCode);

            _client.ClientMessageHandler.TryAddMessageListener(TestCustomText.SendMessage, ListenServerSentMessageRequest);

            _client.Initialize();
        }

        private ResponseMessage ListenServerSentMessageRequest(RequestMessage arg)
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

        private void SendToServerButton_OnClick(object sender, RoutedEventArgs e)
        {
            SentRequestMessage(new RequestMessage()
            {
                Source = TestCustomText.TestClientAppCode,
                Sequence = TestCustomText.Sequence,
                Id = TestCustomText.SendMessage,
                Data = $"【{DateTime.Now.ToLongTimeString()}】" + ClientSendMessageText.Text,
            });
        }

        private void SentRequestMessage(RequestMessage requestMessage)
        {
            RecordControl.AddOrUpdate(requestMessage);
            var response = _client.Request(requestMessage);
            RecordControl.AddOrUpdate(response);
        }
    }
}
