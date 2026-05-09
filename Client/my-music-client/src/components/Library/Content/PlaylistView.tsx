import React from 'react';
import { Typography } from '@mui/material';
import { Play, Upload } from 'lucide-react';
import SongTable from './SongTable';
import './PlaylistView.css';

interface PlaylistViewProps {
    playlistData: any;
    fileInputRef: React.RefObject<HTMLInputElement>;
    onUploadClick: () => void;
    onFileUpload: (event: React.ChangeEvent<HTMLInputElement>) => void;
}

const PlaylistView: React.FC<PlaylistViewProps> = ({ playlistData, fileInputRef, onUploadClick, onFileUpload }) => {
    return (
        <div className="playlist-container">
            <header className="playlist-main-header">
                
                {/* צד ימין: הכפתורים - איפה שסימנת "פה" בעיגול האדום */}
                <div className="header-actions-right">
                    <button className="play-button-main" title="נגן הכל">
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