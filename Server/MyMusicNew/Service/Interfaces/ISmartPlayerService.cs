using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface ISmartPlayerService
    {
        // מקבלת את השיר הנוכחי והמשתמש, ומחזירה את השיר הבא הכי מתאים
        Task<Song> GetNextSmartSongAsync(int currentSongId, int userId);

        // יוצרת או מעדכנת פלייליסט של השירים המושמעים ביותר של המשתמש
        Task<Playlist> CreateTopTracksPlaylistAsync(int userId);
    }
}
