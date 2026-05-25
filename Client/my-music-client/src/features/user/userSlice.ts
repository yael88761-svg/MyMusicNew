import { createSlice } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';
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
    },
  },
});

export const { loginSuccess, logout } = userSlice.actions;
export default userSlice.reducer;