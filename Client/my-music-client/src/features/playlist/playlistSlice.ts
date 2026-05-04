createPlaylist: builder.mutation({
  query: (newPlaylist) => ({
    url: '/Playlist',
    method: 'POST',
    body: newPlaylist, // צריך להכיל playlistName ו-userId
  }),
  invalidatesTags: ['Playlists'], // גורם לריאקט לרענן את הרשימה אוטומטית
}),