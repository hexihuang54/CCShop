using log4net;
using log4net.Config;
using log4net.Repository;
using System.IO;

namespace CCShop.Log4Net
{
    public class LogBaseProxy : ILogBaseProxy
    {
        public LogBaseProxy(string configstring, string repositoryname = "")
        {
            XmlConfigurator.Configure(LogManager.CreateRepository(repositoryname), new FileInfo(configstring));
        }

        public ILogClient CreateLogClient(string name = "")
        {
            return new LogClient(name);
        }
    }
}
