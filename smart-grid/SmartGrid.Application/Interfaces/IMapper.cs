namespace SmartGrid.Application.Interfaces
{
    public interface IMapper<in TSource, out TDestination>
    {
        TDestination Map(TSource source);
    }
}
