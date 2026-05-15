import React from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { Link, useNavigate } from 'react-router-dom';
import type { RootState } from '../app/store.ts';
import { logout } from '../features/user/userSlice';

// ייבוא ה-APIs לניקוי ה-Cache
import { playlistApi } from '../features/playlist/playlistApi';
import { songApi } from '../features/song/songApi';

import './Navbar.css';

interface NavbarProps {
  onLogout: () => void;
}

const Navbar: React.FC<NavbarProps> = ({ onLogout }) => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  
  // שליפת המשתמש מה-Store
  const { currentUser } = useSelector((state: RootState) => state.user);

  const handleLogout = () => {
    // 1. עדכון ה-State של המשתמש ב-Redux (מנקה טוקן ופרטים)
    dispatch(logout());

    // 2. ניקוי ה-Cache של הנתונים כדי שלא יישארו פלייליסטים בזיכרון
    dispatch(playlistApi.util.resetApiState());
    dispatch(songApi.util.resetApiState());

    // 3. עדכון ה-App שהתנתקנו (יעלים את ה-MusicPlayer ב-App.tsx)
    onLogout();

    // 4. הפניה לדף הלוגין
    navigate('/login');
  };

  return (
    <nav className="smart-player-nav">
      <div className="nav-logo">
        <Link to="/">🎵 SmartPlayer</Link>
      </div>

      <div className="nav-links">
        <Link to="/library">ספריה</Link>
        <Link to="/trending">פופולארי</Link>
      </div>

      <div className="nav-auth">
        {currentUser ? (
          <div className="user-profile">
            <span>שלום, <strong>{currentUser.name}</strong></span>
            <button onClick={handleLogout} className="logout-btn">התנתק</button>
          </div>
        ) : (
          <div className="auth-buttons">
            <Link to="/login" className="login-link">התחברות</Link>
            <Link to="/signup" className="signup-btn">הרשמה</Link>
          </div>
        )}
      </div>
    </nav>
  );
};

export default Navbar;