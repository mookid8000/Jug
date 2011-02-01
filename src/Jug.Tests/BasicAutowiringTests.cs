using System;
using NUnit.Framework;
using Shouldly;

namespace Jug.Tests
{
    [TestFixture]
    public class BasicAutowiringTests : FixtureFor<Container>
    {
        protected override Container Create()
        {
            return new Container();
        }

        [Test]
        public void CanAutowireConcreteDependencies()
        {
            // arrange
            service.Register<ConcreteTypes.SomeClass>()
                .Register<ConcreteTypes.SomeDependency>()
                .Register<ConcreteTypes.AnotherDependency>();

            // act
            var instance = service.Resolve<ConcreteTypes.SomeClass>();

            // assert
            instance.ToString().ShouldBe("Some class with SomeDependency and AnotherDependency");
        }

        class ConcreteTypes
        {
            public class SomeClass
            {
                SomeDependency someDependency;
                AnotherDependency anotherDependency;

                public SomeClass(SomeDependency someDependency, AnotherDependency anotherDependency)
                {
                    this.someDependency = someDependency;
                    this.anotherDependency = anotherDependency;
                }

                public override string ToString()
                {
                    return string.Format("Some class with {0} and {1}",
                                         someDependency.GetType().Name,
                                         anotherDependency.GetType().Name);
                }
            }

            public class SomeDependency
            {
                
            }

            public class AnotherDependency
            {
                
            }
        }

        [Test]
        public void CanAutowireAbstractAndInterfaceDependencies()
        {
            // arrange
            service.Register<AbstractedTypes.ISomeService, AbstractedTypes.SomeServiceImpl>()
                .Register<AbstractedTypes.ISomeDependency, AbstractedTypes.SomeDependencyImpl>()
                .Register<AbstractedTypes.IAnotherDependency, AbstractedTypes.AnotherDependencyImpl>()
                .Register<AbstractedTypes.IFirstSubDependency, AbstractedTypes.FirstSubDependencyImpl>()
                .Register<AbstractedTypes.ISecondSubDependency, AbstractedTypes.SecondSubDependencyImpl>()
                .Register<AbstractedTypes.IThirdSubDependency, AbstractedTypes.ThirdSubDependencyImpl>();

            // act
            var someService = service.Resolve<AbstractedTypes.ISomeService>();

            // assert
            someService.GetString().ShouldBe("SomeServiceImpl with SomeDependencyImpl and AnotherDependencyImpl containing FirstSubDependencyImpl, SecondSubDependencyImpl, and ThirdSubDependencyImpl");
        }

        class AbstractedTypes
        {
            public interface ISomeService
            {
                string GetString();
            }

            public class SomeServiceImpl : ISomeService
            {
                readonly ISomeDependency someDependency;
                readonly IAnotherDependency anotherDependency;

                public SomeServiceImpl(ISomeDependency someDependency, IAnotherDependency anotherDependency)
                {
                    this.someDependency = someDependency;
                    this.anotherDependency = anotherDependency;
                }

                public string GetString()
                {
                    return string.Format("SomeServiceImpl with {0} and {1} containing {2}",
                                         someDependency.GetType().Name,
                                         anotherDependency.GetType().Name,
                                         anotherDependency);
                }
            }

            public interface ISomeDependency {}
            
            public class SomeDependencyImpl : ISomeDependency {}

            public interface IAnotherDependency {}

            public class AnotherDependencyImpl : IAnotherDependency
            {
                readonly IFirstSubDependency firstSubDependency;
                readonly ISecondSubDependency secondSubDependency;
                readonly IThirdSubDependency thirdSubDependency;

                public AnotherDependencyImpl(IFirstSubDependency firstSubDependency,
                    ISecondSubDependency secondSubDependency,
                    IThirdSubDependency thirdSubDependency)
                {
                    this.firstSubDependency = firstSubDependency;
                    this.secondSubDependency = secondSubDependency;
                    this.thirdSubDependency = thirdSubDependency;
                }

                public override string ToString()
                {
                    return string.Format("{0}, {1}, and {2}",
                                         firstSubDependency.GetType().Name,
                                         secondSubDependency.GetType().Name,
                                         thirdSubDependency.GetType().Name);
                }
            }
            
            public interface IFirstSubDependency {}

            public class FirstSubDependencyImpl : IFirstSubDependency { }
            
            public interface ISecondSubDependency { }

            public class SecondSubDependencyImpl : ISecondSubDependency { }
            
            public interface IThirdSubDependency { }
            
            public class ThirdSubDependencyImpl : IThirdSubDependency { }
        }
    }
}