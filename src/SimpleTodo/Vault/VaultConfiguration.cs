namespace SimpleTodo.Vault;

public sealed class VaultConfiguration
{
    public VaultConfiguration()
    {
    }

    public string Url { get; set; }
    public string Token { get; set; }
    public string BasePath { get; set; }
    public string MountPoint { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public bool LoadConfiguration { get; set; }
    public bool ConfigRefreshEnabled { get; set; }
    public int ConfigRefreshInterval { get; set; }
    public ICollection<DynamicCredentialsEntry> DynamicCredentials { get; set; }
}
