using Repositories.Entities;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    internal class PlaylistSongRepository : IRepository<PlaylistSong>
    {
        public Task<PlaylistSong> AddItem(PlaylistSong item)
        {
            throw new NotImplementedException();
        }

        public Task DeleteItem(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<PlaylistSong>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<PlaylistSong> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PlaylistSong> UpdateItem(int id, PlaylistSong item)
        {
            throw new NotImplementedException();
        }
    }
}
