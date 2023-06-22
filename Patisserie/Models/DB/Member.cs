using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Patisserie.Models.DB
{
    public partial class Member
    {
        [DisplayName("Member ID")]
        public int MemberId { get; set; }

        [DisplayName("First name")]
        public string? FirstName { get; set; }
        [DisplayName("Last name")]
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string Membership { get; set; } = null!;
        [DisplayName("Membership expiry")]
        //only display the date
        [DataType(DataType.Date)]
        public DateTime MembershipExpiry { get; set; }
        [DisplayName("Membership duration")]

        public int MembershipDuration { get; set; }
    }
}
