namespace Jug
{
    public interface IInstanceFilter<TService>
    {
        TService[] Filter(TService[] services);
    }
}