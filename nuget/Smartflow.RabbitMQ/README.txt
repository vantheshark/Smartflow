Smartflow https://github.com/vanthoainguyen/Smartflow
========

This module allows messages to be distributed to remote RabbitMQ queues
How to use:

You will need following config in your app.config

    <appSettings>
        <add key="MessageQueueConnectionString" value="host=localhost;user=guest;password=guest"/>
        <add key="SetupQueues" value="true"/>
        <add key="TaskConcurrencyLevel" value="1"/>        
    </appSettings>

Because messages in Smartflow can have Priority of any uint value. Apparently, we need to define phisical queueu to route those messages to by their priority.
Therefore, this abstract class is required

    /// <summary>
    /// Implement this interface to provide a mapping logic of converting any logical uint priority using in the application to phisical rabbitMQ priority queue
    /// </summary>
    public abstract class LogicalPriorityMapper
    {
        /// <summary>
        /// Get the priority value of the queue for the provided message priority
        /// </summary>
        /// <param name="logicalPriority"></param>
        /// <returns></returns>
        public abstract uint GetQueuePriority(uint logicalPriority);
    }
	
Then in the code:

var rabbitMqModule = new RabbitMqModule().WithConfig(ConfigurationManager.AppSettings);
rabbitMqModule.Load(new YourLogicalPriorityMapper()).Start(); 