﻿using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using CoviIDApiCore.Models.AppSettings;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.Interfaces.Brokers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CoviIDApiCore.V1.Brokers
{
    public class AmazonS3Broker : IAmazonS3Broker
    {
        private AmazonS3Client _client;
        private const string bucketName = "covid-files-images";
        public AmazonS3Broker(AwsS3BucketCredentials credentials)
        {
            _client = new AmazonS3Client(new BasicAWSCredentials(credentials.Accesskey, credentials.SecretKey),
                RegionEndpoint.EUWest1);
        }

        public async Task<string> AddImageToBucket(string file, string fileName)
        {
            var bytes = Convert.FromBase64String(file);
            var image = $"{fileName}.png";

            var upload = new PutObjectRequest
            {
                CannedACL = S3CannedACL.Private,
                BucketName = bucketName,
                Key = image
            };
            try
            {
                using (var memoryStream = new MemoryStream(bytes))
                {
                    upload.InputStream = memoryStream;
                    await _client.PutObjectAsync(upload);
                }
            }
            catch (Exception)
            {
                throw new AmazonS3Exception(Messages.S3_FailedToAdd);
            }

            return fileName;
        }

        public async Task<string> GetImage(string fileName)
        {
            var preSignedUrl = new GetPreSignedUrlRequest
            {
                ContentType = "image/png",
                BucketName = bucketName,
                Expires = DateTime.Now.AddMinutes(30),
                Key = $"{fileName}.png"
            };
            var urlToPhoto = _client.GetPreSignedURL(preSignedUrl);

            if (string.IsNullOrEmpty(urlToPhoto))
                throw new AmazonS3Exception(Messages.S3_NotFound);

            return urlToPhoto;
        }
    }
}