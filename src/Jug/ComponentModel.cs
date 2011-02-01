using System;
using System.Collections.Generic;

namespace Jug
{
    public class ComponentModel
    {
        public ComponentModel(Type implementationType)
        {
            ServiceTypes = new List<Type>();
            ImplementationType = implementationType;
        }

        public List<Type> ServiceTypes { get; set; }
        public Type ImplementationType { get; set; }

        public void AddServiceType(Type serviceType)
        {
            ServiceTypes.Add(serviceType);
        }
    }
}