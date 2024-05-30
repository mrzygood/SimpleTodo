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
        IConfiguration configuration,
        string configurationSectionName = "Vault")
    {
        if (string.IsNullOrWhiteSpace(configurationSectionName))
        {
            throw new Exception("Vault config not found");
        }

        var vaultConfig = GetSection<VaultConfiguration>(configuration, configurationSectionName);
        services.AddSingleton(configuration);

        if (vaultConfig.LoadConfiguration)
        {
            AbstractAuthMethodInfo authMethod;
            if (string.IsNullOrWhiteSpace(vaultConfig.Token))
            {
                authMethod = new UserPassAuthMethodInfo(vaultConfig.UserName, vaultConfig.Password);
            }
            else
            {
                authMethod = new TokenAuthMethodInfo(vaultConfig.Token);
            }
            
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
