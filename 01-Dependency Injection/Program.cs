using System;
using System.Collections.Generic;
using System.Dynamic;
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
                .AddTransient<ConsumerService>()
                .AddTransient<HelloService>()
                .AddSingleton<MessageService>();

            //初始化依赖解析器
            DependencyResolver dependencyResolver = new DependencyResolver(dependencyContainer);
            //得到对象
            var consumerService1 = dependencyResolver.GetService<ConsumerService>();
            consumerService1.Print();

            var consumerService2 = dependencyResolver.GetService<ConsumerService>();
            consumerService2.Print();

            var consumerService3 = dependencyResolver.GetService<ConsumerService>();
            consumerService3.Print();

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
            var constructor = dependency.Type.GetConstructors().Single();
            //得到构造函数所对应的参数
            var parameters = constructor.GetParameters();

            if (parameters.Length > 0)//如果该构造函数有参数，则初始化参数
            {
                return createImplementation(dependency, type =>
                {
                    var parameterImplementations = new object[parameters.Length];
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        parameterImplementations[i] = GetService(parameters[i].ParameterType);
                    }
                    return Activator.CreateInstance(type, parameterImplementations);
                });
            }
            return createImplementation(dependency, type => Activator.CreateInstance(type));
        }

        private object createImplementation(Dependency dependency, Func<Type, object> factory)
        {
            if (dependency.Implemented)
            {
                return dependency.Implementation;
            }

            var implementation = factory(dependency.Type);
            if (dependency.Lifetime == DependencyLifetime.Singleton)
            {
                dependency.AddImplementation(implementation);
            }
            return implementation;
        }
    }

    /// <summary>
    /// 依赖注入容器
    /// </summary>
    public class DependencyContainer
    {
        List<Dependency> _dependencys;
        public DependencyContainer()
        {
            _dependencys = new List<Dependency>();
        }

        public DependencyContainer AddSingleton<T>()
        {
            _dependencys.Add(new Dependency(typeof(T), DependencyLifetime.Singleton));
            return this;
        }

        public DependencyContainer AddTransient<T>()
        {
            _dependencys.Add(new Dependency(typeof(T), DependencyLifetime.Transient));
            return this;
        }

        public Dependency GetDependency(Type type)
        {
            return this._dependencys.First(c => c.Type.Name == type.Name);
        }
    }

    public class Dependency
    {
        public Type Type;
        public DependencyLifetime Lifetime;
        public object Implementation;
        public bool Implemented;

        public Dependency(Type type, DependencyLifetime lifetime)
        {
            Type = type;
            Lifetime = lifetime;
        }

        public void AddImplementation(object implementation)
        {
            this.Implementation = implementation;
            this.Implemented = true;
        }
    }

    public enum DependencyLifetime
    {
        Singleton = 0,
        Transient = 1,
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

        int _random;
        public HelloService(MessageService messageService)
        {
            _random = new Random().Next();
            this._messageService = messageService;
        }
        public void Print()
        {
            Console.WriteLine($"Hello #{_random} {_messageService.Message()} !");
        }
    }

    public class MessageService
    {
        int _random;
        public MessageService()
        {
            _random = new Random().Next();
        }
        public string Message()
        {
            return $"Yo #{_random}";
        }
    }

}
