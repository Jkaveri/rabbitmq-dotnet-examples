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
                const string exchangeName = "logs";
                var queueName = channel.QueueDeclare().QueueName;

                var message = GetMessage(args);
                var body = Encoding.UTF8.GetBytes(message);


                channel.ExchangeDeclare(exchange: exchangeName, type: "fanout");

                channel.QueueBind(queue: queueName,
                  exchange: exchangeName,
                  routingKey: "");

                channel.BasicPublish(exchange: exchangeName,
                                     routingKey: "",
                                     basicProperties: null,
                                     body: body);

                Console.WriteLine(" [x] Sent {0}", message);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "Hello World....");
        }
    }
}