using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Booking_Mvc.Models;
using Booking_Mvc.Services;

namespace Booking_Mvc.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserService _userService;

        public AdminController(UserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Dashboard(int page = 1, int pageSize = 5)
        {
            var allUsers = await _userService.GetAllUsersAsync();

            var vm = new DashboardViewModel
            {
                TotalUsers = allUsers.Count,
                ActiveUsers = allUsers.Count(u => u.Status == Enums.UserStatus.Connecte),
                InactiveUsers = allUsers.Count(u => u.Status != Enums.UserStatus.Connecte),
                ConnectedUsers = allUsers.Count(u => u.Status == Enums.UserStatus.Connecte),
                PageIndex = page,
                TotalPages = (int)System.Math.Ceiling(allUsers.Count / (double)pageSize),
                Users = allUsers.Skip((page - 1) * pageSize).Take(pageSize).ToList()
            };
            ViewData["Title"] = "Admin Dashboard";
            return View(vm);
        }
    }
}