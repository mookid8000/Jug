using System.Collections.Generic;

namespace Jug
{
    public interface IComponentSelector<TService>
    {
        ComponentModel Select(IEnumerable<ComponentModel> componentModels);
    }
}