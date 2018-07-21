using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCore.Events
{
    public enum ThreadOption
    {
        /// <summary>
        /// 与发布者同一个线程
        /// </summary>
        PublisherThread,
        /// <summary>
        /// UI thread.
        /// </summary>
        UIThread,
        /// <summary>
        /// 异步线程
        /// </summary>
        BackgroundThread
    }
}
