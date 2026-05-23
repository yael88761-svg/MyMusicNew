import React from 'react';
import { Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper } from '@mui/material';
import SongRow from './SongRow'; 
import { useDispatch } from 'react-redux';
import { setCurrentSong, setCurrentPlaylist } from '../../../features/song/songSlice'; 
import './SongTable.css';

interface SongTableProps {
    songs: any[];
}

const SongTable: React.FC<SongTableProps> = ({ songs }) => {
    const dispatch = useDispatch();

    const handlePlaySong = (selectedSong: any) => {
        dispatch(setCurrentSong(selectedSong));
        const songsToPlay = songs.map(ps => ps.song).filter(Boolean);
        dispatch(setCurrentPlaylist(songsToPlay));
    };

    return (
        <TableContainer component={Paper} className="song-table-container">
            <Table size="small" stickyHeader>
                {/* ה-sx כאן פותר את בעיית השקיפות באופן מוחלט על ידי הזרקה ישירה לתתי-האלמנטים */}
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
                    </TableRow>
                </TableHead>
                <TableBody>
                    {songs.map((ps: any, index: number) => (
                        ps.song && (
                            <SongRow 
                                key={ps.song.songId} 
                                song={ps.song} 
                                index={index} 
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