using NUnit.Framework;

namespace Jug.Tests
{
    public abstract class FixtureFor<TService>
    {
        protected TService service;

        [SetUp]
        public void SetUp()
        {
            service = Create();
        }

        protected abstract TService Create();
    }
}