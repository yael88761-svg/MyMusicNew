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
      // 1. Get all songs of the logged in user 
      getSongs: builder.query<any[], void>({
      query: () => 'Song/my-songs',
      providesTags: ['Songs'],
    }),

    uploadSong: builder.mutation<any, { formData: FormData; playlistId: string }>({
      query: ({ formData, playlistId }) => ({
        url: `Song/upload-music/${playlistId}`,
        method: 'POST',
        body: formData,
      }),
      invalidatesTags: (result, error, { playlistId }) => [
        { type: 'Playlists', id: playlistId },
        { type: 'Songs' }
      ],
    }),
  }),
});

export const { useGetSongsQuery, useUploadSongMutation } = songApi;