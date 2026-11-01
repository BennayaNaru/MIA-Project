import { Counter } from "./components/Counter";
import { Weather } from "./components/Weather";
import { Tasks } from "./components/Todo";
import LandingPage from "./pages/LandingPage";
import AuthPage from "./pages/LoginPage"; 
import { Navigate } from "react-router-dom";
import { ProtectedRoute } from "./components/api-authorization/ProtectedRoute";
import CariSekolahPage from './pages/CariSekolahPage';
import BandingkanPage from './pages/BandingkanPage';
import TentangDataPage from './pages/TentangDataPage'

const AppRoutes = [
  { index: true,
    element: <LandingPage />
  },

  {
    path: '/counter',
    element: <Counter />
  },

  {
    path: '/weather',
    element: <ProtectedRoute><Weather /></ProtectedRoute>
  },

  {
    path: '/todo',
    element: <ProtectedRoute><Tasks /></ProtectedRoute>
  },

  { path: '/cari', 
    element: <CariSekolahPage /> 
  },

  { path: '/bandingkan',
     element: <BandingkanPage />
  },

  { path: '/tentang-data',
     element: <TentangDataPage />
  },

  {
  path: '/masuk',
  element: <AuthPage />
  },

  

  {
  path: '/register',
  element: <Navigate to="/masuk?mode=daftar" replace />
  }
];

export default AppRoutes;
