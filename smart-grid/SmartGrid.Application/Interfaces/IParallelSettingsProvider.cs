namespace SmartGrid.Application.Interfaces
{
    public interface IParallelSettingsProvider
    {
        int MaxDegreeOfParallelism { get; }
    }
}