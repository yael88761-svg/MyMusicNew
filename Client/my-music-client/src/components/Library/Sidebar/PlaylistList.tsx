import React from 'react';
import './PlaylistList.css';
import { useDeletePlaylistMutation } from '../../../features/playlist/playlistApi'; // ודא שניתוב הקובץ נכון אצלך

interface Playlist {
    playlistId: string;
    playlistName: string;
    playlistSongs?: any[]; // הוספת מערך השירים לצורך בדיקת המחיקה
}

interface PlaylistListProps {
    playlists: Playlist[] | undefined;
    selectedPlaylistId: string | undefined;
    onSelect: (playlist: Playlist) => void;
}

const PlaylistList: React.FC<PlaylistListProps> = ({ playlists, selectedPlaylistId, onSelect }) => {
    
    // קריאה ל-Hook המחיקה מתוך ה-RTK Query
    const [deletePlaylist] = useDeletePlaylistMutation();

    // הגדרת הפלייליסט המיוחד כאובייקט קבוע
    const recentPlaylist: Playlist = {
        playlistId: 'recent',
        playlistName: '✨  נוספו לאחרונה',
        playlistSongs: []
    };

    const handleDelete = async (e: React.MouseEvent, id: string) => {
        e.stopPropagation(); // מונע מהלחיצה לבחור (Select) את הפלייליסט במקביל למחיקה
        
        if (window.confirm('האם אתה בטוח שברצונך למחוק פלייליסט זה?')) {
            try {
                await deletePlaylist(id).unwrap();
            } catch (err) {
                console.error('שגיאה במחיקת הפלייליסט:', err);
                alert('מחיקת הפלייליסט נכשלה.');
            }
        }
    };

    return (
        <ul className="playlist-list">
            {/* הצגת הפלייליסט המיוחד תמיד בראש הרשימה
            <li 
                key={recentPlaylist.playlistId}
                onClick={() => onSelect(recentPlaylist)}
                className={`special-playlist ${selectedPlaylistId === recentPlaylist.playlistId ? 'active' : ''}`}
            >
                <span className="playlist-name-text">{recentPlaylist.playlistName}</span>
            </li>
 */}
            <hr className="playlist-divider" />

            {/* הצגת שאר הפלייליסטים מהשרת */}
            {playlists?.map((pl) => {
                // בדיקה אם הפלייליסט ריק (מותאם למערך ריק או תנאי חלופי)
                const isPlaylistEmpty = !pl.playlistSongs || pl.playlistSongs.length === 0;

                return (
                    <li 
                        key={pl.playlistId}
                        onClick={() => onSelect(pl)}
                        className={`playlist-item-row ${selectedPlaylistId === pl.playlistId ? 'active' : ''}`}
                    >
                        <span className="playlist-name-text">{pl.playlistName}</span>
                        
                        {/* כפתור הפח יוצג ב-DOM רק אם הפלייליסט ריק */}
                        {isPlaylistEmpty && (
                            <button 
                                className="delete-playlist-btn" 
                                onClick={(e) => handleDelete(e, pl.playlistId)}
                                title="מחק פלייליסט ריק"
                            >
                                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                    <polyline points="3 6 5 6 21 6"></polyline>
                                    <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path>
                                </svg>
                            </button>
                        )}
                    </li>
                );
            })}        
        </ul>
    );
};

export default PlaylistList;