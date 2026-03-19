using Booking_Mvc.Models;
using System.Collections.Generic;

namespace Booking_Mvc.Models
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int ConnectedUsers { get; set; }

        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public List<User> Users { get; set; } = new List<User>();
    }
}