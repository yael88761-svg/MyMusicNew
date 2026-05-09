import React from 'react';
import { Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper } from '@mui/material';
import SongRow from './SongRow'; // תיקון: SongRow נמצא באותה תיקייה (Content)
import { useDispatch } from 'react-redux';

// תיקון הנתיב: צריך לעלות 3 רמות למעלה כדי להגיע ל-features
// 1. מ-Content ל-Library
// 2. מ-Library ל-components
// 3. מ-components ל-src (שם נמצאת תיקיית features)
import { setCurrentSong } from '../../../features/song/songSlice'; 

interface SongTableProps {
    songs: any[];
}

const SongTable: React.FC<SongTableProps> = ({ songs }) => {
    const dispatch = useDispatch();

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
                                onPlay={(song) => dispatch(setCurrentSong(song))} 
                            />
                        )
                    ))}
                </TableBody>
            </Table>
        </TableContainer>
    );
};

export default SongTable;