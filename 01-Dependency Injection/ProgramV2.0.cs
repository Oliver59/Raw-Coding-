using System;

namespace Dependency_Injection
{
    class Program
    {
        static void Main(string[] args)
        {
            HelloService helloService = (HelloService)Activator.CreateInstance(typeof(HelloService));//动态创建对象HelloService
            ConsumerService consumerService = (ConsumerService)Activator.CreateInstance(typeof(ConsumerService), helloService);//动态创建对象ConsumerService
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
