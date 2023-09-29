using System;
using System.Collections.Generic;

namespace Yggdrasil.DbModels;

public partial class Homework
{
    public int Id { get; set; }

    public string? Description { get; set; }

    public DateTime Deadline { get; set; }

    public bool Finished { get; set; }

    public int? Subjectid { get; set; }

    public virtual ICollection<File> Files { get; set; } = new List<File>();

    public virtual Subject? Subject { get; set; }
}
