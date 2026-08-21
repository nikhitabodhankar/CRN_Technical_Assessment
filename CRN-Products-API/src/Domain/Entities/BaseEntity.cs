namespace CRN.Domain.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public string CreatedBy { get; set; } = "system";
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
}
