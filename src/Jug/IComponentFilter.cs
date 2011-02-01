namespace Jug
{
    public interface IComponentFilter
    {
        ComponentModel[] Filter(ComponentModel[] componentModels);
    }

    public interface IComponentFilter<TService> : IComponentFilter
    {
    }
}