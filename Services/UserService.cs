using Booking_Mvc.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Booking_Mvc.Services
{
    public class UserService
    {
        private readonly IHttpClientFactory _clientFactory;

        public UserService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var client = _clientFactory.CreateClient("BookingAPI");
            var users = await client.GetFromJsonAsync<List<User>>("api/users");
            return users ?? new List<User>();
        }
    }
}