import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Navbar from './components/Navbar';
import Login from './features/user/login';
import Signup from './features/user/signup';
import LibraryPage from './components/LibraryPage'; // 1. ייבוא הקומפוננטה החדשה
import './App.css';

function App() {
  return (
    <Router>
      <div className="app-container">
        <Navbar />

        <main className="content">
          <Routes>
            {/* 2. עדכון דף הבית שיציג את הספרייה */}
            <Route path="/" element={<LibraryPage />} />
            
            {/* 3. הוספת נתיב ייעודי לספרייה (אופציונלי) */}
            <Route path="/library" element={<LibraryPage />} />

            <Route path="/login" element={<Login />} />
            <Route path="/signup" element={<Signup />} />

            <Route path="*" element={<Navigate to="/" />} />
          </Routes>
        </main>
      </div>
    </Router>
  );
}

export default App;