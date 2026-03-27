using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Booking_Mvc.Models;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;

namespace Booking_Mvc.Controllers
{
    [Authorize] // protège toutes les actions
    public class DestinationsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DestinationsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Méthode réutilisable pour récupérer HttpClient avec JWT
        private HttpClient CreateApiClient()
        {
            var client = _httpClientFactory.CreateClient("BookingAPI");
            var token = User.FindFirst("Jwt")?.Value;
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        // GET: /Destinations
        public async Task<IActionResult> Index()
    {
        // 🔑 Crée un HttpClient configuré avec le JWT (token)
        // Permet d’accéder aux routes sécurisées de ton API
        var client = CreateApiClient();

        // 📡 Envoie une requête GET à ton API pour récupérer les destinations
        var response = await client.GetAsync("api/destinations");

        // ❌ Si la requête échoue (ex: 401, 404, 500)
        // On retourne une vue avec une liste vide pour éviter les erreurs
        if (!response.IsSuccessStatusCode)
            return View(new List<Destination>());

        // 📥 Récupère la réponse JSON envoyée par l’API
        var json = await response.Content.ReadAsStringAsync();

        // 🔄 Convertit le JSON en liste d’objets Destination
        // PropertyNameCaseInsensitive = ignore les majuscules/minuscules (important)
        var destinations = JsonSerializer.Deserialize<List<Destination>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // ✅ Envoie les données à la vue
        // Si jamais c’est null, on envoie une liste vide pour éviter un crash
        return View(destinations ?? new List<Destination>());
    }

        // GET: /Destinations/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Destinations/Create
        [HttpPost]
        public async Task<IActionResult> Create(Destination destination)
        {
            if (!ModelState.IsValid)
                return View(destination);

            var client = CreateApiClient();

            var json = JsonSerializer.Serialize(destination);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/destinations", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Erreur lors de la création.");
                return View(destination);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Destinations/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var client = CreateApiClient();

            var response = await client.GetAsync($"api/destinations/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();

            var destination = JsonSerializer.Deserialize<Destination>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (destination == null)
                return NotFound();

            return View(destination);
        }

        // POST: /Destinations/Edit/{id}
        [HttpPost]
        public async Task<IActionResult> Edit(string id, Destination destination)
        {
            if (!ModelState.IsValid)
                return View(destination);

            destination.Id = id;

            var client = CreateApiClient();

            var json = JsonSerializer.Serialize(destination);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"api/destinations/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Erreur lors de la modification.");
                return View(destination);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Destinations/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var client = CreateApiClient();

            var response = await client.GetAsync($"api/destinations/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();

            var destination = JsonSerializer.Deserialize<Destination>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (destination == null)
                return NotFound();

            return View(destination);
        }

        // POST: /Destinations/Delete/{id}
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var client = CreateApiClient();

            var response = await client.DeleteAsync($"api/destinations/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Erreur lors de la suppression.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}