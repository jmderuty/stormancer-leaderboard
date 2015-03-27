using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaderboard
{
    class IndexFactory
    {
        public static Nest.ElasticClient GetClient()
        {
            var connection = new Elasticsearch.Net.Connection.HttpClientConnection(
                new ConnectionSettings(),
                new AuthenticatedHttpClientHandler(Constants.IndexKey));

            var nest = new Nest.ElasticClient(new ConnectionSettings(new Uri(Constants.ServerEndpoint + "/" + Constants.IndexAccount + "/_indices/_q"), Constants.IndexName), connection);
            return nest;
        }
    }
}
