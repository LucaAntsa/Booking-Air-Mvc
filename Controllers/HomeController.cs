using System.Diagnostics;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Booking_Mvc.Models;
using Booking_Mvc.Dto;
using Booking_Mvc.ViewModel;

namespace Booking_Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        private HttpClient CreateApiClient()
        {
            var client = _httpClientFactory.CreateClient("BookingAPI");
            var token = User.FindFirst("Jwt")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }
        public async Task<IActionResult> Index()
        {
            // 🔑 Crée le client avec JWT
            var client = CreateApiClient();
            // 📡 Appel API
            var response = await client.GetAsync("api/destinations");
            // ❌ Si erreur → liste vide
            if (!response.IsSuccessStatusCode)
                return View(new List<Destination>());
            // 📥 Lire JSON
            var json = await response.Content.ReadAsStringAsync();
            // 🔄 Convertir en liste
            var destinations = JsonSerializer.Deserialize<List<Destination>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            // ✅ Envoyer à la vue
            return View(destinations ?? new List<Destination>());
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<IActionResult> ListeDestination(
            string search,
            string nomVol,
            string dateDepart,
            string heureDepart)
        {
            var client = CreateApiClient();

            var response = await client.GetAsync("api/destinations");

            if (!response.IsSuccessStatusCode)
                return View(new List<Destination>());

            var json = await response.Content.ReadAsStringAsync();

            var destinations = JsonSerializer.Deserialize<List<Destination>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<Destination>();

            // 🔎 SEARCH
            if (!string.IsNullOrWhiteSpace(search))
            {
                destinations = destinations.Where(x =>
                    x.Lieu_Depart.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    x.Lieu_Arriver.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    x.Nom_Vol.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // ✈️ NOM VOL (FIX IMPORTANT)
            if (!string.IsNullOrWhiteSpace(nomVol))
            {
                destinations = destinations.Where(x =>
                    x.Nom_Vol.ToString().Equals(nomVol, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // 📅 DATE DEPART (FIX SAFE)
            if (!string.IsNullOrWhiteSpace(dateDepart))
            {
                if (DateTime.TryParse(dateDepart, out var date))
                {
                    destinations = destinations.Where(x =>
                        x.Date_Depart.Date == date.Date
                    ).ToList();
                }
            }

            // 🕒 HEURE (FIX ROBUSTE)
            if (!string.IsNullOrWhiteSpace(heureDepart))
            {
                destinations = destinations.Where(x =>
                    x.Heur_Depart.StartsWith(heureDepart)
                ).ToList();
            }

            return View(destinations);
        }
        public async Task<IActionResult> Profile()
        {
            var client = CreateApiClient();
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            // RESERVATIONS
            var resResponse = await client.GetAsync("api/reserva");
            var resJson = await resResponse.Content.ReadAsStringAsync();

            var reservations = JsonSerializer.Deserialize<List<Reservation>>(
                resJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<Reservation>();
            reservations = reservations
            .Where(r => r.User_Id == userId)
            .ToList();

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

            // JOIN
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}