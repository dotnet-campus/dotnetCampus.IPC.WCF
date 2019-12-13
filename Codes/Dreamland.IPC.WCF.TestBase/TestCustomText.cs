namespace Dreamland.IPC.WCF.TestBase
{
    public static class TestCustomText
    {
        private static int _sequence;

        public const string Address = "net.pipe://localhost/dreamland_duplexserver";

        public const string SendMessage = nameof(SendMessage);

        public const string TestClientAppCode = "ff69f761-15c4-4028-89b9-a0a723a4e046";

        /// <summary>
        /// 序列号
        /// </summary>
        public static int Sequence => (_sequence += 1);
    }
}
