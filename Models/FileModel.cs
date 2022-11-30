using Amazon.S3.Model;

namespace MinioWebClient.Models;

public class FileModel
{
    public FileModel(S3Object s3Object)
    {
        Key = s3Object.Key;
        Name = Path.GetFileName(s3Object.Key);
        Size = s3Object.Size;
    }

    public string Key { get; init; }
    public string Name { get; init; }
    public long Size { get; init; }
}