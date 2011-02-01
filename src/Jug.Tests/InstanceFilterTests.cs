using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace Jug.Tests
{
    [TestFixture]
    public class InstanceFilterTests : FixtureFor<Container>
    {
        protected override Container Create()
        {
            return new Container();
        }

        [Test]
        public void CanResolveInstance()
        {
            // arrange
            service.Register<IService, ServiceImpl3>()
                .Register<IService, ServiceImpl2>()
                .Register<IService, ServiceImpl1>();

            service.Register<IComponentFilter<IService>, Something>();

            // act
            var services = service.ResolveAll<IService>();

            // assert
            services.Length.ShouldBe(3);
            services[0].GetType().Name.ShouldBe("ServiceImpl1");
            services[1].GetType().Name.ShouldBe("ServiceImpl2");
            services[2].GetType().Name.ShouldBe("ServiceImpl3");
        }

        class Something : IComponentFilter<IService>
        {
            public ComponentModel[] Filter(ComponentModel[] componentModels)
            {
                return componentModels
                    .OrderBy(componentModel => componentModel.ImplementationType.Name)
                    .ToArray();
            }
        }

        interface IService { }
 
        class ServiceImpl1 : IService { }
        class ServiceImpl2 : IService { }
        class ServiceImpl3 : IService { }
    }
}
