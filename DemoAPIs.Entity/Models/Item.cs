using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DemoAPIs.Entity.Models;

[Table("items")]
public partial class Item
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name", TypeName = "character varying")]
    public string? Name { get; set; }

    [Column("userid")]
    public int? Userid { get; set; }

    [ForeignKey("Userid")]
    [InverseProperty("Items")]
    public virtual User? User { get; set; }
}
