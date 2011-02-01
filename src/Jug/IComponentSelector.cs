using System.Collections.Generic;

namespace Jug
{
    public interface IComponentSelector
    {
        ComponentModel Select(ComponentModel[] componentModels);
    }

    public interface IComponentSelector<TService> : IComponentSelector
    {
    }
}