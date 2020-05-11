namespace CoviIDApiCore.Models.AppSettings
{
    public class AwsS3BucketCredentials
    {
        public string Accesskey { get; set; }
        public string SecretKey { get; set; }
        public string BucketName { get; set; }
        public int ExpiresInMinutes { get; set; }
    }
}
