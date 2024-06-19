using Domain.Entities.Base;

namespace Domain.Entities;

public class User : Entity
{
    public string? Name { get; set; }
}