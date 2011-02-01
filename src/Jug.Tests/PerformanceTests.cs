using System.Diagnostics;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;

namespace Jug.Tests
{
    [TestFixture]
    public class PerformanceTests
    {
        [Test]
        public void DoStuff()
        {
            var jugContainer = new Container();

            jugContainer.Register<Root>()
                .Register<FirstDependency>()
                .Register<SecondDependency>()
                .Register<ISomeService, InstanceCounterClass>();

            var count = 100000;

            new Stopwatch().Measure(() => count.Times(() => { var root = jugContainer.Resolve<Root>(); }),
                                    "Resolving {0} times from Jug",
                                    count);

            var windsorContainer = new WindsorContainer();

            windsorContainer.Register(Component.For<Root>(),
                                      Component.For<FirstDependency>(),
                                      Component.For<SecondDependency>(),
                                      Component.For<ISomeService>().ImplementedBy<InstanceCounterClass>());

            new Stopwatch().Measure(() => count.Times(() => { var root = windsorContainer.Resolve<Root>(); }),
                                    "Resolving {0} times from Windsor",
                                    count);
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