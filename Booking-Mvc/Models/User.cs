using Booking_Mvc.Enums;

namespace Booking_Mvc.Models;

public class User{
public string? Id { get; set; }
    public string Nom { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Adresse { get; set; } = null!;
    public string Numero { get; set; } = null!;
    public string Password { get; set; } = null!;
    public UserRole Role { get; set; } = UserRole.Client;
    public UserStatus Status { get; set; } = UserStatus.Deconnecte;
}