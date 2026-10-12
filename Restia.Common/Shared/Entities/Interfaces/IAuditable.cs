namespace Restia.Common.Entities.Interfaces
{
    public interface IAuditable
    {
        DateTimeOffset? CreatedAt { get; set; }
        string? CreatedUserId { get; set; }
        DateTimeOffset? UpdatedAt { get; set; }
        string? UpdatedUserId { get; set; }
    }
}
