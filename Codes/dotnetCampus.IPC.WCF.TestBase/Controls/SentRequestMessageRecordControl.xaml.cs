using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using dotnetCampus.IPC.WCF.Message;
using dotnetCampus.IPC.WCF.TestBase.ViewModels;

namespace dotnetCampus.IPC.WCF.TestBase.Controls
{
    /// <summary>
    /// SentRequestMessageRecordControl.xaml 的交互逻辑
    /// </summary>
    public partial class SentRequestMessageRecordControl : UserControl
    {
        public SentRequestMessageRecordControl()
        {
            InitializeComponent();
        }

        private static readonly DependencyProperty SentRequestMessageRecordsProperty = DependencyProperty.Register(
            "SentRequestMessageRecords", typeof(ObservableCollection<SentRequestMessageRecord>), typeof(SentRequestMessageRecordControl), new PropertyMetadata(new ObservableCollection<SentRequestMessageRecord>()));

        private ObservableCollection<SentRequestMessageRecord> SentRequestMessageRecords
        {
            get => (ObservableCollection<SentRequestMessageRecord>) GetValue(SentRequestMessageRecordsProperty);
            set => SetValue(SentRequestMessageRecordsProperty, value);
        }

        public void AddOrUpdate(RequestMessage requestMessage)
        {
            if (SentRequestMessageRecords.FirstOrDefault(x =>
                x.Id == requestMessage.Id && x.Sequence == requestMessage.Sequence) == null)
            {
                SentRequestMessageRecords.Add(new SentRequestMessageRecord(requestMessage));
            }
        }

        public void AddOrUpdate(ResponseMessage responseMessage)
        {
            var record = SentRequestMessageRecords.FirstOrDefault(x =>
                x.Id == responseMessage.Id && x.Sequence == responseMessage.Sequence);
            record?.Update(responseMessage.Result);
        }
    }
}
