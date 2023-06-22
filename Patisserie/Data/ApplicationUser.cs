using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace Patisserie.Data
{
    public class ApplicationUser: IdentityUser
    {
        //Custom Fields
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Membership { get; set; }

        public DateTime MembershipExpiry { get; set; }


        public int MembershipDuration { get; set; }
    }
}
