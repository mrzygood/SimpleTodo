namespace SimpleTodo.Vault.DynamicCredentials;

public sealed class LeasesRenewalScheduler : BackgroundService
{
    private static TimeSpan Interval => TimeSpan.FromSeconds(10);
    
    private readonly IConfiguration _configuration;
    private readonly LeasesStore _leasesStore;
    private readonly VaultConfiguration _vaultConfiguration;
    private readonly VaultAccessor _vaultAccessor;
    private readonly ILogger<LeasesRenewalScheduler> _logger;

    public LeasesRenewalScheduler(
        IConfiguration configuration,
        LeasesStore leasesStore,
        VaultConfiguration vaultConfiguration,
        VaultAccessor vaultAccessor,
        ILogger<LeasesRenewalScheduler> logger)
    {
        _configuration = configuration;
        _leasesStore = leasesStore;
        _vaultConfiguration = vaultConfiguration;
        _vaultAccessor = vaultAccessor;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var configurationRoot = _configuration as IConfigurationRoot;
        if (configurationRoot is null)
        {
            throw new Exception($"{nameof(IConfigurationRoot)} is expected to be available");
        }
        
        var configurationProvider = (DynamicCredentialsProvider) configurationRoot.Providers.First(x => x is DynamicCredentialsProvider);
        
        while (stoppingToken.IsCancellationRequested is false)
        {
            await RenewLeaseOrGenerateNewCredentials(configurationProvider, stoppingToken);
        }
    }
    
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        var leases = await _leasesStore.GetAll();
        foreach (var lease in leases)
        {
            await _vaultAccessor.RevokeLease(lease.LeaseId);
        }

        await base.StopAsync(cancellationToken);
    }

    private async Task RenewLeaseOrGenerateNewCredentials(
        DynamicCredentialsProvider configurationProvider,
        CancellationToken stoppingToken)
    {
        var leases = await _leasesStore.GetAll();
        var deadline = DateTime.UtcNow.Add(Interval).AddSeconds(5);
        
        foreach (var lease in leases.Where(x => x.ExpiresAt <= deadline))
        {
            var configEntry = _vaultConfiguration
                .DynamicCredentials
                .FirstOrDefault(x =>
                    x.ConfigSectionToReplace == lease.RelatedConfigKey);
            
            if (configEntry is null || configEntry.AutoRenewal is false)
            {
                continue;
            }

            var newLeaseDuration = await _vaultAccessor.RenewLease(lease.LeaseId, lease.LeaseDuration);
            lease.AdjustExpiration(DateTime.UtcNow.Add(newLeaseDuration));

            _logger.LogInformation("Renewed lease '{LeaseId}' for next {LeaseDuration}s", lease.LeaseId, (int)newLeaseDuration.TotalSeconds);

            var shouldGenerateNewCredentials =
                newLeaseDuration.TotalSeconds < lease.LeaseDuration.TotalSeconds &&
                lease.ExpiresAt <= deadline;
            
            if (shouldGenerateNewCredentials)
            {
                _logger.LogInformation("Renewed lease '{LeaseId}' max TTL will expire soon at {LeaseEnd}", lease.LeaseId, lease.ExpiresAt);
                
                var newlyGeneratedCredentials = await _vaultAccessor.GenerateDatabaseCredentials(
                    configEntry.RoleName,
                    configEntry.MountPoint);
                
                var replacedSetting = configEntry
                    .ValueTemplate
                    .Replace("{{user}}", newlyGeneratedCredentials.Data.Username)
                    .Replace("{{password}}", newlyGeneratedCredentials.Data.Password);

                // For debugging purposes only
                Console.WriteLine(
                    "Generated new credentials: user={0} | password={1} as replacement for lease '{2}'",
                    newlyGeneratedCredentials.Data.Username,
                    newlyGeneratedCredentials.Data.Password,
                    lease.LeaseId);
                
                configurationProvider.UpdateSettings(
                    new KeyValuePair<string, string>(configEntry.ConfigSectionToReplace, replacedSetting));

                await _leasesStore.AddNewLease(
                    new Lease(
                        newlyGeneratedCredentials.LeaseId,
                        TimeSpan.FromSeconds(newlyGeneratedCredentials.LeaseDurationSeconds),
                        configEntry.ConfigSectionToReplace));

                await _leasesStore.RemoveLease(lease.LeaseId);
            }
        }
        
        await Task.Delay(Interval, stoppingToken);
    }
}
