using Nest;
using System;

namespace CCShop.ELS
{
    public interface IELSManagerClient : IDisposable
    {
        ConnectionSettings CreateElsConnectionSettings();
    }
}
