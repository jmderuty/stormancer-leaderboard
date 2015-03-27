using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigureIndex
{
    class Program
    {
        static void Main(string[] args)
        {
            Run(args).Wait();
        }

        private static async Task Run(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage : ConfigureIndex indexName accountId keyName key");
                return;
            }
            var indexName = args[0];
            var accountId = args[1];
            var keyName = args[2];
            var keyValue = args[3];
            var client = Stormancer.Management.Client.AccountClient.CreateClient(accountId, keyName, keyValue, System.Configuration.ConfigurationManager.AppSettings["Server"]);

            var index = (await client.GetIndices()).FirstOrDefault(i=>i.name == indexName);
            if(index != null)
            {
                await client.DeleteIndex(index.name);
               
            }
            index = await client.CreateIndex(indexName, new Stormancer.Management.Client.IndexCreationOptions());

            Console.WriteLine("Index created.");
            Console.WriteLine("Index key : {0}", index.primaryKey);
        }
    }
}
