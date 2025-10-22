using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EtiWeb.Data;



namespace EtiWeb.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Request?> GetRequestAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Request>($"api/Requests/{id}");
        }
    }
}
