import React, { useState, useRef, useEffect } from 'react';
import { useSelector } from 'react-redux';
import { Typography } from '@mui/material';
import { Music, Play, Pause, SkipBack, SkipForward, Repeat, Shuffle, Volume2 } from 'lucide-react';
import type { RootState } from '../../app/store';
import './MusicPlayer.css';

const MusicPlayer = () => {
    const currentSong = useSelector((state: RootState) => state.song.currentSong);
    const audioRef = useRef<HTMLAudioElement>(null);
    
    const [isPlaying, setIsPlaying] = useState(true);
    const [currentTime, setCurrentTime] = useState(0);
    const [duration, setDuration] = useState(0);
    const [volume, setVolume] = useState(0.7);

    // 1. ניהול נגינה/עצירה
    const togglePlay = () => {
        if (audioRef.current) {
            if (isPlaying) {
                audioRef.current.pause();
            } else {
                audioRef.current.play();
            }
            setIsPlaying(!isPlaying);
        }
    };

    // 2. עדכון זמן התקדמות תוך כדי נגינה
    const handleTimeUpdate = () => {
        if (audioRef.current) {
            setCurrentTime(audioRef.current.currentTime);
        }
    };

    // 3. טעינת משך השיר כשהקובץ מוכן
    const handleLoadedMetadata = () => {
        if (audioRef.current) {
            setDuration(audioRef.current.duration);
        }
    };

    // 4. פונקציית הדילוג (Seek) - לחיצה על פס ההתקדמות
    const handleSeek = (e: React.ChangeEvent<HTMLInputElement>) => {
        const seekTime = parseFloat(e.target.value);
        if (audioRef.current) {
            audioRef.current.currentTime = seekTime;
            setCurrentTime(seekTime);
        }
    };

    // 5. שינוי ווליום
    const handleVolumeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const newVolume = parseFloat(e.target.value);
        setVolume(newVolume);
        if (audioRef.current) {
            audioRef.current.volume = newVolume;
        }
    };

    // פונקציית עזר לעיצוב זמן (0:00)
    const formatTime = (time: number) => {
        if (isNaN(time)) return "0:00";
        const minutes = Math.floor(time / 60);
        const seconds = Math.floor(time % 60);
        return `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;
    };

    if (!currentSong) return null;

    return (
        <footer className="player-container">
            {/* צד ימין: פרטי השיר */}
            <div className="current-song-info">
                <div className="player-img-box">
                    <Music size={20} color="white" />
                </div>
                <div className="song-details">
                    <Typography className="song-title">{currentSong.title}</Typography>
                    <Typography className="song-artist">{currentSong.artist}</Typography>
                </div>
            </div>

            {/* מרכז: פקדי שליטה ופס התקדמות אינטראקטיבי */}
            <div className="player-controls">
                <div className="control-buttons">
                    <Shuffle size={18} className="secondary-btn" />
                    <SkipBack size={20} fill="white" color="white" className="secondary-btn" />
                    
                    <button className="play-pause-btn" onClick={togglePlay}>
                        {isPlaying ? <Pause size={20} fill="black" /> : <Play size={20} fill="black" />}
                    </button>
                    
                    <SkipForward size={20} fill="white" color="white" className="secondary-btn" />
                    <Repeat size={18} className="secondary-btn" />
                </div>
                
                <div className="progress-container">
                    <span className="time-label">{formatTime(currentTime)}</span>
                    <div className="progress-bar-wrapper">
                        {/* הסליידר השקוף שמאפשר לחיצה וגרירה */}
                        <input 
                            type="range"
                            min="0"
                            max={duration || 0}
                            value={currentTime}
                            onChange={handleSeek}
                            className="progress-slider"
                        />
                        {/* הפס הויזואלי שרואים מתחת לסליידר */}
                        <div className="progress-bar-background">
                            <div 
                                className="progress-fill-visual" 
                                style={{ width: `${(currentTime / duration) * 100}%` }}
                            ></div>
                        </div>
                    </div>
                    <span className="time-label">{formatTime(duration)}</span>
                </div>

                <audio 
                    ref={audioRef}
                    autoPlay 
                    key={currentSong.songId} 
                    src={`http://localhost:5270${currentSong.filePath}`}
                    onTimeUpdate={handleTimeUpdate}
                    onLoadedMetadata={handleLoadedMetadata}
                    onPlay={() => setIsPlaying(true)}
                    onPause={() => setIsPlaying(false)}
                />
            </div>

            {/* צד שמאל: ווליום */}
            <div className="extra-controls">
                <Volume2 size={18} color="#b3b3b3" />
                <input 
                    type="range" 
                    min="0" 
                    max="1" 
                    step="0.01" 
                    value={volume} 
                    onChange={handleVolumeChange}
                    className="volume-slider"
                />
            </div>
        </footer>
    );
};

export default MusicPlayer;