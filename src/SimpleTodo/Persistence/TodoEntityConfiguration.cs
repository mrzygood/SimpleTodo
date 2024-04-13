using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleTodo.Entities;

namespace SimpleTodo.Persistence;

public sealed class TodoEntityConfiguration : IEntityTypeConfiguration<Todo>
{
    public void Configure(EntityTypeBuilder<Todo> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.Title).IsRequired();
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
}
