﻿using System;
namespace Aliyun.OSS
{
    /// <summary>
    /// Create symlink request.
    /// </summary>
    public class CreateSymlinkRequest
    {
        public CreateSymlinkRequest(string bucketName, string symlink, string target)
        {
            BucketName = bucketName;
            Symlink = symlink;
            Target = target;
        }

        public CreateSymlinkRequest(string bucketName, string symlink, string target, ObjectMetadata metadata)
            :this(bucketName, symlink, target)
        {
            ObjectMetadata = metadata;
        }

        public string BucketName { get; set; }

        public string Symlink { get; set; }

        public string Target { get; set; }

        public ObjectMetadata ObjectMetadata { get; set; }
    }
}
