using RabbitMQ.Client;
using System;
using System.Text;

namespace Sender
{
    class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost"}; // If your broker resides on a different machine, you can specify the name or the IP address.
            using (var connection = factory.CreateConnection()) // Creates the RabbitMQ connection
            using (var channel = connection.CreateModel()) // Creates a channel, which is where most of the API for getting things done resides.
            {
                //Declares the queue
                var queueName = "queue1";
                var durable = false; // true if we are declaring a durable queue(the queue will survive a server restart)
                var exclusive = false; // true if we are declaring an exclusive queue (restricted to this connection)
                var autoDelete = true; // true if we are declaring an auto delete queue (server will delete it when no longer in use)

                channel.QueueDeclare(queueName, durable, exclusive, autoDelete, null);


                Console.WriteLine("Please enter your message. Type 'exit' to exit.");
                while (true)
                {
                    //Converts message to byte array
                    var message = Console.ReadLine();
                    if (message?.ToUpper() == "QUIT")
                    {
                        break;
                    }

                    var data = Encoding.UTF8.GetBytes(message);

                    // publish to the "default exchange", with the queue name as the routing key
                    var exchangeName = "";
                    var routingKey = queueName;
                    channel.BasicPublish(exchangeName, routingKey, null, data);

                    Console.WriteLine("Sent: {0}", message);
                }
            }
        }
    }
}