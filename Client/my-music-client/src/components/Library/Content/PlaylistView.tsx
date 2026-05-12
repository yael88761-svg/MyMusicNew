import React from 'react';
import { useDispatch } from 'react-redux'; // ייבוא הדיספאצ'
import { Typography } from '@mui/material';
import { Play, Upload } from 'lucide-react';
import SongTable from './SongTable';
import { setCurrentSong, setCurrentPlaylist } from '../../../features/song/songSlice'; // וודאי שהנתיב ל-Slice נכון
import './PlaylistView.css';

interface PlaylistViewProps {
    playlistData: any;
    fileInputRef: React.RefObject<HTMLInputElement>;
    onUploadClick: () => void;
    onFileUpload: (event: React.ChangeEvent<HTMLInputElement>) => void;
}

const PlaylistView: React.FC<PlaylistViewProps> = ({ playlistData, fileInputRef, onUploadClick, onFileUpload }) => {
    const dispatch = useDispatch();

    // הפונקציה שמפעילה את הכל
    const handlePlayPlaylist = () => {
        const songs = playlistData?.playlistSongs || [];
        
        if (songs.length > 0) {
            // 1. מעדכנים את הנגן בכל רשימת השירים הנוכחית
            dispatch(setCurrentPlaylist(songs));
            
            // 2. מפעילים את השיר הראשון ברשימה
            dispatch(setCurrentSong(songs[0]));
            
            console.log("Playlist sent to player:", songs.length, "songs");
        }
    };

    return (
        <div className="playlist-container">
            <header className="playlist-main-header">
                
                {/* צד ימין: הכפתורים */}
                <div className="header-actions-right">
                    {/* כאן הוספתי את ה-onClick */}
                    <button 
                        className="play-button-main" 
                        title="נגן הכל" 
                        onClick={handlePlayPlaylist}
                    >
                        <Play fill="black" size={28} />
                    </button>
                    
                    <button className="upload-button-outline" onClick={onUploadClick}>
                        <Upload size={18} />
                        <span>הוסף שיר</span>
                    </button>
                    
                    <input 
                        type="file" 
                        ref={fileInputRef} 
                        style={{ display: 'none' }} 
                        onChange={onFileUpload} 
                        accept="audio/*" 
                    />
                </div>

                {/* צד שמאל: פרטי הפלייליסט */}
                <div className="playlist-details-left">
                    <Typography className="type-label">פלייליסט</Typography>
                    <h1 className="playlist-name-title">
                        {playlistData?.playlistName || "23"}
                    </h1>
                    <Typography variant="body2" className="playlist-stats">
                        {playlistData?.playlistSongs?.length || 0} שירים
                    </Typography>
                </div>

            </header>

            <div className="table-wrapper">
                <SongTable songs={playlistData?.playlistSongs || []} />
            </div>
        </div>
    );
};

export default PlaylistView;