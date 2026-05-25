import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { Table, TableBody, TableContainer, Typography } from '@mui/material'; 
import { Search, X } from 'lucide-react';
import { useGetRecentPlaylistQuery } from '../../../features/playlist/playlistApi';
import { setCurrentSong, setCurrentPlaylist } from '../../../features/song/songSlice';
import SongRow from '../Content/SongRow';
import './RecentPlaylistView.css';

interface RecentPlaylistViewProps {
    playlists?: any[]; 
}

const RecentPlaylistView: React.FC<RecentPlaylistViewProps> = ({ playlists }) => {
    const dispatch = useDispatch();
    const { data, isLoading, isError } = useGetRecentPlaylistQuery();
    const [searchQuery, setSearchQuery] = useState<string>('');

    // לוגיקה חדשה מבוססת מיקום ומזהה (חסינת בעיות תאריכים)
    const getRecentSongsCalculated = () => {
        // אם לא הועברו פלייליסטים, נחזור ישירות לנתוני ה-API
        if (!playlists || !Array.isArray(playlists)) {
            return data?.playlistSongs || [];
        }

        const songsMap = new Map();

        // 1. איסוף כל השירים מכל הפלייליסטים (מונע כפילויות)
        playlists.forEach((playlist: any) => {
            if (playlist.playlistSongs && Array.isArray(playlist.playlistSongs)) {
                playlist.playlistSongs.forEach((item: any) => {
                    const songData = item.song || {};
                    // חילוץ ה-ID של השיר
                    const sId = item.songId || songData.songId || item.id || songData.id;

                    if (sId) {
                        songsMap.set(sId, item);
                    }
                });
            }
        });

        const allSongsArray = Array.from(songsMap.values());

        // 2. מיון השירים כדי שהשיר שהוספת אחרון יופיע ראשון למעלה
        return allSongsArray.sort((a: any, b: any) => {
            const songA = a.song || {};
            const songB = b.song || {};

            // ניסיון ראשון: מיון לפי תאריך יצירה (במידה וקיים ותקין)
            const dateA = songA.createdAt || songA.uploadDate || a.createdAt || 0;
            const dateB = songB.createdAt || songB.uploadDate || b.createdAt || 0;
            
            if (dateA && dateB) {
                return new Date(dateB).getTime() - new Date(dateA).getTime();
            }

            // ניסיון שני וחסין: מיון לפי ה-ID של השיר (ID גבוה יותר = שיר חדש יותר)
            const idA = Number(songA.songId || songA.id || 0);
            const idB = Number(songB.songId || songB.id || 0);
            return idB - idA;
        }).slice(0, 30); // לוקח אוטומטית את 30 השירים הכי חדשים שהתווספו לספרייה!
    };

    // קביעת מקור הנתונים
    const recentSongsSource = playlists ? getRecentSongsCalculated() : (data?.playlistSongs || []);

    // סינון לפי תיבת החיפוש בזמן אמת
    const filteredSongs = recentSongsSource.filter((item: any) => {
        if (!item.song) return false;
        
        const query = searchQuery.toLowerCase().trim();
        const songTitle = (item.song.title || item.songTitle || '').toLowerCase();
        const songArtist = (item.song.artist || '').toLowerCase();
        
        return songTitle.includes(query) || songArtist.includes(query);
    });

    const handleSongClick = (selectedSong: any) => {
        dispatch(setCurrentSong(selectedSong));
        const songsToPlay = filteredSongs.map((item: any) => item.song);
        dispatch(setCurrentPlaylist(songsToPlay));
    };

    if (isLoading && !playlists) return <div className="loading">טוען שירים חדשים...</div>;
    if (isError && !playlists) return <div className="error">שגיאה בטעינת הפלייליסט</div>;

    return (
        <div className="playlist-detail-view">
            <header className="playlist-header">
                <div className="recent-details-left">
                    <h1>{data?.playlistName || "נוספו לאחרונה"}</h1>
                    <Typography variant="body2" className="recent-stats">
                        ({recentSongsSource.length} שירים אחרונים)
                    </Typography>
                </div>

                <div className="header-search-center">
                    <input
                        type="text"
                        placeholder="חפש בשירים שנוספו לאחרונה..."
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

                <div className="header-actions-placeholder"></div>
            </header>
            
            <TableContainer className="recent-table-container">
                {recentSongsSource.length > 0 && filteredSongs.length === 0 ? (
                    <div className="no-search-results">
                        <Typography variant="body2" sx={{ color: '#b3b3b3', textAlign: 'center', mt: 4 }}>
                            לא נמצאו שירים התואמים לחיפוש שלך.
                        </Typography>
                    </div>
                ) : (
                    <Table sx={{ minWidth: 650, borderCollapse: 'collapse' }}>
                        <TableBody>
                            {filteredSongs.map((item: any, index: number) => (
                                <SongRow 
                                    key={item.songId || item.song?.songId || index} 
                                    song={item.song} 
                                    index={index} 
                                    onPlay={handleSongClick} 
                                />
                            ))}
                        </TableBody>
                    </Table>
                )}
            </TableContainer>
        </div>
    );
};

export default RecentPlaylistView;