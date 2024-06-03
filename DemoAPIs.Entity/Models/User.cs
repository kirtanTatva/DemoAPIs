using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DemoAPIs.Entity.Models;

[Table("users")]
public partial class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public int? Name { get; set; }

    [Column("email", TypeName = "character varying")]
    public string? Email { get; set; }

    [Column("password", TypeName = "character varying")]
    public string? Password { get; set; }

    [Column("refreshtoken", TypeName = "character varying")]
    public string? Refreshtoken { get; set; }

    [Column("expiresat")]
    public DateTime? Expiresat { get; set; }

    [Column("forgotpasswordtoken", TypeName = "character varying")]
    public string? Forgotpasswordtoken { get; set; }

    [Column("forgottokenexpiry")]
    public DateTime? Forgottokenexpiry { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
