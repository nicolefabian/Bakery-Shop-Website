using System;
using System.Collections.Generic;

namespace Patisserie.Models.DB
{
    public partial class Member
    {
        public int MemberId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string Membership { get; set; } = null!;
        public DateTime MembershipExpiry { get; set; }
        public int MembershipDuration { get; set; }
    }
}
