using Microsoft.AspNetCore.Http;

namespace SmartGrid.Functions.Extensions
{
    internal static class IFormFileExtensions
    {
        public static async Task<byte[]> ToByteArrayAsync(this IFormFile file)
        {
            if (file == null || file.Length == 0)
                return [];

            using var memoryStream = new MemoryStream();

            await file.CopyToAsync(memoryStream);

            return memoryStream.ToArray();
        }
    }
}