namespace Restia.Common.Entities.Interfaces
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        DateTimeOffset? DeletedAt { get; set; }
        string? DeletedBy { get; set; }
    }
}
