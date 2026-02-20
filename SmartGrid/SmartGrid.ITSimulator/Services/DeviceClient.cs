using SmartGrid.ITSimulator.Models;
using System.Net.Http.Json;

namespace SmartGrid.ITSimulator.Services
{
    public class DeviceClient
    {
        private readonly HttpClient _httpClient;
        public DeviceClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }
        public async Task<string?> RegisterDeviceAsync(DeviceDTO deviceDTO)
        {
            var payload = new
            {
                Name = deviceDTO.DeviceName,
                DeviceType = deviceDTO.DeviceType.ToString(),
                deviceDTO.NominalPower,
                deviceDTO.Location
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_httpClient.BaseAddress}api/ReceiveDevice", payload);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();

                    return responseBody?.Data;
                }

                return null;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to register device.", ex);
            }
        }
    }
}
