using HappyCore.Ioc;

namespace HappyCore
{
    /// <summary>
    /// 单例Ioc容器
    /// </summary>
    public static class ServiceLocator
    {
        public static bool IsSupportBuildType { get; set; }
        private static readonly object _instanceLock = new object();

        private static hpyContainer _instance;
        public static hpyContainer instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                        _instance = new hpyContainer(IsSupportBuildType);
                }
                return _instance;
            }
        }
    }
}
