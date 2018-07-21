using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCore.Ioc
{
    [AttributeUsage(AttributeTargets.Constructor,AllowMultiple =false,Inherited=false)]
    public sealed  class ConstructorAttribute:Attribute
    {
    }
}
