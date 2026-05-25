import type { PayloadAction } from '@reduxjs/toolkit';
import { createSlice } from '@reduxjs/toolkit';

interface SongState {
  currentSong: any | null;      
  currentPlaylist: any[];      
  isPlaying: boolean;          
  volume: number;              
  hasStartedPlaying: boolean;
}

const initialState: SongState = {
  currentSong: null,
  currentPlaylist: [],         
  isPlaying: false,
  volume: 0.7,
  hasStartedPlaying: false,  
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
      state.hasStartedPlaying = true;
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
      state.hasStartedPlaying = false; 
    },
  },
});

export const { 
  setCurrentSong, 
  setCurrentPlaylist, 
  togglePlay, 
  setVolume, 
  stopSong 
} = songSlice.actions;

export default songSlice.reducer;