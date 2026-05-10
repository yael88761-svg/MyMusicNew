import React from 'react';
import { Plus } from 'lucide-react';
// תיקון הנתיב: עולים 3 רמות למעלה
import { useCreatePlaylistMutation } from '../../../features/playlist/playlistApi';

const CreatePlaylistBtn = () => {
    const [createPlaylist] = useCreatePlaylistMutation();

// בתוך CreatePlaylistBtn.tsx
const handleAddPlaylist = async () => {
    const name = prompt("איך לקרוא לפלייליסט החדש?");
    if (!name) return;
    
    try {
        // ננסה לשלוח רק את השדה הנדרש
        await createPlaylist({ 
            playlistName: name,
            playlistSongs: [] // שלח מערך ריק כדי למנוע שגיאות וולידציה על ה-Collection
        }).unwrap();
    } catch (err) {
        console.error("Server Error:", err);
    }
};
    return (
        <button onClick={handleAddPlaylist} className="add-playlist-btn">
            <Plus size={20} />
        </button>
    );
};

export default CreatePlaylistBtn;