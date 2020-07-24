using System;

namespace Dependency_Injection
{
    class Program
    {
        static void Main(string[] args)
        {
            HelloService helloService = new HelloService();
            ConsumerService consumerService = new ConsumerService(helloService);
            consumerService.Print();

            Console.ReadKey();
        }
    }

    public class ConsumerService
    {
        private readonly HelloService _helloService;

        public ConsumerService(HelloService helloService)
        {
            this._helloService = helloService;
        }

        public void Print()
        {
            _helloService.Print();
        }
    }
    public class HelloService
    {
        public void Print()
        {
            Console.WriteLine($"Hello!");
        }
    }

}
