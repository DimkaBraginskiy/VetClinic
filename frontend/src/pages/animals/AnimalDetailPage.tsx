import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import AppLayout from '../../components/AppLayout'
import { getAnimalById } from '../../api/customer'
import { getAppointments } from '../../api/appointment'
import { getCustomerId } from '../../api/auth'
import type { Animal } from '../../types/animal'
import type { AppointmentMinimal } from '../../types/appointment'
import styles from './AnimalDetailPage.module.css'

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

function fmtDate(iso: string) {
    return new Date(iso).toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' })
}

function fmtDateTime(iso: string) {
    const d = new Date(iso)
    return d.toLocaleDateString('en-US', { weekday: 'short', month: 'short', day: 'numeric' })
        + ' · '
        + d.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', hour12: false })
}

function age(dob: string) {
    const diff = Date.now() - new Date(dob).getTime()
    const years = Math.floor(diff / (1000 * 60 * 60 * 24 * 365.25))
    return years === 1 ? '1 year' : `${years} years`
}

export default function AnimalDetailPage() {
    const { id }   = useParams<{ id: string }>()
    const navigate = useNavigate()

    const [animal,       setAnimal]       = useState<Animal | null>(null)
    const [appointments, setAppointments] = useState<AppointmentMinimal[]>([])
    const [loading,      setLoading]      = useState(true)
    const [error,        setError]        = useState<string | null>(null)

    useEffect(() => {
        const customerId = getCustomerId()
        if (!id || !customerId) { setLoading(false); return }

        Promise.all([
            getAnimalById(customerId, id),
            getAppointments(customerId, id),
        ])
            .then(([a, appts]) => { setAnimal(a); setAppointments(appts) })
            .catch(() => setError('Could not load animal details.'))
            .finally(() => setLoading(false))
    }, [id])

    if (loading) return <AppLayout><p className={styles.hint}>Loading…</p></AppLayout>
    if (error || !animal) return <AppLayout><p className={styles.errorMsg}>{error ?? 'Not found.'}</p></AppLayout>

    const upcoming  = appointments.filter(a => a.status === 'Scheduled' || a.status === 'InProgress')
        .sort((a, b) => new Date(a.startDate).getTime() - new Date(b.startDate).getTime())
    const past      = appointments.filter(a => a.status === 'Completed' || a.status === 'Cancelled')
        .sort((a, b) => new Date(b.startDate).getTime() - new Date(a.startDate).getTime())

    return (
        <AppLayout>
            <div className={styles.page}>

                <div className={styles.pageHeader}>
                    <button className={styles.backBtn} onClick={() => navigate('/animals')}>← Back</button>
                </div>

                <h2 className={styles.title}>{animal.name}</h2>

                {/* Animal info */}
                <div className={styles.section}>
                    <h3 className={styles.sectionTitle}>Profile</h3>
                    {animal.species && (
                        <div className={styles.row}>
                            <span className={styles.rowLabel}>Species</span>
                            <span className={styles.rowValue}>{animal.species}</span>
                        </div>
                    )}
                    {animal.breed && (
                        <div className={styles.row}>
                            <span className={styles.rowLabel}>Breed</span>
                            <span className={styles.rowValue}>{animal.breed}</span>
                        </div>
                    )}
                    <div className={styles.row}>
                        <span className={styles.rowLabel}>Date of birth</span>
                        <span className={styles.rowValue}>{fmtDate(animal.dateOfBirth)} ({age(animal.dateOfBirth)})</span>
                    </div>
                    {animal.weight != null && (
                        <div className={styles.row}>
                            <span className={styles.rowLabel}>Weight</span>
                            <span className={styles.rowValue}>{animal.weight} kg</span>
                        </div>
                    )}
                </div>

                {/* Appointments */}
                <div className={styles.section}>
                    <h3 className={styles.sectionTitle}>Appointments</h3>

                    {appointments.length === 0 && (
                        <p className={styles.empty}>No appointments for {animal.name} yet.</p>
                    )}

                    {upcoming.length > 0 && (
                        <>
                            <p className={styles.groupLabel}>Upcoming</p>
                            {upcoming.map(a => <ApptRow key={a.id} appt={a} onNavigate={() => navigate(`/appointments/${a.id}`)} />)}
                        </>
                    )}

                    {past.length > 0 && (
                        <>
                            <p className={styles.groupLabel}>Past</p>
                            {past.map(a => <ApptRow key={a.id} appt={a} onNavigate={() => navigate(`/appointments/${a.id}`)} />)}
                        </>
                    )}
                </div>

            </div>
        </AppLayout>
    )
}

function ApptRow({ appt, onNavigate }: { appt: AppointmentMinimal; onNavigate: () => void }) {
    const title = appt.treatmentType
        ? `${appt.treatmentType}`
        : 'Consultation'

    return (
        <div className={styles.apptRow}>
            <span className={styles.apptMode}>{MODE_LABEL[appt.mode] ?? appt.mode}</span>
            <div className={styles.apptInfo}>
                <span className={styles.apptTitle}>{title}</span>
                <span className={styles.apptMeta}>{appt.veterinarianName} · {fmtDateTime(appt.startDate)}</span>
            </div>
            <span className={`${styles.statusBadge} ${STATUS_CLASS[appt.status] ?? ''}`}>
                {appt.status}
            </span>
            <span className={styles.apptPrice}>€{appt.totalPrice.toFixed(2)}</span>
            <button className={styles.detailsBtn} onClick={onNavigate}>Details</button>
        </div>
    )
}