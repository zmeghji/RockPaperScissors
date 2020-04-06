using Microsoft.EntityFrameworkCore;
using RockPaperScissors.Data;
using RockPaperScissors.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockPaperScissors.Services
{
    public interface IGameDataService
    {
        //Find a game which has a Player1Session but no Player2Session
        Task<Game> FindGameWaitingForPlayer();

        //Create a new game and add a PlayerSession with the provided id to it
        Task<Game> CreateGameWithPlayer(string playerSessionId);

        //Set Player2Session of the provided game using the provided playerSessionId
        Task<Game> AddPlayerToGame(Game game, string playerSessionId);

        //Find the game which the player is a part of 
        Task<Game> FindGameBySessionId(string playerSessionId);

        //Updates what choice the player made (Rock, Paper or Scissors) during the game
        Task<Game> UpdatePlayForPlayer(Game game, string playerSessionId, Play play);
    }
    public class GameDataService : IGameDataService
    {
        private readonly IGameDbContextFactory _gameDbContextFactory;

        //We inject the GameDbContextFactory into the service so we can create a new instance of DbContext when needed
        public GameDataService(IGameDbContextFactory gameDbContextFactory)
        {
            _gameDbContextFactory = gameDbContextFactory;
        }



        public async Task<Game> FindGameWaitingForPlayer()
        {
            using var gameDbContext = _gameDbContextFactory.Create();
            return await gameDbContext.Games
                .Include(g => g.Player1)
                .Include(g => g.Player2)
                .FirstOrDefaultAsync(g => g.Player2 == null);
        }

        public async Task<Game> CreateGameWithPlayer(string playerSessionId)
        {
            using var gameDbContext = _gameDbContextFactory.Create();
            var game = new Game
            {
                Id = Guid.NewGuid(),
                Player1 = new GameSession
                {
                    Id = playerSessionId
                }
            };
            gameDbContext.Games.Add(game);
            await gameDbContext.SaveChangesAsync();
            return game;
        }

        public async Task<Game> AddPlayerToGame(Game game, string playerSessionId)
        {
            using var gameDbContext = _gameDbContextFactory.Create();
            gameDbContext.Attach(game);
            game.Player2 = new GameSession
            {
                Id = playerSessionId
            };
            await gameDbContext.SaveChangesAsync();
            return game;
        }

        public async Task<Game> FindGameBySessionId(string playerSessionId)
        {
            using var gameDbContext = _gameDbContextFactory.Create();
            return await gameDbContext.Games
                .Include(g => g.Player1)
                .Include(g => g.Player2)
                .FirstAsync(g =>
                    g.Player1.Id == playerSessionId || g.Player2.Id == playerSessionId);
        }
        public async Task<Game> UpdatePlayForPlayer(Game game, string playerSessionId, Play play)
        {
            using var gameDbContext = _gameDbContextFactory.Create();
            gameDbContext.Attach(game);
            var player = game.GetPlayerSession(playerSessionId);
            player.Play = play;
            await gameDbContext.SaveChangesAsync();
            return game;
        }
    }
}
