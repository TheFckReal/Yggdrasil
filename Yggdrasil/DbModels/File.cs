using System;
using System.Collections.Generic;
using Microsoft.Build.Framework;

namespace Yggdrasil.DbModels;

public partial class File
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public byte[] Data { get; set; } = null!;

    public int HomeworkId { get; set; }

    public virtual Homework Homework { get; set; } = null!;
}
