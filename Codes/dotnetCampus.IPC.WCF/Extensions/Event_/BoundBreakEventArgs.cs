using System;

namespace dotnetCampus.IPC.WCF.Extensions
{ 
    /// <summary>
    /// 绑定服务器中断事件
    /// </summary>
    public class BoundBreakEventArgs : EventArgs
    {
        internal BoundBreakEventArgs(string message)
        {
            Message = message;
        }

        /// <summary>
        /// 断连消息
        /// </summary>
        public string Message { get; } 
    }
}
