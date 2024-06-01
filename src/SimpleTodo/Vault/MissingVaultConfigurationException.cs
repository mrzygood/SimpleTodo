namespace SimpleTodo.Vault;

public sealed class MissingVaultConfigurationException : Exception
{
    public MissingVaultConfigurationException(string configSectionName)
        : base($"Configuration section '{configSectionName}' for Vault is missing")
    {
        
    }
}
