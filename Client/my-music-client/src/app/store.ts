import { configureStore } from '@reduxjs/toolkit';
import { userApi } from '../features/user/userApi';
import userReducer from '../features/user/userSlice';
import { playlistApi } from '../features/playlist/playlistApi';
import songReducer from '../features/song/songSlice'; 
import { songApi } from '../features/song/songApi';

export const store = configureStore({
  reducer: {
    user: userReducer,
    song: songReducer, 
    
    [userApi.reducerPath]: userApi.reducer,
    [playlistApi.reducerPath]: playlistApi.reducer,
    [songApi.reducerPath]: songApi.reducer,
  },
  
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware()
      .concat(userApi.middleware)
      .concat(playlistApi.middleware)
      .concat(songApi.middleware), 
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;