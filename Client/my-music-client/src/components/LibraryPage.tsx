import React, { useState, useRef } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { 
    useGetPlaylistsQuery, 
    useCreatePlaylistMutation, 
    useUploadSongToPlaylistMutation 
} from '../features/playlist/playlistApi';
import { setCurrentSong } from '../features/song/songSlice';
import type { RootState } from '../app/store';

// ספריות עיצוב
import { 
    Button, Table, TableBody, TableCell, TableContainer, 
    TableHead, TableRow, Paper, Typography, Box 
} from '@mui/material';
import { 
    Plus, Music, Upload, Play, Library, 
    SkipBack, SkipForward, Repeat, Shuffle, Volume2 
} from 'lucide-react';

import './LibraryPage.css';

const LibraryPage = () => {
    const dispatch = useDispatch();
    const currentSong = useSelector((state: RootState) => state.song.currentSong);
    
    const { data: playlists, isLoading, error } = useGetPlaylistsQuery(); 
    const [createPlaylist] = useCreatePlaylistMutation();
    const [uploadSongToPlaylist] = useUploadSongToPlaylistMutation();

    const [selectedPlaylist, setSelectedPlaylist] = useState<any>(null);
    const fileInputRef = useRef<HTMLInputElement>(null);

    const activePlaylistData = playlists?.find((pl: any) => pl.playlistId === selectedPlaylist?.playlistId);

    const handleAddPlaylist = async () => {
        const name = prompt("איך לקרוא לפלייליסט החדש?");
        if (!name) return;
        try {
            await createPlaylist({ playlistName: name, isSmartPlaylist: false }).unwrap();
        } catch (err) {
            alert("שגיאה ביצירת הפלייליסט");
        }
    };

    const handleFileUpload = async (event: React.ChangeEvent<HTMLInputElement>) => {
        const file = event.target.files?.[0];
        if (!file || !selectedPlaylist) {
            alert("אנא בחר פלייליסט לפני העלאת שיר");
            return;
        }

        try {
            await uploadSongToPlaylist({ 
                file, 
                playlistId: selectedPlaylist.playlistId 
            }).unwrap();
            if (event.target) event.target.value = ''; 
        } catch (err) {
            alert("חלה שגיאה בעת העלאת השיר");
        }
    };

    if (isLoading) return <div className="loading">טוען ספרייה...</div>;
    if (error) return <div className="error">חלה שגיאה בתקשורת עם השרת.</div>;

    return (
        <div className="library-layout">
            <aside className="sidebar">
                <div className="sidebar-header">
                    <div className="header-title">
                        <Library size={24} color="#b3b3b3" />
                        <Typography variant="h6" sx={{ fontWeight: 'bold' }}>הספרייה שלך</Typography>
                    </div>
                    <button onClick={handleAddPlaylist} className="add-playlist-btn">
                        <Plus size={20} />
                    </button>
                </div>
                <ul className="playlist-list">
                    {playlists?.map((pl: any) => (
                        <li 
                            key={pl.playlistId}
                            onClick={() => setSelectedPlaylist(pl)}
                            className={selectedPlaylist?.playlistId === pl.playlistId ? 'active' : ''}
                        >
                            <div className="playlist-icon-box">
                                <Music size={16} />
                            </div>
                            <span className="playlist-name-text">{pl.playlistName}</span>
                        </li>
                    ))}        
                </ul>
            </aside>

            <main className="main-content">
                {selectedPlaylist ? (
                    <div className="playlist-view">
                        <header className="playlist-view-header">
                            <div className="playlist-image-big">
                                <Music size={64} color="#b3b3b3" />
                            </div>
                            <div className="playlist-info-text">
                                <Typography variant="overline" sx={{ color: 'white' }}>פלייליסט</Typography>
                                <Typography variant="h1" className="playlist-title-display">
                                    {activePlaylistData?.playlistName || selectedPlaylist.playlistName}
                                </Typography>
                            </div>
                        </header>

                        <div className="action-bar">
                            <button className="big-play-btn"><Play fill="black" size={24} /></button>
                            <input 
                                type="file" 
                                accept="audio/*" 
                                style={{ display: 'none' }} 
                                ref={fileInputRef}
                                onChange={handleFileUpload}
                            />
                            <Button 
                                variant="outlined" 
                                startIcon={<Upload />}
                                onClick={() => fileInputRef.current?.click()}
                                sx={{ color: 'white', borderColor: '#b3b3b3', borderRadius: '20px', textTransform: 'none' }}
                            >
                                הוסף שיר
                            </Button>
                        </div>

                        <TableContainer component={Paper} sx={{ backgroundColor: 'transparent', boxShadow: 'none' }}>
                            <Table size="small">
                                <TableHead>
                                    <TableRow sx={{ '& th': { borderBottom: '1px solid rgba(255,255,255,0.1)', color: '#b3b3b3' } }}>
                                        <TableCell width="50">#</TableCell>
                                        <TableCell>כותרת</TableCell>
                                        <TableCell align="left">אמן</TableCell>
                                    </TableRow>
                                </TableHead>
                                <TableBody>
                                    {activePlaylistData?.playlistSongs?.map((ps: any, index: number) => {
                                        const song = ps.song;
                                        if (!song) return null;
                                        return (
                                            <TableRow 
                                                key={song.songId}
                                                hover
                                                onClick={() => dispatch(setCurrentSong(song))}
                                                className="song-row"
                                            >
                                                <TableCell className="index-cell">
                                                    <span className="num">{index + 1}</span>
                                                    <Play size={14} className="row-play-icon" fill="white" />
                                                </TableCell>
                                                <TableCell sx={{ border: 'none' }}>
                                                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                                                        <Box className="song-img-placeholder"><Music size={16} /></Box>
                                                        <Box>
                                                            <Typography sx={{ color: 'white', fontSize: '1rem' }}>{song.title}</Typography>
                                                        </Box>
                                                    </Box>
                                                </TableCell>
                                                <TableCell sx={{ color: '#b3b3b3', border: 'none' }}>{song.artist || "אמן לא ידוע"}</TableCell>
                                            </TableRow>
                                        );
                                    })}
                                </TableBody>
                            </Table>
                        </TableContainer>
                    </div>
                ) : (
                    <div className="empty-state">
                        <Music size={64} color="#282828" />
                        <Typography variant="h5">בחר פלייליסט כדי להתחיל להאזין</Typography>
                    </div>
                )}
            </main>

            {currentSong && (
                <footer className="spotify-player">
                    {/* צד ימין: פרטי השיר */}
                    <div className="player-song-info">
                        <div className="player-img-box"><Music size={20} /></div>
                        <div className="player-text">
                            <Typography className="player-title">{currentSong.title}</Typography>
                            <Typography className="player-artist">{currentSong.artist}</Typography>
                        </div>
                    </div>

                    {/* מרכז: פקדי שליטה */}
                    <div className="player-controls">
                        <div className="control-buttons">
                            <Shuffle size={18} color="#b3b3b3" />
                            <SkipBack size={20} fill="white" color="white" />
                            <div className="play-pause-circle"><Play size={20} fill="black" /></div>
                            <SkipForward size={20} fill="white" color="white" />
                            <Repeat size={18} color="#b3b3b3" />
                        </div>
                        <div className="progress-container">
                            <span className="time">0:00</span>
                            <div className="progress-bar"><div className="progress-fill"></div></div>
                            <span className="time">3:45</span>
                        </div>
                        <audio autoPlay key={currentSong.songId} style={{ display: 'none' }}>
                            <source src={`http://localhost:5270${currentSong.filePath}`} type="audio/mpeg" />
                        </audio>
                    </div>

                    {/* צד שמאל: ווליום */}
                    <div className="player-extra">
                        <Volume2 size={18} color="#b3b3b3" />
                        <div className="volume-bar"></div>
                    </div>
                </footer>
            )}
        </div>
    );
};

export default LibraryPage;