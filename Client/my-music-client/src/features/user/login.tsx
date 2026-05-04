import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { useNavigate } from 'react-router-dom'; // ייבוא ה-Hook לניווט
import { useLoginMutation } from './userApi';
import { loginSuccess } from './userSlice';

const Login: React.FC = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [login, { isLoading, error }] = useLoginMutation();
  const dispatch = useDispatch();
  const navigate = useNavigate(); // אתחול פונקציית הניווט

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      // ביצוע פעולת ההתחברות מול השרת
      const response = await login({ email, password }).unwrap();
      
      // שמירת פרטי המשתמש והטוקן ב-Redux וב-LocalStorage
      dispatch(loginSuccess({ user: response, token: response.token }));
      
      // מעבר אוטומטי לדף הפלייליסטים
      navigate('/library'); 
    } catch (err) {
      console.error('Failed to login:', err);
    }
  };

  return (
    <div className="login-container">
      <h2>התחברות למערכת</h2>
      <form onSubmit={handleLogin}>
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
          {isLoading ? 'מתחבר...' : 'כניסה'}
        </button>
        {error && <p className="error-message">טעות בפרטים, נסו שוב</p>}
      </form>
    </div>
  );
};

export default Login;