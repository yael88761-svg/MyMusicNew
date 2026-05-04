// 1. ייבוא ספריות (במידת הצורך)
import React from 'react';
import './MusicPlayer.css'; // אם תרצה עיצוב נפרד

// 2. הגדרת הפונקציה (שם הקומפוננטה תמיד מתחיל באות גדולה!)
const MusicPlayer = () => {
  
  // כאן אפשר לכתוב לוגיקה (משתנים, פונקציות וכו')
  const songTitle = "שיר לדוגמה";

  // 3. ה-return - מה שיוצג על המסך
  return (
    <div className="player-bar">
      <p>מנגן עכשיו: {songTitle}</p>
      <audio controls>
        <source src="link-to-audio.mp3" type="audio/mpeg" />
      </audio>
    </div>
  );
};

// 4. ייצוא הקומפוננטה כדי שנוכל להשתמש בה במקומות אחרים
export default MusicPlayer;