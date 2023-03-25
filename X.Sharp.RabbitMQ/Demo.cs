using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace X.Sharp.RabbitMQ
{
    public class Demo
    {
        static ManualResetEvent _event = new ManualResetEvent(false);
        static void Main(string[] args)
        {
            Task.Factory.StartNew(() => { SendByExchange(true); });
            Thread.Sleep(1000);
            //Task.Factory.StartNew(() => { Receive("11"); });
            Receive("22");
            Console.ReadKey();
        }
        // queue-persistent
        public static void SendByQueue(bool isLoopSend = false)
        {
            // 创建连接工厂
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
            // 从连接工厂创建一个连接
            using var connection = factory.CreateConnection();
            // 创建一个信道
            using var channel = connection.CreateModel();
            // 申明一个队列
            channel.QueueDeclare(queue: "demo", durable: true, exclusive: false, autoDelete: false,
                arguments: null);

            // 消息持久化 persistent=true
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            // 发送消息
            var count = 1;
            var message = $"第{count}条消息,{DateTime.Now.ToLocalTime()}";
            while (isLoopSend)
            {
                message = $"第{count}条消息,{DateTime.Now.ToLocalTime()}";
                var body = System.Text.Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange:"",routingKey:"demo",basicProperties:properties,body:body);
                Console.WriteLine($"开始发送第{count}条消息");
                count++;
                Thread.Sleep(1000);
            }
            if(!isLoopSend)
            {
                var body = System.Text.Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "", routingKey: "demo", basicProperties: properties, body: body);
            }
        }
        // exchange

        public static void SendByExchange(bool isLoopSend = false)
        {
            // 创建连接工厂
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
            // 从连接工厂创建一个连接
            using var connection = factory.CreateConnection();
            // 创建一个信道
            using var channel = connection.CreateModel();
            // 申明一个交换机
            channel.ExchangeDeclare(exchange: "exchange_direct", type: ExchangeType.Direct);
            channel.QueueDeclare(queue: "direct_demo", durable: false, exclusive: false, autoDelete: false,
                arguments: null);
            channel.QueueBind(queue: "direct_demo", exchange: "exchange_direct", routingKey: "rk_direct");

            // 消息持久化 persistent=true
            var properties = channel.CreateBasicProperties();
            //properties.Persistent = true;

            // 发送消息
            var count = 1;
            var tag = "[exchange_direct]";
            var message = $"[{tag}]第{count}条消息,{DateTime.Now.ToLocalTime()}";
            while (isLoopSend)
            {
                message = $"[{tag}]第{count}条消息,{DateTime.Now.ToLocalTime()}";
                var body = System.Text.Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "exchange_direct", routingKey: "rk_direct", basicProperties: properties, body: body);
                Console.WriteLine($"开始发送第{count}条消息");
                count++;
                Thread.Sleep(1000);
            }
            if (!isLoopSend)
            {
                var body = System.Text.Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "exchange_direct", routingKey: "rk_direct", basicProperties: properties, body: body);
            }
        }

        public static void Receive(string name)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
            // 从连接工厂创建一个连接
            using var connection = factory.CreateConnection();
            // 创建一个信道
            using var channel = connection.CreateModel();
            // prefetchCount: 1表示 未收到消费端确认时 不继续分发消息
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            // 创建消息消费者
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, ea) =>
            {
                var message = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine($"{name}-> [{ea.DeliveryTag}] Received {message}" );
                Thread.Sleep(2000);
                // 发送消息确认信号 手动消息确认
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
            // 启动消费者
            //autoAck:true 自动进行消息确认  false 关闭自动消息确认，通过调用BasicAck方法手动进行消息确认
            channel.BasicConsume(queue:"demo", autoAck: false, consumer);
            Console.WriteLine("消费者启动完成");
            _event.WaitOne();
            //Console.ReadLine();
        }

    }
}