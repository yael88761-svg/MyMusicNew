import React, { useState, useRef, useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useGetPlaylistsQuery, useCreatePlaylistMutation } from '../features/playlist/playlistApi';
import { useUploadSongMutation } from '../features/song/songApi'; 
import { setCurrentSong } from '../features/song/songSlice';
import type { RootState } from '../app/store';

// ספריות עיצוב
import { Button, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper, Typography } from '@mui/material';
import { Plus, Music, Upload, Play, Library } from 'lucide-react';

import './LibraryPage.css';

const LibraryPage = () => {
    const dispatch = useDispatch();
    const currentSong = useSelector((state: RootState) => state.song.currentSong);
    
    // קבלת הנתונים מהשרת
    const { data: playlists, isLoading, error } = useGetPlaylistsQuery(); 
    const [createPlaylist] = useCreatePlaylistMutation();
    const [uploadSong] = useUploadSongMutation();

    const [selectedPlaylist, setSelectedPlaylist] = useState<any>(null);
    const fileInputRef = useRef<HTMLInputElement>(null);

    // מציאת הנתונים המלאים של הפלייליסט הנבחר מתוך רשימת הפלייליסטים
    const activePlaylistData = playlists?.find((pl: any) => pl.PlaylistId === selectedPlaylist?.PlaylistId);

    // --- בדיקת נתונים ב-Console ---
    useEffect(() => {
        if (playlists) {
            console.log("רשימת פלייליסטים כללית:", playlists);
        }
        if (selectedPlaylist) {
            console.log("פלייליסט שנבחר (בצד):", selectedPlaylist);
        }
        if (activePlaylistData) {
            console.log("נתוני פלייליסט פעיל (כולל שירים):", activePlaylistData);
            console.log("מערך שירים שנמצא:", activePlaylistData.PlaylistSongs);
        }
    }, [playlists, selectedPlaylist, activePlaylistData]);

    const handleAddPlaylist = async () => {
        const name = prompt("איך לקרוא לפלייליסט החדש?");
        if (!name) return;
        try {
            await createPlaylist({ PlaylistName: name, IsSmartPlaylist: false }).unwrap();
            alert("הפלייליסט נוצר בהצלחה!");
        } catch (err) {
            alert("שגיאה ביצירת הפלייליסט");
        }
    };

    const handleFileUpload = async (event: React.ChangeEvent<HTMLInputElement>) => {
        const file = event.target.files?.[0];
        if (!file || !selectedPlaylist) return;

        const formData = new FormData();
        formData.append('file', file);
        formData.append('PlaylistId', selectedPlaylist.PlaylistId.toString());

        try {
            console.log("מעלה שיר לפלייליסט ID:", selectedPlaylist.PlaylistId);
            await uploadSong({ 
                formData, 
                playlistId: selectedPlaylist.PlaylistId.toString() 
            }).unwrap();
            
            alert("השיר הועלה בהצלחה!");
            if (event.target) event.target.value = ''; 
        } catch (err) {
            console.error("שגיאה בהעלאה:", err);
            alert("שגיאה בהעלאת השיר");
        }
    };

    if (isLoading) return <div className="loading">טוען ספרייה...</div>;
    if (error) return <div className="error">חלה שגיאה. וודא שאתה מחובר.</div>;

    return (
        <div className="library-layout">
            <aside className="sidebar">
                <div className="sidebar-header" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '10px' }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                        <Library size={20} color="#1DB954" />
                        <Typography variant="h6">הספרייה שלך</Typography>
                    </div>
                    <button onClick={handleAddPlaylist} className="add-playlist-btn">
                        <Plus size={20} />
                    </button>
                </div>
                <ul className="playlist-list">
                {playlists?.map((pl: any) => (
                    <li 
                        key={pl.PlaylistId}
                        onClick={() => setSelectedPlaylist(pl)}
                        className={selectedPlaylist?.PlaylistId === pl.PlaylistId ? 'active' : ''}
                    >
                        <Music size={16} />
                        {pl.PlaylistName}
                    </li>
                ))}       
            </ul>
            </aside>

            <main className="main-content">
                {selectedPlaylist ? (
                    <div style={{ padding: '20px' }}>
                        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
                            <Typography variant="h4">{activePlaylistData?.PlaylistName || selectedPlaylist.PlaylistName}</Typography>
                            
                            <div>
                                <input 
                                    type="file" 
                                    accept="audio/*" 
                                    style={{ display: 'none' }} 
                                    ref={fileInputRef}
                                    onChange={handleFileUpload}
                                />
                                <Button 
                                    variant="contained" 
                                    startIcon={<Upload />}
                                    onClick={() => fileInputRef.current?.click()}
                                    sx={{ backgroundColor: '#1DB954', '&:hover': { backgroundColor: '#1ed760' }, borderRadius: '20px' }}
                                >
                                    הוסף שיר
                                </Button>
                            </div>
                        </div>

                        <TableContainer component={Paper} sx={{ backgroundColor: 'transparent', boxShadow: 'none' }}>
                            <Table>
                                <TableHead>
                                    <TableRow>
                                        <TableCell sx={{ color: '#b3b3b3' }}>#</TableCell>
                                        <TableCell sx={{ color: '#b3b3b3' }}>כותרת</TableCell>
                                        <TableCell sx={{ color: '#b3b3b3' }}>אמן</TableCell>
                                    </TableRow>
                                </TableHead>
                                <TableBody>
                                    {activePlaylistData?.PlaylistSongs && activePlaylistData.PlaylistSongs.length > 0 ? (
                                        activePlaylistData.PlaylistSongs.map((ps: any, index: number) => {
                                            // בדיקה לכל שיר בנפרד בתוך ה-map
                                            console.log(`מרנדר שיר מספר ${index + 1}:`, ps.Song);
                                            
                                            return (
                                                <TableRow 
                                                    key={ps.Song.SongId}
                                                    hover
                                                    onClick={() => dispatch(setCurrentSong(ps.Song))}
                                                    sx={{ cursor: 'pointer', '&:hover': { backgroundColor: 'rgba(255,255,255,0.08)' } }}
                                                >
                                                    <TableCell sx={{ color: 'white', border: 'none' }}>{index + 1}</TableCell>
                                                    <TableCell sx={{ color: 'white', border: 'none' }}>
                                                        <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
                                                            <Play size={14} fill="#1DB954" color="#1DB954" />
                                                            {ps.Song.Title}
                                                        </div>
                                                    </TableCell>
                                                    <TableCell sx={{ color: '#b3b3b3', border: 'none' }}>{ps.Song.Artist || "אמן לא ידוע"}</TableCell>
                                                </TableRow>
                                            );
                                        })
                                    ) : (
                                        <TableRow>
                                            <TableCell colSpan={3} sx={{ color: '#b3b3b3', textAlign: 'center', py: 4, border: 'none' }}>
                                                אין שירים בפלייליסט זה. נסה להעלות שיר חדש.
                                            </TableCell>
                                        </TableRow>
                                    )}
                                </TableBody>
                            </Table>
                        </TableContainer>
                    </div>
                ) : (
                    <div className="empty-state">בחר פלייליסט כדי להתחיל להאזין</div>
                )}
            </main>

            {currentSong && (
                <footer className="player-bar">
                    <div className="song-info">
                        <strong>{currentSong.Title}</strong>
                        <span>{currentSong.Artist}</span>
                    </div>
                    <audio controls autoPlay key={currentSong.SongId}>
                        <source src={`http://localhost:5270/${currentSong.FilePath}`} type="audio/mpeg" />
                    </audio>
                </footer>
            )}
        </div>
    );
};

export default LibraryPage;