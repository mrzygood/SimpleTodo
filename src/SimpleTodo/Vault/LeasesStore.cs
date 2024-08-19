using System.Collections.ObjectModel;

namespace SimpleTodo.Vault;

public sealed class LeasesStore
{
    private readonly List<Lease> _leases = new ();

    public Task<ReadOnlyCollection<Lease>> GetAll() => Task.FromResult(_leases.ToList().AsReadOnly());
    
    public Task AddNewLease(Lease lease)
    {
        _leases.Add(lease);
        return Task.CompletedTask;
    }
    
    public Task RemoveLease(string leaseId)
    {
        _leases.RemoveAll(x => x.LeaseId == leaseId);
        return Task.CompletedTask;
    }
}

public record Lease(string LeaseId, TimeSpan LeaseDuration, string RelatedConfigKey)
{
    public DateTime ExpiresAt { get; private set; } = DateTime.UtcNow.Add(LeaseDuration);

    public void AdjustExpiration(DateTime newExpiration) => ExpiresAt = newExpiration;
};
