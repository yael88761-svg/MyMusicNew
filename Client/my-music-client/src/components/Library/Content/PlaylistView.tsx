import React from 'react';
import { Typography, Button } from '@mui/material';
import { Music, Play, Upload } from 'lucide-react';
import SongTable from './SongTable';

interface PlaylistViewProps {
    playlistData: any;
    fileInputRef: React.RefObject<HTMLInputElement>;
    onUploadClick: () => void;
    onFileUpload: (event: React.ChangeEvent<HTMLInputElement>) => void;
}

const PlaylistView: React.FC<PlaylistViewProps> = ({ playlistData, fileInputRef, onUploadClick, onFileUpload }) => {
    return (
        <div className="playlist-view">
            <header className="playlist-view-header">
                <div className="playlist-image-big">
                    <Music size={64} color="#b3b3b3" />
                </div>
                <div className="playlist-info-text">
                    <Typography variant="overline" sx={{ color: 'white' }}>פלייליסט</Typography>
                    <Typography variant="h1" className="playlist-title-display">
                        {playlistData?.playlistName}
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
                    onChange={onFileUpload}
                />
                <Button 
                    variant="outlined" 
                    startIcon={<Upload />}
                    onClick={onUploadClick}
                    sx={{ color: 'white', borderColor: '#b3b3b3', borderRadius: '20px', textTransform: 'none' }}
                >
                    הוסף שיר
                </Button>
            </div>

            <SongTable songs={playlistData?.playlistSongs || []} />
        </div>
    );
};

export default PlaylistView;