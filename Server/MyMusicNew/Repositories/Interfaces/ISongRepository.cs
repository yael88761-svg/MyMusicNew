using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface ISongRepository<Song>
    {
        Task<List<Song>> GetAll(int id);

    }
}
