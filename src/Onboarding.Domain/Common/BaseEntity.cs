// Domain/Common/BaseEntity.cs (Update existing)
namespace Onboarding.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; internal set; }
    public DateTime CreatedAt { get; internal set; }
    public Guid? CreatedBy { get; internal set; }
    public DateTime? UpdatedAt { get; internal set; }
    public Guid? UpdatedBy { get; internal set; }
}