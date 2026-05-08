import React from 'react';
import { TableRow, TableCell, Box, Typography } from '@mui/material';
import { Music, Play } from 'lucide-react';

interface SongRowProps {
    song: any;
    index: number;
    onPlay: (song: any) => void;
}

const SongRow: React.FC<SongRowProps> = ({ song, index, onPlay }) => {
    return (
        <TableRow 
            hover
            onClick={() => onPlay(song)}
            className="song-row"
        >
            <TableCell className="index-cell">
                <span className="num">{index + 1}</span>
                <Play size={14} className="row-play-icon" fill="white" />
            </TableCell>
            <TableCell sx={{ border: 'none' }}>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                    <Box className="song-img-placeholder"><Music size={16} /></Box>
                    <Box>
                        <Typography sx={{ color: 'white', fontSize: '1rem' }}>{song.title}</Typography>
                    </Box>
                </Box>
            </TableCell>
            <TableCell sx={{ color: '#b3b3b3', border: 'none' }}>
                {song.artist || "אמן לא ידוע"}
            </TableCell>
        </TableRow>
    );
};

export default SongRow;