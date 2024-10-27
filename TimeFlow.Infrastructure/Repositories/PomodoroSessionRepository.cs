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
    public class PomodoroSessionRepository : IPomodoroSessionRepository
    {
        private readonly AppDbContext _context;

        public PomodoroSessionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddSessionAsync(PomodoroSession session)
        {
            _context.PomodoroSessions.Add(session);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSessionAsync(int id)
        {
            var session = await _context.PomodoroSessions.FindAsync(id);
            if (session != null)
            {
                _context.PomodoroSessions.Remove(session);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<PomodoroSession>> GetAllSessionsAsync()
        {
            return await _context.PomodoroSessions.ToListAsync();
        }

        public async Task<PomodoroSession> GetSessionByIdAsync(int id)
        {
            return await _context.PomodoroSessions.FindAsync(id);
        }

        public async Task UpdateSessionAsync(PomodoroSession session)
        {
            _context.PomodoroSessions.Update(session);
            await _context.SaveChangesAsync();
        }
    }
}
