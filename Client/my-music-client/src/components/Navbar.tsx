import React from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { Link, useNavigate } from 'react-router-dom';
import type { RootState } from '../app/store.ts';
import { logout } from '../features/user/userSlice';

// הוספת הייבוא של ה-APIs כדי לאפשר את ניקוי הזיכרון (Cache)
import { playlistApi } from '../features/playlist/playlistApi';
import { songApi } from '../features/song/songApi';

import './Navbar.css';

const Navbar: React.FC = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  
  // שליפת המשתמש מה-Store
  const { currentUser } = useSelector((state: RootState) => state.user);

  const handleLogout = () => {
    // 1. עדכון ה-State של המשתמש (מחיקת טוקן ופרטי משתמש)
    dispatch(logout());

    // 2. ניקוי מוחלט של כל הנתונים השמורים ב-API
    // זה מה שיגרום לרשימת הפלייליסטים ב-Sidebar להיעלם מיד
    dispatch(playlistApi.util.resetApiState());
    dispatch(songApi.util.resetApiState());

    // 3. הפניה לדף הבית או להתחברות
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
            <span>שלום, <strong>{currentUser.userName}</strong></span>
            {/* שינוי הקריאה ל-handleLogout המעודכן */}
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