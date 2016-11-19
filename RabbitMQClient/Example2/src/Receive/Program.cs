using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace Receive
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
                                        durable: true, // Keep meesage until it was proccessed success full.
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                // In order to defeat that we can use the basicQos method with the prefetchCount = 1
                // setting.This tells RabbitMQ not to give more than one message to a worker at a
                // time. Or, in other words, don't dispatch a new message to a worker until it has
                // processed and acknowledged the previous one. Instead, it will dispatch it to the
                // next worker that is not still busy.
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);

                    int dots = message.Split('.').Length - 1;
                    Thread.Sleep(dots * 1000);

                    Console.WriteLine(" [x] Done");

                    // Let the queue knowns message was delivered success.
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                
                // Message acknowledgments are turned on by default. In
                // previous examples we explicitly turned them off by setting the noAck ("no manual
                // acks") parameter to true. It's time to remove this flag and send a proper
                // acknowledgment from the worker, once we're done with a task
                channel.BasicConsume(queue: "task_queue_2",
                                     noAck: false,
                                     consumer: consumer);


                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();

            }
        }
    }
}