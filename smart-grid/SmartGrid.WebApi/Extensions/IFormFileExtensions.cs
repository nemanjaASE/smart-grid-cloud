namespace SmartGrid.WebApi.Extensions
{
    internal static class IFormFileExtensions
    {
        public static async Task<byte[]> ToByteArrayAsync(this IFormFile file, CancellationToken ct = default)
        {
            if (file == null || file.Length == 0)
                return [];

            using var memoryStream = new MemoryStream();

            await file.CopyToAsync(memoryStream, ct);

            return memoryStream.ToArray();
        }
    }
}