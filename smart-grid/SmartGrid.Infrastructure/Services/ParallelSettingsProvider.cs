using Microsoft.Extensions.Options;
using SmartGrid.Application.Common.Options;
using SmartGrid.Application.Interfaces;

namespace SmartGrid.Infrastructure.Services
{
    public class ParallelSettingsProvider(IOptions<ParallelSettings> options) : IParallelSettingsProvider
    {
        private readonly ParallelSettings _settings = options.Value;

        public int MaxDegreeOfParallelism => _settings.MaxDegreeOfParallelism;
    }
}
