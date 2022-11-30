namespace MinioWebClient;

public static class Config
{
    public static Uri MinioUri => new Uri(GetRequiredValue("MINIO_URI"));
    public static string MinioBucket => GetRequiredValue("MINIO_BUCKET");
    public static string MinioAccessKey => GetRequiredValue("MINIO_ACCESS_KEY");
    public static string MinioSecretKey => GetRequiredValue("MINIO_SECRET_KEY");
    
    private static string GetRequiredValue(string name) =>
        Environment.GetEnvironmentVariable(name)
        ?? throw new NullReferenceException($"Environment variable {name} is not set");
}