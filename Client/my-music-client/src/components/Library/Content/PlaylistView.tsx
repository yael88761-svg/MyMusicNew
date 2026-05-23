import React from 'react';
import { useDispatch } from 'react-redux';
import { Typography } from '@mui/material';
import { Play, Upload } from 'lucide-react';
import SongTable from './SongTable';
import { setCurrentSong, setCurrentPlaylist } from '../../../features/song/songSlice'; 
import './PlaylistView.css';

interface PlaylistViewProps {
    playlistData: any;
    fileInputRef: React.RefObject<HTMLInputElement>;
    onUploadClick: () => void;
    onFileUpload: (event: React.ChangeEvent<HTMLInputElement>) => void;
}

const PlaylistView: React.FC<PlaylistViewProps> = ({ playlistData, fileInputRef, onUploadClick, onFileUpload }) => {
    const dispatch = useDispatch();

    const handlePlayPlaylist = () => {
        const rawSongs = playlistData?.playlistSongs || [];
        
        if (rawSongs.length > 0) {
            const formattedSongs = rawSongs.map((item: any) => {
                const songData = item.song || {}; 
                const songPath = songData.filePath || songData.path || songData.url;

                return {
                    ...item,
                    songId: songData.songId || item.songId, 
                    title: songData.title || item.songTitle || "שיר ללא שם",
                    artist: songData.artist || "אמן לא ידוע",
                    filePath: songPath || ""
                };
            });

            dispatch(setCurrentPlaylist(formattedSongs));
            dispatch(setCurrentSong(formattedSongs[0]));
        }
    };

    return (
        <div className="playlist-container">
            <header className="playlist-main-header">
                
                {/* צד ימין של השורה: שם הפלייליסט והפרטים */}
                <div className="playlist-details-left">
                    <Typography className="type-label">פלייליסט:</Typography>
                    <h1 className="playlist-name-title">
                        {playlistData?.playlistName || "23"}
                    </h1>
                    <Typography variant="body2" className="playlist-stats">
                        ({playlistData?.playlistSongs?.length || 0} שירים)
                    </Typography>
                </div>

                {/* צד שמאל של השורה: כפתורי פעולה קומפקטיים */}
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

            <div className="table-wrapper">
                <SongTable songs={playlistData?.playlistSongs || []} />
            </div>
        </div>
    );
};

export default PlaylistView;