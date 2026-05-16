import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

export const songApi = createApi({
  reducerPath: 'songApi',
  baseQuery: fetchBaseQuery({
    baseUrl: 'http://localhost:5270/api/',
    prepareHeaders: (headers) => {
      const token = localStorage.getItem('token');
      if (token) {
        headers.set('authorization', `Bearer ${token}`);
      }
      return headers;
    },
  }),
  tagTypes: ['Playlists', 'Songs'],
  
  endpoints: (builder) => ({
    // 1. קבלת כל השירים של המשתמש המחובר (לפי ה-Swagger)
    getSongs: builder.query<any[], void>({
      query: () => 'Song/my-songs',
      providesTags: ['Songs'],
    }),

    // 2. העלאת שיר לפלייליסט מסוים - ה-playlistId עובר כעת בתוך ה-URL
    uploadSong: builder.mutation<any, { formData: FormData; playlistId: string }>({
      query: ({ formData, playlistId }) => ({
        url: `Song/upload-music/${playlistId}`,
        method: 'POST',
        body: formData,
      }),
      // מרענן גם את הפלייליסט הספציפי וגם את רשימת כל השירים בספרייה הכללית
      invalidatesTags: (result, error, { playlistId }) => [
        { type: 'Playlists', id: playlistId },
        { type: 'Songs' }
      ],
    }),
  }),
});

export const { useGetSongsQuery, useUploadSongMutation } = songApi;