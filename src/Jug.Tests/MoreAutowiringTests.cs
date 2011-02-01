using System.Threading;
using NUnit.Framework;
using Shouldly;

namespace Jug.Tests
{
    [TestFixture]
    public class MoreAutowiringTests : FixtureFor<Container>
    {
        protected override Container Create()
        {
            return new Container();
        }

        [Test]
        public void SameInstanceIsInjectedWhenMultipleTypesDependOnSameService()
        {
            // arrange
            service.Register<Root>()
                .Register<FirstDependency>()
                .Register<SecondDependency>()
                .Register<ISomeService, InstanceCounterClass>();

            // act
            var root = service.Resolve<Root>();

            // assert
            root.FirstDependency.SomeService.InstanceId.ShouldBe(1);
            root.SecondDependency.SomeService.InstanceId.ShouldBe(1);
        }

        interface ISomeService
        {
            int InstanceId { get; }
        }

        class InstanceCounterClass : ISomeService
        {
            static int instanceCounter = 0;

            int instanceId;

            public InstanceCounterClass()
            {
                instanceId = ++instanceCounter;
            }

            public int InstanceId
            {
                get { return instanceId; }
            }
        }

        class FirstDependency
        {
            readonly ISomeService someService;

            public FirstDependency(ISomeService someService)
            {
                this.someService = someService;
            }

            public ISomeService SomeService
            {
                get { return someService; }
            }
        }

        class SecondDependency
        {
            readonly ISomeService someService;

            public SecondDependency(ISomeService someService)
            {
                this.someService = someService;
            }

            public ISomeService SomeService
            {
                get { return someService; }
            }
        }

        class Root
        {
            readonly FirstDependency firstDependency;
            readonly SecondDependency secondDependency;

            public Root(FirstDependency firstDependency, SecondDependency secondDependency)
            {
                this.firstDependency = firstDependency;
                this.secondDependency = secondDependency;
            }

            public FirstDependency FirstDependency
            {
                get { return firstDependency; }
            }

            public SecondDependency SecondDependency
            {
                get { return secondDependency; }
            }
        }
    }
}