using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Patisserie.Models.DB;

namespace Patisserie.Controllers
{
    public class OrderReportController : Controller
    {
        private readonly FSWD2023fabi18Context _context;

        public OrderReportController(FSWD2023fabi18Context context)
        {
            _context = context;
        }
        
        public IActionResult Index()
        {
            string sql = @"SELECT o.OrderID, o.Email, o.FirstName, o.LastName, o.Total, m.Membership, m.MembershipExpiry, m.MembershipDuration, od.ProductID, od.Price
                        FROM [Order] o
                        JOIN Member m ON o.MemberID = m.MemberID
						JOIN OrderDetail od ON o.OrderID = od.OrderID
                        ";

            var results = _context.OrderReports.FromSqlRaw(sql).ToList();
            return View(results);
        }
    }
}
