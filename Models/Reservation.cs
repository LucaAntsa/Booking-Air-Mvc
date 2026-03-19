using Booking_Mvc.Enums;

namespace Booking_Mvc.Models;

public class Reservation
{
    public string? Id { get; set; }
    public string UserId { get; set; } = null!;
    public string Id_billet { get; set; } = null!;
    public int NombreBillets { get; set; } = 1;
    public DateTime DateReservation { get; set; } = DateTime.Now;
    public ReservationStatus Status { get; set; } = ReservationStatus.EnAttente;
}