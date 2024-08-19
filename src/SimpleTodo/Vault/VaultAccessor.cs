using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.AppRole;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;
using VaultSharp.V1.SecretsEngines;

namespace SimpleTodo.Vault;

public sealed class VaultAccessor
{
    private readonly VaultConfiguration _vaultConfiguration;

    public VaultAccessor(VaultConfiguration vaultConfiguration)
    {
        _vaultConfiguration = vaultConfiguration;
    }

    public async Task RevokeLease(string leaseId)
    {
        var vaultClientSettings = new VaultClientSettings(_vaultConfiguration.Url, await GetAuthMethod());
        IVaultClient vaultClient = new VaultClient(vaultClientSettings);
        await vaultClient.V1.System.RevokeLeaseAsync(leaseId);
    }

    public async Task<TimeSpan> RenewLease(string leaseId, TimeSpan renewTtl)
    {
        var vaultClientSettings = new VaultClientSettings(_vaultConfiguration.Url, await GetAuthMethod());
        IVaultClient vaultClient = new VaultClient(vaultClientSettings);
        var result = await vaultClient
            .V1.System.RenewLeaseAsync(leaseId, (int)renewTtl.TotalSeconds);

        if (result.Renewable is false || result.Warnings is not null && result.Warnings.Any())
        {
            Console.WriteLine("Is not renewable");
        }
        
        return TimeSpan.FromSeconds(result.LeaseDurationSeconds);
    }

    public async Task<Secret<UsernamePasswordCredentials>> GenerateDatabaseCredentials(
        string roleName,
        string mountPoint)
    {
        var vaultClientSettings = new VaultClientSettings(_vaultConfiguration.Url, await GetAuthMethod());
        IVaultClient vaultClient = new VaultClient(vaultClientSettings);
        
        return await vaultClient
            .V1.Secrets.Database
            .GetCredentialsAsync(roleName, mountPoint);
    }
    
    public Task<AbstractAuthMethodInfo> GetAuthMethod()
    {
        AbstractAuthMethodInfo authMethod;
        if (string.IsNullOrWhiteSpace(_vaultConfiguration.Token))
        {
            authMethod = new AppRoleAuthMethodInfo(_vaultConfiguration.AppRoleId, _vaultConfiguration.AppRoleSecretId);
        }
        else
        {
            authMethod =  new TokenAuthMethodInfo(_vaultConfiguration.Token);
        }

        return Task.FromResult(authMethod);
    }
}
