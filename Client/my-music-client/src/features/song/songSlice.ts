import type { PayloadAction } from '@reduxjs/toolkit';
import { createSlice } from '@reduxjs/toolkit';

interface SongState {
  currentSong: any | null; // השיר שמתנגן כרגע
  isPlaying: boolean;      // האם הנגן פעיל
  volume: number;          // עוצמת שמע (0 עד 100)
}

const initialState: SongState = {
  currentSong: null,
  isPlaying: false,
  volume: 50,
};

export const songSlice = createSlice({
  name: 'song',
  initialState,
  reducers: {
    // הגדרת השיר הנוכחי והפעלה אוטומטית
    setCurrentSong: (state, action: PayloadAction<any>) => {
      state.currentSong = action.payload;
      state.isPlaying = true;
    },
    // שינוי מצב ניגון (Play/Pause)
    togglePlay: (state) => {
      state.isPlaying = !state.isPlaying;
    },
    // שינוי ווליום
    setVolume: (state, action: PayloadAction<number>) => {
      state.volume = action.payload;
    },
    // עצירת הנגן
    stopSong: (state) => {
      state.currentSong = null;
      state.isPlaying = false;
    },
  },
});

export const { setCurrentSong, togglePlay, setVolume, stopSong } = songSlice.actions;
export default songSlice.reducer;