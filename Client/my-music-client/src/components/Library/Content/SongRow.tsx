import React from 'react';
import { TableRow, TableCell, Box, Typography } from '@mui/material';
import { Music, Play } from 'lucide-react';

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
            hover
            onClick={() => onPlay(song)}
            className="song-row"
            sx={{ 
                cursor: 'pointer',
                '&:hover .num': { display: 'none' },
                '&:hover .row-play-icon': { display: 'block' },
                '& td': { border: 'none' } // הסרת קווים מפרידים לפי העיצוב שלך
            }}
        >
            <TableCell className="index-cell" sx={{ width: '50px' }}>
                {/* וידוא שהאינדקס הוא מספר תקין למניעת NaN */}
                <span className="num">{typeof index === 'number' ? index + 1 : ''}</span>
                <Play size={14} className="row-play-icon" style={{ display: 'none' }} fill="white" />
            </TableCell>

            <TableCell>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                    <Box 
                        className="song-img-placeholder" 
                        sx={{ 
                            width: 40, 
                            height: 40, 
                            bgcolor: '#282828', 
                            display: 'flex', 
                            alignItems: 'center', 
                            justifyContent: 'center',
                            borderRadius: '4px'
                        }}
                    >
                        <Music size={16} color="#b3b3b3" />
                    </Box>
                    <Box>
                        <Typography sx={{ color: 'white', fontSize: '0.9rem', fontWeight: 500 }}>
                            {song.title || "ללא כותרת"}
                        </Typography>
                    </Box>
                </Box>
            </TableCell>

            <TableCell sx={{ color: '#b3b3b3' }}>
                {song.artist || "אמן לא ידוע"}
            </TableCell>
        </TableRow>
    );
};

export default SongRow;