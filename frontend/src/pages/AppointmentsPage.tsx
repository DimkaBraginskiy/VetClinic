import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import AppLayout from '../components/AppLayout'
import { getAppointments } from '../api/appointment'
import { getCustomerId } from '../api/auth'
import type { Appointment } from '../types/appointment'
import styles from './AppointmentsPage.module.css'

const MODE_ICON: Record<string, string> = {
    Online: '💻',
    Home:   '🏠',
    Clinic: '🏥',
}

const STATUS_CLASS: Record<string, string> = {
    Scheduled:  styles.statusScheduled,
    InProgress: styles.statusInProgress,
    Completed:  styles.statusCompleted,
    Cancelled:  styles.statusCancelled,
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

function locationLine(appt: Appointment): string {
    if (appt.mode === 'Online') return 'Video call'
    if (appt.mode === 'Home' && appt.homeAddress)
        return `At home · ${appt.homeAddress.street} ${appt.homeAddress.building}, ${appt.homeAddress.city}`
    if (appt.mode === 'Clinic' && appt.cabinetNumber != null)
        return `Cabinet ${appt.cabinetNumber}`
    return appt.mode
}

function ApptCard({ appt }: { appt: Appointment }) {
    return (
        <div className={`${styles.card} ${appt.status === 'Cancelled' ? styles.cardCancelled : ''}`}>
            <div className={styles.cardTop}>
                <span className={styles.modeIcon}>{MODE_ICON[appt.mode] ?? '📋'}</span>
                <div className={styles.cardMain}>
                    <span className={styles.cardTitle}>{apptTitle(appt)}</span>
                    <span className={styles.cardMeta}>{appt.veterinarian.fullName} · {appt.veterinarian.type}</span>
                    <span className={styles.cardMeta}>{locationLine(appt)}</span>
                </div>
                <span className={`${styles.statusBadge} ${STATUS_CLASS[appt.status] ?? ''}`}>
                    {appt.status}
                </span>
            </div>
            <div className={styles.cardBottom}>
                <span className={styles.cardDate}>{fmtDateTime(appt.startDate)}</span>
                <span className={styles.cardPrice}>€{appt.totalPrice.toFixed(2)}</span>
            </div>
        </div>
    )
}

export default function AppointmentsPage() {
    const navigate = useNavigate()
    const [appointments, setAppointments] = useState<Appointment[]>([])
    const [loading,      setLoading]      = useState(true)
    const [error,        setError]        = useState<string | null>(null)

    useEffect(() => {
        const id = getCustomerId()
        if (!id) { setLoading(false); return }
        getAppointments(id)
            .then(setAppointments)
            .catch(() => setError('Could not load appointments.'))
            .finally(() => setLoading(false))
    }, [])

    const upcoming = appointments.filter(a => a.status === 'Scheduled' || a.status === 'InProgress')
    const past     = appointments.filter(a => a.status === 'Completed'  || a.status === 'Cancelled')

    return (
        <AppLayout>
            <div className={styles.panel}>
                <div className={styles.panelHeader}>
                    <h2>My Appointments</h2>
                </div>

                {loading && <p className={styles.hint}>Loading…</p>}
                {error   && <p className={styles.errorMsg}>{error}</p>}

                {!loading && !error && appointments.length === 0 && (
                    <div className={styles.emptyState}>
                        <span className={styles.emptyIcon}>📅</span>
                        <span className={styles.emptyText}>You have no appointments yet.</span>
                    </div>
                )}

                {upcoming.length > 0 && (
                    <section className={styles.section}>
                        <h3 className={styles.sectionTitle}>Upcoming</h3>
                        <div className={styles.list}>
                            {upcoming.map(a => <ApptCard key={a.id} appt={a} />)}
                        </div>
                    </section>
                )}

                {past.length > 0 && (
                    <section className={styles.section}>
                        <h3 className={styles.sectionTitle}>Past</h3>
                        <div className={styles.list}>
                            {past.map(a => <ApptCard key={a.id} appt={a} />)}
                        </div>
                    </section>
                )}
            </div>

            <button className={styles.fab} onClick={() => navigate('/appointments/schedule')}>
                + Schedule appointment
            </button>
        </AppLayout>
    )
}