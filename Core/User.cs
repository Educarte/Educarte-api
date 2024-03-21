using Core.Enums;
using Core.Interfaces;

namespace Core;

public class User : IEntity, IDeletable
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Cellphone { get; set; }
    public string LegalGuardianType { get; set; }
    public string Profession { get; set; }
    public string Workplace { get; set; }

    public Profile Profile { get; set; }
    public Status Status { get; set; }

    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }

    public Guid AddressId { get; set; }
    public Address Address { get; set; }

    public IList<ResetPasswordCode> ResetPasswordCodes { get; set; }
    public IList<Student> Childs { get; set; }
    public IList<Classroom> Classrooms { get; set; }
    public IList<Diary> Diaries { get; set; }

    public DateTime? FirstAccess { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}