using RockPaperScissors.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockPaperScissors.Extensions
{
    public static class GameExtensions
    {
        public static GameSession GetOpponentSession(this Game game, GameSession player)
        {
            if (game.Player1.Id == player.Id)
            {
                return game.Player2;
            }
            else
            {
                return game.Player1;
            }
        }
        public static GameSession GetPlayerSession(this Game game, string playerSessionId)
        {
            if (game.Player1.Id == playerSessionId)
            {
                return game.Player1;
            }
            else
            {
                return game.Player2;
            }
        }
    }
}
