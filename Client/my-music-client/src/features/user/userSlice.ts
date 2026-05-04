import { createSlice } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';
// ייבוא ה-API כדי שנוכל לאפס אותו
import { playlistApi } from '../playlist/playlistApi';
import { songApi } from '../song/songApi';

interface UserState {
  currentUser: any | null;
  token: string | null;
}

const initialState: UserState = {
  currentUser: JSON.parse(localStorage.getItem('user') || 'null'),
  token: localStorage.getItem('token') || null,
};

const userSlice = createSlice({
  name: 'user',
  initialState,
  reducers: {
    loginSuccess: (state, action: PayloadAction<{ user: any; token: string }>) => {
      state.currentUser = action.payload.user;
      state.token = action.payload.token;
      localStorage.setItem('user', JSON.stringify(action.payload.user));
      localStorage.setItem('token', action.payload.token);
    },
    logout: (state) => {
      state.currentUser = null;
      state.token = null;
      localStorage.removeItem('user');
      localStorage.removeItem('token');
      // הערה: את ה-resetApiState נפעיל בדרך כלל בתוך ה-Component או ב-Middleware
      // כי Slice לא אמור להכיל לוגיקה של API ישירות בתוך ה-Reducer
    },
  },
  // שימוש ב-extraReducers כדי להגיב לאיפוס מצב כללי אם תרצי בעתיד
});

export const { loginSuccess, logout } = userSlice.actions;
export default userSlice.reducer;