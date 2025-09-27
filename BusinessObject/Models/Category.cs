﻿using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public int? UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Icon { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual User? User { get; set; }
}
