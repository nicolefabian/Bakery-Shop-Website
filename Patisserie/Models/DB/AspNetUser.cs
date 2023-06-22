using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Patisserie.Models.DB
{
    public partial class AspNetUser
    {
        public AspNetUser()
        {
            AspNetUserClaims = new HashSet<AspNetUserClaim>();
            AspNetUserLogins = new HashSet<AspNetUserLogin>();
            AspNetUserTokens = new HashSet<AspNetUserToken>();
            Roles = new HashSet<AspNetRole>();
        }

        public string Id { get; set; } = null!;
        public string? UserName { get; set; }
        public string? NormalizedUserName { get; set; }
        public string? Email { get; set; }
        public string? NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? PasswordHash { get; set; }
        public string? SecurityStamp { get; set; }
        public string? ConcurrencyStamp { get; set; }

        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Invalid phone number format.")]
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }

        [DisplayName("First name")]

        public string? FirstName { get; set; }

        [DisplayName("Last name")]
        public string? LastName { get; set; }
        public string? Membership { get; set; }

        [DisplayName("Membership duration")]

        public int MembershipDuration { get; set; }

        [DisplayName("Membership expiry")]
        //only display the date
        [DataType(DataType.Date)]
        public DateTime MembershipExpiry { get; set; }

        public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual ICollection<AspNetUserToken> AspNetUserTokens { get; set; }

        public virtual ICollection<AspNetRole> Roles { get; set; }
    }
}
