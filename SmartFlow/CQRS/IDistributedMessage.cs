
namespace Smartflow.Core.CQRS
{
    /// <summary>
    /// A command or event that implements this interface can be deliverd to remote queue using RabbitMQ
    /// Ofcourse it depends on the ICommandSender and IEventPublisher implementation to deal with that
    /// <para>A message implement this interface should not have properties of type Action/Func</para>
    /// <para>Message implements this interface must have parameterless constructor</para>
    /// </summary>
    public interface IDistributedMessage
    {
    }
}
