using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

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
                const string exchangeName = "direct_logs";

                var message = GetMessage(args);
                var type = GetLogType(args);

                var body = Encoding.UTF8.GetBytes(message);

                channel.ExchangeDeclare(exchange: exchangeName, type: "direct");

                channel.BasicPublish(exchange: exchangeName,
                                     routingKey: type,
                                     basicProperties: null,
                                     body: body);

                Console.WriteLine(" [x] Sent {0}", message);

                // Console.WriteLine(" Press [enter] to exit.");
                // Console.ReadLine();
            }
        }

        private static string GetLogType(string[] args)
        {
            return (args.Length > 0) ? args[0].ToLower() : "info";
        }

        private static string GetMessage(string[] args)
        {
            return (args.Length > 1) ? string.Join(" ", args.Skip(1).ToArray()) : "Hello World....";
        }
    }
}