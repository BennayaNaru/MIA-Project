import LandingPage from "./pages/LandingPage";
import AuthPage from "./pages/LoginPage"; 
import { Navigate } from "react-router-dom";
import CariSekolahPage from './pages/CariSekolahPage';
import BandingkanPage from './pages/BandingkanPage';
import TentangDataPage from './pages/TentangDataPage'

const AppRoutes = [
  { index: true,
    element: <LandingPage />
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
