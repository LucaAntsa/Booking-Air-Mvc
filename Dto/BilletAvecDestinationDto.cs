using Booking_Mvc.Models;
using Booking_Mvc.Enums;

namespace Booking_Mvc.Dto;

public class BilletAvecDestinationDto
{
    public string? Id { get; set; }
    public string Num_Billet { get; set; } = null!;
    public int Total_Billet { get; set; }
    public TypeClasse Type { get; set; }
    public string Id_Destination { get; set; } = null!;
    public double Prix { get; set; }

    public Destination? Destination { get; set; }
}