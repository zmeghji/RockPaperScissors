using Microsoft.AspNetCore.SignalR;
using RockPaperScissors.Constants;
using RockPaperScissors.Data;
using RockPaperScissors.Extensions;
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
        private readonly IGameService _gameService;
        public GameHub(IGameDataService gameDataService, IGameService gameService)
        {
            _gameDataService = gameDataService;
            _gameService = gameService;
        }
        public async Task SelectPlay(string play)
        {
            //find the game which the player is playing
            var game = await _gameDataService.FindGameBySessionId(Context.ConnectionId);
            //update the player session with the choice the player made
            await _gameDataService.UpdatePlayForPlayer(game, Context.ConnectionId, (Play)Enum.Parse(typeof(Play), play));

            var playerSession = game.GetPlayerSession(Context.ConnectionId);
            var opponentSession = game.GetOpponentSession(playerSession);

            if (opponentSession.Play.HasValue)
            {
                //If the opposing player has already made their play, we are ready to compute the game results and notify both players
                //Compute game result for current player
                var playerResult = _gameService.GetGameResult(
                    playerSession.Play.Value,
                    opponentSession.Play.Value).ToString();
                //Notify current player of their game result
                await Clients.Client(playerSession.Id)
                    .SendAsync(GameEventNames.GameEnd, playerResult);
                //Compute game result for opposing player
                var opponentResult = _gameService.GetGameResult(
                    opponentSession.Play.Value,
                    playerSession.Play.Value).ToString();
                //Notify opposing player of their game result
                await Clients.Client(opponentSession.Id)
                    .SendAsync(GameEventNames.GameEnd, opponentResult);
            }
            else
            {
                //The opposing player has not made their play yet. We notify the current player to wait for the opposing player to make a choice.
                await Clients.Caller.SendAsync(GameEventNames.WaitingForPlayerToPlay);
            }
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
