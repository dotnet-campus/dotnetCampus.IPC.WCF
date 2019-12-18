namespace Dreamland.IPC.WCF.TestBase
{
    public static class TestCustomText
    {
        private static int _sequence;

        public const string Address = "net.pipe://localhost/dreamland_duplexserver";

        public const string SendMessage = nameof(SendMessage);

        /// <summary>
        /// 序列号
        /// </summary>
        public static int Sequence => (_sequence += 1);
    }
}
