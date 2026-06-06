import AppLayout from '../components/AppLayout'
import styles from './AppointmentsPage.module.css'

export default function AppointmentsPage() {
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

            <button className={styles.fab}>+ Schedule appointment</button>
        </AppLayout>
    )
}