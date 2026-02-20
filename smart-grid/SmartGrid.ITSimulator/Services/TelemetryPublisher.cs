using System.Net.Http.Json;
using SmartGrid.ITSimulator.Models;

namespace SmartGrid.ITSimulator.Services
{
    public class TelemetryPublisher
    {
        private readonly HttpClient _httpClient;

        public TelemetryPublisher(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }
        public async Task<bool> PublishAsync(TelemetryDTO telemetry)
        {
            ArgumentNullException.ThrowIfNull(telemetry);

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_httpClient.BaseAddress}api/ReceiveTelemetry", telemetry);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to publish telemetry.", ex);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> PublishSafeAsync(TelemetryDTO telemetry)
        {
            try
            {
                var success = await PublishAsync(telemetry);
                return (success, success ? null : "API returned error status");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
