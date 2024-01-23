using Core.Enums;
using Core.Interfaces;

namespace Core;

public class Student : IEntity, IDeletable
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string Naturalidade { get; set; }
    public string HealthProblem { get; set; }
    public string AllergicFood { get; set; }
    public string AllergicMedicine { get; set; }
    public bool Epilepsy { get; set; }
    public string AllergicBugBite { get; set; }
    public string SpecialChild { get; set; }
    public bool SpecialChildHasReport { get; set; }


    public Genre Genre { get; set; }
    public BloodType BloodType { get; set; }
    public Time Time { get; set; }
    public Status Status { get; set; }

    //TODO: Criar entidade para armazenar contrato com horas contratadas
    //TODO: Relatório para retornar horas a mais (contradado)

    public IList<User> LegalGuardians { get; set; }

    public Guid ClassroomId { get; set; }
    public Classroom Classroom { get; set; }

    public IList<AccessControl> AccessControls { get; set; }
    public IList<Diary> Diaries { get; set; }
    public IList<ContractedHour> ContractedHours { get; set; }
    public IList<EmergencyContact> EmergencyContacts { get; set; }

    public DateTime BirthDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
