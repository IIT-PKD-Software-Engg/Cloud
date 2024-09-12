// This is done using Ceph

using System;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;


// Creating a connection
string accessKey = "Put your access key here!";
string secretKey = "Put your secret key here!";

AmazonS3Config config = new AmazonS3Config();
config.ServiceURL = "objects.dreamhost.com";

AmazonS3Client s3Client = new AmazonS3Client(
        accessKey,
        secretKey,
        config
        );

// Listing buckets
// There should only be one bucket, which is further divided
ListBucketsResponse response = client.ListBuckets();
foreach (S3Bucket b in response.Buckets)
{
    Console.WriteLine("{0}\t{1}", b.BucketName, b.CreationDate);
}

// Creating a new Bucket
PutBucketRequest request = new PutBucketRequest();
request.BucketName = "my-new-bucket";
client.PutBucket(request);

// Listing a bucket's content
ListObjectsRequest request = new ListObjectsRequest();
request.BucketName = "my-new-bucket";
ListObjectsResponse response = client.ListObjects(request);
foreach (S3Object o in response.S3Objects)
{
    Console.WriteLine("{0}\t{1}\t{2}", o.Key, o.Size, o.LastModified);
}

// Delete Empty Bucket (cannot be done if non-empty)
DeleteBucketRequest request = new DeleteBucketRequest();
request.BucketName = "my-new-bucket";
client.DeleteBucket(request);

// Creating an object and putting it into a bucket
PutObjectRequest request = new PutObjectRequest();
request.BucketName = "my-new-bucket";
request.Key = "hello.txt";
request.ContentType = "text/plain";
request.ContentBody = "Hello World!";
client.PutObject(request);

// Access setters
PutACLRequest request = new PutACLRequest();
request.BucketName = "my-new-bucket";
request.Key = "hello.txt";
request.CannedACL = S3CannedACL.PublicRead;
client.PutACL(request);

PutACLRequest request2 = new PutACLRequest();
request2.BucketName = "my-new-bucket";
request2.Key = "secret_plans.txt";
request2.CannedACL = S3CannedACL.Private;
client.PutACL(request2);

// Download object into file
GetObjectRequest request = new GetObjectRequest();
request.BucketName = "my-new-bucket";
request.Key = "file_name";
GetObjectResponse response = client.GetObject(request);
response.WriteResponseStreamToFile("file address");

// Delete an object
DeleteObjectRequest request = new DeleteObjectRequest();
request.BucketName = "my-new-bucket";
request.Key = "file_name";
client.DeleteObject(request);

// Getting signed urls
GetPreSignedUrlRequest request = new GetPreSignedUrlRequest();
request.BucketName = "my-bucket-name";
request.Key = "secret_plans.txt";
request.Expires = DateTime.Now.AddHours(1);
request.Protocol = Protocol.HTTP;
string url = client.GetPreSignedURL(request);
Console.WriteLine(url);
