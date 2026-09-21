using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyTaskManager.Data;
using Microsoft.EntityFrameworkCore;

namespace DailyTaskManagerTests.Helpers
{
    public static class TestDbContext
    {
        public static DailyTaskManagerContext Create()
        {
            var options = new DbContextOptionsBuilder<DailyTaskManagerContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new DailyTaskManagerContext(options);
        }
    }
}