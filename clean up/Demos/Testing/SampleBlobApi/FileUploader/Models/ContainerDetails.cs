using Azure.Storage.Blobs.Models;

namespace FileUploader.Models
{
    public class ContainerDetails
    {
        public string? Name { get; set; }
        public DateTimeOffset? LastModified { get; set; }
        public IDictionary<string, string> MetaData { get; set; }
        public PublicAccessType? PublicAccess {  get; set; }
    }
}
