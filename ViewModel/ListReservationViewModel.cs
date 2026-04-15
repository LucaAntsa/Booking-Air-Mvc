using Booking_Mvc.Models;
namespace Booking_Mvc.ViewModel;

public class ListReservationViewModel
{
    public string BilletId { get; set; }
    public string DestinationId { get; set; }

    public string DestinationNom { get; set; }
    public string BilletType { get; set; }

    public int Qty { get; set; }
    public decimal Prix { get; set; }
}