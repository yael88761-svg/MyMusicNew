import React, { useState, useRef, useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { Typography } from '@mui/material';
import { Music, Play, Pause, SkipBack, SkipForward, Repeat, Shuffle, Volume2, Repeat1, Sparkles } from 'lucide-react';
import type { RootState } from '../../app/store';
import { setCurrentSong } from '../../../features/song/songSlice';
import './MusicPlayer.css';

const MusicPlayer = () => {
    const dispatch = useDispatch();
    
    // קבלת הנתונים מה-Store (הפלייליסט/הספרייה הנוכחית של המשתמש)
    const currentSong = useSelector((state: RootState) => state.song.currentSong);
    const playlist = useSelector((state: RootState) => state.song.currentPlaylist) || [];

    const audioRef = useRef<HTMLAudioElement>(null);
    const [isPlaying, setIsPlaying] = useState(false);
    const [currentTime, setCurrentTime] = useState(0);
    const [duration, setDuration] = useState(0);
    const [volume, setVolume] = useState(0.7);
    const [isShuffle, setIsShuffle] = useState(false);
    const [isSmartPlay, setIsSmartPlay] = useState(false); // מצב בחירה חכמה
    const [repeatMode, setRepeatMode] = useState<'none' | 'all' | 'one'>('none');

    // פונקציית עזר לכתובת השיר
    const getAudioUrl = (song: any) => {
        if (!song) return "";
        const path = song.filePath || song.path || song.url;
        if (!path) return "";
        return path.startsWith('http') ? path : `http://localhost:5270${path}`;
    };

    // מציאת אינדקס חסינה במיוחד
    const getCurrentIndex = () => {
        if (!currentSong || playlist.length === 0) return -1;
        
        return playlist.findIndex((s) => {
            const sId = String(s.id || s.songId || "");
            const currId = String(currentSong.id || currentSong.songId || "");
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

    // ✅ אלגוריתם בחירה חכמה ישירות מתוך הפלייליסט/ספרייה הנוכחית
    const playSmartNextSong = () => {
        if (playlist.length <= 1) return false;

        // 1. סינון השיר הנוכחי כדי שלא יחזור על עצמו מיד
        const currentId = String(currentSong?.id || currentSong?.songId || "");
        const otherSongs = playlist.filter(s => String(s.id || s.songId || "") !== currentId);

        if (otherSongs.length === 0) return false;

        // 2. חילוץ מדדי ה-AI של השיר הנוכחי (אנרגיה ומצב רוח)
        const currentEnergy = currentSong?.audioFeatures?.energy ?? currentSong?.energy ?? 0.5;
        const currentValence = currentSong?.audioFeatures?.valence ?? currentSong?.valence ?? 0.5;

        // 3. חישוב המרחק לכל השירים האחרים ברשימה
        const songsWithDistance = otherSongs.map(song => {
            const songEnergy = song?.audioFeatures?.energy ?? song?.energy ?? 0.5;
            const songValence = song?.audioFeatures?.valence ?? song?.valence ?? 0.5;
            
            // נוסחת מרחק מנהטן (בדיוק כמו בשרת)
            const distance = Math.abs(songEnergy - currentEnergy) + Math.abs(songValence - currentValence);
            return { song, distance };
        });

        // 4. מיון השירים מהקרוב ביותר לרחוק ביותר
        songsWithDistance.sort((a, b) => a.distance - b.distance);

        // 🔥 מניעת הלולאה האינסופית: במקום לקחת תמיד את המקום ה-1, נגריל שיר מתוך ה-3 הכי קרובים!
        const poolSize = Math.min(3, songsWithDistance.length); 
        const randomIndex = Math.floor(Math.random() * poolSize);
        const selectedSong = songsWithDistance[randomIndex].song;

        if (selectedSong) {
            console.log("Smart Play selected from playlist:", selectedSong);
            dispatch(setCurrentSong(selectedSong));
            return true;
        }

        return false;
    };

    const playNextSong = () => {
        if (!currentSong) return;

        // ✅ אם מצב הפעלה חכמה פעיל - נשתמש באלגוריתם המקומי על הפלייליסט
        if (isSmartPlay) {
            const success = playSmartNextSong();
            if (success) return; 
        }

        if (playlist.length === 0) {
            console.error("Playlist is empty");
            return;
        }

        const currentIndex = getCurrentIndex();
        let nextIndex: number;

        if (isShuffle) {
            nextIndex = Math.floor(Math.random() * playlist.length);
        } else {
            nextIndex = currentIndex + 1;
        }

        if (nextIndex >= 0 && nextIndex < playlist.length) {
            dispatch(setCurrentSong(playlist[nextIndex]));
        } else if (repeatMode === 'all' || currentIndex === -1) {
            dispatch(setCurrentSong(playlist[0]));
        } else {
            setIsPlaying(false);
        }
    };

    const playPrevSong = () => {
        if (playlist.length === 0) return;

        const currentIndex = getCurrentIndex();
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
                    <button 
                        className={`secondary-btn ${isShuffle ? 'active-icon' : ''}`} 
                        onClick={() => {
                            setIsShuffle(!isShuffle);
                            if (!isShuffle) setIsSmartPlay(false);
                        }}
                    >
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

                    <button 
                        className={`secondary-btn ${isSmartPlay ? 'active-smart-icon' : ''}`} 
                        onClick={() => {
                            setIsSmartPlay(!isSmartPlay);
                            if (!isSmartPlay) setIsShuffle(false);
                        }}
                        title="הפעלה חכמה מבוססת AI"
                    >
                        <Sparkles size={18} />
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
                    key={getAudioUrl(currentSong)}
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