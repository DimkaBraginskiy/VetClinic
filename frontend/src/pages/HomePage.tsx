import { useEffect, useState } from 'react'
import AppLayout from '../components/AppLayout'
import { getAnimals, getAppointmentCount } from '../api/customer'
import { getCustomerId } from '../api/auth'
import styles from './HomePage.module.css'

export default function HomePage() {
    const [animalCount,      setAnimalCount]      = useState<number | null>(null)
    const [appointmentCount, setAppointmentCount] = useState<number | null>(null)

    useEffect(() => {
        const customerId = getCustomerId()
        if (!customerId) return

        getAnimals(customerId).then(a => setAnimalCount(a.length)).catch(() => setAnimalCount(0))
        getAppointmentCount(customerId).then(setAppointmentCount).catch(() => setAppointmentCount(0))
    }, [])

    const display = (val: number | null) => val === null ? '…' : String(val)

    return (
        <AppLayout>
            <div className={styles.cards}>
                <div className={styles.card}>
                    <span className={styles.cardLabel}>Upcoming appointments</span>
                    <span className={styles.cardValue}>{display(appointmentCount)}</span>
                </div>
                <div className={styles.card}>
                    <span className={styles.cardLabel}>My animals</span>
                    <span className={styles.cardValue}>{display(animalCount)}</span>
                </div>
            </div>

            <div className={styles.section}>
                <h3>Upcoming appointments</h3>
                <p className={styles.empty}>You have no upcoming appointments.</p>
            </div>
        </AppLayout>
    )
}