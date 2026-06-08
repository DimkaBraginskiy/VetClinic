import { BrowserRouter, Routes, Route } from 'react-router-dom'
import Layout from './components/Layout.tsx'
import LoginPage from './pages/LoginPage.tsx'
import HomePage from './pages/HomePage.tsx'
import AppointmentsPage from './pages/appointments/AppointmentsPage.tsx'
import AppointmentDetailPage from './pages/appointments/AppointmentDetailPage.tsx'
import AnimalsPage from './pages/animals/AnimalsPage.tsx'
import AnimalDetailPage from './pages/animals/AnimalDetailPage.tsx'
import AddAnimalPage from './pages/animals/AddAnimalPage.tsx'
import ScheduleAppointmentPage from './pages/appointments/ScheduleAppointmentPage.tsx'
import VetDashboardPage from './pages/veterinarian/VetDashboardPage.tsx'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/vet" element={<VetDashboardPage />} />
        <Route path="*" element={
          <Layout>
            <Routes>
              <Route path="/"             element={<LoginPage />} />
              <Route path="/home"         element={<HomePage />} />
              <Route path="/appointments" element={<AppointmentsPage />} />
              <Route path="/animals"      element={<AnimalsPage />} />
              <Route path="/animals/add"          element={<AddAnimalPage />} />
              <Route path="/animals/:id"          element={<AnimalDetailPage />} />
              <Route path="/appointments/schedule" element={<ScheduleAppointmentPage />} />
              <Route path="/appointments/:id"     element={<AppointmentDetailPage />} />
            </Routes>
          </Layout>
        } />
      </Routes>
    </BrowserRouter>
  )
}

export default App