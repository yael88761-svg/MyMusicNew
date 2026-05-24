import React from 'react';
import { Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper } from '@mui/material';
import SongRow from './SongRow'; 
import { useDispatch } from 'react-redux';
import { setCurrentSong, setCurrentPlaylist } from '../../../features/song/songSlice'; 
import './SongTable.css';

interface SongTableProps {
    songs: any[];
    onDeleteSong: (id: string) => void;
}

const SongTable: React.FC<SongTableProps> = ({ songs, onDeleteSong }) => {
    const dispatch = useDispatch();

    const handlePlaySong = (selectedSong: any) => {
        dispatch(setCurrentSong(selectedSong));
        const songsToPlay = songs.map(ps => ps.song).filter(Boolean);
        dispatch(setCurrentPlaylist(songsToPlay));
    };

    // 🌟 פונקציית הגנה חדשה: מקפיצה התראה לפני ביצוע המחיקה
    const handleDeleteWithConfirmation = (id: string) => {
        const isConfirmed = window.confirm("האם אתה בטוח שברצונך למחוק את השיר ואת כל המאפיינים שלו לחלוטין מהמערכת?");
        if (isConfirmed) {
            onDeleteSong(id); // מפעיל את המחיקה רק אם המשתמש לחץ אישור
        }
    };

    return (
        <TableContainer component={Paper} className="song-table-container">
            <Table size="small" stickyHeader>
                <TableHead 
                    sx={{
                        '& .MuiTableCell-stickyHeader': {
                            backgroundColor: '#000000 !important',
                            color: '#b3b3b3 !important',
                            borderBottom: '1px solid rgba(255,255,255,0.1) !important',
                            zIndex: 3
                        }
                    }}
                >
                    <TableRow className="song-table-header">
                        <TableCell width="50">#</TableCell>
                        <TableCell>כותרת</TableCell>
                        <TableCell align="left">אמן</TableCell>
                        <TableCell width="50" align="center"></TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {songs.map((ps: any, index: number) => (
                        ps.song && (
                            <SongRow 
                                key={ps.song.songId || ps.song.id} 
                                song={ps.song} 
                                index={index} 
                                onPlay={handlePlaySong} 
                                // 🌟 משנים כאן: מעבירים את הפונקציה החדשה עם האזהרה במקום את המקורית
                                onDelete={handleDeleteWithConfirmation} 
                            />
                        )
                    ))}
                </TableBody>
            </Table>
        </TableContainer>
    );
};

export default SongTable;