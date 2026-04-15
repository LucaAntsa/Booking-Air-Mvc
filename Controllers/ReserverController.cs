using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Booking_Mvc.ViewModel;
using Booking_Mvc.Models;
using System.Text;
using System.Text.Json;
using Booking_Mvc.Dto;
using System.Security.Claims;

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
            var client = _httpClientFactory.CreateClient("BookingAPI");
            var token = User.FindFirst("Jwt")?.Value;
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        // 🔥 PAGE RESERVATION (GET)
        [HttpGet]
        public async Task<IActionResult> reserver(string idBillet ,string idDestination, double prixBillet)
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
                    Id = idBillet,
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
            var client = CreateApiClient(); //appel user

            if (string.IsNullOrEmpty(model.Billet.Id))
                return BadRequest("Billet manquant");

            var qty = model.Billet.Total_Billet;
            decimal prixUnitaire = (decimal)model.Billet.Prix;

            decimal total = prixUnitaire * qty;

            // 🔥 réduction 10%
            if (qty > 2)
            {
                total = total * 0.9m;
            }

            var reservation = new ReservaDto
            {
                Billet_id = model.Billet.Id,
                Destination_Id = model.Destination.Id,
                Qty = model.Billet.Total_Billet,
                Prix = total
            };

            var json = JsonSerializer.Serialize(reservation);
            // Console.WriteLine(json);

            var response = await client.PostAsJsonAsync(
                "api/reserva", // ✅ pas besoin du localhost si BaseAddress configurée
                reservation
            );

            var content = await response.Content.ReadAsStringAsync();
            TempData["alert"] = content;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Profile", "Home");
            }
            return BadRequest(content);
        }

        public async Task<IActionResult> MesReservations()
        {
            var client = CreateApiClient();

            // RESERVATIONS
            var resResponse = await client.GetAsync("api/reserva");
            var resJson = await resResponse.Content.ReadAsStringAsync();

            var reservations = JsonSerializer.Deserialize<List<Reservation>>(
                resJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<Reservation>();

            // BILLETS
            var bilResponse = await client.GetAsync("api/billets");
            var bilJson = await bilResponse.Content.ReadAsStringAsync();

            var billets = JsonSerializer.Deserialize<List<Billet>>(
                bilJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<Billet>();

            // DESTINATIONS
            var destResponse = await client.GetAsync("api/destinations");
            var destJson = await destResponse.Content.ReadAsStringAsync();

            var destinations = JsonSerializer.Deserialize<List<Destination>>(
                destJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<Destination>();

            // 🔥 4. JOIN MANUEL
            var result = reservations.Select(r =>
            {
                var billet = billets.FirstOrDefault(b => b.Id == r.Billet_Id);
                var destination = destinations.FirstOrDefault(d => d.Id == r.Destination_Id);

                return new ListReservationViewModel
                {
                    BilletId = r.Billet_Id,
                    DestinationId = r.Destination_Id,
                    Qty = r.Qty,
                    Prix = r.Prix,

                    BilletType = billet?.Type.ToString() ?? "Inconnu",

                    DestinationNom = destination != null
                        ? $"{destination.Lieu_Depart} → {destination.Lieu_Arriver}"
                        : "Inconnue"
                };
            }).ToList();

            return View(result);
        }
    }
}