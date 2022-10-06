namespace Catalog.Settings;

public class MongoDBSettings
{
    public String Host { get; set; }
    public int Port { get; set; }
    public string User { get; set; }
    public string Password { get; set; }

    public string ConnectionString 
    { 
        get
        {
            return $"mongodb://{User}:{Password}@{Host}:{Port}";
        } 
    }
}