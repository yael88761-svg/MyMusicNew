import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import { useSignupMutation } from './userApi';
import { loginSuccess } from './userSlice';
import { playlistApi } from '../playlist/playlistApi';
import { songApi } from '../song/songApi';
const Signup: React.FC = () => {
  const [name, setName] = useState(''); // שינוי ל-name כדי להתאים ל-Controller
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  
  const [signup, { isLoading }] = useSignupMutation();
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const handleSignup = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      // 1. ביצוע ההרשמה מול השרת
      const response = await signup({ name, email, password }).unwrap();
      
      // 2. ניקוי שאריות מה-Cache (ליתר ביטחון אם היה משתמש קודם ללא רענון דף)
      dispatch(playlistApi.util.resetApiState());
      dispatch(songApi.util.resetApiState());

      // 3. שמירת המשתמש החדש והטוקן ב-Store
      dispatch(loginSuccess({ user: response, token: response.token }));
      
      // 4. מעבר אוטומטי לספריה
      navigate('/library');
    } catch (err) {
      console.error('Signup error:', err);
      alert('שגיאה ביצירת החשבון, ייתכן שהאימייל כבר קיים במערכת');
    }
  };

  return (
    <div className="auth-container">
      <h2>יצירת חשבון חדש</h2>
      <form onSubmit={handleSignup}>
        <input 
          placeholder="שם משתמש" 
          required
          onChange={(e) => setName(e.target.value)} 
        />
        <input 
          type="email" 
          placeholder="אימייל" 
          required
          onChange={(e) => setEmail(e.target.value)} 
        />
        <input 
          type="password" 
          placeholder="סיסמה" 
          required
          onChange={(e) => setPassword(e.target.value)} 
        />
        <button type="submit" disabled={isLoading}>
          {isLoading ? 'יוצר חשבון...' : 'הרשמה'}
        </button>
      </form>
    </div>
  );
};

export default Signup;