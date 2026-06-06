import { useNavigate } from 'react-router-dom'
import AppLayout from '../components/AppLayout'
import styles from './AppointmentsPage.module.css'

export default function AppointmentsPage() {
    const navigate = useNavigate()

    return (
        <AppLayout>
            <div className={styles.panel}>
                <div className={styles.panelHeader}>
                    <h2>My Appointments</h2>
                </div>

                <div className={styles.emptyState}>
                    <span className={styles.emptyText}>You have no appointments yet.</span>
                </div>
            </div>

            <button className={styles.fab} onClick={() => navigate('/appointments/schedule')}>
                + Schedule appointment
            </button>
        </AppLayout>
    )
}