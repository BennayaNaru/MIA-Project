import { Routes, Route } from 'react-router-dom'
import AppRoutes from './AppRoutes'
import { AuthProvider } from './components/api-authorization/AuthContext'
import { ThemeProvider } from './components/ThemeContext'
import { CompareProvider } from './context/CompareContext'
import DemoDataBadge from './components/common/DemoDataBadge'
import Navbar from './components/layout/Navbar'
import Footer from './components/layout/Footer'
import CompareDock from './components/schools/CompareDock'
import SchoolDetailModal from './components/schools/SchoolDetailModal'

export default function App() {
  return (
    <ThemeProvider>
      <AuthProvider>
        <CompareProvider>
          <div className="flex min-h-screen flex-col">
            <DemoDataBadge />
            <Navbar />
            <main className="flex-grow">
              <Routes>
                {AppRoutes.map((route, index) => {
                  const { element, ...rest } = route
                  return <Route key={index} {...rest} element={element} />
                })}
              </Routes>
            </main>
            <Footer />
          </div>
          <CompareDock />
          <SchoolDetailModal />
        </CompareProvider>
      </AuthProvider>
    </ThemeProvider>
  )
}