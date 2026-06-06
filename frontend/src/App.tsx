import { BrowserRouter, Routes, Route } from 'react-router-dom'
import Layout from './components/Layout.tsx'
import LoginPage from './pages/LoginPage.tsx'
import HomePage from './pages/HomePage.tsx'
import AppointmentsPage from './pages/AppointmentsPage.tsx'
import AnimalsPage from './pages/AnimalsPage.tsx'

function App() {
  return (
    <BrowserRouter>
      <Layout>
        <Routes>
          <Route path="/"             element={<LoginPage />} />
          <Route path="/home"         element={<HomePage />} />
          <Route path="/appointments" element={<AppointmentsPage />} />
          <Route path="/animals"      element={<AnimalsPage />} />
        </Routes>
      </Layout>
    </BrowserRouter>
  )
}

export default App