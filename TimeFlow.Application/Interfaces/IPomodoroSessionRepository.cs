using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeFlow.Domain.Entities;

namespace TimeFlow.Core.Interfaces
{
    public interface IPomodoroSessionRepository : IBaseRepository<PomodoroSession>
    {
        //Для специфичного функционала
    }
}
