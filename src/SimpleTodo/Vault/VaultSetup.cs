using Microsoft.Extensions.Configuration.Memory;
using VaultSharp;
using VaultSharp.Extensions.Configuration;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods.UserPass;

namespace SimpleTodo.Vault;

public static class VaultSetup
{
    public static IServiceCollection AddVault(
        this IServiceCollection services,
        IConfigurationBuilder configurationBuilder,
        string configurationSectionName = "Vault")
    {
        if (string.IsNullOrWhiteSpace(configurationSectionName))
        {
            throw new MissingVaultConfigurationException(configurationSectionName);
        }

        var vaultConfig = GetSection<VaultConfiguration>(configurationBuilder.Build(), configurationSectionName);

        AbstractAuthMethodInfo authMethod;
        if (string.IsNullOrWhiteSpace(vaultConfig.Token))
        {
            authMethod = new UserPassAuthMethodInfo(vaultConfig.UserName, vaultConfig.Password);
        }
        else
        {
            authMethod = new TokenAuthMethodInfo(vaultConfig.Token);
        }
        
        if (vaultConfig.LoadConfiguration)
        {
            configurationBuilder.AddVaultConfiguration(
                () => new VaultOptions(
                    vaultConfig.Url,
                    authMethod,
                    reloadOnChange: vaultConfig.ConfigRefreshEnabled,
                    reloadCheckIntervalSeconds: vaultConfig.ConfigRefreshInterval),
                vaultConfig.BasePath,
                mountPoint: vaultConfig.MountPoint
            );

            if (vaultConfig.ConfigRefreshEnabled)
            {
                services.AddHostedService<VaultChangeWatcher>();
            }
        }

        var dynamicCredentials = vaultConfig.DynamicCredentials.First();
        var vaultClientSettings = new VaultClientSettings(vaultConfig.Url, authMethod);
        IVaultClient vaultClient = new VaultClient(vaultClientSettings);
        var result = vaultClient
            .V1.Secrets.Database
            .GetCredentialsAsync(dynamicCredentials.RoleName, dynamicCredentials.MountPoint)
            .GetAwaiter().GetResult();
        var data = result.Data;

        var replacedSetting = dynamicCredentials
            .ValueTemplate
            .Replace("{{user}}", data.Username)
            .Replace("{{password}}", data.Password);

        var configToReplace = new Dictionary<string, string>
        {
            { dynamicCredentials.ConfigSectionToReplace, replacedSetting }
        };
        
        var source = new MemoryConfigurationSource { InitialData = configToReplace };
        configurationBuilder.Add(source);
        
        return services;
    }

    public static IServiceCollection AddVault2(
        this IServiceCollection services,
        IConfigurationBuilder configurationBuilder,
        string configurationSectionName = "Vault")
    {
        IAuthMethodInfo authMethod = new TokenAuthMethodInfo("MY_VAULT_TOKEN");
        var vaultClientSettings = new VaultClientSettings("https://MY_VAULT_SERVER:8200", authMethod);
        IVaultClient vaultClient = new VaultClient(vaultClientSettings);
        var result = vaultClient.V1.Secrets.Database.GetCredentialsAsync("todo-app-role", "todo-postgresql").GetAwaiter().GetResult();
        return services;
    }

    private static T GetSection<T>(IConfiguration configuration, string sectionName) where T : class, new()
    {
        var section = configuration.GetRequiredSection(sectionName);
        var model = new T();
        section.Bind(model);

        return model;
    }
}
