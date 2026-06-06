import AppLayout from '../components/AppLayout'
import styles from './HomePage.module.css'

export default function HomePage() {
    return (
        <AppLayout>
            <div className={styles.cards}>
                <div className={styles.card}>
                    <span className={styles.cardLabel}>Upcoming appointments</span>
                    <span className={styles.cardValue}>—</span>
                </div>
                <div className={styles.card}>
                    <span className={styles.cardLabel}>My animals</span>
                    <span className={styles.cardValue}>—</span>
                </div>
            </div>

            <div className={styles.section}>
                <h3>Upcoming appointments</h3>
                <p className={styles.empty}>You have no upcoming appointments.</p>
            </div>
        </AppLayout>
    )
}