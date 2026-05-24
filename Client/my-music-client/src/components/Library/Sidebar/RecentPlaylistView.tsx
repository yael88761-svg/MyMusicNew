import React from 'react';
import { useDispatch } from 'react-redux';
import { Table, TableBody, TableContainer } from '@mui/material'; 
import { useGetRecentPlaylistQuery } from '../../../features/playlist/playlistApi';
import { setCurrentSong, setCurrentPlaylist } from '../../../features/song/songSlice';
import SongRow from '../Content/SongRow';
import './RecentPlaylistView.css'; // ייבוא ה-CSS המעודכן שכולל את חוקי הגלילה

const RecentPlaylistView: React.FC = () => {
    const dispatch = useDispatch();
    const { data, isLoading, isError } = useGetRecentPlaylistQuery();

    const handleSongClick = (selectedSong: any) => {
        dispatch(setCurrentSong(selectedSong));

        if (data?.playlistSongs) {
            const allSongs = data.playlistSongs.map((item: any) => item.song);
            dispatch(setCurrentPlaylist(allSongs));
        }
    };

    if (isLoading) return <div className="loading">טוען שירים חדשים...</div>;
    if (isError) return <div className="error">שגיאה בטעינת הפלייליסט</div>;

    return (
        <div className="playlist-detail-view">
            <header className="playlist-header">
                <h1>{data?.playlistName || "נוספו לאחרונה"}</h1>
            </header>
            
            {/* הוספת המחלקה המאפשרת גלילה עצמאית לטבלה בלבד */}
            <TableContainer className="recent-table-container">
                <Table sx={{ minWidth: 650, borderCollapse: 'collapse' }}>
                    <TableBody>
                        {data?.playlistSongs?.map((item: any, index: number) => (
                            <SongRow 
                                key={item.songId || index} 
                                song={item.song} 
                                index={index} 
                                onPlay={handleSongClick} 
                            />
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
        </div>
    );
};

export default RecentPlaylistView;