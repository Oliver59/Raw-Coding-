using System;
using System.Collections.Generic;
using System.Linq;

namespace Dependency_Injection
{
    class Program
    {
        static void Main(string[] args)
        {
            //创建依赖容器
            DependencyContainer dependencyContainer = new DependencyContainer();
            //添加依赖对象
            dependencyContainer
                .AddDependency<HelloService>()
                .AddDependency<ConsumerService>()
                .AddDependency<MessageService>();

            //初始化依赖解析器
            DependencyResolver dependencyResolver = new DependencyResolver(dependencyContainer);
            //得到对象
            var consumerService = dependencyResolver.GetService<ConsumerService>();
            consumerService.Print();

            Console.ReadKey();
        }
    }

    /// <summary>
    /// 依赖注入解析器
    /// </summary>
    public class DependencyResolver
    {
        private DependencyContainer _dependencyContainer;

        public DependencyResolver(DependencyContainer dependencyContainer)
        {
            this._dependencyContainer = dependencyContainer;
        }

        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        public object GetService(Type type)
        {
            var dependency = this._dependencyContainer.GetDependency(type);
            //得到需要注入对象的构造函数
            var constructor = dependency.GetConstructors().Single();
            //得到构造函数所对应的参数
            var parameters = constructor.GetParameters();

            if (parameters.Length > 0)//如果该构造函数有参数，则初始化参数
            {
                var parameterImplementations = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    parameterImplementations[i] = GetService(parameters[i].ParameterType);
                }
                return Activator.CreateInstance(dependency, parameterImplementations);
            }
            return Activator.CreateInstance(dependency);
        }
    }

    /// <summary>
    /// 依赖注入容器
    /// </summary>
    public class DependencyContainer
    {
        List<Type> _typeList;
        public DependencyContainer()
        {
            _typeList = new List<Type>();
        }

        public DependencyContainer AddDependency<T>()
        {
            _typeList.Add(typeof(T));
            return this;
        }

        public Type GetDependency(Type type)
        {
            return this._typeList.First(c => c.Name == type.Name);
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
        private readonly MessageService _messageService;

        public HelloService(MessageService messageService)
        {
            this._messageService = messageService;
        }
        public void Print()
        {
            Console.WriteLine($"Hello {_messageService.Message()} !");
        }
    }

    public class MessageService
    {
        public string Message()
        {
            return $"Yo";
        }

    }

}
