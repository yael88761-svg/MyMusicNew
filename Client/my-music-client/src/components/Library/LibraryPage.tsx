import React, { useState, useRef } from 'react';
import { useGetPlaylistsQuery, useUploadSongToPlaylistMutation } from '../../features/playlist/playlistApi';
import { Typography } from '@mui/material';
import { Library, Music } from 'lucide-react';


import PlaylistList from './Sidebar/PlaylistList';     // הסרנו נקודה אחת כי Sidebar הוא אח של LibraryPage
import CreatePlaylistBtn from './Sidebar/CreatePlaylistBtn'; 
import PlaylistView from './Content/PlaylistView';     // כנ"ל לגבי Contentimport './LibraryPage.css';

const LibraryPage = () => {
    const { data: playlists, isLoading, error } = useGetPlaylistsQuery(); 
    const [uploadSongToPlaylist] = useUploadSongToPlaylistMutation();
    const [selectedPlaylist, setSelectedPlaylist] = useState<any>(null);
    const fileInputRef = useRef<HTMLInputElement>(null);

    const activePlaylistData = playlists?.find((pl: any) => pl.playlistId === selectedPlaylist?.playlistId);

    const handleFileUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (!file || !selectedPlaylist) return;
        try {
            await uploadSongToPlaylist({ file, playlistId: selectedPlaylist.playlistId }).unwrap();
        } catch (err) { alert("שגיאה בהעלאה"); }
    };

    if (isLoading) return <div className="loading">טוען...</div>;
    if (error) return <div className="error">שגיאה בתקשורת</div>;

    return (
        <div className="library-layout">
            <aside className="sidebar">
                <div className="sidebar-header">
                    <div className="header-title"><Library size={24} /><Typography variant="h6">הספרייה שלך</Typography></div>
                    <CreatePlaylistBtn />
                </div>
                <PlaylistList playlists={playlists} selectedPlaylistId={selectedPlaylist?.playlistId} onSelect={setSelectedPlaylist} />
            </aside>

            <main className="main-content">
                {activePlaylistData ? (
                    <PlaylistView 
                        playlistData={activePlaylistData} 
                        fileInputRef={fileInputRef} 
                        onUploadClick={() => fileInputRef.current?.click()} 
                        onFileUpload={handleFileUpload} 
                    />
                ) : (
                    <div className="empty-state">
                        <Music size={64} color="#282828" />
                        <Typography variant="h5">בחר פלייליסט כדי להתחיל</Typography>
                    </div>
                )}
            </main>
        </div>
    );
};

export default LibraryPage;