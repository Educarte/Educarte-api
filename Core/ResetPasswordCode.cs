namespace Core;

public class ResetPasswordCode
{
    public Guid Id { get; set; }
    public string Code { get; set; }

    public DateTime ExpiresAt { get; set; }
    public DateTime? ConsumedAt { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }
}
