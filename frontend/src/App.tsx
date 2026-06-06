import { BrowserRouter, Routes, Route } from 'react-router-dom'
import Layout from './components/Layout.tsx'
import LoginPage from './pages/LoginPage.tsx'
import HomePage from './pages/HomePage.tsx'
import AppointmentsPage from './pages/AppointmentsPage.tsx'
import AnimalsPage from './pages/AnimalsPage.tsx'
import AddAnimalPage from './pages/AddAnimalPage.tsx'
import ScheduleAppointmentPage from './pages/ScheduleAppointmentPage.tsx'

function App() {
  return (
    <BrowserRouter>
      <Layout>
        <Routes>
          <Route path="/"             element={<LoginPage />} />
          <Route path="/home"         element={<HomePage />} />
          <Route path="/appointments" element={<AppointmentsPage />} />
          <Route path="/animals"      element={<AnimalsPage />} />
          <Route path="/animals/add"          element={<AddAnimalPage />} />
          <Route path="/appointments/schedule" element={<ScheduleAppointmentPage />} />
        </Routes>
      </Layout>
    </BrowserRouter>
  )
}

export default App