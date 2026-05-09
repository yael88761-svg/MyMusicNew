import React from 'react';
import { Plus } from 'lucide-react';
// תיקון הנתיב: עולים 3 רמות למעלה
import { useCreatePlaylistMutation } from '../../../features/playlist/playlistApi';

const CreatePlaylistBtn = () => {
    const [createPlaylist] = useCreatePlaylistMutation();

    const handleAddPlaylist = async () => {
        const name = prompt("איך לקרוא לפלייליסט החדש?");
        if (!name) return;
        try {
            await createPlaylist({ playlistName: name }).unwrap();
        } catch (err) {
            alert("שגיאה ביצירת הפלייליסט");
        }
    };

    return (
        <button onClick={handleAddPlaylist} className="add-playlist-btn">
            <Plus size={20} />
        </button>
    );
};

export default CreatePlaylistBtn;