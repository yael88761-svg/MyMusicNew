import React from 'react';

interface Playlist {
    playlistId: string;
    playlistName: string;
}

interface PlaylistListProps {
    playlists: Playlist[] | undefined;
    selectedPlaylistId: string | undefined;
    onSelect: (playlist: Playlist) => void;
}

const PlaylistList: React.FC<PlaylistListProps> = ({ playlists, selectedPlaylistId, onSelect }) => {
    
    // הגדרת הפלייליסט המיוחד כאובייקט קבוע
    const recentPlaylist: Playlist = {
        playlistId: 'recent',
        playlistName: '✨ נוספו לאחרונה'
    };

    return (
        <ul className="playlist-list">
            {/* הצגת הפלייליסט המיוחד תמיד בראש הרשימה */}
            <li 
                key={recentPlaylist.playlistId}
                onClick={() => onSelect(recentPlaylist)}
                className={selectedPlaylistId === recentPlaylist.playlistId ? 'active special-playlist' : 'special-playlist'}
            >
                <span className="playlist-name-text">{recentPlaylist.playlistName}</span>
            </li>

            <hr className="playlist-divider" /> {/* קו מפריד אופציונלי */}

            {/* הצגת שאר הפלייליסטים מהשרת */}
            {playlists?.map((pl) => (
                <li 
                    key={pl.playlistId}
                    onClick={() => onSelect(pl)}
                    className={selectedPlaylistId === pl.playlistId ? 'active' : ''}
                >
                    <span className="playlist-name-text">{pl.playlistName}</span>
                </li>
            ))}        
        </ul>
    );
};

export default PlaylistList;