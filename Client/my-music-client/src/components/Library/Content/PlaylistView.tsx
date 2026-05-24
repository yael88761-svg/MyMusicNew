import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { Typography } from '@mui/material';
import { Play, Upload, Search, X } from 'lucide-react';
import SongTable from './SongTable';
import { setCurrentSong, setCurrentPlaylist } from '../../../features/song/songSlice'; 
import './PlaylistView.css';

interface PlaylistViewProps {
    playlistData: any;
    fileInputRef: React.RefObject<HTMLInputElement>;
    onUploadClick: () => void;
    onFileUpload: (event: React.ChangeEvent<HTMLInputElement>) => void;
    onDeleteSong: (id: string) => void;
}

const PlaylistView: React.FC<PlaylistViewProps> = ({ playlistData, fileInputRef, onUploadClick, onFileUpload, onDeleteSong }) => {
    const dispatch = useDispatch();
    
    // 1. הוספת ה-State של החיפוש
    const [searchQuery, setSearchQuery] = useState<string>('');

    const getFormattedSongs = () => {
        const rawSongs = playlistData?.playlistSongs || [];
        return rawSongs.map((item: any) => {
            const songData = item.song || {}; 
            const songPath = songData.filePath || songData.path || songData.url;
            return {
                ...item,
                songId: songData.songId || item.songId, 
                title: songData.title || item.songTitle || "שיר ללא שם",
                artist: songData.artist || "אמן לא ידוע",
                filePath: songPath || ""
            };
        }).filter((s: any) => s.songId);
    };

    const allFormattedSongs = getFormattedSongs();

    // 2. פונקציית סינון השירים על פי הקלדת המשתמש
    const filteredSongs = allFormattedSongs.filter((song: any) => {
        const query = searchQuery.toLowerCase().trim();
        const songTitle = (song.title || '').toLowerCase();
        const songArtist = (song.artist || '').toLowerCase();
        return songTitle.includes(query) || songArtist.includes(query);
    });

    const handlePlayPlaylist = () => {
        if (filteredSongs.length > 0) {
            dispatch(setCurrentPlaylist(filteredSongs));
            dispatch(setCurrentSong(filteredSongs[0]));
        }
    };

    return (
        <div className="playlist-container">
            <header className="playlist-main-header">
                
                {/* צד ימין: פרטי הפלייליסט */}
                <div className="playlist-details-left">
                    <Typography className="type-label">פלייליסט:</Typography>
                    <h1 className="playlist-name-title">
                        {playlistData?.playlistName || "23"}
                    </h1>
                    <Typography variant="body2" className="playlist-stats">
                        ({allFormattedSongs.length} שירים)
                    </Typography>
                </div>

                {/* מרכז בדיוק: רכיב החיפוש החדש */}
                <div className="header-search-center">
                    <input 
                        type="text" 
                        placeholder="חפש שיר או אמן בפלייליסט..." 
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

                {/* צד שמאל: כפתורי הפעולה והעלאת הקובץ */}
                <div className="header-actions-right">
                    <button className="upload-button-outline" onClick={onUploadClick}>
                        <Upload size={13} />
                        <span>הוסף שיר</span>
                    </button>
                    
                    <button 
                        className="play-button-main" 
                        title="נגן הכל" 
                        onClick={handlePlayPlaylist}
                    >
                        <Play fill="black" size={16} />
                    </button>
                    
                    <input 
                        type="file" 
                        ref={fileInputRef} 
                        style={{ display: 'none' }} 
                        onChange={onFileUpload} 
                        accept="audio/*" 
                    />
                </div>
            </header>

            {/* גוף הטבלה - מציג את השירים המסוננים או הודעה אם אין תוצאות */}
            <div className="table-wrapper">
                {allFormattedSongs.length > 0 && filteredSongs.length === 0 ? (
                    <div className="no-search-results">
                        <Typography variant="body2" sx={{ color: '#b3b3b3', textAlign: 'center', mt: 4 }}>
                            לא נמצאו שירים התואמים לחיפוש שלך בפלייליסט זה.
                        </Typography>
                    </div>
                ) : (
                    <SongTable 
                        songs={filteredSongs} 
                        onDeleteSong={onDeleteSong} 
                    />
                )}
            </div>
        </div>
    );
};

export default PlaylistView;