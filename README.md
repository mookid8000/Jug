What?
====

Jug is an experimental DI container.

It is nowhere near finished. Actually it is probably not even the slightest bit functional.

Why?
====

After having messed a little bit with Castle Windsor's source code, I have been having some nasty thoughts on how container extensibility could be implemented. It may or may not work out.

What's interesting about it?
====

Not much, really. I was just a little bit inspired by one of the posts on Castle Windsor's UserVoice, that suggested that the container be put more to use in extensibility scenarios.

E.g. it is annoying with Windsor that IHandlerSelector is just a plan old object instance, that sits right there in the container - instantiated and all. Why not pull IHandlerSelectors from the container?

In Jug, you select between various implementations of the same service like so:

	var container = new Jug.Container();
	
	// register services
	container.Register<ISomeService, FirstImplementation>()
		.Register<ISomeService, SecondImplementation>();

	// register selector
	container.Register<IComponentSelector<ISomeService>>, MySelector>();

	// selector will be asked for its opinion now:
	var instance = container.Resolve<ISomeService>();

and MySelector could look something like this:

	public class MySelector : IComponentSelector<ISomeService>
	{
		public ComponentModel Select(ComponentModel[] componentModels)
		{
			return componentModels.Single(c => c.ImplementationType.Name.StartsWith("Second"));
		}
	}

ensuring that @SecondImplementation@ will be picked.

Now, imaging how nifty this could be when the selector depends on various business services and stuff, thus providing the hook that has always been missing in Windsor!

Disclaimer
====

As I said: "It is nowhere near finished. Actually it is probably not even the slightest bit functional."

Seriously!