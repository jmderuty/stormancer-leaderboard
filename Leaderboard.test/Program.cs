using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stormancer;
using System.Reactive.Linq;

namespace Leaderboard.test
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                Run(args[0], int.Parse(args[1])).Wait();
            }
            else
            {
                Run(null, 0).Wait();
            }
        }

        private static async Task Run(string username, int scoreValue)
        {
            var config = ClientConfiguration.ForAccount("91368576-b314-1fa3-2506-1a9a8811d90d", "test2");
            config.ServerEndpoint = "http://localhost.fiddler:8081";
            var client = new Client(config);

            var scene = await client.GetPublicScene("test-league", new User { Name = username });

            await scene.Connect();
            if (username != null)
            {
                await scene.Rpc("leaderboard.submitScore", s => scene.Host.Serializer().Serialize(scoreValue, s)).FirstOrDefaultAsync();
            }
            
            var result = await scene.Rpc("leaderboard.getScores", s => scene.Host.Serializer().Serialize(new GetScoresOptions
            {
                Skip = 0,
                Take = 10
            }, s));

            var scores = result.ReadObject<List<ScoreViewModel>>();

            foreach(var score in scores)
            {
                Console.WriteLine("{0} : {1} with {2} points", score.Order, score.Username, score.Score);
            }
        }
    }

    public class User
    {
        public string Name { get; set; }

    }
}
