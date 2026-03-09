using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IPlayHistoryRepository: IRepository<PlayHistory>
    {
        Task<List<PlayHistory>> GetByUserId(int userId);
    }
}
