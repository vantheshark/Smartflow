Smartflow https://github.com/vanthoainguyen/Smartflow
========

Facilitate SRP & make a friendly module for those who has been familiar with ASP.NET MVC

Smartflow is a small & simple module inspired by Greg Young SimpleCQRS. It also borrows alot of ASP.NET MVC source code to facilitate API filtering style such as Attribute filters, Global filters. Dependency injection is also fully supported and on top of it, the aim of this module is keep your code simple and easy to maintain if you want to build CQRS and event/messaging style application. 

However, Smartflow is not a CQRS framework. It doesn't have anything like EventStore or Aggregate Root. It's more like a messaging framework which you can register your logic to handle a message, send command and publish event. It supports priority on your messages so higher prority messages will be handled before the lower one. I doesn't support message validation because these are system generated messages, not something input by user; but if you want, it won't stop you to add a custom IHandlerInvoker and introduce "ModelState" to take care of validation like ASP.NET MVC.

This module is very opinionated. I used Greg Young SimpleCQRS as the background and change it in my way to use for our harvesting system. In our company, we've been using it successfully with RabbitMQ so I think It's a good idea to share with community.

Apparently, to build a messaging system, you must have messages. A message can be a Command or an Event. A command must have only 1 handler but an event can have multiple handlers. I'm sure there is a great guild line for naming convention by Greg Young, so just follow him to name your messages.

To get started, let's build a simple console application to search for news articles on BingNews by some keywords. It will save everything it found, save an audit for last article date and then do it again every 30 seconds.

1/ Define Command, Event


public class NewsSearchCommand : Command
{
	public string Query { get; set; }
	public DateTime LatestArticleDate { get; set; }
}
	
public class SearchResult
{
	public string Title { get; set; }
	public string Link { get; set; }
	public string Description { get; set; }
	public DateTime PublishDate { get; set; }
}

public class NewsArticleFound : Event
{
	public SearchResult Article { get; set; }
}


2/ Register handlers:


HandlerProvider.Providers.RegisterHandler(new BingNewsSearchCommandHandler());


3/ Send commands, publish events in your Program.cs


t = new Timer(_ =>
{
	var trackingQuery = new [] {"#MH370", "Manchester United"};

	var allAudit = _auditService.GetAll();

	var commands = trackingQuery.GroupJoin(allAudit, x => x, a => a.Keyword, (x,a) => new NewsSearchCommand
	{
		LatestArticleDate = a.DefaultIfEmpty().FirstOrDefault() != null 
						  ? a.DefaultIfEmpty().First().LastArticleDate 
						  : DateTime.MinValue,
		Query = x
	}).ToList();
	commands.ForEach(c => _commandSender.Send(c));
});
t.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(20));


4/ Integrated with Autofac


public class AutofacDependencyResolver : IDependencyResolver
{
	private readonly IContainer _container;

	public AutofacDependencyResolver(IContainer container)
	{
		_container = container;
	}

	public T GetService<T>() where T : class
	{
		return _container.Resolve<T>();
	}

	public IEnumerable<T> GetServices<T>() where T : class
	{
		return _container.Resolve<IEnumerable<T>>();
	}

	public IEnumerable<object> GetServices(Type serviceType)
	{
		var svc =  _container.Resolve(serviceType);
		return svc != null ? new[] {svc} : new object[0];
	}
}

var builder = new ContainerBuilder();
// type registrations ...
_container = builder.Build();
DependencyResolver.SetDependencyResolver(new AutofacDependencyResolver(_container));
