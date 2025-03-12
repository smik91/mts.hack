using Midiot.Data.Entities;

namespace Midiot.Infrastructure.Entities;

public class ConfirmationCodeEntity
{
    public Guid Id { get; set; }
    public string Value { get; set; }
    public bool IsUsed { get; set; }
    public DateTime Expiration { get; set; }
    public string Metadata { get; set; }
    public Guid UserId { get; set; }
    public virtual UserEntity User { get; set; }
}
