using System;

namespace CCShop.ELS
{
    public class ELSBaseProxy : IELSBaseProxy
    {
        private IELSManagerClient elsManagerClient = null;

        private string indexName = string.Empty;
        private string strDocType = string.Empty;

        public ELSBaseProxy(string elsconstring)
        {
            this.elsManagerClient = new ELSManagerClient(elsconstring);
        }

        public ELSBaseProxy(string elsconstring, string indexname, string doctype)
        {
            this.elsManagerClient = new ELSManagerClient(elsconstring);
            this.indexName = indexname;
            this.strDocType = doctype;
        }

        public ELSBaseProxy(Uri[] nodes)
        {
            this.elsManagerClient = new ELSManagerClient(nodes);
        }

        public ELSBaseProxy(Uri[] nodes, string indexname, string doctype)
        {
            this.elsManagerClient = new ELSManagerClient(nodes);
            this.indexName = indexname;
            this.strDocType = doctype;
        }

        public IELSClient GetElsClient()
        {
            if (!string.IsNullOrEmpty(this.indexName) && !string.IsNullOrEmpty(strDocType))
            {
                return new ELSClient(this.elsManagerClient.CreateElsConnectionSettings(), this.indexName, this.strDocType);
            }
            else
            {
                return new ELSClient(this.elsManagerClient.CreateElsConnectionSettings());
            }
        }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            this.elsManagerClient?.Dispose();
            Console.WriteLine("elsManagerClient 被释放 ");
        }

    }


}
