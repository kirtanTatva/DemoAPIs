using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace AppointmentRemainder.Models
{
    [Table("appointments")]
    public partial class Appointments
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("email", TypeName = "character varying")]
        public string Email { get; set; }
        [Column("appointmenttime")]
        public DateTime? Appointmenttime { get; set; }
    }
}
