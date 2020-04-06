using Microsoft.AspNetCore.SignalR;
using RockPaperScissors.Constants;
using RockPaperScissors.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockPaperScissors.Hubs
{
    public class GameHub : Hub
    {
        private readonly IGameDataService _gameDataService;
        public GameHub(IGameDataService gameDataService)
        {
            _gameDataService = gameDataService;
        }

        public async Task JoinGame()
        {
            //Try to find a game which is waiting for a player to join
            var existingGame = await _gameDataService.FindGameWaitingForPlayer();

            if (existingGame == null)
            {
                //If we couldn't find a game, create a new one
                await _gameDataService.CreateGameWithPlayer(Context.ConnectionId);
                //Then notify the player that they are waiting for another player to join
                await Clients.Client(Context.ConnectionId).SendAsync(GameEventNames.WaitingForPlayerToJoin);
            }
            else
            {
                //If we found one, the player will join that one
                await _gameDataService.AddPlayerToGame(existingGame, Context.ConnectionId);

                //Then Notify both players that the game has begun
                await Clients.Client(existingGame.Player1.Id).SendAsync(GameEventNames.GameStart);
                await Clients.Client(existingGame.Player2.Id).SendAsync(GameEventNames.GameStart);
            }
        }
    }
}
