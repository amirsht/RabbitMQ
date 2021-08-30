using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Receiver
{
    class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" }; // If your broker resides on a different machine, you can specify the name or the IP address.
            using (var connection = factory.CreateConnection()) // Creates the RabbitMQ connection
            using (var channel = connection.CreateModel()) // Creates a channel, which is where most of the API for getting things done resides.
            {
                //Declares the queue
                var queueName = "queue1";
                var durable = false; // true if we are declaring a durable queue(the queue will survive a server restart)
                var exclusive = false; // true if we are declaring an exclusive queue (restricted to this connection)
                var autoDelete = true; // true if we are declaring an auto delete queue (server will delete it when no longer in use)

                channel.QueueDeclare(queueName, durable, exclusive, autoDelete, null);


                var consumer = new EventingBasicConsumer(channel);
                // Callback
                consumer.Received += (model, deliveryEventArgs) =>
                {
                    var body = deliveryEventArgs.Body.ToArray();
                    // convert the message back from byte[] to a string
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("** Received message: {0} by Consumer thread **", message);
                };

                // Start receiving messages
                channel.BasicConsume(queue: queueName, // the name of the queue
                                     autoAck: true, // true if the server should consider messages acknowledged once delivered;
                                     consumer: consumer); // an interface to the consumer object
                Console.WriteLine("Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}