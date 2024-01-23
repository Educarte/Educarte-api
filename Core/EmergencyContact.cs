using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core;

public class EmergencyContact : IEntity, IDeletable
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string Telephone { get; set; }

    public Guid StudentId { get; set; }
    public Student Student { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
