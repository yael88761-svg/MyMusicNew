import React from 'react';
import { TableRow, TableCell, Box, Typography } from '@mui/material';
import { Music, Play, Trash2 } from 'lucide-react'; 
import './SongRow.css'; 

interface SongRowProps {
    song: any;
    index: number;
    onPlay: (song: any) => void;
    onDelete: (id: string) => void; 
}

const SongRow: React.FC<SongRowProps> = ({ song, index, onPlay, onDelete }) => {
    if (!song) return null;

    const handleDeleteClick = (e: React.MouseEvent<HTMLButtonElement>) => {
    e.preventDefault();
    e.stopPropagation(); 

    if (typeof onDelete !== 'function') {
        console.error(
            "טעות: הקומפוננטה שקוראת ל-SongRow לא העבירה את הפונקציה onDelete!",
            "ערך נוכחי שנתקבל:", onDelete
        );
        alert("שגיאת פיתוח: פונקציית המחיקה לא הועברה כראוי מקומפוננטת האב.");
        return;
    }

    const targetId = song.songId || song.id;
    if (targetId) {
        onDelete(targetId);
    } else {
        console.error("לא נמצא מזהה (ID) עבור השיר הנוכחי", song);
    }
};
    return (
        <TableRow 
            onClick={() => onPlay(song)}
            className="my-song-row"
        >
            {/* Number column/player icon */}
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

        {/* Image and song title column */}     
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

            {/* Artist column */}
            <TableCell className="song-artist-text">
                {song.artist || "אמן לא ידוע"}
            </TableCell>

            {/* Delete action column */}
            <TableCell className="delete-cell" sx={{ width: '50px', textAlign: 'center' }}>
                <button 
                    className="row-delete-icon" 
                    onClick={handleDeleteClick}
                    title="מחק שיר"
                >
                    <Trash2 size={16} />
                </button>
            </TableCell>
        </TableRow>
    );
};

export default SongRow;