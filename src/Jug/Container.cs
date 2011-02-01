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

        public TService[] ResolveAll<TService>()
        {
            var serviceType = typeof(TService);
            var objects = ResolveAll(serviceType);
            var targetArray = new TService[objects.Length];
            Array.Copy(objects, targetArray, objects.Length);
            return targetArray;
        }

        public TService Resolve<TService>()
        {
            var context = new ResolutionContext();
            return (TService) ActivateFromServiceType(typeof (TService), context);
        }

        object[] ResolveAll(Type serviceType)
        {
            if (!componentModelsByServiceType.ContainsKey(serviceType))
            {
                return new object[0];
            }

            var componentModels = componentModelsByServiceType[serviceType].ToArray();

            var filters = ResolveAll(typeof (IComponentFilter<>).MakeGenericType(serviceType))
                .Cast<IComponentFilter>();

            foreach(var filter in filters)
            {
                componentModels = filter.Filter(componentModels);
            }

            return componentModels
                .Select(c => Activator.CreateInstance(c.ImplementationType))
                .ToArray();
        }

        object ActivateFromServiceType(Type serviceType, ResolutionContext context)
        {
            var componentModels = componentModelsByServiceType[serviceType];

            var selectors = ResolveAll(typeof (IComponentSelector<>).MakeGenericType(serviceType))
                .Cast<IComponentSelector>();

            Type implementationType = null;

            foreach (var selector in selectors)
            {
                var componentModel = selector.Select(componentModels.ToArray());
                if (componentModel != null)
                {
                    implementationType = componentModel.ImplementationType;
                }
            }

            var typeToCreate = implementationType ?? DefaultImplementationType(componentModels);

            var instance = context.GetInstance(typeToCreate)
                           ?? ActivateFromImplementationType(typeToCreate, context);

            context.AddInstance(instance);

            return instance;
        }

        object ActivateFromImplementationType(Type implementationType, ResolutionContext context)
        {
            var firstConstructor = implementationType.GetConstructors().First();

            var constructorArguments = firstConstructor
                .GetParameters()
                .Select(p => ActivateFromServiceType(p.ParameterType, context))
                .ToArray();

            return firstConstructor.Invoke(constructorArguments);
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

        Type DefaultImplementationType(List<ComponentModel> componentModels)
        {
            return componentModels.First().ImplementationType;
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

        class ResolutionContext
        {
            readonly Dictionary<Type, object> instances = new Dictionary<Type, object>();

            public void AddInstance(object instance)
            {
                instances[instance.GetType()] = instance;
            }

            public object GetInstance(Type typeToCreate)
            {
                return instances.ContainsKey(typeToCreate)
                           ? instances[typeToCreate]
                           : null;
            }
        }
    }
}
