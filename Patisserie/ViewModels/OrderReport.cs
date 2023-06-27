using System.ComponentModel.DataAnnotations;

namespace Patisserie.ViewModels
{
    public class OrderReport
    {
        [Key]
        public int OrderId { get; set; }
        public string Email { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Total { get; set; }
        //from OrderDetail
        public int? ProductId { get; set; }
        public decimal Price { get; set; }
        //from Membership
        public string Membership { get; set; }
        public DateTime MembershipExpiry { get; set; }
        public int MembershipDuration { get; set; }

    }
}
