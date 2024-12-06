using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeFlow.Core.Interfaces;
using TimeFlow.Domain.Entities;
using TimeFlow.Infrastructure.Data;

namespace TimeFlow.Infrastructure.Repositories
{
    public class PomodoroSessionRepository : BaseRepository<PomodoroSession>, IPomodoroSessionRepository
    {
        public PomodoroSessionRepository(AppDbContext context) : base(context) { }

    }
}
