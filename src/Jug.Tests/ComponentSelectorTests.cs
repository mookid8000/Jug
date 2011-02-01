using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace Jug.Tests
{
    [TestFixture]
    public class ComponentSelectorTests : FixtureFor<Container>
    {
        protected override Container Create()
        {
            return new Container();
        }

        [Test]
        public void NoComponentSelectorYieldsActivationOfFirstRegisteredImplementation()
        {
            // arrange
            service.Register<ISomeService, SecondImplementation>()
                .Register<ISomeService, FirstImplementation>()
                .Register<ISomeService, ThirdImplementation>();

            // act
            var instance = service.Resolve<ISomeService>();

            // assert
            instance.GetType().ShouldBe(typeof (SecondImplementation));
        }

        [Test]
        public void ComponentSelectorIsInvokedWhenRegistered()
        {
            // arrange
            service.Register<ISomeService, SecondImplementation>()
                .Register<ISomeService, FirstImplementation>()
                .Register<ISomeService, ThirdImplementation>();

            service.Register<IComponentSelector<ISomeService>, WillSelectThirdImplementation>();

            // act
            var instance = service.Resolve<ISomeService>();

            // assert
            instance.GetType().ShouldBe(typeof(ThirdImplementation));
        }

        class WillSelectThirdImplementation : IComponentSelector<ISomeService>
        {
            public ComponentModel Select(ComponentModel[] componentModels)
            {
                return componentModels.Single(c => c.ImplementationType == typeof (ThirdImplementation));
            }
        }

        interface ISomeService {}

        class FirstImplementation : ISomeService {}
        class SecondImplementation : ISomeService {}
        class ThirdImplementation : ISomeService {}
    }
}