using Booking_Mvc.Enums;

namespace Booking_Mvc.Models;

public class Billet{
    public string? Id { get; set; }
    public string Num_Billet { get; set; } = null!;
    public int Total_Billet { get; set; } = 50;
    public TypeClasse Type { get; set; }
    public string Id_Destination { get; set; } = null!;
    public double Prix { get; set; }
}