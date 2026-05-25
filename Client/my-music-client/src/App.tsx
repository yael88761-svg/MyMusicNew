import React, { useState } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { useSelector } from 'react-redux'; // ייבוא ה-Hook לשליפת נתונים מרידאקס
import type { RootState } from './app/store'; // וודאי שהנתיב לסטור נכון

import Navbar from './components/Navbar';
import Login from './features/user/login';
import Signup from './features/user/signup';
import LibraryPage from "./components/Library/LibraryPage";
import MusicPlayer from "./components/Library/Player/MusicPlayer";
import PlaylistList from './components/Library/Sidebar/PlaylistList';
import RecentPlaylistView from '../src/components/Library/Sidebar/RecentPlaylistView';
import ProtectedRoute from './components/ProtectedRoute';
import './App.css';

function App() {
  const [token, setToken] = useState<string | null>(localStorage.getItem('token'));

  // שליפת הדגל שהוספנו ל-songSlice
  const { hasStartedPlaying } = useSelector((state: RootState) => state.song);

  const updateToken = () => {
    setToken(localStorage.getItem('token'));
  };

  return (
    <Router>
      <div className="app-container">
        
        <Navbar onLogout={updateToken} />

        <main className="content">
          <Routes>
            <Route path="/login" element={<Login onLoginSuccess={updateToken} />} />
            <Route path="/signup" element={<Signup onLoginSuccess={updateToken} />} />

            <Route path="/" element={
              <ProtectedRoute>
                <LibraryPage />
              </ProtectedRoute>
            } />
            
            <Route path="/library" element={
              <ProtectedRoute>
                <LibraryPage />
              </ProtectedRoute>
            } />

            <Route path="/playlist/:id" element={
              <ProtectedRoute>
                <PlaylistList />
              </ProtectedRoute>
            } />
            
            {/* <Route path="/playlist/recent" element={
              <ProtectedRoute>
                <RecentPlaylistView />
              </ProtectedRoute>
            } />
 */}
            <Route path="*" element={<Navigate to="/" />} />
          </Routes>
        </main>

        {/* השינוי המרכזי כאן:
            הנגן יוצג רק אם:
            1. יש טוקן (המשתמש מחובר)
            2. hasStartedPlaying הוא true (המשתמש בחר שיר לפחות פעם אחת)
        */}
        {token && hasStartedPlaying && <MusicPlayer />}
        
      </div>
    </Router>
  );
}

export default App;