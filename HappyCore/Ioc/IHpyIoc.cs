using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCore.Ioc
{
    /// <summary>
    /// 
    /// </summary>
    public interface IhpyIoc
    {
        void RegisterType(Type from, Type to, string name = null);

        void RegisterType<TFrom, TTo>(string name = null);

        void RegisterType(Type from, string name = null);

        void RegisterType<TFrom>(string name = null);

        void RegisterInstance(Type from, object instance, string name = null);

        void RegisterInstance<TFrom>(object instance, string name = null);

        void RegisterSingleton(Type from, Type to);

        void RegisterSingleton<TFrom, TTo>();

        void RegisterSingleton(Type from);

        void RegisterSingleton<TFrom>();

        bool IsTypeRegisterd(Type from, string name = null);

        bool IsTypeRegisterd<TFrom>(string name = null);

        T Resolve<T>(string name = null);

        object Resolve(Type @Type, string name = null);

    }
}
