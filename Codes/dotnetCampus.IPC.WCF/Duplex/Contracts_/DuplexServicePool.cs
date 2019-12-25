using System.Collections.Concurrent;
using System.ServiceModel;
using dotnetCampus.IPC.WCF.Message;

namespace dotnetCampus.IPC.WCF.Duplex
{
    /// <summary>
    /// 服务池
    /// </summary>
    internal static class DuplexServicePool
    {
        private static readonly ConcurrentDictionary<ServiceHostBase, IMessageHandler> ServerServices =
            new ConcurrentDictionary<ServiceHostBase, IMessageHandler>();

        private static readonly ConcurrentDictionary<IDuplexCallbackContract, IMessageHandler> ClientServices =
            new ConcurrentDictionary<IDuplexCallbackContract, IMessageHandler>();

        /// <summary>
        /// 添加或更新服务
        /// </summary>
        /// <param name="serviceHost"></param>
        /// <param name="messageHandler"></param>
        public static void AddOrUpdateServiceHost(ServiceHostBase serviceHost, IMessageHandler messageHandler)
        {
            ServerServices.AddOrUpdate(serviceHost, messageHandler, (host, handler) => messageHandler);
        }

        /// <summary>
        /// 添加或更新服务
        /// </summary>
        /// <param name="clientContract"></param>
        /// <param name="messageHandler"></param>
        public static void AddOrUpdateServiceHost(IDuplexCallbackContract clientContract,
            IMessageHandler messageHandler)
        {
            ClientServices.AddOrUpdate(clientContract, messageHandler, (host, handler) => messageHandler);
        }

        /// <summary>
        /// 获取服务对应的消息处理器
        /// </summary>
        /// <param name="serviceHost"></param>
        /// <param name="messageHandler"></param>
        /// <returns></returns>
        public static bool TryGetMessageHandler(ServiceHostBase serviceHost, out IMessageHandler messageHandler)
        {
            return ServerServices.TryGetValue(serviceHost, out messageHandler);
        }

        /// <summary>
        /// 获取服务对应的消息处理器
        /// </summary>
        /// <param name="clientContract"></param>
        /// <param name="messageHandler"></param>
        /// <returns></returns>
        public static bool TryGetMessageHandler(IDuplexCallbackContract clientContract,
            out IMessageHandler messageHandler)
        {
            return ClientServices.TryGetValue(clientContract, out messageHandler);
        }

        /// <summary>
        /// 移除服务
        /// </summary>
        /// <param name="serviceHost"></param>
        /// <returns></returns>
        public static bool TryRemoveServiceHost(ServiceHostBase serviceHost)
        {
            return ServerServices.TryRemove(serviceHost, out _);
        }

        /// <summary>
        /// 移除服务
        /// </summary>
        /// <param name="clientContract"></param>
        /// <returns></returns>
        public static bool TryRemoveServiceHost(IDuplexCallbackContract clientContract)
        {
            return ClientServices.TryRemove(clientContract, out _);
        }
    }
}