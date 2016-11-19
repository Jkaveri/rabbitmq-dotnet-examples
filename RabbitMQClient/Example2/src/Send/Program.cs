using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace Send
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "task_queue_2",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var message = GetMessage(args);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                // Let RabbitMQ known it should store a message on disk.
                properties.Persistent = true;
                channel.BasicPublish(exchange: "",
                                     routingKey: "task_queue_2",
                                     basicProperties: properties,
                                     body: body);

                Console.WriteLine(" [x] Sent {0}", message);

                //Console.WriteLine(" Press [enter] to exit.");
                //Console.ReadLine();
            }
        }

        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "Hello World....");
        }
    }
}