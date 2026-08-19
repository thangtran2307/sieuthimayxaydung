namespace Marketplace.Domain.Common;

/// <summary>
/// Base for all persistence-tracked types that carry a typed primary key. Most domain types use
/// <c>Entity&lt;Guid&gt;</c>. The key property is always named <c>Id</c>; the mapped column name
/// is configured explicitly in the EF configuration file.
/// </summary>
public abstract class Entity<TKey> : Entity
{
    public TKey Id { get; protected set; }
}
