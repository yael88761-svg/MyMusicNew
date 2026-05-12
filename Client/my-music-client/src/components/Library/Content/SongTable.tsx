import React from 'react';
import { Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper } from '@mui/material';
import SongRow from './SongRow'; 
import { useDispatch } from 'react-redux';

// 1. הוסף את setPlaylist לייבוא
import { setCurrentSong, setCurrentPlaylist } from '../../../features/song/songSlice'; 

interface SongTableProps {
    songs: any[];
}

const SongTable: React.FC<SongTableProps> = ({ songs }) => {
    const dispatch = useDispatch();

    // 2. צור פונקציה שמטפלת בניגון ושומרת את כל הרשימה
    const handlePlaySong = (selectedSong: any) => {
        // שליחת השיר הספציפי לנגן
        dispatch(setCurrentSong(selectedSong));
        
        // שליחת כל רשימת השירים שקיימת בטבלה לנגן
        // אנחנו מבצעים map כדי לשלוח רק את אובייקט ה-song מתוך המבנה של ps.song
        const songsToPlay = songs.map(ps => ps.song).filter(Boolean);
        dispatch(setCurrentPlaylist(songsToPlay));
    };

    return (
        <TableContainer component={Paper} sx={{ backgroundColor: 'transparent', boxShadow: 'none' }}>
            <Table size="small">
                <TableHead>
                    <TableRow sx={{ '& th': { borderBottom: '1px solid rgba(255,255,255,0.1)', color: '#b3b3b3' } }}>
                        <TableCell width="50">#</TableCell>
                        <TableCell>כותרת</TableCell>
                        <TableCell align="left">אמן</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {songs.map((ps: any, index: number) => (
                        ps.song && (
                            <SongRow 
                                key={ps.song.songId} 
                                song={ps.song} 
                                index={index} 
                                // 3. השתמש בפונקציה החדשה שיצרנו
                                onPlay={handlePlaySong} 
                            />
                        )
                    ))}
                </TableBody>
            </Table>
        </TableContainer>
    );
};

export default SongTable;