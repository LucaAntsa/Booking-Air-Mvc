using Microsoft.AspNetCore.Mvc;
using Booking_Mvc.ViewModel;
using Booking_Mvc.Models;
using System.Text;
using System.Text.Json;
using Booking_Mvc.Dto;

namespace Booking_Mvc.Controllers
{
    public class ReserverController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;

        public ReserverController(IHttpClientFactory httpClientFactory, HttpClient httpClient)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClient;
        }

        private HttpClient CreateApiClient()
        {
            return _httpClientFactory.CreateClient("BookingAPI");
        }

        // 🔥 PAGE RESERVATION (GET)
        [HttpGet]
        public async Task<IActionResult> reserver(string idDestination, double prixBillet)
        {
            var client = CreateApiClient();

            var response = await client.GetAsync($"api/destinations/{idDestination}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();

            var destination = JsonSerializer.Deserialize<Destination>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? new Destination();

            var model = new ReserverViewModel
            {
                Billet = new Billet
                {
                    Id_Destination = idDestination,
                    Prix = prixBillet,
                    Total_Billet = 1
                },
                Destination = destination
            };

            return View(model);
        }

        // 🔥 POST RESERVATION
       [HttpPost]
        public async Task<IActionResult> Acheter(ReserverViewModel model)
        {
            var userId = "USER_ID_FIXE";

            var qty = model.Billet.Total_Billet;
            decimal prixUnitaire = (decimal)model.Billet.Prix;

            var reservation = new ReservaDto
            {
                User_Id = userId,
                Destination_Id = model.Destination.Id,
                Qty = model.Billet.Total_Billet,
                Prix = (decimal)model.Billet.Prix * model.Billet.Total_Billet
            };

            var json = JsonSerializer.Serialize(reservation);
            Console.WriteLine(json);

            var response = await _httpClient.PostAsJsonAsync(
                "http://localhost:5289/api/reserva",
                reservation
            );

            var content = await response.Content.ReadAsStringAsync();
            TempData["alert"] = content;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListeDestination", "Home");
            }

            return BadRequest(content);
        }
    }
}