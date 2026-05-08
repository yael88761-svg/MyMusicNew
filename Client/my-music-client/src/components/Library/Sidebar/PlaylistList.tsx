import React from 'react';
import { Music } from 'lucide-react';

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
    return (
        <ul className="playlist-list">
            {playlists?.map((pl) => (
                <li 
                    key={pl.playlistId}
                    onClick={() => onSelect(pl)}
                    className={selectedPlaylistId === pl.playlistId ? 'active' : ''}
                >
                    <div className="playlist-icon-box">
                        <Music size={16} />
                    </div>
                    <span className="playlist-name-text">{pl.playlistName}</span>
                </li>
            ))}        
        </ul>
    );
};

export default PlaylistList;