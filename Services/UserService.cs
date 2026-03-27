using Booking_Mvc.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Booking_Mvc.Services
{
    public class UserService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var client = _clientFactory.CreateClient("BookingAPI");

            // 🔑 Récupère le JWT depuis les claims
            var token = _httpContextAccessor.HttpContext?.User.FindFirst("Jwt")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync("api/users");

            if (!response.IsSuccessStatusCode)
                return new List<User>();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<User>>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<User>();
        }
    }
}