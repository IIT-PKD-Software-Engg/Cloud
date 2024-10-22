using FileUploader.Models;

namespace FileUploader
{
    public class BlobResponseDto
    {
        public BlobResponseDto()
        {
            Blob = new BlobDetails();
        }

        public string? Status {  get; set; }
        public bool Error { get; set; }

        public BlobDetails Blob { get; set; }
    }
}
