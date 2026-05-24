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
    onDeleteSong: (id: string) => void; // קבלה ישירה מהאב
}

const PlaylistView: React.FC<PlaylistViewProps> = ({ playlistData, fileInputRef, onUploadClick, onFileUpload, onDeleteSong }) => {
    const dispatch = useDispatch();

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

    const handlePlayPlaylist = () => {
        const formattedSongs = getFormattedSongs();
        if (formattedSongs.length > 0) {
            dispatch(setCurrentPlaylist(formattedSongs));
            dispatch(setCurrentSong(formattedSongs[0]));
        }
    };

    return (
        <div className="playlist-container">
            <header className="playlist-main-header">
                <div className="playlist-details-left">
                    <Typography className="type-label">פלייליסט:</Typography>
                    <h1 className="playlist-name-title">
                        {playlistData?.playlistName || "23"}
                    </h1>
                    <Typography variant="body2" className="playlist-stats">
                        ({playlistData?.playlistSongs?.length || 0} שירים)
                    </Typography>
                </div>

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
                <SongTable 
                    songs={playlistData?.playlistSongs || []} 
                    onDeleteSong={onDeleteSong} 
                />
            </div>
        </div>
    );
};

export default PlaylistView;