using ComUnity.Application.Common;

namespace ComUnity.Application.Features.Authentication.Entities;

public class AuthenticationUser : IHasDomainEvent
{
    public Guid Id { get; private set; }

    public string Email { get; private set; }

    public string HashedPassword { get; private set; }

    public bool IsEmailVerified { get; private set; }

    public string SecurityCode { get; private set; }

    public DateTime SecurityCodeExpirationDate { get; private set; }

    public string Role { get; private set; }

    public List<DomainEvent> DomainEvents { get; } = new List<DomainEvent>();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public AuthenticationUser() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public AuthenticationUser(Guid id, string email, string hashedPassword, string emailVerificationCode, DateTime emailVerificationCodeExpirationDate)
    {
        Id = id;
        Email = email;
        HashedPassword = hashedPassword;
        IsEmailVerified = false;
        SecurityCode = emailVerificationCode;
        SecurityCodeExpirationDate = emailVerificationCodeExpirationDate;
        Role = UserRoles.User;
    }

    public void VerifyEmail()
    {
        IsEmailVerified = true;
        SecurityCode = string.Empty;
        SecurityCodeExpirationDate = DateTime.MinValue;
    }

    public void SetPasswordResetSecurityCode(string securityCode, DateTime expirationDate)
    {
        SecurityCode = securityCode;
        SecurityCodeExpirationDate = expirationDate;
    }

    public void SetNewPassword(string hashedPassword)
    {
        HashedPassword = hashedPassword;
    }
}