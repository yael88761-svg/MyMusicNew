import React, { useState, useRef, useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { Typography } from '@mui/material';
import { Music, Play, Pause, SkipBack, SkipForward, Repeat, Shuffle, Volume2, Repeat1 } from 'lucide-react';
import type { RootState } from '../../app/store';
import { setCurrentSong } from '../../../features/song/songSlice';
import './MusicPlayer.css';

const MusicPlayer = () => {
    const dispatch = useDispatch();
    
    // קבלת הנתונים מה-Store
    const currentSong = useSelector((state: RootState) => state.song.currentSong);
    const playlist = useSelector((state: RootState) => state.song.currentPlaylist) || [];

    const audioRef = useRef<HTMLAudioElement>(null);
    const [isPlaying, setIsPlaying] = useState(false);
    const [currentTime, setCurrentTime] = useState(0);
    const [duration, setDuration] = useState(0);
    const [volume, setVolume] = useState(0.7);
    const [isShuffle, setIsShuffle] = useState(false);
    const [repeatMode, setRepeatMode] = useState<'none' | 'all' | 'one'>('none');

    // פונקציית עזר לכתובת השיר
    const getAudioUrl = (song: any) => {
        if (!song) return "";
        const path = song.filePath || song.path || song.url;
        if (!path) return "";
        return path.startsWith('http') ? path : `http://localhost:5270${path}`;
    };

    // ✅ מציאת אינדקס חסינה במיוחד
    const getCurrentIndex = () => {
        if (!currentSong || playlist.length === 0) return -1;
        
        return playlist.findIndex((s) => {
            const sId = String(s.id || s.songId || "");
            const currId = String(currentSong.id || currentSong.songId || "");
            
            // בדיקה לפי ID או לפי כותרת (ליתר ביטחון)
            return (sId !== "" && sId === currId) || (s.title === currentSong.title);
        });
    };

    // הפעלה אוטומטית כשמתחלף שיר
    useEffect(() => {
        const url = getAudioUrl(currentSong);
        if (url && audioRef.current) {
            audioRef.current.load();
            audioRef.current.play()
                .then(() => setIsPlaying(true))
                .catch(() => setIsPlaying(false));
        }
    }, [currentSong?.id, currentSong?.songId, currentSong?.title]);

    const playNextSong = () => {
        if (playlist.length === 0) {
            console.error("Playlist is empty");
            return;
        }

        const currentIndex = getCurrentIndex();
        console.log("Current Index:", currentIndex);
        console.log("Playlist Length:", playlist.length);

        let nextIndex: number;

        if (isShuffle) {
            nextIndex = Math.floor(Math.random() * playlist.length);
        } else {
            nextIndex = currentIndex + 1;
        }

        // בדיקה שהאינדקס הבא קיים ברשימה
        if (nextIndex >= 0 && nextIndex < playlist.length) {
            console.log("Setting next song:", playlist[nextIndex]);
            dispatch(setCurrentSong(playlist[nextIndex]));
        } else if (repeatMode === 'all' || currentIndex === -1) {
            // אם הגענו לסוף או שלא מצאנו את השיר הנוכחי - חזור להתחלה
            dispatch(setCurrentSong(playlist[0]));
        } else {
            setIsPlaying(false);
        }
    };

    const playPrevSong = () => {
        if (playlist.length === 0) return;

        const currentIndex = getCurrentIndex();
        
        // אם עברו יותר מ-3 שניות - פשוט נחזור להתחלת השיר
        if (audioRef.current && audioRef.current.currentTime > 3) {
            audioRef.current.currentTime = 0;
            return;
        }

        let prevIndex = currentIndex - 1;

        if (prevIndex >= 0) {
            dispatch(setCurrentSong(playlist[prevIndex]));
        } else if (repeatMode === 'all') {
            dispatch(setCurrentSong(playlist[playlist.length - 1]));
        } else {
            // אם אין שיר קודם - חזור להתחלת הנוכחי
            if (audioRef.current) audioRef.current.currentTime = 0;
        }
    };

    const togglePlayPause = () => {
        if (!audioRef.current) return;
        if (isPlaying) {
            audioRef.current.pause();
        } else {
            audioRef.current.play().catch(() => {});
        }
        setIsPlaying(!isPlaying);
    };

    const formatTime = (time: number) => {
        if (isNaN(time)) return '0:00';
        const minutes = Math.floor(time / 60);
        const seconds = Math.floor(time % 60);
        return `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;
    };

    // אם אין שיר נוכחי - לא מציגים כלום כדי למנוע כפתורים ללא שיוך
    if (!currentSong) return null;

    return (
        <footer className="player-container">
            <div className="current-song-info">
                <div className="player-img-box">
                    <Music size={20} color="white" />
                </div>
                <div className="song-details">
                    <Typography className="song-title">{currentSong.title || "שיר ללא שם"}</Typography>
                    <Typography className="song-artist">{currentSong.artist || "אמן לא ידוע"}</Typography>
                </div>
            </div>

            <div className="player-controls">
                <div className="control-buttons">
                    <button className={`secondary-btn ${isShuffle ? 'active-icon' : ''}`} onClick={() => setIsShuffle(!isShuffle)}>
                        <Shuffle size={18} />
                    </button>

                    <button className="secondary-btn" onClick={playPrevSong}>
                        <SkipBack size={20} fill="white" color="white" />
                    </button>

                    <button className="play-pause-btn" onClick={togglePlayPause}>
                        {isPlaying ? <Pause size={20} fill="black" /> : <Play size={20} fill="black" />}
                    </button>

                    <button className="secondary-btn" onClick={playNextSong}>
                        <SkipForward size={20} fill="white" color="white" />
                    </button>

                    <button className="secondary-btn" onClick={() => setRepeatMode(m => m === 'none' ? 'all' : m === 'all' ? 'one' : 'none')}>
                        {repeatMode === 'one' ? <Repeat1 size={18} className="active-icon" /> : <Repeat size={18} className={repeatMode === 'all' ? 'active-icon' : ''} />}
                    </button>
                </div>

                <div className="progress-container">
                    <span className="time-label">{formatTime(currentTime)}</span>
                    <div className="progress-bar-wrapper">
                        <input
                            type="range"
                            min="0"
                            max={duration || 0}
                            value={currentTime}
                            onChange={(e) => {
                                const val = parseFloat(e.target.value);
                                setCurrentTime(val);
                                if (audioRef.current) audioRef.current.currentTime = val;
                            }}
                            className="progress-slider"
                        />
                        <div className="progress-bar-background">
                            <div className="progress-fill-visual" style={{ width: `${(currentTime / (duration || 1)) * 100}%` }}></div>
                        </div>
                    </div>
                    <span className="time-label">{formatTime(duration)}</span>
                </div>

                <audio
                    ref={audioRef}
                    key={getAudioUrl(currentSong)} // שינוי ה-Key גורם ל-React לטעון את האלמנט מחדש כשיש שיר חדש
                    src={getAudioUrl(currentSong)}
                    onTimeUpdate={() => setCurrentTime(audioRef.current?.currentTime || 0)}
                    onLoadedMetadata={() => setDuration(audioRef.current?.duration || 0)}
                    onEnded={() => (repeatMode === 'one' ? (audioRef.current!.currentTime = 0, audioRef.current!.play()) : playNextSong())}
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
                    onChange={(e) => {
                        const v = parseFloat(e.target.value);
                        setVolume(v);
                        if (audioRef.current) audioRef.current.volume = v;
                    }}
                    className="volume-slider"
                />
            </div>
        </footer>
    );
};

export default MusicPlayer;