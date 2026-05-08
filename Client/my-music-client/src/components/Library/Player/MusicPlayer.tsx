import React from 'react';
import { useSelector } from 'react-redux';
import { Typography, Box } from '@mui/material';
import { Music, Play, SkipBack, SkipForward, Repeat, Shuffle, Volume2 } from 'lucide-react';
import type { RootState } from '../../app/store';

const MusicPlayer = () => {
    const currentSong = useSelector((state: RootState) => state.song.currentSong);

    if (!currentSong) return null;

    return (
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
    );
};

export default MusicPlayer;