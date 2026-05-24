import React from 'react';
import { TableRow, TableCell, Box, Typography } from '@mui/material';
import { Music, Play, Trash2 } from 'lucide-react'; // ייבוא Trash2
import './SongRow.css'; // ייבוא קובץ ה-CSS המופרד

interface SongRowProps {
    song: any;
    index: number;
    onPlay: (song: any) => void;
    onDelete: (id: string) => void; // פרופ חדש למחיקה
}

const SongRow: React.FC<SongRowProps> = ({ song, index, onPlay, onDelete }) => {
    // הגנה מפני קריסה אם אובייקט השיר לא קיים
    if (!song) return null;

    // פונקציה שמטפלת בלחיצה על המחיקה ללא הפעלת הנגן
    const handleDeleteClick = (e: React.MouseEvent<HTMLButtonElement>) => {
    e.preventDefault();
    e.stopPropagation(); 

    // הגנה מוחלטת מפני קריסה: בדיקה האם onDelete הוא אכן פונקציה
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

            {/* עמודת פעולת מחיקה (פח) */}
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