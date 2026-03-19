using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Booking_Mvc.Models;

namespace Booking_Mvc.Controllers;

public class DestinationsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public DestinationsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var client = _httpClientFactory.CreateClient("BookingAPI");

        var response = await client.GetAsync("api/destinations");

        if (!response.IsSuccessStatusCode)
            return View(new List<Destination>());

        var json = await response.Content.ReadAsStringAsync();

        var destinations = JsonSerializer.Deserialize<List<Destination>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(destinations ?? new List<Destination>());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Destination destination)
    {
        if (!ModelState.IsValid)
            return View(destination);

        var client = _httpClientFactory.CreateClient("BookingAPI");

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

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var client = _httpClientFactory.CreateClient("BookingAPI");

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

    [HttpPost]
    public async Task<IActionResult> Edit(string id, Destination destination)
    {
        if (!ModelState.IsValid)
            return View(destination);

        destination.Id = id;

        var client = _httpClientFactory.CreateClient("BookingAPI");

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

    [HttpGet]
    public async Task<IActionResult> Delete(string id)
    {
        var client = _httpClientFactory.CreateClient("BookingAPI");

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

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var client = _httpClientFactory.CreateClient("BookingAPI");

        var response = await client.DeleteAsync($"api/destinations/{id}");

        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] = "Erreur lors de la suppression.";
        }

        return RedirectToAction(nameof(Index));
    }
}