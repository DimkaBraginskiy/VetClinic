import AppLayout from '../components/AppLayout'
import styles from './AnimalsPage.module.css'

export default function AnimalsPage() {
    return (
        <AppLayout>
            <div className={styles.panel}>
                <div className={styles.panelHeader}>
                    <h2>My Animals</h2>
                </div>

                <div className={styles.emptyState}>
                    <span className={styles.emptyText}>You have no animals registered yet.</span>
                </div>
            </div>

            <button className={styles.fab}>+ Add animal</button>
        </AppLayout>
    )
}