using Stormancer;
using Stormancer.Core;
using Stormancer.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Leaderboard
{
    public class App : IStartup
    {
        public void Run(IAppBuilder builder)
        {
            builder.SceneTemplate("leaderboard", scene =>
            {
                scene.Starting.Add(async p => {
                    var client = IndexFactory.GetClient();
                    var mapping = await client.MapAsync<Score>(md => md.Properties(props => props.String(s => s
                                                    .Name(a => a.Leaderboard)
                                                    .Index(Nest.FieldIndexOption.NotAnalyzed)
                                                   )));
                    

                });
                scene.AddProcedure("leaderboard.submitScore", async ctx =>
                {
                    var score = ctx.RemotePeer.Serializer().Deserialize<int>(ctx.InputStream);
                    var userDataStream = new MemoryStream(ctx.RemotePeer.UserData);
                    var user = ctx.RemotePeer.Serializer().Deserialize<User>(userDataStream);
                    var client = IndexFactory.GetClient();

                    var record = new Score { Username = user.Name, Value = score, Leaderboard = scene.Id };

                    await client.IndexAsync(record);
                });
                scene.AddProcedure("leaderboard.getScores", async ctx =>
                {
                    var options = ctx.RemotePeer.Serializer().Deserialize<GetScoresOptions>(ctx.InputStream);

                    var client = IndexFactory.GetClient();

                    var results = await client.SearchAsync<Score>(sd => sd
                        .Filter(f => f.Term(s => s.Leaderboard, scene.Id))
                        .SortDescending(s => s.Value)
                        .Skip(options.Skip)
                        .Take(options.Take)
                        );
                    if (!results.IsValid)
                    {
                        throw new ClientException("Failed to get leaderboard data.");
                    }
                    else
                    {
                        var order = options.Skip;
                        var scores = results.Hits.Select(s => new ScoreViewModel { Score = s.Source.Value, Username = s.Source.Username, Order = order++ }).ToList();
                        ctx.SendValue(stream => ctx.RemotePeer.Serializer().Serialize(scores, stream), PacketPriority.MEDIUM_PRIORITY);
                    }
                });
            },
            new Dictionary<string, string> { { "description", "Simple leaderboard running on Stormancer." } });
        }
    }
}
