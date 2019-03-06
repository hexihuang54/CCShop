using Elasticsearch.Net;
using Nest;
using System;

namespace CCShop.ELS
{
    public class ELSManagerClient : IELSManagerClient
    {
        private IConnectionPool elsConnectionPool = null;

        public ELSManagerClient(string elsconstring)
        {
            var nodes = new Uri[] { new Uri(elsconstring) };
            this.elsConnectionPool = new StaticConnectionPool(nodes);
        }

        public ELSManagerClient(Uri[] nodes)
        {
            this.elsConnectionPool = new StaticConnectionPool(nodes);
        }

        public ConnectionSettings CreateElsConnectionSettings()
        {
            return new ConnectionSettings(this.elsConnectionPool).DefaultIndex("default");
        }

        public void Dispose()
        {
            elsConnectionPool?.Dispose();
            Console.WriteLine("elsConnectionPool 被释放 ");
        }
    }
}
