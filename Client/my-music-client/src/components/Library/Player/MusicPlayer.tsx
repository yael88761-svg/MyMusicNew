import React, { useState, useRef, useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { Typography } from '@mui/material';
import { Music, Play, Pause, SkipBack, SkipForward, Repeat, Shuffle, Volume2, Repeat1 } from 'lucide-react';
import type { RootState } from '../../app/store';
import { setCurrentSong } from '../../../features/song/songSlice'; 
import './MusicPlayer.css';

const MusicPlayer = () => {
    const dispatch = useDispatch();
    const currentSong = useSelector((state: RootState) => state.song.currentSong);
    const playlist = useSelector((state: RootState) => state.song.currentPlaylist) || [];
    
    const audioRef = useRef<HTMLAudioElement>(null);
    const [isPlaying, setIsPlaying] = useState(false);
    const [currentTime, setCurrentTime] = useState(0);
    const [duration, setDuration] = useState(0);
    const [volume, setVolume] = useState(0.7);
    const [isShuffle, setIsShuffle] = useState(false);
    const [repeatMode, setRepeatMode] = useState<'none' | 'all' | 'one'>('none');

    const getAudioUrl = (song: any) => {
        const path = song?.filePath || song?.path || song?.url;
        if (!path || path.trim() === "") return null;
        return path.startsWith('http') ? path : `http://localhost:5270${path}`;
    };

    const getCurrentIndex = () => {
        return playlist.findIndex(s => 
            (s.songId && s.songId === currentSong?.songId) || 
            (s.id && s.id === currentSong?.id) ||
            (s.title === currentSong?.title)
        );
    };

    useEffect(() => {
        const url = getAudioUrl(currentSong);
        if (url && audioRef.current) {
            audioRef.current.load();
            audioRef.current.play().then(() => setIsPlaying(true)).catch(() => setIsPlaying(false));
        }
    }, [currentSong]);

    const playNextSong = () => {
        if (playlist.length === 0) return;
        const currentIndex = getCurrentIndex();
        let nextIndex = isShuffle ? Math.floor(Math.random() * playlist.length) : currentIndex + 1;
        if (nextIndex < playlist.length && nextIndex !== -1) {
            dispatch(setCurrentSong(playlist[nextIndex]));
        } else if (repeatMode === 'all') {
            dispatch(setCurrentSong(playlist[0]));
        }
    };

    const playPrevSong = () => {
        if (playlist.length === 0) return;
        if (audioRef.current && audioRef.current.currentTime > 3) {
            audioRef.current.currentTime = 0;
            return;
        }
        const currentIndex = getCurrentIndex();
        let prevIndex = currentIndex - 1;
        if (prevIndex >= 0) dispatch(setCurrentSong(playlist[prevIndex]));
        else if (repeatMode === 'all') dispatch(setCurrentSong(playlist[playlist.length - 1]));
    };

    const formatTime = (time: number) => {
        if (isNaN(time)) return "0:00";
        const minutes = Math.floor(time / 60);
        const seconds = Math.floor(time % 60);
        return `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;
    };

    const audioUrl = getAudioUrl(currentSong);
    if (!currentSong || !audioUrl) return null;

    return (
        <footer className="player-container">
            <div className="current-song-info">
                <div className="player-img-box"><Music size={20} color="white" /></div>
                <div className="song-details">
                    <Typography className="song-title">{currentSong.title || "שיר ללא שם"}</Typography>
                    <Typography className="song-artist">{currentSong.artist || "אמן לא ידוע"}</Typography>
                </div>
            </div>

            <div className="player-controls">
                <div className="control-buttons">
                    <button className={`secondary-btn ${isShuffle ? 'active-icon' : ''}`} onClick={() => setIsShuffle(!isShuffle)}><Shuffle size={18} /></button>
                    <button className="secondary-btn" onClick={playPrevSong}><SkipBack size={20} fill="white" color="white" /></button>
                    <button className="play-pause-btn" onClick={() => {
                        if (isPlaying) audioRef.current?.pause(); else audioRef.current?.play();
                        setIsPlaying(!isPlaying);
                    }}>{isPlaying ? <Pause size={20} fill="black" /> : <Play size={20} fill="black" />}</button>
                    <button className="secondary-btn" onClick={playNextSong}><SkipForward size={20} fill="white" color="white" /></button>
                    <button className="secondary-btn" onClick={() => setRepeatMode(m => m === 'none' ? 'all' : m === 'all' ? 'one' : 'none')}>
                        {repeatMode === 'one' ? <Repeat1 size={18} className="active-icon" /> : <Repeat size={18} className={repeatMode === 'all' ? 'active-icon' : ''} />}
                    </button>
                </div>
                
                <div className="progress-container">
                    <span className="time-label">{formatTime(currentTime)}</span>
                    <div className="progress-bar-wrapper">
                        <input type="range" min="0" max={duration || 0} value={currentTime} onChange={(e) => { if(audioRef.current) audioRef.current.currentTime = parseFloat(e.target.value); }} className="progress-slider" />
                        <div className="progress-bar-background">
                            <div className="progress-fill-visual" style={{ width: `${(currentTime / (duration || 1)) * 100}%` }}></div>
                        </div>
                    </div>
                    <span className="time-label">{formatTime(duration)}</span>
                </div>

                <audio 
                    ref={audioRef}
                    key={audioUrl} 
                    src={audioUrl}
                    onTimeUpdate={() => setCurrentTime(audioRef.current?.currentTime || 0)}
                    onLoadedMetadata={() => setDuration(audioRef.current?.duration || 0)}
                    onEnded={playNextSong}
                />
            </div>

            <div className="extra-controls">
                <Volume2 size={18} color="#b3b3b3" />
                <input type="range" min="0" max="1" step="0.01" value={volume} onChange={(e) => {
                    const vol = parseFloat(e.target.value);
                    setVolume(vol);
                    if (audioRef.current) audioRef.current.volume = vol;
                }} className="volume-slider" />
            </div>
        </footer>
    );
};

export default MusicPlayer;