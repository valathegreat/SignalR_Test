using System;
using EasyNetQ;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var myRequest = new MyRequest { Text = "Hello Server" };
            var bus = RabbitHutch.CreateBus("host=localhost");
            //var task = bus.RequestAsync<TestRequestMessage, TestResponseMessage>(request);
            //task.ContinueWith(response => {
            //    Console.WriteLine("Got response: '{0}'", response.Result.Text);
            //});

           
        }
    }

    public class MyRequest
    {
        public string Text { get; set; }
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
