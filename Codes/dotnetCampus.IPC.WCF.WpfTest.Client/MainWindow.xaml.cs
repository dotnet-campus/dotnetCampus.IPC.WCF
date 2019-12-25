using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using dotnetCampus.IPC.WCF.Message;
using dotnetCampus.IPC.WCF.TestBase;

namespace dotnetCampus.IPC.WCF.WpfTest.Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _clientId = Guid.NewGuid().ToString();

        private readonly Duplex.Pipe.Client _client;

        private readonly ObservableCollection<RequestMessage> _receivedMessages = new ObservableCollection<RequestMessage>();

        public MainWindow()
        {
            InitializeComponent();
            ReceivedDataGrid.ItemsSource = _receivedMessages;
            Title = $"客户端:{_clientId}";

            _client = new Duplex.Pipe.Client(new Uri(TestCustomText.Address), _clientId);

            _client.ClientMessageHandler.TryAddMessageListener(TestCustomText.SendToClientMessage, ListenServerSentMessageRequest);

            _client.BindingServer();
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
                Source = _clientId,
                Sequence = TestCustomText.Sequence,
                Id = TestCustomText.SendToServerMessageAsync,
                Data = $"【{DateTime.Now.ToLongTimeString()}】" + ClientSendMessageText.Text,
            });
        }

        private async void SentRequestMessage(RequestMessage requestMessage)
        {
            RecordControl.AddOrUpdate(requestMessage);
            var response = await _client.RequestAsync(requestMessage);
            RecordControl.AddOrUpdate(response);
        }
    }
}
