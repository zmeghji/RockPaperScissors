using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockPaperScissors.Data
{
    public interface IGameDbContextFactory
    {
        GameDbContext Create();
    }
    public class GameDbContextFactory : IGameDbContextFactory
    {
        public GameDbContext Create()
        {
            var options = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: "Game")
                .Options;
            return new GameDbContext(options);
        }
    }
}
