using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockPaperScissors.Constants
{
    public class GameEventNames
    {
        //Player has created a game and must wait for opponent to join
        public const string WaitingForPlayerToJoin = "WaitingForPlayerToJoin";

        //Both players have joined game
        public const string GameStart = "GameStart";

        //Player has chosen rock,paper or scissors but the opponent has not. Player must wait for opponent to make a choice
        public const string WaitingForPlayerToPlay = "WaitingForPlayerToPlay";

        //Both Players have made their choice and a game result (win/draw/lose) is sent back to both players
        public const string GameEnd = "GameEnd";
    }
}
