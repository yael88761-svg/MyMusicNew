import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

export const userApi = createApi({
  reducerPath: 'userApi',
  baseQuery: fetchBaseQuery({ baseUrl: 'http://localhost:5270/api' }), 
  endpoints: (builder) => ({
    login: builder.mutation({
      query: (credentials) => ({
        url: '/User/login', // ודאי ב-Swagger שזה הנתיב
        method: 'POST',
        body: credentials,
      }),
    }),
    signup: builder.mutation({
      query: (newUser) => ({
        url: '/User/register', // ודאי ב-Swagger שזה הנתיב
        method: 'POST',
        body: newUser,
      }),
    }),
  }),
});

// כאן נוצרים ה-Hooks שאת מייבאת ב-Login
export const { useLoginMutation, useSignupMutation } = userApi;