import type { PayloadAction } from '@reduxjs/toolkit';
import { createSlice } from '@reduxjs/toolkit';

interface SongState {
  currentSong: any | null;      // השיר שמתנגן כרגע
  currentPlaylist: any[];      // רשימת השירים הנוכחית (התוספת הקריטית!)
  isPlaying: boolean;          // האם הנגן פעיל
  volume: number;              // עוצמת שמע (0 עד 1)
}

const initialState: SongState = {
  currentSong: null,
  currentPlaylist: [],         // מאותחל כמערך ריק
  isPlaying: false,
  volume: 0.7,
};

export const songSlice = createSlice({
  name: 'song',
  initialState,
  reducers: {
    // הגדרת השיר הנוכחי
    setCurrentSong: (state, action: PayloadAction<any>) => {
      state.currentSong = action.payload;
      state.isPlaying = true;
    },
    // עדכון רשימת השירים כולה (כדי שיהיה אפשר לעבור קדימה/אחורה)
    setCurrentPlaylist: (state, action: PayloadAction<any[]>) => {
      state.currentPlaylist = action.payload;
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
      state.currentPlaylist = [];
      state.isPlaying = false;
    },
  },
});

export const { setCurrentSong, setCurrentPlaylist, togglePlay, setVolume, stopSong } = songSlice.actions;
export default songSlice.reducer;