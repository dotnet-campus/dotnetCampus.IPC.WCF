using System;
using System.Reflection;
using System.Threading.Tasks;
using Dreamland.IPC.WCF.Message;

namespace Dreamland.IPC.WCF.Extensions
{
    /// <summary>
    /// 消息处理器工厂
    /// </summary>
    public static class MessageHandlerFactory
    {
        /// <summary>
        /// 通过特性自动注册Handler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageHandler"></param>
        /// <param name="instance"></param>
        public static void RegisterHandlers<T>(this IMessageHandler messageHandler, T instance) where T : class
        {
            var methods = typeof(T).GetMethods();
            foreach (var methodInfo in methods)
            {
                try
                {
                    var attribute = methodInfo.GetCustomAttribute<MessageHandlerAttribute>();
                    if (attribute != null)
                    {
                        switch (attribute.Type)
                        {
                            case MessageHandlerType.Notify:
                                messageHandler.TryAddMessageListener(attribute.MessageId,
                                    message => NotifyAction(instance, methodInfo, message));
                                break;
                            case MessageHandlerType.Request:
                                messageHandler.TryAddMessageListener(attribute.MessageId,
                                    message => RequestFunc(instance, methodInfo, message));
                                break;
                            case MessageHandlerType.RequestAsync:
                                messageHandler.TryAddMessageListener(attribute.MessageId,
                                    message => RequestAsyncFunc(instance, methodInfo, message));
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    //
                }
            }
        }

        /// <summary>
        /// 通过反射和特性自动注册Handler
        /// 注：使用该方法需要确保<see cref="Type"/>对应的类拥有默认构造函数
        /// </summary>
        /// <param name="messageHandler"></param>
        /// <param name="type"></param>
        public static void RegisterHandlersFromType(this IMessageHandler messageHandler, Type type)
        {
            var obj = Activator.CreateInstance(type);
            messageHandler.RegisterHandlers(obj);
        }

        private static void NotifyAction(object instance, MethodInfo methodInfo, NotifyMessage arg)
        {
            methodInfo.Invoke(instance, new object[] { arg });
        }

        private static ResponseMessage RequestFunc(object instance, MethodInfo methodInfo, RequestMessage arg)
        {
            return (ResponseMessage) methodInfo.Invoke(instance, new object[] { arg });
        }

        private static Task<ResponseMessage> RequestAsyncFunc(object instance, MethodInfo methodInfo,
            RequestMessage arg)
        {
            return (Task<ResponseMessage>) methodInfo.Invoke(instance, new object[] { arg });
        }
    }
}