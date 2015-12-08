﻿/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using Aliyun.OSS.Common;
using System.Collections.Generic;

namespace Aliyun.OSS.Samples
{
    /// <summary>
    /// Sample for creating bucket.
    /// </summary>
    public static class DeleteBucketSample
    {
        static string accessKeyId = Config.AccessKeyId;
        static string accessKeySecret = Config.AccessKeySecret;
        static string endpoint = Config.Endpoint;
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        public static void DeleteBucket(string bucketName)
        {
            try
            {
                client.DeleteBucket(bucketName);

                Console.WriteLine("Delete bucket name:{0} succeeded ", bucketName);
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error info: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}", 
                                  ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
        }

        public static void DeleteNoEmptyBucket(string bucketName)
        {
            try
            {
                var keys = new List<string>();
                ObjectListing result = null;
                string nextMarker = string.Empty;
                do
                {
                    var listObjectsRequest = new ListObjectsRequest(bucketName)
                    {
                        Marker = nextMarker,
                        MaxKeys = 100
                    };
                    result = client.ListObjects(listObjectsRequest);

                    foreach (var summary in result.ObjectSummaries)
                    {
                        keys.Add(summary.Key);
                    }
                    
                    nextMarker = result.NextMarker;

					if (keys.Count != 0)
					{
                    	client.DeleteObjects(new DeleteObjectsRequest(bucketName, keys));
                    	keys.Clear();
					}
                } while (result.IsTruncated);

				client.DeleteBucket(bucketName);

                Console.WriteLine("Delete bucket name:{0} succeeded ", bucketName);
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error info: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                                  ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
        }
    }
}