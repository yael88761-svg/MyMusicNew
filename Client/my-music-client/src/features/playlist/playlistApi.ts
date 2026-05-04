import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

export const playlistApi = createApi({
  reducerPath: 'playlistApi',
  baseQuery: fetchBaseQuery({ 
    // הוספת לוכסן בסוף ה-baseUrl לדיוק בנתיבים
    baseUrl: 'http://localhost:5270/api/', 
    prepareHeaders: (headers) => {
      // שליפת הטוקן מהאחסון המקומי
      const token = localStorage.getItem('token');
      
      // אם קיים טוקן, הזרקתו ל-Header תחת Authorization בפורמט Bearer
      if (token) {
        headers.set('authorization', `Bearer ${token}`);
      }
      return headers;
    },
  }),
  tagTypes: ['Playlists'],
  endpoints: (builder) => ({
    // שליפת הפלייליסטים של המשתמש המחובר
    getPlaylists: builder.query<any, void>({
      query: () => 'Playlist/my-playlists',
      providesTags: ['Playlists'],
    }),
    
    // יצירת פלייליסט חדש
    createPlaylist: builder.mutation({
      query: (newPlaylist) => ({
        url: 'Playlist',
        method: 'POST',
        body: newPlaylist,
      }),
      invalidatesTags: ['Playlists'],
    }),
          // בתוך playlistApi.ts
      getPlaylistById: builder.query<any, string>({
        query: (id) => `Playlist/${id}`,
        providesTags: (result, error, id) => [{ type: 'Playlists', id }],
      }),
  }),
});

// ייצוא ה-Hooks לשימוש בקומפוננטות
export const { useGetPlaylistsQuery, useCreatePlaylistMutation } = playlistApi;