import React, { useState, useRef, useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { Typography } from '@mui/material';
import { Music, Play, Pause, SkipBack, SkipForward, Repeat, Shuffle, Volume2, Repeat1 } from 'lucide-react';
import type { RootState } from '../../app/store';
import { setCurrentSong } from '../../../features/song/songSlice'; // וודא שהנתיב נכון
import './MusicPlayer.css';

const MusicPlayer = () => {
    const dispatch = useDispatch();
    
    // שליפת נתונים מה-Redux
    const currentSong = useSelector((state: RootState) => state.song.currentSong);
    const playlist = useSelector((state: RootState) => state.song.currentPlaylist) || [];
    
    const audioRef = useRef<HTMLAudioElement>(null);
    
    // States מקומיים לנגן
    const [isPlaying, setIsPlaying] = useState(true);
    const [currentTime, setCurrentTime] = useState(0);
    const [duration, setDuration] = useState(0);
    const [volume, setVolume] = useState(0.7);
    
    // לוגיקה של כפתורי בקרה
    const [isShuffle, setIsShuffle] = useState(false);
    const [repeatMode, setRepeatMode] = useState<'none' | 'all' | 'one'>('none');

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

    // 2. פונקציית מעבר לשיר הבא (Next)
    const playNextSong = () => {
        if (playlist.length === 0) return;

        let nextIndex;
        const currentIndex = playlist.findIndex(s => s.songId === currentSong?.songId);

        if (isShuffle) {
            nextIndex = Math.floor(Math.random() * playlist.length);
        } else {
            nextIndex = currentIndex + 1;
        }

        if (nextIndex < playlist.length) {
            dispatch(setCurrentSong(playlist[nextIndex]));
        } else if (repeatMode === 'all') {
            dispatch(setCurrentSong(playlist[0])); // חזרה להתחלה
        } else {
            setIsPlaying(false); // סוף הרשימה
        }
    };

    // 3. פונקציית מעבר לשיר הקודם (Prev)
    const playPrevSong = () => {
        if (playlist.length === 0) return;
        
        // אם עברו יותר מ-3 שניות, לחיצה על "הקודם" רק תאפס את השיר הנוכחי
        if (audioRef.current && audioRef.current.currentTime > 3) {
            audioRef.current.currentTime = 0;
            return;
        }

        const currentIndex = playlist.findIndex(s => s.songId === currentSong?.songId);
        let prevIndex = currentIndex - 1;

        if (prevIndex >= 0) {
            dispatch(setCurrentSong(playlist[prevIndex]));
        } else if (repeatMode === 'all') {
            dispatch(setCurrentSong(playlist[playlist.length - 1]));
        }
    };

    // 4. טיפול בסיום שיר (Auto Next)
    const handleSongEnd = () => {
        if (repeatMode === 'one') {
            if (audioRef.current) {
                audioRef.current.currentTime = 0;
                audioRef.current.play();
            }
        } else {
            playNextSong();
        }
    };

    const handleTimeUpdate = () => {
        if (audioRef.current) setCurrentTime(audioRef.current.currentTime);
    };

    const handleLoadedMetadata = () => {
        if (audioRef.current) setDuration(audioRef.current.duration);
    };

    const handleSeek = (e: React.ChangeEvent<HTMLInputElement>) => {
        const seekTime = parseFloat(e.target.value);
        if (audioRef.current) {
            audioRef.current.currentTime = seekTime;
            setCurrentTime(seekTime);
        }
    };

    const handleVolumeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const newVolume = parseFloat(e.target.value);
        setVolume(newVolume);
        if (audioRef.current) audioRef.current.volume = newVolume;
    };

    const toggleRepeat = () => {
        if (repeatMode === 'none') setRepeatMode('all');
        else if (repeatMode === 'all') setRepeatMode('one');
        else setRepeatMode('none');
    };

    const formatTime = (time: number) => {
        if (isNaN(time)) return "0:00";
        const minutes = Math.floor(time / 60);
        const seconds = Math.floor(time % 60);
        return `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;
    };

    if (!currentSong) return null;

    return (
        <footer className="player-container">
            <div className="current-song-info">
                <div className="player-img-box">
                    <Music size={20} color="white" />
                </div>
                <div className="song-details">
                    <Typography className="song-title">{currentSong.title}</Typography>
                    <Typography className="song-artist">{currentSong.artist}</Typography>
                </div>
            </div>

            <div className="player-controls">
                <div className="control-buttons">
                    <Shuffle 
                        size={18} 
                        className={`secondary-btn ${isShuffle ? 'active-icon' : ''}`} 
                        onClick={() => setIsShuffle(!isShuffle)}
                    />
                    <SkipBack 
                        size={20} 
                        fill="white" 
                        color="white" 
                        className="secondary-btn" 
                        onClick={playPrevSong}
                    />
                    
                    <button className="play-pause-btn" onClick={togglePlay}>
                        {isPlaying ? <Pause size={20} fill="black" /> : <Play size={20} fill="black" />}
                    </button>
                    
                    <SkipForward 
                        size={20} 
                        fill="white" 
                        color="white" 
                        className="secondary-btn" 
                        onClick={playNextSong}
                    />
                    
                    <div onClick={toggleRepeat} className="repeat-wrapper">
                        {repeatMode === 'one' ? 
                            <Repeat1 size={18} className="active-icon" /> : 
                            <Repeat size={18} className={repeatMode === 'all' ? 'active-icon' : 'secondary-btn'} />
                        }
                    </div>
                </div>
                
                <div className="progress-container">
                    <span className="time-label">{formatTime(currentTime)}</span>
                    <div className="progress-bar-wrapper">
                        <input 
                            type="range"
                            min="0"
                            max={duration || 0}
                            value={currentTime}
                            onChange={handleSeek}
                            className="progress-slider"
                        />
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
                    onEnded={handleSongEnd}
                    onPlay={() => setIsPlaying(true)}
                    onPause={() => setIsPlaying(false)}
                />
            </div>

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