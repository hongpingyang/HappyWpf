using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace HappyCore.Ioc
{
    /// <summary>
    /// hpy 简单的Ioc容器  
    /// </summary>
    public class hpyContainer:IhpyIoc
    {
        #region 字段

        /// <summary>
        /// 类型映射表
        /// </summary>
        private ConcurrentDictionary<Type, Type> typeMapping = new ConcurrentDictionary<Type, Type>();

        /// <summary>
        /// 单例映射表
        /// </summary>
        private ConcurrentDictionary<Type, Type> singletonMapping = new ConcurrentDictionary<Type, Type>();

        /// <summary>
        /// 类型名称映射表
        /// </summary>
        private ConcurrentDictionary<CustomKey, Type> typeNameMapping = new ConcurrentDictionary<CustomKey, Type>();

        /// <summary>
        /// 实例映射表
        /// </summary>
        private ConcurrentDictionary<Type, object> ObjectMapping = new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// 实例名称映射表
        /// </summary>
        private ConcurrentDictionary<CustomKey, object> ObjectNameMapping = new ConcurrentDictionary<CustomKey, object>();

        /// <summary>
        /// 锁
        /// </summary>
        private object thislock = new object();

        /// <summary>
        /// 是否支持内置类型
        /// </summary>
        private bool isSupportBuildType = false;
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public hpyContainer()
        {

        }

        /// <summary>
        /// 构造函数是否支持内置类型
        /// </summary>
        /// <param name="isSupportBuildType">是否支持内置类型</param>
        public hpyContainer(bool isSupportBuildType)
        {
            this.isSupportBuildType = isSupportBuildType;
        }
      

        #region 类型注册
        /// <summary>
        /// 注册类型  
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void RegisterType(Type from, Type to,string name=null)
        {
            RegisterTypeMethod(from, to, name);
        }

        public void RegisterType<TFrom, TTo>(string name=null)
        {
            RegisterTypeMethod(typeof(TFrom), typeof(TTo), name);
        }

        public void RegisterType(Type from,string name=null)
        {
            RegisterTypeMethod(from, from, name);
        }

        public void RegisterType<TFrom>(string name=null)
        {
            RegisterTypeMethod(typeof(TFrom), typeof(TFrom), name);
        }

        private void RegisterTypeMethod(Type from, Type to, string name)
        {
            object objectValue;
            Type keyValue;
            lock (thislock)
            {
                if (name == null)
                {
                    typeMapping[from] = to;

                    ObjectMapping.TryRemove(from, out objectValue);

                    singletonMapping.TryRemove(from, out keyValue);
                }
                else
                {
                    CustomKey key = new CustomKey(from, name);
                    typeNameMapping[key] = to;

                    ObjectNameMapping.TryRemove(key, out objectValue);
                }
            }
        }

        /// <summary>
        /// 实例注册
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="name"></param>
        public void RegisterInstance(Type from, object instance,string name=null)
        {
            RegisterInstanceMethod(from, instance, name);
        }

        public void RegisterInstance<TFrom>(object instance, string name=null)
        {
            RegisterInstanceMethod(typeof(TFrom), instance, name);
        }

        private void RegisterInstanceMethod(Type from, object instance, string name)
        {
            Type value;
            lock (thislock)
            {
                if (name == null)
                {
                    ObjectMapping[from] = instance;

                    typeMapping.TryRemove(from, out value);

                    singletonMapping.TryRemove(from, out value);
                }
                else
                {
                    CustomKey key = new CustomKey(from, name);
                    ObjectNameMapping[key] = instance;

                    typeNameMapping.TryRemove(key, out value);
                }
            }
        }

        /// <summary>
        /// 单例注册
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="name"></param>
        public void RegisterSingleton(Type from, Type to)
        {
            RegisterSingletonMethod(from, to);
        }

        public void RegisterSingleton<TFrom, TTo>()
        {
            RegisterSingletonMethod(typeof(TFrom), typeof(TTo));
        }

        public void RegisterSingleton(Type from)
        {
            RegisterSingletonMethod(from, from);
        }

        public void RegisterSingleton<TFrom>()
        {
            RegisterSingletonMethod(typeof(TFrom), typeof(TFrom));
        }

        private void RegisterSingletonMethod(Type from,Type to)
        {
            Type KeyValue;

            object objectValue;

            lock (thislock)
            {
                singletonMapping[from] = to;

                typeMapping.TryRemove(from, out KeyValue);

                ObjectMapping.TryRemove(from, out objectValue);
            }
        }

        /// <summary>
        /// 是否已经注册过类型
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public bool IsTypeRegisterd(Type from,string name=null)
        {
            return IsTypeRegisterdMethod(from,name);
        }

        public bool IsTypeRegisterd<TFrom>(string name=null)
        {
            return IsTypeRegisterdMethod(typeof(TFrom),name);
        }

        private bool IsTypeRegisterdMethod(Type from, string name)
        {
            lock (thislock)
            {
                if (name == null)
                {
                    return typeMapping.ContainsKey(from) || ObjectMapping.ContainsKey(from) || singletonMapping.ContainsKey(from);
                }
                else
                {
                    CustomKey key = new CustomKey(from, name);
                    return typeNameMapping.ContainsKey(key) || ObjectNameMapping.ContainsKey(key);
                }
            }
        }


        #endregion

        #region  解析类型

        /// <summary>
        /// 解析类型
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public T Resolve<T>(string name=null)
        {
            Type type = typeof(T);
            object obj = ResolveMethod(name, type);
            return (T)obj;
        }

        public object Resolve(Type @Type, string name=null)
        {
            object obj = ResolveMethod(name, @Type);
            return obj;
        }

        private object ResolveMethod(string name, Type type)
        {
            object obj;
            if (name == null)
            {
                obj = this.DoResolve(type);

                if (singletonMapping.ContainsKey(type))
                {
                    if (!ObjectMapping.ContainsKey(type))
                    {
                        ObjectMapping[type] = obj;
                    }
                }
            }
            else
            {
                obj = this.DoResolve(type, name);
            }

            return obj;
        }


        /// <summary>
        /// 解析函数 会递归调用解析构造参数；
        /// 对于不在映射表里的，如果是可实例化的类型也能解析
        /// 对于内置类型，如果支持的话，构造为默认值
        /// </summary>
        /// <param name="Type">类型</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        private object DoResolve(Type @Type,string name=null)
        {
            bool isMapping = false;
            Type type;
            object instance;

            if (name == null)
            {
                if (typeMapping.TryGetValue(@Type, out type))
                {
                    isMapping = true;
                }
                
                else if (ObjectMapping.TryGetValue(@Type, out instance))
                {
                    isMapping = true;
                    return instance;
                }
                else if (singletonMapping.TryGetValue(@Type, out type))//第一次进入
                {
                    isMapping = true;
                }
                else
                {
                    type = @Type;
                    isMapping = false;
                }
            }
            else  
            {
                CustomKey key = new CustomKey(@Type, name);

                if (typeNameMapping.TryGetValue(key, out type))
                {
                    isMapping = true;
                }
                else if (ObjectNameMapping.TryGetValue(key, out instance))
                {
                    isMapping = true;
                    return instance;
                }
                else
                {

                    if (typeNameMapping.ContainsKey(key))
                    {

                    }
                        type = @Type;
                    isMapping = false;
                }
            }

            if (!isMapping &&!isSupportBuildType&&IsBulitinType(@Type))
            {
                throw new TargetException($"当前{@Type.Name}不支持,可通过设置isSupportBuildType支持");
            }

            if (type.IsInterface)
            {
                throw new TargetException($"当前{type.Name}是个接口,不能被构造,是否缺少对照");
            }
            else if (type.IsAbstract)
            {
                throw new TargetException($"当前{type.Name}是个抽象类，不能被构造,是否缺少对照");
            }


            //通过反射，获取对象的构造函数
            ConstructorInfo constructor = GetConstructor(type);
            if (null == constructor)
            { return null; }

            // 获取构造函数的参数，以及使用 递归得到实参来初始化
            object[] arguments = constructor.GetParameters().Select(p => this.DoResolve(p.ParameterType)).ToArray();

            //获得构造函数创造的对象
            object service = constructor.Invoke(arguments);

            return service;
        }

        /// <summary>
        /// 获取构造函数
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual ConstructorInfo GetConstructor(Type type)
        {
            //找到构造函数
            ConstructorInfo[] c = type.GetConstructors();

            return c.FirstOrDefault(p => p.GetCustomAttribute(typeof(ConstructorAttribute)) != null) ?? c.FirstOrDefault();
        }




        #endregion

        /// <summary>
        /// 判断是否是内置类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsBulitinType(Type type)
        {
            return (type == typeof(object) || Type.GetTypeCode(type) != TypeCode.Object);
        }
    }


    /// <summary>
    /// 自定义key类型
    /// </summary>
    public class CustomKey 
    {
        private Type type;
        private string name;

        public CustomKey(Type type,string name)
        {
            this.type = type;
            this.name = name;
        }

        public override  bool Equals(Object other)
        {
            if (other == null) return false;
            var custom = (CustomKey)other;
            if (ReferenceEquals(this, custom)) return true;
            return name == custom.name && type == custom.type;
        }

        public override int GetHashCode()
        {
            return (type + name).GetHashCode();
        }
    }
}
