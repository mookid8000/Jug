using System;
using System.Collections.Generic;
using System.Linq;

namespace Jug
{
    public class Container
    {
        readonly Dictionary<Type, List<ComponentModel>> componentModelsByServiceType = new Dictionary<Type, List<ComponentModel>>();
        readonly Dictionary<Type, ComponentModel> componentModelsByImplementationType = new Dictionary<Type, ComponentModel>();

        public Container Register<TImplementation>()
        {
            return Register<TImplementation, TImplementation>();
        }

        public Container Register<TService, TImplementation>() where TImplementation : TService
        {
            var componentModel = ForImplementationType(typeof (TImplementation));

            AddServiceType(componentModel, typeof (TService));

            return this;
        }

        void AddServiceType(ComponentModel componentModel, Type serviceType)
        {
            componentModel.AddServiceType(serviceType);

            if (!componentModelsByServiceType.ContainsKey(serviceType))
            {
                componentModelsByServiceType[serviceType] = new List<ComponentModel>();
            }

            var list = componentModelsByServiceType[serviceType];

            if (!list.Contains(componentModel))
            {
                list.Add(componentModel);
            }
        }

        public T[] ResolveAll<T>()
        {
            var serviceType = typeof(T);
            var objects = ResolveAll(serviceType);
            var targetArray = new T[objects.Length];
            Array.Copy(objects, targetArray, objects.Length);
            return targetArray;
        }

        object[] ResolveAll(Type serviceType)
        {
            var componentModels = componentModelsByServiceType[serviceType];

            return componentModels
                .Select(c => Activator.CreateInstance(c.ImplementationType))
                .ToArray();
        }

        public T Resolve<T>()
        {
            var serviceType = typeof(T);

            return (T) Resolve(serviceType);
        }

        object Resolve(Type serviceType)
        {
            return Activate(serviceType);
        }

        object Activate(Type serviceType)
        {
            var componentModels = componentModelsByServiceType[serviceType];

            dynamic selectors = ResolveAll(typeof (IComponentSelector<>).MakeGenericType(serviceType));

            var implementationType = (Type) (selectors.Length == 0
                                                 ? DefaultImplementationType(componentModels)
                                                 : GetComponentModelFromSelectors(selectors, componentModels));

            var firstConstructor = implementationType.GetConstructors().First();

            var constructorArguments = firstConstructor
                .GetParameters()
                .Select(p => Activate(p.ParameterType))
                .ToArray();

            return firstConstructor.Invoke(constructorArguments);
        }

        Type DefaultImplementationType(List<ComponentModel> componentModels)
        {
            return componentModels.First().ImplementationType;
        }

        Type GetComponentModelFromSelectors(dynamic selectors, IEnumerable<ComponentModel> componentModels)
        {
            foreach(dynamic selector in selectors)
            {
                var componentModel = (ComponentModel) selector.Select(componentModels);
            
                if (componentModel != null)
                {
                    return componentModel.ImplementationType;
                }
            }

            return null;
        }

        ComponentModel ForImplementationType(Type implementationType)
        {
            return componentModelsByImplementationType.ContainsKey(implementationType)
                       ? componentModelsByImplementationType[implementationType]
                       : CreateAndAddComponentModel(implementationType);
        }

        ComponentModel CreateAndAddComponentModel(Type implementationType)
        {
            var componentModel = new ComponentModel(implementationType);

            componentModelsByImplementationType[implementationType] = componentModel;

            return componentModel;
        }
    }
}
