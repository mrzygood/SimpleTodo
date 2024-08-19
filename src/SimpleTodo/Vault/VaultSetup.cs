using SimpleTodo.Vault.DynamicCredentials;
using VaultSharp.Extensions.Configuration;

namespace SimpleTodo.Vault;

public static class VaultSetup
{
    public static async Task<IServiceCollection> AddVault(
        this IServiceCollection services,
        IConfigurationBuilder configurationBuilder,
        string configurationSectionName = "Vault")
    {
        if (string.IsNullOrWhiteSpace(configurationSectionName))
        {
            throw new MissingVaultConfigurationException(configurationSectionName);
        }

        var vaultConfig = GetSection<VaultConfiguration>(configurationBuilder.Build(), configurationSectionName);
        services.AddSingleton(vaultConfig);

        var vaultAccessor = new VaultAccessor(vaultConfig);
        services.AddSingleton(vaultAccessor);

        var vaultAuthMethod = await vaultAccessor.GetAuthMethod();
        
        if (vaultConfig.LoadConfiguration)
        {
            configurationBuilder.AddVaultConfiguration(
                () => new VaultOptions(
                    vaultConfig.Url,
                    vaultAuthMethod),
                vaultConfig.BasePath,
                mountPoint: vaultConfig.MountPoint
            );
        }

        await services.GenerateDynamicCredentials(configurationBuilder, vaultConfig, vaultAccessor);
        
        return services;
    }

    private static async Task GenerateDynamicCredentials(
        this IServiceCollection services,
        IConfigurationBuilder configurationBuilder,
        VaultConfiguration vaultConfig,
        VaultAccessor vaultAccessor)
    {        
        var leasesStore = new LeasesStore();
        services.AddSingleton(leasesStore);
        
        services.AddHostedService<LeasesRenewalScheduler>();

        var dynamicCredentialsConfigInitialData = new List<KeyValuePair<string, string?>>();
        
        foreach (var dynamicCredentials in vaultConfig.DynamicCredentials)
        {
            var result = await vaultAccessor.GenerateDatabaseCredentials(dynamicCredentials.RoleName, dynamicCredentials.MountPoint);

            await leasesStore.AddNewLease(
                new Lease(
                    result.LeaseId,
                    TimeSpan.FromSeconds(result.LeaseDurationSeconds),
                    dynamicCredentials.ConfigSectionToReplace));
            
            var replacedSetting = dynamicCredentials
                .ValueTemplate
                .Replace("{{user}}", result.Data.Username)
                .Replace("{{password}}", result.Data.Password);

            // For debugging purposes only
            Console.WriteLine("Generated credentials: user={0} | password={1} | lease_id={2}", result.Data.Username, result.Data.Password, result.LeaseId);
            
            dynamicCredentialsConfigInitialData
                .Add(new KeyValuePair<string, string?>(dynamicCredentials.ConfigSectionToReplace, replacedSetting));
        }

        var source = new DynamicCredentialsConfigurationSource { InitialData = dynamicCredentialsConfigInitialData };
        configurationBuilder.Sources.Add(source);
    }
    
    private static T GetSection<T>(IConfiguration configuration, string sectionName) where T : class, new()
    {
        var section = configuration.GetRequiredSection(sectionName);
        var model = new T();
        section.Bind(model);

        return model;
    }
}
