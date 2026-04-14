using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Booking_Mvc.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Booking_Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult VerifyOtp()
        {
            return View();
        }
        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        // Page AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }
         // ------------------------------------------
        // Méthode réutilisable pour API avec JWT
        // ------------------------------------------
        private HttpClient CreateApiClient()
        {
            var client = _httpClientFactory.CreateClient("BookingAPI");

            // Récupère le JWT depuis les claims
            var token = User.FindFirst("Jwt")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(string code)
        {
            var email = HttpContext.Session.GetString("Email");

            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Session expirée");
                return View();
            }

            var client = _httpClientFactory.CreateClient("BookingAPI");

            var payload = new
            {
                Email = email,
                Code = code
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/auth/verify-otp", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "OTP invalide");
                return View();
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<AuthResponseDto>(responseJson,new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id ?? ""),
                new Claim(ClaimTypes.Name, user.Nom),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("Jwt", user.Token)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
            );

            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient("BookingAPI");

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/auth/login", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Email ou mot de passe invalide.");
                return View(model);
            }

            // 🔥 STOCKAGE PROPRE
            HttpContext.Session.SetString("Email", model.Email);

            return RedirectToAction("VerifyOtp");
        }
        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient("BookingAPI");

            var apiModel = new
            {
                nom = model.Nom,
                email = model.Email,
                adresse = model.Adresse,
                numero = model.Numero,
                password = model.Password
            };

            var json = JsonSerializer.Serialize(apiModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/auth/register", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", "Inscription impossible : " + error);
                return View(model);
            }

            return RedirectToAction("Login");
        }

        // Déconnexion
        public async Task<IActionResult> Logout()
        {
            // Récupérer l'Id de l'utilisateur depuis les claims (Login a stocké l'Id)
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                // Créer le HttpClient avec le JWT
                var client = CreateApiClient();

                // Appeler l'API logout
                var response = await client.PostAsync($"api/auth/logout/{userId}", null);

                if (!response.IsSuccessStatusCode)
                {
                    // Optionnel : gérer l'erreur si le logout API échoue
                    // Par exemple loguer l'erreur ou afficher un message
                }
            }
            // delete cokie cote mvc
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}