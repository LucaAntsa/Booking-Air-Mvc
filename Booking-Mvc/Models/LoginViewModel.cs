using System.ComponentModel.DataAnnotations;

namespace Booking_Mvc.Models;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}