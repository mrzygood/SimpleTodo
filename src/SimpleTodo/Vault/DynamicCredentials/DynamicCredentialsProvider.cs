namespace SimpleTodo.Vault.DynamicCredentials;

// Inspired by MemoryConfigurationProvider
// https://github.com/dotnet/runtime/blob/0d79e6de8dbc26a44dde336a9a680940e92e9904/src/libraries/Microsoft.Extensions.Configuration/src/MemoryConfigurationProvider.cs
public class DynamicCredentialsProvider : ConfigurationProvider
{
    private readonly DynamicCredentialsConfigurationSource _source;

    public DynamicCredentialsProvider(DynamicCredentialsConfigurationSource source)
    {
        _source = source;

        if (_source.InitialData != null)
        {
            foreach (KeyValuePair<string, string?> pair in _source.InitialData)
            {
                Data.Add(pair.Key, pair.Value);
            }
        }
    }

    public void UpdateSettings(KeyValuePair<string, string> settings)
    {
        Data[settings.Key] = settings.Value;
        
        OnReload();
    }
}
