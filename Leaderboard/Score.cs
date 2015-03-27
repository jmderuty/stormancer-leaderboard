using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaderboard
{
    public class Score
    {
        public string Id
        {
            get
            {
                return Leaderboard + "-" + Username;
            }
        }
        public string Leaderboard { get; set; }

        public string Username { get; set; }

        public int Value { get; set; }
    }


    
}
