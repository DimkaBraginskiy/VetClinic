import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import AppLayout from '../components/AppLayout'
import { getAnimals } from '../api/customer'
import { getAppointments } from '../api/appointment'
import { getCustomerId } from '../api/auth'
import type { Appointment } from '../types/appointment'
import styles from './HomePage.module.css'

const MODE_LABEL: Record<string, string> = {
    Online: 'Online',
    Home:   'At home',
    Clinic: 'In clinic',
}

function fmtDateTime(iso: string) {
    const d = new Date(iso)
    return d.toLocaleDateString('en-US', { weekday: 'short', month: 'short', day: 'numeric' })
        + ' · '
        + d.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', hour12: false })
}

function apptTitle(appt: Appointment): string {
    const animal = appt.animals[0]?.name ?? 'Unknown'
    return appt.treatment ? `${appt.treatment.type} for ${animal}` : `Consultation for ${animal}`
}

export default function HomePage() {
    const navigate = useNavigate()
    const [animalCount,  setAnimalCount]  = useState<number | null>(null)
    const [appointments, setAppointments] = useState<Appointment[]>([])
    const [apptLoading,  setApptLoading]  = useState(true)

    useEffect(() => {
        const customerId = getCustomerId()
        if (!customerId) { setApptLoading(false); return }

        getAnimals(customerId).then(a => setAnimalCount(a.length)).catch(() => setAnimalCount(0))

        getAppointments(customerId)
            .then(setAppointments)
            .catch(() => setAppointments([]))
            .finally(() => setApptLoading(false))
    }, [])

    const now = new Date()
    const upcoming = appointments
        .filter(a => a.status === 'Scheduled' && new Date(a.startDate) > now)
        .sort((a, b) => new Date(a.startDate).getTime() - new Date(b.startDate).getTime())
        .slice(0, 3)

    const upcomingCount = appointments.filter(
        a => a.status === 'Scheduled' && new Date(a.startDate) > now
    ).length

    return (
        <AppLayout>
            <div className={styles.cards}>
                <div className={styles.card}>
                    <span className={styles.cardLabel}>Upcoming appointments</span>
                    <span className={styles.cardValue}>{upcomingCount}</span>
                </div>
                <div className={styles.card}>
                    <span className={styles.cardLabel}>My animals</span>
                    <span className={styles.cardValue}>{animalCount === null ? '…' : animalCount}</span>
                </div>
            </div>

            <div className={styles.section}>
                <div className={styles.sectionHeader}>
                    <h3>Upcoming appointments</h3>
                    <button className={styles.seeAllBtn} onClick={() => navigate('/appointments')}>
                        See all →
                    </button>
                </div>

                {apptLoading && <p className={styles.empty}>Loading…</p>}

                {!apptLoading && upcoming.length === 0 && (
                    <p className={styles.empty}>No upcoming appointments.</p>
                )}

                {upcoming.map(appt => (
                    <div key={appt.id} className={styles.apptRow}>
                        <span className={styles.apptMode}>{MODE_LABEL[appt.mode] ?? appt.mode}</span>
                        <div className={styles.apptInfo}>
                            <span className={styles.apptTitle}>{apptTitle(appt)}</span>
                            <span className={styles.apptMeta}>
                                {appt.veterinarian.fullName} · {fmtDateTime(appt.startDate)}
                            </span>
                        </div>
                        <span className={styles.apptPrice}>€{appt.totalPrice.toFixed(2)}</span>
                    </div>
                ))}
            </div>
        </AppLayout>
    )
}