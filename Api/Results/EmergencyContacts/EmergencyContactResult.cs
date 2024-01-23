using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Results.EmergencyContacts;

public class EmergencyContactResult
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Telephone { get; set; }
}
