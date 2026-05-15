import type { PayloadAction } from '@reduxjs/toolkit';
import { createSlice } from '@reduxjs/toolkit';

interface SongState {
  currentSong: any | null;      
  currentPlaylist: any[];      
  isPlaying: boolean;          
  volume: number;              
  hasStartedPlaying: boolean; // ✅ הדגל החדש ששולט על הצגת הנגן לראשונה
}

const initialState: SongState = {
  currentSong: null,
  currentPlaylist: [],         
  isPlaying: false,
  volume: 0.7,
  hasStartedPlaying: false,   // ✅ ברירת מחדל: לא התחיל לנגן (הנגן מוסתר)
};

export const songSlice = createSlice({
  name: 'song',
  initialState,
  reducers: {
    setCurrentPlaylist: (state, action: PayloadAction<any[]>) => {
      state.currentPlaylist = action.payload;
    },
    
    setCurrentSong: (state, action: PayloadAction<any>) => {
      state.currentSong = action.payload;
      state.isPlaying = true;
      state.hasStartedPlaying = true; // ✅ ברגע שנבחר שיר, הנגן יורשה להופיע
    },

    togglePlay: (state) => {
      state.isPlaying = !state.isPlaying;
    },

    setVolume: (state, action: PayloadAction<number>) => {
      state.volume = action.payload;
    },

    // פונקציה שתקראי לה גם בזמן Logout
    stopSong: (state) => {
      state.currentSong = null;
      state.currentPlaylist = [];
      state.isPlaying = false;
      state.hasStartedPlaying = false; // ✅ מאפס את המצב כך שבכניסה הבאה הנגן יהיה מוסתר
    },
  },
});

// ייצוא הפעולות
export const { 
  setCurrentSong, 
  setCurrentPlaylist, 
  togglePlay, 
  setVolume, 
  stopSong 
} = songSlice.actions;

export default songSlice.reducer;