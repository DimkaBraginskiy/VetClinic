import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import AppLayout from '../../components/AppLayout'
import { getAppointments, cancelAppointment } from '../../api/appointment'
import { getCustomerId } from '../../api/auth'
import type { AppointmentMinimal } from '../../types/appointment'
import styles from './AppointmentsPage.module.css'

const MODE_LABEL: Record<string, string> = {
    Online: 'Online',
    Home:   'At home',
    Clinic: 'In clinic',
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

function ApptCard({ appt, onCancel }: { appt: AppointmentMinimal; onCancel: (id: string) => Promise<void> }) {
    const navigate = useNavigate()
    const [confirming, setConfirming] = useState(false)
    const [cancelling, setCancelling] = useState(false)

    const handleCancel = async () => {
        setCancelling(true)
        await onCancel(appt.id)
        setCancelling(false)
        setConfirming(false)
    }

    const title = appt.treatmentType
        ? `${appt.treatmentType} for ${appt.animalName}`
        : `Consultation for ${appt.animalName}`

    return (
        <div className={`${styles.card} ${appt.status === 'Cancelled' ? styles.cardCancelled : ''}`}>
            <div className={styles.cardTop}>
                <span className={styles.modeBadge}>{MODE_LABEL[appt.mode] ?? appt.mode}</span>
                <div className={styles.cardMain}>
                    <span className={styles.cardTitle}>{title}</span>
                    <span className={styles.cardMeta}>{appt.veterinarianName}</span>
                </div>
                <span className={`${styles.statusBadge} ${STATUS_CLASS[appt.status] ?? ''}`}>
                    {appt.status}
                </span>
            </div>
            <div className={styles.cardBottom}>
                <span className={styles.cardDate}>{fmtDateTime(appt.startDate)}</span>
                <span className={styles.cardPrice}>€{appt.totalPrice.toFixed(2)}</span>

                <div className={styles.cardActions}>
                    <button
                        className={styles.detailsBtn}
                        onClick={() => navigate(`/appointments/${appt.id}`)}
                    >
                        See details
                    </button>

                    {(appt.status === 'Scheduled' || appt.status === 'InProgress') && !confirming && (
                        <button className={styles.deleteBtn} onClick={() => setConfirming(true)}>
                            Cancel
                        </button>
                    )}
                    {confirming && (
                        <div className={styles.confirm}>
                            <span className={styles.confirmText}>Cancel?</span>
                            <button className={styles.confirmYes} onClick={handleCancel} disabled={cancelling}>
                                {cancelling ? '…' : 'Yes'}
                            </button>
                            <button className={styles.confirmNo} onClick={() => setConfirming(false)} disabled={cancelling}>
                                No
                            </button>
                        </div>
                    )}
                </div>
            </div>
        </div>
    )
}

export default function AppointmentsPage() {
    const navigate = useNavigate()
    const [appointments, setAppointments] = useState<AppointmentMinimal[]>([])
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

    const handleCancel = async (apptId: string) => {
        await cancelAppointment(apptId)
        setAppointments(prev => prev.map(a =>
            a.id === apptId ? { ...a, status: 'Cancelled' } : a
        ))
    }

    const upcoming  = appointments
        .filter(a => a.status === 'Scheduled' || a.status === 'InProgress')
        .sort((a, b) => new Date(a.startDate).getTime() - new Date(b.startDate).getTime())
    const completed = appointments
        .filter(a => a.status === 'Completed')
        .sort((a, b) => new Date(b.startDate).getTime() - new Date(a.startDate).getTime())
    const cancelled = appointments
        .filter(a => a.status === 'Cancelled')
        .sort((a, b) => new Date(b.startDate).getTime() - new Date(a.startDate).getTime())

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
                        <span className={styles.emptyText}>You have no appointments yet.</span>
                    </div>
                )}

                {upcoming.length > 0 && (
                    <section className={styles.section}>
                        <h3 className={styles.sectionTitle}>Upcoming</h3>
                        <div className={styles.list}>
                            {upcoming.map(a => <ApptCard key={a.id} appt={a} onCancel={handleCancel} />)}
                        </div>
                    </section>
                )}

                {completed.length > 0 && (
                    <section className={styles.section}>
                        <h3 className={styles.sectionTitle}>Completed</h3>
                        <div className={styles.list}>
                            {completed.map(a => <ApptCard key={a.id} appt={a} onCancel={handleCancel} />)}
                        </div>
                    </section>
                )}

                {cancelled.length > 0 && (
                    <section className={styles.section}>
                        <h3 className={styles.sectionTitle}>Cancelled</h3>
                        <div className={styles.list}>
                            {cancelled.map(a => <ApptCard key={a.id} appt={a} onCancel={handleCancel} />)}
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