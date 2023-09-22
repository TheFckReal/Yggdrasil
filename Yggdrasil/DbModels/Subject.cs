using System;
using System.Collections.Generic;

namespace Yggdrasil.DbModels;

public partial class Subject
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Teacher { get; set; }

    public DateOnly? Ending { get; set; }

    public virtual ICollection<Homework> Homeworks { get; set; } = new List<Homework>();
}
