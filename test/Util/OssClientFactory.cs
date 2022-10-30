﻿/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using Aliyun.OSS;
using Aliyun.OSS.Common;

namespace Aliyun.OSS.Test.Util
{
    internal static class OssClientFactory
    {
        private const string HttpProto = "http://";

        public static IOss CreateOssClient()
        {
            return CreateOssClient(AccountSettings.Load());
        }

        public static IOss CreateOssClientWithProxy()
        {
            return CreateOssClientWithProxy(AccountSettings.Load());
        }

        public static IOss CreateOssClient(AccountSettings settings)
        {
            return new OssClient(settings.OssEndpoint,
                                 settings.OssAccessKeyId,
                                 settings.OssAccessKeySecret);
        }

        public static IOss CreateOssClientWithProxy(AccountSettings settings)
        {
            return CreateOssClientWithProxy(settings.OssEndpoint, 
                                            settings.OssAccessKeyId, 
                                            settings.OssAccessKeySecret,
                                            settings.ProxyHost, 
                                            settings.ProxyPort, 
                                            settings.ProxyUser, 
                                            settings.ProxyPassword);
        }

        public static IOss CreateOssClientWithProxy(string endpoint, 
                                                    string accessKeyId, 
                                                    string accessKeySecret, 
                                                    string proxyHost,
                                                    int proxyPort, 
                                                    string proxyUser, 
                                                    string proxyPassword)
        {
            var clientConfiguration = new ClientConfiguration();
            if (!String.IsNullOrEmpty(proxyHost))
            {
                clientConfiguration.ProxyHost = proxyHost;
                clientConfiguration.ProxyPort = proxyPort;
                if (!String.IsNullOrEmpty(proxyUser) && !String.IsNullOrEmpty(proxyPassword))
                {
                    clientConfiguration.ProxyUserName = proxyUser;
                    clientConfiguration.ProxyPassword = proxyPassword;
                }
            }

            var uri = new Uri(endpoint.ToLower().Trim().StartsWith(HttpProto) ?
                endpoint.Trim() : HttpProto + endpoint.Trim());

            return new OssClient(uri, accessKeyId, accessKeySecret, clientConfiguration);
        }
    }
}
