namespace Midiot.Data.Entities;

public class RefreshTokenEntity
{
    public Guid Id { get; set; }
    public string Value { get; set; }
    public DateTime Expiration { get; set; }
    public Guid UserId { get; set; }
    public virtual UserEntity User { get; set; }
}
