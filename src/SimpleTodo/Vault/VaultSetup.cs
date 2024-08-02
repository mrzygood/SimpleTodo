using VaultSharp.Extensions.Configuration;
using VaultSharp.V1.AuthMethods.Token;

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

        if (vaultConfig.LoadConfiguration)
        {
            var authMethod = new TokenAuthMethodInfo(vaultConfig.Token);
            
            configurationBuilder.AddVaultConfiguration(
                () => new VaultOptions(
                    vaultConfig.Url,
                    authMethod),
                vaultConfig.BasePath,
                mountPoint: vaultConfig.MountPoint
            );
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
