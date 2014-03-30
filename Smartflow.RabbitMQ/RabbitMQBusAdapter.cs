using Smartflow.Core.CQRS;

namespace Smartflow.RabbitMQ
{
    internal class RabbitMqBusAdapter : InternalBus
    {
        private readonly RabbitMqBus _rabbitBus;

        public RabbitMqBusAdapter(RabbitMqBus rabbitBus) : base(null, null)
        {
            _rabbitBus = rabbitBus;
        }

        public override void Send<T>(T command)
        {
            _rabbitBus.Send(command);
        }

        public override void Publish<T>(T @event)
        {
            _rabbitBus.Publish(@event);
        }
    }
}