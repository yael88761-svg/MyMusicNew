import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

export const playlistApi = createApi({
  reducerPath: 'playlistApi',
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
  tagTypes: ['Playlists'],
  endpoints: (builder) => ({
    
    // שליפת כל הפלייליסטים של המשתמש
    getPlaylists: builder.query<any[], void>({
      query: () => 'Playlist/my-playlists',
      providesTags: (result) =>
        result
          ? [
              ...result.map(({ playlistId }) => ({ type: 'Playlists' as const, id: playlistId })),
              { type: 'Playlists', id: 'LIST' },
            ]
          : [{ type: 'Playlists', id: 'LIST' }],
    }),

    // שליפת פלייליסט ספציפי
    getPlaylistById: builder.query<any, number | string>({
      query: (id) => `Playlist/${id}`,
      providesTags: (result, error, id) => [{ type: 'Playlists', id }],
    }),
    
    // יצירת פלייליסט חדש
    createPlaylist: builder.mutation<any, { playlistName: string; isSmartPlaylist: boolean }>({
      query: (newPlaylist) => ({
        url: 'Playlist',
        method: 'POST',
        body: newPlaylist,
      }),
      invalidatesTags: [{ type: 'Playlists', id: 'LIST' }],
    }),

    // העלאת שיר וקישורו לפלייליסט ספציפי
    uploadSongToPlaylist: builder.mutation<any, { file: File; playlistId: number }>({
      query: ({ file, playlistId }) => {
        const formData = new FormData();
        formData.append('file', file);

        return {
          url: `Song/upload-music/${playlistId}`, // הכתובת המעודכנת ב-Backend
          method: 'POST',
          body: formData,
          // RTK Query מזהה אוטומטית FormData ומגדיר Content-Type מתאים
        };
      },
      // גורם לרענון הפלייליסט הספציפי כדי שהשיר החדש יופיע מיד
      invalidatesTags: (result, error, { playlistId }) => [
        { type: 'Playlists', id: playlistId },
        { type: 'Playlists', id: 'LIST' }
      ],
    }),

    // עדכון פלייליסט קיים
    updatePlaylist: builder.mutation<any, { id: number; data: any }>({
      query: ({ id, data }) => ({
        url: `Playlist/${id}`,
        method: 'PUT',
        body: data,
      }),
      invalidatesTags: (result, error, { id }) => [{ type: 'Playlists', id }],
    }),

    // מחיקת פלייליסט
    deletePlaylist: builder.mutation<void, number>({
      query: (id) => ({
        url: `Playlist/${id}`,
        method: 'DELETE',
      }),
      invalidatesTags: [{ type: 'Playlists', id: 'LIST' }],
    }),
  }),
});

export const { 
  useGetPlaylistsQuery, 
  useGetPlaylistByIdQuery,
  useCreatePlaylistMutation,
  useUploadSongToPlaylistMutation, // ה-Hook החדש להעלאת שירים
  useUpdatePlaylistMutation,
  useDeletePlaylistMutation 
} = playlistApi;