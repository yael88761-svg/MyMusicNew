import React from 'react';
import { Typography } from '@mui/material';
import { Music } from 'lucide-react';
import SongTable from './SongTable';

interface AllSongsViewProps {
  playlists: any[];
}

const AllSongsView: React.FC<AllSongsViewProps> = ({ playlists }) => {
  
  const getAllSongsFromPlaylists = () => {
    if (!playlists || !Array.isArray(playlists)) return [];
    const songsMap = new Map();
    
    playlists.forEach((playlist: any) => {
      if (playlist.playlistSongs && Array.isArray(playlist.playlistSongs)) {
        playlist.playlistSongs.forEach((item: any) => {
          const songId = item.songId;
          if (songId) {
            songsMap.set(songId, {
              id: songId,
              songId: songId,
              title: item.songTitle || "ללא שם",
              artist: item.artist || "אמן לא ידוע",
              ...item
            });
          }
        });
      }
    });
    return Array.from(songsMap.values());
  };

  const allSongs = getAllSongsFromPlaylists();

  return (
    <div className="content-wrapper">
      <div className="library-all-songs-header">
        <Typography variant="h6" sx={{ color: 'white', fontWeight: 'semi-bold', mb: 1 }}>הספרייה שלי</Typography>
      </div>
      
      {allSongs && allSongs.length > 0 ? (
        <SongTable songs={allSongs} />
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