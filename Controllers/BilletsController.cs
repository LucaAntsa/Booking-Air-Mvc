using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Booking_Mvc.Models;
using Booking_Mvc.Dto;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Booking_Mvc.ViewModel;

namespace Booking_Mvc.Controllers
{
    [Authorize]
    public class BilletsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BilletsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // 🔑 HttpClient avec JWT
        private HttpClient CreateApiClient()
        {
            var client = _httpClientFactory.CreateClient("BookingAPI");
            var token = User.FindFirst("Jwt")?.Value;

            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        // 📄 GET: /Billets
        public async Task<IActionResult> Index()
        {
            var client = CreateApiClient();
            var response = await client.GetAsync("api/billets/avec-destination");

            if (!response.IsSuccessStatusCode)
                return View(new List<BilletAvecDestinationDto>());

            var json = await response.Content.ReadAsStringAsync();

            var billets = JsonSerializer.Deserialize<List<BilletAvecDestinationDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(billets ?? new List<BilletAvecDestinationDto>());
        }

        // 📄 GET: /Billets/{id} (Billet + Destination)
        public async Task<IActionResult> Billet(string idDestination)
        {
            var client = CreateApiClient();

            // 🔥 récupérer TOUS les billets avec destination
            var response = await client.GetAsync("api/billets/avec-destination");

            if (!response.IsSuccessStatusCode)
                return View(new List<BilletAvecDestinationDto>());

            var json = await response.Content.ReadAsStringAsync();

            var billets = JsonSerializer.Deserialize<List<BilletAvecDestinationDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // 🔥 FILTRER PAR DESTINATION
            var filtered = billets?
                .Where(b => b.Id_Destination == idDestination)
                .OrderBy(b => int.Parse(b.Num_Billet))
                .ToList();

            return View(filtered);
        }

        // ➕ GET: /Billets/Create (avec idDestination)
        [HttpGet]
        public IActionResult Create(string idDestination, double prixBillet)
        {
            if (string.IsNullOrEmpty(idDestination))
                return BadRequest("Destination manquante");

            var billet = new Billet
            {
                Id_Destination = idDestination,
                Prix = prixBillet
            };

            return View(billet);
        }

        // ➕ POST: /Billets/Create
        [HttpPost]
        public async Task<IActionResult> Create(Billet billet)
        {
            if (!ModelState.IsValid)
                return View(billet);

            var client = CreateApiClient();

            var json = JsonSerializer.Serialize(billet);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/billets", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Erreur lors de la création.");
                return View(billet);
            }

            return RedirectToAction(nameof(Index));
        }

        // ✏️ GET: /Billets/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var client = CreateApiClient();
            var response = await client.GetAsync($"api/billets/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();

            var billet = JsonSerializer.Deserialize<Billet>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(billet);
        }

        // ✏️ POST: /Billets/Edit/{id}
        [HttpPost]
        public async Task<IActionResult> Edit(string id, Billet billet)
        {
            if (!ModelState.IsValid)
                return View(billet);

            billet.Id = id;

            var client = CreateApiClient();

            var json = JsonSerializer.Serialize(billet);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"api/billets/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Erreur lors de la modification.");
                return View(billet);
            }

            return RedirectToAction(nameof(Index));
        }

        // ❌ GET: /Billets/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var client = CreateApiClient();
            var response = await client.GetAsync($"api/billets/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();

            var billet = JsonSerializer.Deserialize<Billet>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(billet);
        }

        // ❌ POST: /Billets/Delete/{id}
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var client = CreateApiClient();

            var response = await client.DeleteAsync($"api/billets/{id}");

            if (!response.IsSuccessStatusCode)
                TempData["Error"] = "Erreur lors de la suppression.";

            return RedirectToAction(nameof(Index));
        }

    }
}