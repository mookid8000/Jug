using System;
using System.Diagnostics;
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

    public static class Extensions
    {
        public static void Times(this int count, Action action)
        {
            for (var counter = 0; counter < count; counter++)
            {
                action();
            }
        }

        public static void Measure(this Stopwatch stopwatch, Action action, string message, params object[] objs)
        {
            Console.WriteLine(message, objs);
            stopwatch.Start();
            action();
            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;
            Console.WriteLine("{0:0.0} s", elapsed.TotalSeconds);
        }
    }
}