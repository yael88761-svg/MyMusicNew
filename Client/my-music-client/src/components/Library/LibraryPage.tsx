import React, { useState, useRef } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useGetPlaylistsQuery } from '../../features/playlist/playlistApi';
import { useUploadSongMutation } from '../../features/song/songApi'; 
import { Typography } from '@mui/material';
import { Library } from 'lucide-react';

import PlaylistList from './Sidebar/PlaylistList'; 
import CreatePlaylistBtn from './Sidebar/CreatePlaylistBtn'; 
import PlaylistView from '../Library/Content/PlaylistView'; 
import RecentPlaylistView from '../Library/Sidebar/RecentPlaylistView';
import AllSongsView from '../Library/Content/AllSongsView'; 
import { setCurrentSong } from '../../features/song/songSlice';

import './LibraryPage.css';

const LibraryPage = () => {
    const dispatch = useDispatch();
    const { data: playlists, isLoading: playlistsLoading, error: playlistsError, refetch } = useGetPlaylistsQuery(); 
    const [uploadSong] = useUploadSongMutation();
    const [selectedPlaylist, setSelectedPlaylist] = useState<any>({ playlistId: 'all-songs' });
    const fileInputRef = useRef<HTMLInputElement>(null);

    const currentSong = useSelector((state: any) => state.song.currentSong);

    const isAllSongsSelected = selectedPlaylist?.playlistId === 'all-songs';
    const isRecentSelected = selectedPlaylist?.playlistId === 'recent';
    const activePlaylistData = playlists?.find((pl: any) => pl.playlistId === selectedPlaylist?.playlistId);

    // פונקציית מחיקה גלובלית ברמת העמוד
    const handleDeleteSong = async (songIdToDelete: string) => {
        const isConfirmed = window.confirm("האם אתה בטוח שברצונך למחוק את השיר לחלוטין מהמערכת?");
        if (!isConfirmed) return;

        // 1. העברה אוטומטית לשיר הבא במידה והשיר הנוכחי מתוך הפלייליסט מתנגן
        if (currentSong && (currentSong.songId === songIdToDelete || currentSong.id === songIdToDelete)) {
            dispatch(setCurrentSong(null));
        }

        // 2. שליחת הבקשה לשרת ה-NET.
        try {
            const token = localStorage.getItem('token') || sessionStorage.getItem('token'); 

            // 🌟 התיקון הקריטי: שינוי מ- /api/songs/ ל- /api/Song/ (לשון יחיד, בדיוק כמו ב-Swagger)
            const response = await fetch(`/api/Song/${songIdToDelete}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': token ? `Bearer ${token}` : ''
                }
            });

            if (response.status === 204) { 
                console.log("השיר נמחק בהצלחה מהשרת");
                refetch(); // מרענן אוטומטית את רשימת השירים על המסך דרך RTK Query!
            } else if (response.status === 401) {
                alert("שגיאה (401): אינך מחובר למערכת או שהטוקן פג תוקף.");
            } else if (response.status === 403) {
                alert("שגיאה (403): אין לך הרשאה למחוק שיר זה (השיר לא שייך למשתמש שלך).");
            } else if (response.status === 404) {
                alert("שגיאה (404): השיר לא נמצא בשרת. ודא שמזהה השיר תקין.");
            } else {
                const errorText = await response.text();
                alert(`השרת החזיר שגיאה (${response.status}): ${errorText}`);
            }
        } catch (error: any) {
            console.error("שגיאה בתקשורת עם השרת:", error);
            alert(`שגיאת תקשורת: ${error.message || error}`);
        }
    };

    const handleFileUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (!file || !selectedPlaylist || isRecentSelected || isAllSongsSelected) return; 
        
        const formData = new FormData();
        formData.append('file', file);

        try {
            await uploadSong({ formData, playlistId: selectedPlaylist.playlistId }).unwrap();
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
                    <div className="header-title" onClick={() => setSelectedPlaylist({ playlistId: 'all-songs' })}>
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
                    /* העברת פונקציית המחיקה המעודכנת למסך כל השירים */
                    <AllSongsView playlists={playlists || []} onDeleteSong={handleDeleteSong} />
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
                            onDeleteSong={handleDeleteSong} /* העברת הפונקציה המשותפת */
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