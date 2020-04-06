using Microsoft.EntityFrameworkCore;
using RockPaperScissors.Data;
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
    }
}
