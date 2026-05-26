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
    
    // שליפת פלייליסט "נוספו לאחרונה"
    getRecentPlaylist: builder.query<any, void>({
      query: () => 'Playlist/recent-playlist',
      // שימוש בתגית כדי לאפשר רענון אוטומטי כששיר חדש מועלה
      providesTags: [{ type: 'Playlists', id: 'RECENT' }],
    }),

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
    
    //create playlist
    createPlaylist: builder.mutation<any, { playlistName: string }>({
      query: (newPlaylist) => ({
        url: 'Playlist',
        method: 'POST',
        body: newPlaylist,
      }),
      invalidatesTags: [{ type: 'Playlists', id: 'LIST' }],
    }),

// Upload a song and link it to a specific playlist
    uploadSongToPlaylist: builder.mutation<any, { file: File; playlistId: number }>({
      query: ({ file, playlistId }) => {
        const formData = new FormData();
        formData.append('file', file);

        return {
          url: `Song/upload-music/${playlistId}`,
          method: 'POST',
          body: formData,
        };
      },
      invalidatesTags: (result, error, { playlistId }) => [
        { type: 'Playlists', id: playlistId },
        { type: 'Playlists', id: 'LIST' },
       // { type: 'Playlists', id: 'RECENT' }
      ],
    }),

    //update plylist
    updatePlaylist: builder.mutation<any, { id: number; data: any }>({
      query: ({ id, data }) => ({
        url: `Playlist/${id}`,
        method: 'PUT',
        body: data,
      }),
      invalidatesTags: (result, error, { id }) => [{ type: 'Playlists', id }],
    }),

    // delete playlist
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
  useGetRecentPlaylistQuery, 
  useCreatePlaylistMutation,
  useUploadSongToPlaylistMutation,
  useUpdatePlaylistMutation,
  useDeletePlaylistMutation 
} = playlistApi;