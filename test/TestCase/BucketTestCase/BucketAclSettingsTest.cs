﻿using System.Linq;
using Aliyun.OSS;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.BucketTestClass
{
    [TestFixture]
    public partial class BucketSettingsTest
    {

        [Test]
        public void SetBucketAclApiTest()
        {
            //set to read
            _ossClient.SetBucketAcl(_bucketName, CannedAccessControlList.PublicRead);
            OssTestUtils.WaitForCacheExpire();
            var acl = _ossClient.GetBucketAcl(_bucketName);
            var grants = acl.Grants.ToArray();
            Assert.AreEqual(1, grants.Length);
            Assert.AreEqual(GroupGrantee.AllUsers, grants[0].Grantee);
            Assert.AreEqual(Permission.Read, grants[0].Permission);

            //set to readwrite
            _ossClient.SetBucketAcl(_bucketName, CannedAccessControlList.PublicReadWrite);
            OssTestUtils.WaitForCacheExpire();
            acl = _ossClient.GetBucketAcl(_bucketName);
            grants = acl.Grants.ToArray();
            Assert.AreEqual(1, grants.Length);
            Assert.AreEqual(GroupGrantee.AllUsers, grants[0].Grantee);
            Assert.AreEqual(Permission.FullControl, grants[0].Permission);

            //set to private
            _ossClient.SetBucketAcl(_bucketName, CannedAccessControlList.Private);
            OssTestUtils.WaitForCacheExpire();
            acl = _ossClient.GetBucketAcl(_bucketName);
            grants = acl.Grants.ToArray();
            Assert.AreEqual(0, grants.Length);
        }

        [Test]
        public void SetBucketAclUseRequestTest()
        {
            _ossClient.SetBucketAcl(
                new SetBucketAclRequest(_bucketName, CannedAccessControlList.PublicRead));
            OssTestUtils.WaitForCacheExpire();
            var acl = _ossClient.GetBucketAcl(_bucketName);
            var grants = acl.Grants.ToArray();
            Assert.AreEqual(1, grants.Length);
            Assert.AreEqual(GroupGrantee.AllUsers, grants[0].Grantee);
            Assert.AreEqual(Permission.Read, grants[0].Permission);

            //set to readwrite
            _ossClient.SetBucketAcl(
                new SetBucketAclRequest(_bucketName, CannedAccessControlList.PublicReadWrite));
            OssTestUtils.WaitForCacheExpire();
            acl = _ossClient.GetBucketAcl(_bucketName);
            grants = acl.Grants.ToArray();
            Assert.AreEqual(1, grants.Length);
            Assert.AreEqual(GroupGrantee.AllUsers, grants[0].Grantee);
            Assert.AreEqual(Permission.FullControl, grants[0].Permission);

            //set to private
            _ossClient.SetBucketAcl(
                new SetBucketAclRequest(_bucketName, CannedAccessControlList.Private));
            OssTestUtils.WaitForCacheExpire();
            acl = _ossClient.GetBucketAcl(_bucketName);
            grants = acl.Grants.ToArray();
            Assert.AreEqual(0, grants.Length);
        }
    }
}
