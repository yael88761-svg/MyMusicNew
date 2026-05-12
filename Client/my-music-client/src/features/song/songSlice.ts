import type { PayloadAction } from '@reduxjs/toolkit';
import { createSlice } from '@reduxjs/toolkit';

interface SongState {
  currentSong: any | null;      
  currentPlaylist: any[];      // השם הנבחר
  isPlaying: boolean;          
  volume: number;              
}

const initialState: SongState = {
  currentSong: null,
  currentPlaylist: [],         
  isPlaying: false,
  volume: 0.7,
};

export const songSlice = createSlice({
  name: 'song',
  initialState,
  reducers: {
    // איחדתי את setPlaylist ו-setCurrentPlaylist לאותו שם כדי למנוע בלבול
    setCurrentPlaylist: (state, action: PayloadAction<any[]>) => {
      state.currentPlaylist = action.payload; // ✅ עכשיו זה תואם ל-initialState
    },
    
    setCurrentSong: (state, action: PayloadAction<any>) => {
      state.currentSong = action.payload;
      state.isPlaying = true;
    },

    togglePlay: (state) => {
      state.isPlaying = !state.isPlaying;
    },

    setVolume: (state, action: PayloadAction<number>) => {
      state.volume = action.payload;
    },

    stopSong: (state) => {
      state.currentSong = null;
      state.currentPlaylist = [];
      state.isPlaying = false;
    },
  },
});

// ייצוא השמות המדויקים
export const { 
  setCurrentSong, 
  setCurrentPlaylist, 
  togglePlay, 
  setVolume, 
  stopSong 
} = songSlice.actions;

export default songSlice.reducer;