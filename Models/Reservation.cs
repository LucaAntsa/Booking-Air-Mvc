using Booking_Mvc.Enums;
namespace Booking_Mvc.Models;
public class Reservation
{
    public string? Id { get; set; }
    public string? User_Id { get; set; }
    public string? Billet_Id { get; set; }
    public string? Destination_Id { get; set; }
    public int Qty { get; set; }
    public decimal Prix { get; set; }
}