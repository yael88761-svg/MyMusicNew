createPlaylist: builder.mutation({
  query: (newPlaylist) => ({
    url: '/Playlist',
    method: 'POST',
    body: newPlaylist,
  }),
  invalidatesTags: ['Playlists'], 
  // songSlice.ts
}
}),