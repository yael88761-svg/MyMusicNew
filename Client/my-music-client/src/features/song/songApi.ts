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
  tagTypes: ['Playlists'],
  endpoints: (builder) => ({
    // הגדרה אחת ומדויקת של הפעולה
    uploadSong: builder.mutation<any, { formData: FormData; playlistId: string }>({
      query: ({ formData }) => ({
        url: 'Song/upload-music',
        method: 'POST',
        body: formData,
      }),
      // ריענון הפלייליסט הספציפי שבו הוסף השיר
      invalidatesTags: (result, error, { playlistId }) => [
        { type: 'Playlists', id: playlistId }
      ],
    }),
  }),
});

export const { useUploadSongMutation } = songApi;