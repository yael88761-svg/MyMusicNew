import React from 'react';
import { TableRow, TableCell, Box, Typography } from '@mui/material';
import { Music, Play } from 'lucide-react';
import './SongRow.css'; // ייבוא קובץ ה-CSS המופרד

interface SongRowProps {
    song: any;
    index: number;
    onPlay: (song: any) => void;
}

const SongRow: React.FC<SongRowProps> = ({ song, index, onPlay }) => {
    // הגנה מפני קריסה אם אובייקט השיר לא קיים
    if (!song) return null;

    return (
        <TableRow 
            onClick={() => onPlay(song)}
            className="my-song-row"
        >
            {/* עמודת מספר/אייקון נגן */}
            <TableCell className="index-cell" sx={{ width: '50px' }}>
                <Box className="index-cell-container">
                    <span className="num">
                        {typeof index === 'number' ? index + 1 : ''}
                    </span>
                    <Play 
                        size={14} 
                        className="row-play-icon" 
                        fill="white" 
                        color="white" 
                    />
                </Box>
            </TableCell>

            {/* עמודת תמונה וכותרת השיר */}
            <TableCell>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                    <Box className="song-img-placeholder">
                        <Music size={16} color="#b3b3b3" />
                    </Box>
                    <Box>
                        <Typography className="song-title-text">
                            {song.title || "ללא כותרת"}
                        </Typography>
                    </Box>
                </Box>
            </TableCell>

            {/* עמודת שם האמן */}
            <TableCell className="song-artist-text">
                {song.artist || "אמן לא ידוע"}
            </TableCell>
        </TableRow>
    );
};

export default SongRow;