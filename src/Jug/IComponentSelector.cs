using System.Collections.Generic;

namespace Jug
{
    public interface IComponentSelector
    {
        ComponentModel Select(IEnumerable<ComponentModel> componentModels);
    }

    public interface IComponentSelector<TService> : IComponentSelector
    {
    }
}