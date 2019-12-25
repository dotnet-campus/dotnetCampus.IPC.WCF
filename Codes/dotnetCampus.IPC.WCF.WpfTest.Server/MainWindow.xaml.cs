using System;
using System.Collections.ObjectModel;
using System.Windows;
using dotnetCampus.IPC.WCF.Message;
using dotnetCampus.IPC.WCF.TestBase;
using dotnetCampus.IPC.WCF.Extensions;

namespace dotnetCampus.IPC.WCF.WpfTest.Server
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
            var serverMessageHandlers = new ServerMessageHandlers(_receivedMessages);
            _server.ServerMessageHandler.RegisterHandlers(serverMessageHandlers);

            _server.Initialize();
        }

        private void SendToClientButton_OnClick(object sender, RoutedEventArgs e)
        {
            SentRequestMessage(new RequestMessage()
            {
                Sequence = TestCustomText.Sequence,
                Id = TestCustomText.SendToClientMessage,
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
