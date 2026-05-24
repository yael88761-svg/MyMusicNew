import React from 'react';
import { Typography } from '@mui/material';
import { Music } from 'lucide-react';
import SongTable from './SongTable';

interface AllSongsViewProps {
  playlists: any[];
  onDeleteSong: (id: string) => void;
}

const AllSongsView: React.FC<AllSongsViewProps> = ({ playlists, onDeleteSong }) => {
  
  const getAllSongsFromPlaylists = () => {
    if (!playlists || !Array.isArray(playlists)) return [];
    const songsMap = new Map();
    
    playlists.forEach((playlist: any) => {
      if (playlist.playlistSongs && Array.isArray(playlist.playlistSongs)) {
        playlist.playlistSongs.forEach((item: any) => {
          
          // חילוץ ה-ID של השיר (לפי מה שראינו ב-Console)
          const sId = item.songId || item.song?.songId;
          
          if (sId) {
            // אנחנו שומרים ב-Map לפי ה-songId כדי שלא יהיו כפילויות של אותו שיר במסך "כל השירים"
            songsMap.set(sId, {
              // ה-SongTable/SongRow מצפה לקבל את ה-ID ברמה הראשית בשם id או songId
              id: sId,
              songId: sId,
              // מעבירים את אובייקט ה-song המלא כפי שהוא מגיע מהשרת
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

  return (
    <div className="content-wrapper">
      <div className="library-all-songs-header">
        <Typography variant="h6" sx={{ color: 'white', fontWeight: 'semi-bold', mb: 1 }}>הספרייה שלי</Typography>
      </div>
      
      {allSongs && allSongs.length > 0 ? (
        <SongTable songs={allSongs} onDeleteSong={onDeleteSong} />
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