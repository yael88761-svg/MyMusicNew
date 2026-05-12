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
                // הנתונים האמיתיים נמצאים בתוך item.song לפי הדיבאג שלך!
                const songData = item.song || {}; 
                
                // חילוץ הנתיב מתוך תת-האובייקט song
                const songPath = songData.filePath || songData.path || songData.url;

                return {
                    ...item, // שומר על נתוני הקישור (playlistId וכו')
                    songId: songData.songId || item.songId, 
                    title: songData.title || item.songTitle || "שיר ללא שם",
                    artist: songData.artist || "אמן לא ידוע",
                    filePath: songPath || "" // עכשיו זה לא יהיה ריק!
                };
            });

            console.log("Success! First song path found:", formattedSongs[0].filePath);

            dispatch(setCurrentPlaylist(formattedSongs));
            dispatch(setCurrentSong(formattedSongs[0]));
        }
    };
    return (
        <div className="playlist-container">
            <header className="playlist-main-header">
                
                {/* צד ימין: כפתור הפעלה והוספה */}
                <div className="header-actions-right">
                    <button 
                        className="play-button-main" 
                        title="נגן הכל" 
                        onClick={handlePlayPlaylist}
                    >
                        {/* הכפתור הירוק הגדול שסימנת */}
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

                {/* צד שמאל: כותרת וסטטיסטיקה */}
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
                {/* העברת השירים לטבלה */}
                <SongTable songs={playlistData?.playlistSongs || []} />
            </div>
        </div>
    );
};

export default PlaylistView;