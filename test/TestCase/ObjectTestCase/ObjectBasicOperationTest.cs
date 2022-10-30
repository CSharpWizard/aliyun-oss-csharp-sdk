﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Test.Util;

using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.ObjectTestClass
{
    [TestFixture]
    public class ObjectBasicOperationTest
    {
        private static IOss _ossClient;
        private static string _className;
        private static string _bucketName;
        private static string _objectKey;
        private static string _objectETag;

        [TestFixtureSetUp]
        public static void ClassInitialize()
        {
            //get a OSS client object
            _ossClient = OssClientFactory.CreateOssClient();
            //get current class name, which is prefix of bucket/object
            _className = TestContext.CurrentContext.Test.FullName;
            _className = _className.Substring(_className.LastIndexOf('.') + 1).ToLowerInvariant();
            //create the bucket
            _bucketName = OssTestUtils.GetBucketName(_className);
            _ossClient.CreateBucket(_bucketName);
            //create sample object
            _objectKey = OssTestUtils.GetObjectKey(_className);
            var poResult = OssTestUtils.UploadObject(_ossClient, _bucketName, _objectKey,
                Config.UploadSampleFile, new ObjectMetadata());
            _objectETag = poResult.ETag;
        }

        [TestFixtureTearDown]
        public static void ClassCleanup()
        {
            OssTestUtils.CleanBucket(_ossClient, _bucketName);
        }

        #region stream upload

        [Test]
        public void UploadObjectBasicSettingsTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                //upload the object
                OssTestUtils.UploadObject(_ossClient, _bucketName, key,
                    Config.UploadSampleFile, new ObjectMetadata());
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        [Test]
        public void UploadObjectNullMetadataTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                //upload the object
                OssTestUtils.UploadObject(_ossClient, _bucketName, key,
                    Config.UploadSampleFile, null);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        [Test]
        public void UploadObjectDefaultMetadataTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                //upload the object
                OssTestUtils.UploadObject(_ossClient, _bucketName, key,
                    Config.UploadSampleFile);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        [Test]
        public void UploadObjectFullSettingsTest()
        {
            //test folder structure
            var folder = OssTestUtils.GetObjectKey(_className);
            var key = folder + "/" + OssTestUtils.GetObjectKey(_className);

            //config metadata
            var metadata = new ObjectMetadata
            {
                CacheControl = "no-cache",
                ContentDisposition = "abc.zip",
                ContentEncoding = "gzip"
            };
            var eTag = FileUtils.ComputeContentMd5(Config.UploadSampleFile);
            metadata.ETag = eTag;
            //enable server side encryption
            const string encryption = "AES256";
            metadata.ServerSideEncryption = encryption;
            //user metadata
            metadata.UserMetadata.Add("MyKey1", "MyValue1");
            metadata.UserMetadata.Add("MyKey2", "MyValue2");

            try
            {
                //upload object
                OssTestUtils.UploadObject(_ossClient, _bucketName, key,
                    Config.UploadSampleFile, metadata);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var uploadedObjectMetadata = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual(eTag.ToLowerInvariant(), uploadedObjectMetadata.ETag.ToLowerInvariant());
                Assert.AreEqual(encryption, uploadedObjectMetadata.ServerSideEncryption);
                Assert.AreEqual(2, uploadedObjectMetadata.UserMetadata.Count);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        #endregion

        #region file upload

        [Test]
        public void UploadObjectSpecifyFilePathTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                _ossClient.PutObject(_bucketName, key, Config.UploadSampleFile, new ObjectMetadata());
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        [Test]
        public void UploadObjectSpecifyFilePathDefaultMetadataTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                _ossClient.PutObject(_bucketName, key, Config.UploadSampleFile);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        [Test]
        public void UploadBigObjectTestWithObjectLessThanPartSize()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var fileInfo = new FileInfo(Config.MultiUploadSampleFile);
                var fileSize = fileInfo.Length;

                var result = _ossClient.PutBigObject(_bucketName, key, Config.MultiUploadSampleFile, new ObjectMetadata(), fileSize + 1);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
                Assert.IsTrue(result.ETag.Length > 0);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        [Test]
        public void UploadBigObjectTestWithObjectEqualPartSize()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var fileInfo = new FileInfo(Config.MultiUploadSampleFile);
                var fileSize = fileInfo.Length;

                var result = _ossClient.PutBigObject(_bucketName, key, Config.MultiUploadSampleFile, new ObjectMetadata(), fileSize);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
                Assert.IsTrue(result.ETag.Length > 0);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        [Test]
        public void UploadBigObjectTestWithObjectMoreThanPartSize()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var fileInfo = new FileInfo(Config.MultiUploadSampleFile);
                var fileSize = fileInfo.Length;

                var result = _ossClient.PutBigObject(_bucketName, key, Config.MultiUploadSampleFile, new ObjectMetadata(), fileSize - 1);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
                Assert.IsTrue(result.ETag.Length > 0);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        [Test]
        public void UploadBigObjectTestWithObjectPartSizeTooSmall()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var fileInfo = new FileInfo(Config.MultiUploadSampleFile);
                var fileSize = fileInfo.Length;

                var result = _ossClient.PutBigObject(_bucketName, key, Config.MultiUploadSampleFile, new ObjectMetadata(), 1);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
                Assert.IsTrue(result.ETag.Length > 0);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        [Test]
        public void UploadBigObjectTestWithSmallObjectCheckContentType()
        {
            var key = OssTestUtils.GetObjectKey(_className);
            var newFileName = Path.GetDirectoryName(Config.UploadSampleFile) + "/newfile.js";

            try
            {
                File.Copy(Config.UploadSampleFile, newFileName);

                var result = _ossClient.PutBigObject(_bucketName, key, newFileName, new ObjectMetadata());
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
                Assert.IsTrue(result.ETag.Length > 0);

                var objectMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual("application/x-javascript", objectMeta.ContentType);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
                File.Delete(newFileName);
            }
        }

        [Test]
        public void UploadBigObjectTestWithBigObjectCheckContentType()
        {
            var key = OssTestUtils.GetObjectKey(_className);
            var newFileName = Path.GetDirectoryName(Config.MultiUploadSampleFile) + "/newfile.js";

            try
            {
                File.Copy(Config.MultiUploadSampleFile, newFileName, true);
                var fileInfo = new FileInfo(newFileName);
                var fileSize = fileInfo.Length;

                var result = _ossClient.PutBigObject(_bucketName, key, newFileName, new ObjectMetadata(), fileSize / 3);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
                Assert.IsTrue(result.ETag.Length > 0);

                var objectMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual("application/x-javascript", objectMeta.ContentType);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
                File.Delete(newFileName);
            }
        }

        #endregion

        #region invalid key
        [Test]
        public void InvalidObjectKeyTest()
        {
            foreach (var invalidKeyName in OssTestUtils.InvalidObjectKeyNamesList)
            {
                try
                {
                    //try to upload the object with invalid key name
                    OssTestUtils.UploadObject(_ossClient, _bucketName, invalidKeyName,
                        Config.UploadSampleFile, new ObjectMetadata());
                    Assert.Fail("Upload should not pass for invalid object key name {0}", invalidKeyName);
                }
                catch (ArgumentException)
                {
                    Assert.IsTrue(true);
                }
                finally
                {
                    if (OssTestUtils.ObjectExists(_ossClient, _bucketName, invalidKeyName))
                    {
                        _ossClient.DeleteObject(_bucketName, invalidKeyName);
                    }
                }
            }
        }

        #endregion

        #region get object

        [Test]
        public void GetAndDeleteNonExistObjectTest()
        {
            var key = OssTestUtils.GetObjectKey(_className);
            //non exist object
            Assert.IsFalse(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));
            try
            {
                _ossClient.GetObject(_bucketName, key);
                Assert.Fail("Get non exist object should not pass");
            }
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.NoSuchKey, e.ErrorCode);
            }
            //according to API doc, delete non-exist object return 204
            _ossClient.DeleteObject(_bucketName, key);
        }

        [Test]
        public void GetObjectBasicTest()
        {
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);
            try
            {
                OssTestUtils.DownloadObject(_ossClient, _bucketName, _objectKey, targetFile);
                var expectedETag = _ossClient.GetObjectMetadata(_bucketName, _objectKey).ETag;
                var downloadedFileETag = FileUtils.ComputeContentMd5(targetFile);
                Assert.AreEqual(expectedETag.ToLowerInvariant(), downloadedFileETag.ToLowerInvariant());
            }
            finally
            {
                try
                {
                    FileUtils.DeleteFile(targetFile);
                }
                catch (Exception e)
                {
                    LogUtility.LogWarning("Delete file {0} failed with Exception {1}", targetFile, e.Message);
                }
            }
        }

        [Test]
        public void GetObjectUsingRangeTest()
        {
            var targetFile = OssTestUtils.GetTargetFileName(_className);
            targetFile = Path.Combine(Config.DownloadFolder, targetFile);
            try
            {
                OssTestUtils.DownloadObjectUsingRange(_ossClient, _bucketName, _objectKey, targetFile);
                var expectedETag = _ossClient.GetObjectMetadata(_bucketName, _objectKey).ETag;
                var downloadedFileETag = FileUtils.ComputeContentMd5(targetFile);
                Assert.AreEqual(expectedETag.ToLowerInvariant(), downloadedFileETag.ToLowerInvariant());
            }
            finally
            {
                try
                {
                    FileUtils.DeleteFile(targetFile);
                }
                catch (Exception e)
                {
                    LogUtility.LogWarning("Delete file {0} failed with Exception {1}", targetFile, e.Message);
                }
            }
        }

        [Test]
        public void ListAllObjectsTest()
        {
            //test folder structure
            var folderName = OssTestUtils.GetObjectKey(_className);
            var key = folderName + "/" + OssTestUtils.GetObjectKey(_className);

            try
            {
                //upload the object
                OssTestUtils.UploadObject(_ossClient, _bucketName, key,
                    Config.UploadSampleFile, new ObjectMetadata());

                //list objects by specifying bucket name
                var allObjects = _ossClient.ListObjects(_bucketName);
                //default value is 100
                Assert.AreEqual(100, allObjects.MaxKeys);
                var allObjectsSumm = allObjects.ObjectSummaries.ToList();
                //there is already one sample object
                Assert.AreEqual(2, allObjectsSumm.Count);

                //list objects by specifying folder as prefix
                allObjects = _ossClient.ListObjects(_bucketName, folderName);
                Assert.AreEqual(folderName, allObjects.Prefix);
                allObjectsSumm = allObjects.ObjectSummaries.ToList();
                Assert.AreEqual(1, allObjectsSumm.Count);

                var loRequest = new ListObjectsRequest(_bucketName);
                loRequest.Prefix = folderName;
                loRequest.MaxKeys = 10;
                loRequest.Delimiter = "/";
                allObjects = _ossClient.ListObjects(loRequest);
                Assert.AreEqual(folderName, allObjects.Prefix);
                Assert.AreEqual("/", allObjects.Delimiter);
                //Assert.Fail("List objects using full conditions");
            }
            finally
            {
                _ossClient.DeleteObject(_bucketName, key);
            }
        }

        [Test]
        public void GetObjectMatchingETagPositiveTest()
        {
            var coRequest = new GetObjectRequest(_bucketName, _objectKey);
            coRequest.MatchingETagConstraints.Add(_objectETag);

            _ossClient.GetObject(coRequest);
        }

        [Test]
        public void GetObjectMatchingETagNegativeTest()
        {
            var coRequest = new GetObjectRequest(_bucketName, _objectKey);
            coRequest.MatchingETagConstraints.Add("Dummy");

            try
            {
                _ossClient.GetObject(coRequest);
                Assert.Fail("Get object should not pass with MatchingETag set to wrong value");
            }
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.PreconditionFailed, e.ErrorCode);
            }
        }

        [Test]
        public void GetObjectModifiedSincePositiveTest()
        {
            var coRequest = new GetObjectRequest(_bucketName, _objectKey);
            coRequest.ModifiedSinceConstraint = DateTime.UtcNow.AddDays(-1);

            _ossClient.GetObject(coRequest);
        }

        [Test]
        public void GetObjectModifiedSinceNegativeTest()
        {
            var coRequest = new GetObjectRequest(_bucketName, _objectKey)
            {
                ModifiedSinceConstraint = DateTime.UtcNow.AddDays(1)
            };

            try
            {
                _ossClient.GetObject(coRequest);
                Assert.Fail("Get object should not pass with ModifiedSince set to wrong value");
            }
            //according to API, return 304
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.NotModified, e.ErrorCode);
            }
        }

        [Test]
        public void GetObjectNonMatchingETagPositiveTest()
        {
            var coRequest = new GetObjectRequest(_bucketName, _objectKey);
            coRequest.NonmatchingETagConstraints.Add("Dummy");

            _ossClient.GetObject(coRequest);
        }

        [Test]
        public void GetObjectNonMatchingETagNegativeTest()
        {
            var coRequest = new GetObjectRequest(_bucketName, _objectKey);
            coRequest.NonmatchingETagConstraints.Add(_objectETag);

            try
            {
                _ossClient.GetObject(coRequest);
                Assert.Fail("Get object should not pass with NonMatchingETag set to wrong value");
            }
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.NotModified, e.ErrorCode);
            }
        }

        [Test]
        public void GetObjectUnmodifiedSincePositiveTest()
        {
            var coRequest = new GetObjectRequest(_bucketName, _objectKey);
            coRequest.UnmodifiedSinceConstraint = DateTime.UtcNow.AddDays(1);

            _ossClient.GetObject(coRequest);
        }

        [Test]
        public void GetObjectUnmodifiedSinceNegativeTest()
        {
            var coRequest = new GetObjectRequest(_bucketName, _objectKey);
            coRequest.UnmodifiedSinceConstraint = DateTime.UtcNow.AddDays(-1);

            try
            {
                _ossClient.GetObject(coRequest);
                Assert.Fail("Get object should not pass with UnmodifiedSince set to wrong value");
            }
            catch (OssException e)
            {
                Assert.AreEqual(OssErrorCode.PreconditionFailed, e.ErrorCode);
            }
        }

        #endregion

        #region delete object

        [Test]
        public void DeleteMultiObjectsVerboseTest()
        {
            var count = new Random().Next(2, 20);
            LogUtility.LogMessage("Will create {0} objects for multi delete this time", count);
            var objectkeys = CreateMultiObjects(count);
            LogUtility.LogMessage("{0} objects have been created", count);

            var doRequest = new DeleteObjectsRequest(_bucketName, objectkeys, false);
            var doResponse = _ossClient.DeleteObjects(doRequest);
            //verbose mode would return all object keys
            Assert.AreEqual(count, doResponse.Keys.Count());
        }

        [Test]
        public void DeleteMultiObjectsQuietTest()
        {
            var count = new Random().Next(2, 20);
            LogUtility.LogMessage("Will create {0} objects for multi delete this time", count);
            var objectkeys = CreateMultiObjects(count);
            LogUtility.LogMessage("{0} objects have been created", count);

            var doRequest = new DeleteObjectsRequest(_bucketName, objectkeys);
            var doResponse = _ossClient.DeleteObjects(doRequest);
            //quiet mode won't return object keys
            Assert.IsNull(doResponse.Keys);
        }

        [Test]
        public void UploadObjectWithNoSurfix()
        {
            var key = OssTestUtils.GetObjectKey(_className) + "";
            try
            {
                using (var fs = File.Open(Config.UploadSampleFile, FileMode.Open))
                {
                    _ossClient.PutObject(_bucketName, key, fs, null);
                }

                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var coRequest = new GetObjectRequest(_bucketName, key);
                var result = _ossClient.GetObject(coRequest);
                Assert.AreEqual("application/octet-stream", result.Metadata.ContentType);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        #endregion

        #region content-type

        [Test]
        public void UploadObjectWithGenerateContentTypeByKeySurfix()
        {
            var key = OssTestUtils.GetObjectKey(_className) + ".js";
            try
            {
                using (var fs = File.Open(Config.UploadSampleFile, FileMode.Open))
                {
                    _ossClient.PutObject(_bucketName, key, fs, null);
                }

                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var coRequest = new GetObjectRequest(_bucketName, key);
                var result = _ossClient.GetObject(coRequest);
                Assert.AreEqual("application/x-javascript", result.Metadata.ContentType);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        [Test]
        public void UploadObjectWithKeySurfixAndMetadata()
        {
            var key = OssTestUtils.GetObjectKey(_className) + ".js";
            try
            {
                var metadata = new ObjectMetadata();
                metadata.ContentType = "application/vnd.android.package-archive";

                using (var fs = File.Open(Config.UploadSampleFile, FileMode.Open))
                {
                    _ossClient.PutObject(_bucketName, key, fs, metadata);
                }

                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var coRequest = new GetObjectRequest(_bucketName, key);
                var result = _ossClient.GetObject(coRequest);
                Assert.AreEqual("application/vnd.android.package-archive", result.Metadata.ContentType);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }


        [Test]
        public void UploadObjectWithKeySurfix()
        {
            var key = OssTestUtils.GetObjectKey(_className) + ".js";
            var newFileName = Path.GetDirectoryName(Config.UploadSampleFile) + "/newfile";
            try
            {
                File.Copy(Config.UploadSampleFile, newFileName, true);
                OssTestUtils.UploadObject(_ossClient, _bucketName, key, newFileName, null);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var coRequest = new GetObjectRequest(_bucketName, key);
                var result = _ossClient.GetObject(coRequest);
                Assert.AreEqual("application/octet-stream", result.Metadata.ContentType);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                    File.Delete(newFileName);
                }
            }
        }

        [Test]
        public void UploadObjectWithFileSurfix()
        {
            var key = OssTestUtils.GetObjectKey(_className);
            var newFileName = Path.GetDirectoryName(Config.UploadSampleFile) + "/newfile.js";

            try
            {
                File.Copy(Config.UploadSampleFile, newFileName, true);
                OssTestUtils.UploadObject(_ossClient, _bucketName, key, newFileName, null);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var coRequest = new GetObjectRequest(_bucketName, key);
                var result = _ossClient.GetObject(coRequest);
                Assert.AreEqual("application/x-javascript", result.Metadata.ContentType);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                    File.Delete(newFileName);
                }
            }
        }

        [Test]
        public void UploadObjectWithFileSurfix2()
        {
            var key = OssTestUtils.GetObjectKey(_className);
            var newFileName = Path.GetDirectoryName(Config.UploadSampleFile) + "/newfile.m4u";

            try
            {
                File.Copy(Config.UploadSampleFile, newFileName, true);
                OssTestUtils.UploadObject(_ossClient, _bucketName, key, newFileName, null);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var coRequest = new GetObjectRequest(_bucketName, key);
                var result = _ossClient.GetObject(coRequest);
                Assert.AreEqual("video/vnd.mpegurl", result.Metadata.ContentType);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                    File.Delete(newFileName);
                }
            }
        }

        [Test]
        public void UploadObjectWithFileSurfixAndFileSurfix()
        {
            var key = OssTestUtils.GetObjectKey(_className) + ".potx";
            var newFileName = Path.GetDirectoryName(Config.UploadSampleFile) + "/newFile.docx";

            try
            {
                File.Copy(Config.UploadSampleFile, newFileName, true);
                OssTestUtils.UploadObject(_ossClient, _bucketName, key, newFileName, null);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var coRequest = new GetObjectRequest(_bucketName, key);
                var result = _ossClient.GetObject(coRequest);
                Assert.AreEqual("application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                                result.Metadata.ContentType);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                    File.Delete(newFileName);
                }
            }
        }

        #endregion

        #region does object exist

        [Test]
        public void DoesObjectExistWithBucketNotExist()
        {
            try
            {
                const string bucketName = "not-exist-bucket";
                const string key = "one";
                try
                {
                    _ossClient.DeleteBucket(bucketName);
                }
                catch (Exception)
                {
                    //nothing
                }

                bool isExist = _ossClient.DoesObjectExist(bucketName, key);
                Assert.False(isExist);
            }
            catch (Exception e)
            {
                Assert.True(false, e.Message);
            }
        }

        [Test]
        public void DoesObjectExistWithBucketExistAndObjectNotExist()
        {
            try
            {
                string bucketName = _bucketName;
                const string key = "one";
                try
                {
                    _ossClient.DeleteObject(bucketName, key);
                }
                catch (Exception)
                {
                    //nothing
                }

                bool isExist = _ossClient.DoesObjectExist(bucketName, key);
                Assert.False(isExist);
            }
            catch (Exception e)
            {
                Assert.True(false, e.Message);
            }
        }

        [Test]
        public void DoesObjectExistWithObjecttExist()
        {
            string bucketName = _bucketName;
            const string key = "one";

            try
            {
                try
                {
                    _ossClient.PutObject(bucketName, key, Config.UploadSampleFile);
                }
                catch (Exception)
                {
                    //nothing
                }

                bool isExist = _ossClient.DoesObjectExist(bucketName, key);
                Assert.True(isExist);
            }
            catch (Exception e)
            {
                Assert.True(false, e.Message);
            }
            finally
            {
                _ossClient.DeleteObject(bucketName, key);
            }
        }

        #endregion

        #region modify object meta

        /// <summary>
        /// 原先的meta中是A，新的里面是A->B
        /// </summary>
        [Test]
        public void ModifyObjectMetaWithA2B()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var meta = new ObjectMetadata()
                {
                    ContentType = "text/rtf"
                };

                _ossClient.PutObject(_bucketName, key, Config.UploadSampleFile, meta);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var oldMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual(meta.ContentType, oldMeta.ContentType);

                meta.ContentType = "application/mac-binhex40";
                _ossClient.ModifyObjectMeta(_bucketName, key, meta);

                var newMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual(meta.ContentType, newMeta.ContentType);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        /// <summary>
        /// 原先的meta中是A，新的中仍然是A->A
        /// </summary>
        [Test]
        public void ModifyObjectMetaWithA2A()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var meta = new ObjectMetadata()
                {
                    ContentType = "text/rtf"
                };

                _ossClient.PutObject(_bucketName, key, Config.UploadSampleFile, meta);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var oldMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual(meta.ContentType, oldMeta.ContentType);

                _ossClient.ModifyObjectMeta(_bucketName, key, meta);

                var newMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual(meta.ContentType, newMeta.ContentType);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        /// <summary>
        /// 原先有两个meta：A和B。新的meta里面只有：A->C
        /// </summary>
        [Test]
        public void ModifyObjectMetaWithAB2C()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var meta = new ObjectMetadata()
                {
                    ContentType = "text/rtf",
                    CacheControl = "public"
                };

                _ossClient.PutObject(_bucketName, key, Config.UploadSampleFile, meta);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var oldMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual(meta.ContentType, oldMeta.ContentType);
                Assert.AreEqual(meta.CacheControl, oldMeta.CacheControl);

                _ossClient.ModifyObjectMeta(_bucketName, key, new ObjectMetadata() { ContentType = "application/vnd.wap.wmlc" });

                var newMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual("application/vnd.wap.wmlc", newMeta.ContentType);
                Assert.AreEqual(null, newMeta.CacheControl);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        /// <summary>
        /// 原先有一个meta：A。新的meta里面有：A->C,新增B
        /// </summary>
        [Test]
        public void ModifyObjectMetaWithA2CB()
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var meta = new ObjectMetadata()
                {
                    ContentType = "text/rtf",
                };

                _ossClient.PutObject(_bucketName, key, Config.UploadSampleFile, meta);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var oldMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual(meta.ContentType, oldMeta.ContentType);
                Assert.AreEqual(meta.CacheControl, oldMeta.CacheControl);

                var toModifyMeta = new ObjectMetadata()
                {
                    ContentType = "application/vnd.wap.wmlc",
                    CacheControl = "max-stale"
                };
                _ossClient.ModifyObjectMeta(_bucketName, key, toModifyMeta);

                var newMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual("application/vnd.wap.wmlc", newMeta.ContentType);
                Assert.AreEqual("max-stale", newMeta.CacheControl);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        /// <summary>
        /// 清空meta
        /// </summary>
        [Test]
        public void ModifyObjectMetaWithToEmpty() 
        {
            var key = OssTestUtils.GetObjectKey(_className);

            try
            {
                var meta = new ObjectMetadata()
                {
                    ContentType = "text/rtf",
                };

                _ossClient.PutObject(_bucketName, key, Config.UploadSampleFile, meta);
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var oldMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual(meta.ContentType, oldMeta.ContentType);
                Assert.AreEqual(meta.CacheControl, oldMeta.CacheControl);

                _ossClient.ModifyObjectMeta(_bucketName, key, new ObjectMetadata());

                var newMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual("application/octet-stream", newMeta.ContentType);
                Assert.AreEqual(null, newMeta.CacheControl);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
            }
        }

        /// <summary>
        /// 新设置meta,Content-Type用的是默认值
        /// </summary>
        [Test]
        public void ModifyObjectMetaWithAddMeta() 
        {
            var key = OssTestUtils.GetObjectKey(_className);
            var newFileName = Path.GetDirectoryName(Config.UploadSampleFile) + "/newfile";

            try
            {
                File.Copy(Config.UploadSampleFile, newFileName, true);

                _ossClient.PutObject(_bucketName, key, newFileName, new ObjectMetadata());
                Assert.IsTrue(OssTestUtils.ObjectExists(_ossClient, _bucketName, key));

                var oldMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual("application/octet-stream", oldMeta.ContentType);
                Assert.AreEqual(null, oldMeta.CacheControl);

                _ossClient.ModifyObjectMeta(_bucketName, key, new ObjectMetadata() { CacheControl = "public" });

                var newMeta = _ossClient.GetObjectMetadata(_bucketName, key);
                Assert.AreEqual("application/octet-stream", newMeta.ContentType);
                Assert.AreEqual("public", newMeta.CacheControl);
            }
            finally
            {
                if (OssTestUtils.ObjectExists(_ossClient, _bucketName, key))
                {
                    _ossClient.DeleteObject(_bucketName, key);
                }
                File.Delete(newFileName);
            }
        }

        #endregion

        #region private
        private static List<string> CreateMultiObjects(int objectsCount)
        {
            var sampleObjects = new List<string>();
            for (var i = 0; i < objectsCount; i++)
            {
                var objectKey = OssTestUtils.GetObjectKey(_className);
                OssTestUtils.UploadObject(_ossClient, _bucketName, objectKey,
                    Config.UploadSampleFile, new ObjectMetadata());
                sampleObjects.Add(objectKey);
                System.Threading.Thread.Sleep(100);
            }
            return sampleObjects;
        }
        #endregion
    };
}

