namespace SmartGrid.Domain.Constants
{
    public class DeviceStatusLimits
    {
        public const double UnderperformingLoad = 20;
        public const double OverloadedLoad = 90;
        public static readonly TimeSpan OfflineThreshold = TimeSpan.FromSeconds(20);
    }
}
