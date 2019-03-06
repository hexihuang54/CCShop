using System;

namespace CCShop.ELS
{
    public interface IELSBaseProxy : IDisposable
    {
        IELSClient GetElsClient();
    }
}
