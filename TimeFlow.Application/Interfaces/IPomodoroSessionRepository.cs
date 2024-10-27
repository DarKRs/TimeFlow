using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeFlow.Domain.Entities;

namespace TimeFlow.Core.Interfaces
{
    public interface IPomodoroSessionRepository
    {
        Task<IEnumerable<PomodoroSession>> GetAllSessionsAsync();
        Task<PomodoroSession> GetSessionByIdAsync(int id);
        Task AddSessionAsync(PomodoroSession session);
        Task UpdateSessionAsync(PomodoroSession session);
        Task DeleteSessionAsync(int id);
    }
}
