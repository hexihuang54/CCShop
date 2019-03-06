using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCShop.Log4Net
{
    public interface ILogClient
    {
        
        void Info(string msg);

        void Err(Exception ex, string msg = null);

        void Debug(string msg);

        void Shutdown(); 

    }
}
