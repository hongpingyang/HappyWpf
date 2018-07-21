using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCore.Events
{
    public interface IEventSubscription
    {
        /// <summary>
        /// 执行
        /// </summary>
        void InvokePublish(params object[] arguments);
    }

}
