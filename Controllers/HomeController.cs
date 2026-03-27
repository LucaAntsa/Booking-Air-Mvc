using System.Diagnostics;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Booking_Mvc.Models;

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
        public IActionResult Billet()
        {
            return View();
        }
        public async Task<IActionResult> ListeDestination()
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

        public IActionResult Profile()
        {
            return View();
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