using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCShop.Log4Net
{
    public class LogClient : ILogClient
    {
        private ILog _Log = null;

        public LogClient(string repository, string name = "")
        {
            this._Log = LogManager.GetLogger(repository, name);
        }

        public void Debug(string msg)
        {
            this._Log.Debug(msg);
        }

        public void Err(Exception ex, string msg = null)
        {
            this._Log.Error(msg, ex);
        }

        public void Info(string msg)
        {
            this._Log.Info(msg);
        }

        public void Shutdown()
        {
            LogManager.Shutdown();
        }
    }
}
