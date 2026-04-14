using Booking_Mvc.Models;
namespace Booking_Mvc.ViewModel;
public class ReserverViewModel
{
    public Billet Billet { get; set; } = new Billet();
    public Destination Destination { get; set; } = new Destination();
}