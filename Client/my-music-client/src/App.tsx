import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Navbar from './components/Navbar';
import Login from './features/user/login';
import Signup from './features/user/signup';
import LibraryPage from "./components/Library/LibraryPage";
import MusicPlayer from "./components/Library/Player/MusicPlayer";
import './App.css';

function App() {
  return (
    <Router>
      {/* ה-app-container עוטף את הכל כדי לאפשר עיצוב של מסך מלא */}
      <div className="app-container">
        
        {/* Navbar עליון - נשאר קבוע */}
        <Navbar />

        {/* האזור המרכזי שמשתנה לפי הנתיב (Route) */}
        <main className="content">
          <Routes>
            {/* דף הבית והספרייה */}
            <Route path="/" element={<LibraryPage />} />
            <Route path="/library" element={<LibraryPage />} />

            {/* דפי משתמש */}
            <Route path="/login" element={<Login />} />
            <Route path="/signup" element={<Signup />} />

            {/* ניתוב למקרה של דף לא קיים */}
            <Route path="*" element={<Navigate to="/" />} />
          </Routes>
        </main>

        {/* כאן הוספנו את הקומפוננטה החדשה. 
            במקום עשרות שורות של קוד הנגן שהיו קודם בתוך ה-LibraryPage, 
            הן עכשיו נמצאות בתוך הקובץ MusicPlayer.tsx 
        */}
        <MusicPlayer />
        
      </div>
    </Router>
  );
}

export default App;