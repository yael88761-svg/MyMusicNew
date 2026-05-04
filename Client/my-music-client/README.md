# 🎵 My Music Client

A modern, responsive music streaming client application built with React, TypeScript, and Redux. Stream, manage, and organize your favorite music with an intuitive user interface.

---

## ✨ Features

- **User Authentication** - Secure login and signup functionality
- **Music Library** - Browse and manage your music collection
- **Music Player** - Built-in audio player with playback controls
- **Playlists** - Create and manage custom playlists
- **Responsive Design** - Works seamlessly across desktop and mobile devices
- **Modern UI** - Material-UI components for a polished user experience
- **State Management** - Redux Toolkit for predictable state management

---

## 🛠️ Technology Stack

### Core Technologies
- **React** (v19.2.4) - UI library
- **TypeScript** (v6.0.2) - Type-safe JavaScript
- **Vite** (v8.0.4) - Fast build tool and dev server

### State Management & Routing
- **Redux Toolkit** (v2.11.2) - State management
- **React-Redux** (v9.2.0) - React bindings for Redux
- **React Router DOM** (v7.14.1) - Client-side routing

### UI & Styling
- **Material-UI (MUI)** (v9.0.0) - UI component library
- **MUI Icons** (v9.0.0) - Icon library
- **Emotion** (v11.14.0+) - CSS-in-JS styling
- **Lucide React** (v1.8.0) - Icon library

### Form Handling
- **React Hook Form** (v7.72.1) - Efficient form state management

### Development Tools
- **ESLint** (v9.39.4) - Code linting
- **TypeScript ESLint** - TypeScript-specific linting

---

## 📁 Project Structure

```
my-music-client/
├── src/
│   ├── app/
│   │   └── store.ts              # Redux store configuration
│   ├── components/
│   │   ├── Navbar.tsx            # Navigation component
│   │   ├── Navbar.css
│   │   ├── LibraryPage.tsx        # Music library display
│   │   ├── LibraryPage.css
│   │   └── MusicPlayer.tsx        # Audio player component
│   ├── features/
│   │   ├── playlist/
│   │   │   ├── playlistApi.ts     # Playlist API calls
│   │   │   └── playlistSlice.ts   # Playlist Redux slice
│   │   ├── song/
│   │   │   ├── songApi.ts         # Song API calls
│   │   │   └── songSlice.ts       # Song Redux slice
│   │   └── user/
│   │       ├── login.tsx          # Login page
│   │       ├── signup.tsx         # Signup page
│   │       ├── userApi.ts         # User API calls
│   │       └── userSlice.ts       # User Redux slice
│   ├── assets/                    # Static assets
│   ├── App.tsx                    # Main app component
│   ├── App.css
│   ├── main.tsx                   # Entry point
│   └── index.css
├── public/                        # Public static files
├── vite.config.ts                 # Vite configuration
├── tsconfig.json                  # TypeScript base config
├── tsconfig.app.json              # TypeScript app config
├── tsconfig.node.json             # TypeScript node config
├── eslint.config.js               # ESLint configuration
├── index.html                     # HTML entry point
├── package.json                   # Dependencies & scripts
└── README.md                      # This file
```

---

## 🚀 Getting Started

### Prerequisites
- **Node.js** (v16 or higher)
- **npm** or **yarn** package manager

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd my-music-client
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Start the development server**
   ```bash
   npm run dev
   ```
   The application will be available at `http://localhost:5173`

---

## 📜 Available Scripts

### Development
```bash
npm run dev
```
Starts the Vite development server with hot module replacement.

### Build
```bash
npm run build
```
Compiles TypeScript and builds the project for production.

### Preview
```bash
npm run preview
```
Locally preview the production build.

### Lint
```bash
npm run lint
```
Run ESLint to check code quality and style.

---

## 🎯 Application Routes

| Route | Component | Description |
|-------|-----------|-------------|
| `/` | LibraryPage | Main music library view |
| `/library` | LibraryPage | Music library (alternative route) |
| `/login` | Login | User login page |
| `/signup` | Signup | User registration page |
| `*` | - | Redirects to home page |

---

## 🏗️ Architecture

### Redux Store Structure

The application uses Redux Toolkit for state management with the following slices:

- **userSlice** - User authentication and profile data
- **songSlice** - Songs and audio data
- **playlistSlice** - Playlist management

### API Integration

Each feature module includes dedicated API handlers:
- `userApi.ts` - Authentication and user endpoints
- `songApi.ts` - Music and song endpoints
- `playlistApi.ts` - Playlist endpoints

---

## 🎨 UI Components

### Navbar
Main navigation component displayed at the top of the application.

### LibraryPage
Displays the user's music library with search, filtering, and playlist management.

### MusicPlayer
Audio player component with playback controls, volume adjustment, and progress tracking.

---

## 🔐 Authentication

The application includes user authentication with:
- User registration (Signup page)
- User login (Login page)
- Session management through Redux store

---

## 📝 Code Quality

- **TypeScript** - Full type safety throughout the codebase
- **ESLint** - Code style and quality checks
- **React Best Practices** - Functional components with hooks

---

## 🤝 Contributing

To contribute to this project:

1. Create a feature branch (`git checkout -b feature/amazing-feature`)
2. Commit your changes (`git commit -m 'Add amazing feature'`)
3. Push to the branch (`git push origin feature/amazing-feature`)
4. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

---

## 📧 Support

For questions, issues, or suggestions, please create an issue in the repository or contact the development team.

---

**Happy Listening! 🎶**
