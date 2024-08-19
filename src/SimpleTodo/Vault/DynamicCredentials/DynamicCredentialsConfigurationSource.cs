namespace SimpleTodo.Vault.DynamicCredentials;

public class DynamicCredentialsConfigurationSource : IConfigurationSource
{
    public IEnumerable<KeyValuePair<string, string?>>? InitialData { get; set; }
    
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new DynamicCredentialsProvider(this);
    }
}
