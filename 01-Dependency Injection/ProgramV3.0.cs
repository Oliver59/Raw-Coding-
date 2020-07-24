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
            dependencyContainer.AddDependency<HelloService>();

            //初始化依赖解析器
            DependencyResolver dependencyResolver = new DependencyResolver(dependencyContainer);
            //得到对象
            var helloService = dependencyResolver.GetService<HelloService>();
            helloService.Print();

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
        public void Print()
        {
            Console.WriteLine($"Hello!");
        }
    }

}
