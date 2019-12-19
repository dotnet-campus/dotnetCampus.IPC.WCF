namespace Dreamland.IPC.WCF.TestBase
{
    public static class TestCustomText
    {
        private static int _sequence;

        public const string Address = "net.pipe://localhost/dreamland_duplexserver";

        public const string SendToClientMessage = nameof(SendToClientMessage);

        public const string SendToServerMessageAsync = nameof(SendToServerMessageAsync);

        /// <summary>
        /// 序列号
        /// </summary>
        public static int Sequence => (_sequence += 1);
    }
}
