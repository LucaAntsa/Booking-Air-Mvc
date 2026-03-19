using System.ComponentModel.DataAnnotations;

namespace Booking_Mvc.Models;

public class RegisterViewModel
{
    [Required]
    public string Nom { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Adresse { get; set; } = null!;

    [Required]
    public string Numero { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas.")]
    public string ConfirmPassword { get; set; } = null!;
}