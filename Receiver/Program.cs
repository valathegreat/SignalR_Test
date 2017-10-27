using System;
using EasyNetQ;

namespace Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = RabbitHutch.CreateBus("host=localhost");
            bus.Subscribe<MyMessage>("test", message => Console.WriteLine(message.Text));
        }
    }
    public class MyMessage
    {
        public string Text { get; set; }
    }
    public class PlayerPosition
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
