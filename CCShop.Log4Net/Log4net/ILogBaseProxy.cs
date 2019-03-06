using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCShop.Log4Net
{
    public interface ILogBaseProxy
    {
        ILogClient CreateLogClient(string name = "");

    }
}
