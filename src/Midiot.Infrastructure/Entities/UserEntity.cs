using Midiot.Infrastructure.Entities;

namespace Midiot.Data.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email {  get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsTwoFactorAuthEnabled { get; set; }
    public string PasswordHash { get; set; }
    public virtual ICollection<RefreshTokenEntity> RefreshTokens { get; set; }
    public virtual ICollection<ConfirmationCodeEntity> ConfirmationCodes { get; set; }
}
