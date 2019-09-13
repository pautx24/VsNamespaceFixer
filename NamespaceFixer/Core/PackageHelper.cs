using Microsoft.VisualStudio;
using System;

namespace NamespaceFixer.Core
{
    internal class PackageHelper
    {
        /// <summary>
        /// Returns the service represented by the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T GetService<T>(Type type)
        {
            IServiceProvider provider = null;

            if (NamespaceAdjuster.Instance != null)
            {
                provider = NamespaceAdjuster.Instance.ServiceProvider;
            }

            return GetService<T>(provider, type);
        }

        /// <summary>
        /// Returns the service represented by the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T GetService<T>(IServiceProvider provider, Type type)
        {
            T rslt = default(T);

            if (provider != null && type != null)
            {
                object service = provider.GetService(type);

                if (service != null && service is T)
                    rslt = (T)service;
            }

            return rslt;
        }

        /// <summary>
        /// Determines if the result is a success.
        /// </summary>
        /// <param name="requestRslt"></param>
        /// <returns></returns>
        public static bool Success(int requestRslt)
        {
            return (requestRslt == VSConstants.S_OK);
        }
    }
}
