import React, { useState } from 'react';
import { Typography } from '@mui/material';
import { Music, Search, X } from 'lucide-react';
import SongTable from './SongTable';
import './AllSongsView.css'; // ייבוא ה-CSS המעודכן

interface AllSongsViewProps {
  playlists: any[];
  onDeleteSong: (id: string) => void;
}

const AllSongsView: React.FC<AllSongsViewProps> = ({ playlists, onDeleteSong }) => {
  // 1. הוספת ה-State עבור החיפוש
  const [searchQuery, setSearchQuery] = useState<string>('');
  
  const getAllSongsFromPlaylists = () => {
    if (!playlists || !Array.isArray(playlists)) return [];
    const songsMap = new Map();
    
    playlists.forEach((playlist: any) => {
      if (playlist.playlistSongs && Array.isArray(playlist.playlistSongs)) {
        playlist.playlistSongs.forEach((item: any) => {
          
          const sId = item.songId || item.song?.songId;
          
          if (sId) {
            songsMap.set(sId, {
              id: sId,
              songId: sId,
              song: item.song ? {
                ...item.song,
                id: item.song.id || item.song.songId,
                songId: item.song.songId
              } : {
                id: sId,
                songId: sId,
                title: item.songTitle || "ללא שם",
                artist: "אמן לא ידוע"
              }
            });
          }
        });
      }
    });
    return Array.from(songsMap.values());
  };

  const allSongs = getAllSongsFromPlaylists();

  // 2. סינון כל שירה ספריה בזמן אמת לפי כותרת או אמן
  const filteredSongs = allSongs.filter((item: any) => {
    if (!item.song) return false;
    
    const query = searchQuery.toLowerCase().trim();
    const songTitle = (item.song.title || '').toLowerCase();
    const songArtist = (item.song.artist || '').toLowerCase();
    
    return songTitle.includes(query) || songArtist.includes(query);
  });

  return (
    <div className="content-wrapper">
      
      {/* מבנה ה-Header החדש עם שלוש עמודות (ימין, מרכז, שמאל) */}
      <div className="library-all-songs-header">
        
        {/* צד ימין: כותרת העמוד */}
        <div className="library-details-left">
          <Typography variant="h6" sx={{ color: 'white', fontWeight: 'semibold', m: 0, lineHeight: 1 }}>
            הספרייה שלי
          </Typography>
        </div>

        {/* מרכז: תיבת החיפוש הממורכזת */}
        <div className="header-search-center">
          <input
            type="text"
            placeholder="חפש שיר או אמן בספרייה..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="header-search-input"
          />
          <Search size={15} className="input-search-icon" />
          
          {searchQuery && (
            <X 
              size={15} 
              className="input-clear-icon" 
              onClick={() => setSearchQuery('')} 
            />
          )}
        </div>

        {/* צend שמאל: פלייסהולדר ריק לשמירה על איזון התיבה באמצע */}
        <div className="header-actions-placeholder"></div>
      </div>
      
      {/* הצגת השירים המסוננים או הודעת שגיאה */}
      {allSongs && allSongs.length > 0 ? (
        filteredSongs.length > 0 ? (
          <SongTable songs={filteredSongs} onDeleteSong={onDeleteSong} />
        ) : (
          <div className="no-search-results">
            <Typography variant="body2" sx={{ color: '#b3b3b3', textAlign: 'center', mt: 4 }}>
              לא נמצאו שירים התואמים לחיפוש שלך בספרייה.
            </Typography>
          </div>
        )
      ) : (
        <div className="empty-state">
          <div className="big-icon-circle">
            <Music size={48} color="#b3b3b3" />
          </div>
          <Typography variant="h5" sx={{ color: 'white', mt: 2 }}>עדיין אין לך שירים בספרייה</Typography>
          <Typography variant="body2" sx={{ color: '#b3b3b3', mt: 1 }}>כנס לפלייליסט כלשהו והעלה אליו שירים כדי להתחיל</Typography>
        </div>
      )}
    </div>
  );
};

export default AllSongsView;