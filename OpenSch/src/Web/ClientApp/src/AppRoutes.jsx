import LandingPage from "./pages/LandingPage";
import AuthPage from "./pages/LoginPage"; 
import { Navigate } from "react-router-dom";
import CariSekolahPage from './pages/CariSekolahPage';
import PeringkatPage from './pages/PeringkatPage';
import BandingkanPage from './pages/BandingkanPage';
import TentangDataPage from './pages/TentangDataPage'
import AdminImportPage from './pages/AdminImportPage'
import { ProtectedRoute } from './components/api-authorization/ProtectedRoute'

const AppRoutes = [
  { index: true,
    element: <LandingPage />
  },





  { path: '/cari', 
    element: <CariSekolahPage /> 
  },

  { path: '/peringkat',
    element: <PeringkatPage />
  },

  { path: '/bandingkan',
     element: <BandingkanPage />
  },

  { path: '/tentang-data',
     element: <TentangDataPage />
  },

  {
  path: '/admin/impor',
  element: (
    <ProtectedRoute>
      <AdminImportPage />
    </ProtectedRoute>
  )
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
