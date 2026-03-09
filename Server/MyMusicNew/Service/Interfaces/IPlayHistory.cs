using Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IPlayHistory
    {
        Task<List<PlayHistoryDto>> GetUserHistory(int userId);
        Task<PlayHistoryDto> AddToHistory(PlayHistoryDto dto, int userId);
    }
}
