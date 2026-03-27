namespace Booking_Mvc.Models
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public string Id { get; set; } = null!;
        public string Nom { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}