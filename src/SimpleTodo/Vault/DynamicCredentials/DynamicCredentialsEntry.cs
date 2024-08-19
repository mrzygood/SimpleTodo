namespace SimpleTodo.Vault.DynamicCredentials;

public sealed class DynamicCredentialsEntry
{
    public DynamicCredentialsEntry()
    {
    }

    public string ConfigSectionToReplace { get; set; }
    public string ValueTemplate { get; set; }
    public string MountPoint { get; set; }
    public string RoleName { get; set; }
    public bool AutoRenewal { get; set; }
}
