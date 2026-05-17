import React, { useState, useRef } from 'react';
import { useGetPlaylistsQuery } from '../../features/playlist/playlistApi';
import { useUploadSongMutation } from '../../features/song/songApi'; 
import { Typography } from '@mui/material';
import { Library } from 'lucide-react';

import PlaylistList from './Sidebar/PlaylistList'; 
import CreatePlaylistBtn from './Sidebar/CreatePlaylistBtn'; 
import PlaylistView from '../Library/Content/PlaylistView'; 
import RecentPlaylistView from '../Library/Sidebar/RecentPlaylistView';
import AllSongsView from '../Library/Content/AllSongsView'; 

import './LibraryPage.css';

const LibraryPage = () => {
    // ✅ שינוי 1: חילצנו את פונקציית ה-refetch מתוך ה-Hook של הפלייליסטים
    const { data: playlists, isLoading: playlistsLoading, error: playlistsError, refetch } = useGetPlaylistsQuery(); 
    const [uploadSong] = useUploadSongMutation();
    const [selectedPlaylist, setSelectedPlaylist] = useState<any>({ playlistId: 'all-songs' });
    const fileInputRef = useRef<HTMLInputElement>(null);

    const isAllSongsSelected = selectedPlaylist?.playlistId === 'all-songs';
    const isRecentSelected = selectedPlaylist?.playlistId === 'recent';
    const activePlaylistData = playlists?.find((pl: any) => pl.playlistId === selectedPlaylist?.playlistId);

    const handleFileUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (!file || !selectedPlaylist || isRecentSelected || isAllSongsSelected) return; 
        
        const formData = new FormData();
        formData.append('file', file);

        try {
            // העלאת השיר לשרת
            await uploadSong({ formData, playlistId: selectedPlaylist.playlistId }).unwrap();
            
            // ✅ שינוי 2: פוקדים על השאילתה לרוץ שוב ולמשוך את הרשימה המעודכנת מהשרת!
            refetch(); 
            
            alert("השיר הועלה בהצלחה!");
        } catch (err) { 
            alert("שגיאה בהעלאת השיר"); 
        }
    };

    if (playlistsLoading) return <div className="loading">טוען...</div>;
    if (playlistsError) return <div className="error">שגיאה בתקשורת</div>;

    return (
        <div className="library-layout">
            <aside className="sidebar">
                <div className="sidebar-header">
                    <div className="header-title" onClick={() => setSelectedPlaylist({ playlistId: 'all-songs' })} style={{ cursor: 'pointer' }}>
                        <Library size={24} color="#b3b3b3" />
                        <Typography variant="h6" sx={{ color: '#b3b3b3', fontWeight: 'bold' }}>הספרייה שלך</Typography>
                    </div>
                    <CreatePlaylistBtn />
                </div>
                <PlaylistList 
                    playlists={playlists} 
                    selectedPlaylistId={selectedPlaylist?.playlistId} 
                    onSelect={setSelectedPlaylist} 
                />
            </aside>

            <main className="main-content">
                {isAllSongsSelected ? (
                    <AllSongsView playlists={playlists || []} />
                ) : isRecentSelected ? (
                    <div className="content-wrapper">
                        <RecentPlaylistView />
                    </div>
                ) : activePlaylistData ? (
                    <div className="content-wrapper">
                        <PlaylistView 
                            playlistData={activePlaylistData} 
                            fileInputRef={fileInputRef} 
                            onUploadClick={() => fileInputRef.current?.click()} 
                            onFileUpload={handleFileUpload} 
                        />
                    </div>
                ) : (
                    <div className="empty-state">
                        <Typography variant="h5" sx={{ color: 'white' }}>בחר פלייליסט כדי להתחיל</Typography>
                    </div>
                )}
            </main>
        </div>
    );
};

export default LibraryPage;