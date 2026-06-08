import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { getVetId, logout } from '../../api/auth'
import { getVetAppointments } from '../../api/appointment'
import { getVetProfile, getVetShifts, createShift, deleteShift, type Shift, type VetProfile } from '../../api/shifts'
import type { AppointmentMinimal } from '../../types/appointment'
import styles from './VetDashboardPage.module.css'

type Tab = 'appointments' | 'shifts'

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

function fmtRange(start: string, end: string) {
    const d = new Date(start)
    const date = d.toLocaleDateString('en-US', { weekday: 'short', month: 'short', day: 'numeric' })
    const s = new Date(start).toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', hour12: false })
    const e = new Date(end).toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', hour12: false })
    return `${date} · ${s} – ${e}`
}

export default function VetDashboardPage() {
    const navigate = useNavigate()
    const vetId    = getVetId()

    const [tab,          setTab]          = useState<Tab>('appointments')
    const [profile,      setProfile]      = useState<VetProfile | null>(null)
    const [appointments, setAppointments] = useState<AppointmentMinimal[]>([])
    const [shifts,       setShifts]       = useState<Shift[]>([])
    const [loading,      setLoading]      = useState(true)
    const [error,        setError]        = useState<string | null>(null)

    // shift form
    const [formDate,  setFormDate]  = useState('')
    const [formStart, setFormStart] = useState('')
    const [formEnd,   setFormEnd]   = useState('')
    const [adding,    setAdding]    = useState(false)
    const [addError,  setAddError]  = useState<string | null>(null)

    // delete confirm
    const [deletingId, setDeletingId] = useState<string | null>(null)
    const [deleteError, setDeleteError] = useState<string | null>(null)

    useEffect(() => {
        if (!vetId) { navigate('/'); return }
        Promise.all([
            getVetProfile(vetId).then(setProfile),
            getVetAppointments(vetId).then(setAppointments),
            getVetShifts(vetId).then(setShifts),
        ])
            .catch(() => setError('Could not load data.'))
            .finally(() => setLoading(false))
    }, [vetId]) // eslint-disable-line react-hooks/exhaustive-deps

    const handleLogout = () => { logout(); navigate('/') }

    const handleAddShift = async (e: React.FormEvent) => {
        e.preventDefault()
        if (!vetId || !profile || !formDate || !formStart || !formEnd) return
        setAdding(true)
        setAddError(null)
        try {
            await createShift({
                veterinarianId: vetId,
                clinicId: profile.clinicId,
                startTime: `${formDate}T${formStart}:00`,
                endTime:   `${formDate}T${formEnd}:00`,
            })
            const updated = await getVetShifts(vetId)
            setShifts(updated)
            setFormDate(''); setFormStart(''); setFormEnd('')
        } catch (e) {
            setAddError((e as Error).message)
        } finally {
            setAdding(false)
        }
    }

    const handleDeleteShift = async (id: string) => {
        setDeletingId(id)
        setDeleteError(null)
        try {
            await deleteShift(id)
            setShifts(prev => prev.filter(s => s.id !== id))
        } catch (e) {
            setDeleteError((e as Error).message)
        } finally {
            setDeletingId(null)
        }
    }

    const upcoming  = appointments.filter(a => a.status === 'Scheduled' || a.status === 'InProgress')
    const past      = appointments.filter(a => a.status === 'Completed' || a.status === 'Cancelled')
    const upcoming_shifts = shifts.filter(s => new Date(s.startTime) > new Date())
    const past_shifts     = shifts.filter(s => new Date(s.startTime) <= new Date())

    return (
        <div className={styles.layout}>
            {/* sidebar */}
            <nav className={styles.sidebar}>
                <div className={styles.sidebarInner}>
                    <span className={styles.brand}>Vet Portal</span>
                    {profile && (
                        <div className={styles.profileBlock}>
                            <span className={styles.profileName}>{profile.fullName}</span>
                            <span className={styles.profileSub}>{profile.type} · {profile.clinicVetId}</span>
                            <span className={styles.profileSub}>{profile.clinicName}</span>
                        </div>
                    )}
                    <div className={styles.divider} />
                    <button
                        className={`${styles.navBtn} ${tab === 'appointments' ? styles.active : ''}`}
                        onClick={() => setTab('appointments')}
                    >
                        Appointments
                    </button>
                    <button
                        className={`${styles.navBtn} ${tab === 'shifts' ? styles.active : ''}`}
                        onClick={() => setTab('shifts')}
                    >
                        Shifts
                    </button>
                    <div className={styles.spacer} />
                    <button className={styles.logoutBtn} onClick={handleLogout}>Log out</button>
                </div>
            </nav>

            {/* content */}
            <div className={styles.content}>
                {loading && <p className={styles.hint}>Loading…</p>}
                {error   && <p className={styles.errorMsg}>{error}</p>}

                {!loading && !error && tab === 'appointments' && (
                    <div className={styles.panel}>
                        <h2 className={styles.panelTitle}>My Appointments</h2>

                        {appointments.length === 0 && (
                            <p className={styles.empty}>No appointments yet.</p>
                        )}

                        {upcoming.length > 0 && (
                            <section className={styles.section}>
                                <h3 className={styles.sectionTitle}>Upcoming</h3>
                                {upcoming.map(a => <ApptRow key={a.id} appt={a} />)}
                            </section>
                        )}
                        {past.length > 0 && (
                            <section className={styles.section}>
                                <h3 className={styles.sectionTitle}>Past</h3>
                                {past.map(a => <ApptRow key={a.id} appt={a} />)}
                            </section>
                        )}
                    </div>
                )}

                {!loading && !error && tab === 'shifts' && (
                    <div className={styles.panel}>
                        <h2 className={styles.panelTitle}>My Shifts</h2>

                        {/* Add shift form */}
                        <div className={styles.formCard}>
                            <h3 className={styles.formTitle}>Add shift</h3>
                            <form className={styles.shiftForm} onSubmit={handleAddShift}>
                                <div className={styles.formRow}>
                                    <label className={styles.fieldLabel}>Date</label>
                                    <input
                                        type="date"
                                        className={styles.input}
                                        value={formDate}
                                        min={new Date().toISOString().slice(0, 10)}
                                        onChange={e => setFormDate(e.target.value)}
                                        required
                                    />
                                </div>
                                <div className={styles.formRow}>
                                    <label className={styles.fieldLabel}>Start</label>
                                    <input
                                        type="time"
                                        className={styles.input}
                                        value={formStart}
                                        onChange={e => setFormStart(e.target.value)}
                                        required
                                    />
                                </div>
                                <div className={styles.formRow}>
                                    <label className={styles.fieldLabel}>End</label>
                                    <input
                                        type="time"
                                        className={styles.input}
                                        value={formEnd}
                                        onChange={e => setFormEnd(e.target.value)}
                                        required
                                    />
                                </div>
                                {addError && <p className={styles.errorMsg}>{addError}</p>}
                                <button
                                    type="submit"
                                    className={styles.addBtn}
                                    disabled={adding}
                                >
                                    {adding ? 'Adding…' : 'Add shift'}
                                </button>
                            </form>
                        </div>

                        {deleteError && <p className={styles.errorMsg}>{deleteError}</p>}

                        {shifts.length === 0 && <p className={styles.empty}>No shifts scheduled.</p>}

                        {upcoming_shifts.length > 0 && (
                            <section className={styles.section}>
                                <h3 className={styles.sectionTitle}>Upcoming</h3>
                                {upcoming_shifts.map(s => (
                                    <ShiftRow
                                        key={s.id}
                                        shift={s}
                                        deleting={deletingId === s.id}
                                        onDelete={handleDeleteShift}
                                    />
                                ))}
                            </section>
                        )}
                        {past_shifts.length > 0 && (
                            <section className={styles.section}>
                                <h3 className={styles.sectionTitle}>Past</h3>
                                {past_shifts.map(s => (
                                    <ShiftRow
                                        key={s.id}
                                        shift={s}
                                        deleting={deletingId === s.id}
                                        onDelete={handleDeleteShift}
                                    />
                                ))}
                            </section>
                        )}
                    </div>
                )}
            </div>
        </div>
    )
}

function ApptRow({ appt }: { appt: AppointmentMinimal }) {
    const title = appt.treatmentType
        ? `${appt.treatmentType} for ${appt.animalName}`
        : `Consultation for ${appt.animalName}`

    return (
        <div className={`${styles.card} ${appt.status === 'Cancelled' ? styles.cardDim : ''}`}>
            <div className={styles.cardLeft}>
                <span className={styles.cardTitle}>{title}</span>
                <span className={styles.cardMeta}>{fmtDateTime(appt.startDate)}</span>
            </div>
            <span className={`${styles.statusBadge} ${STATUS_CLASS[appt.status] ?? ''}`}>
                {appt.status}
            </span>
        </div>
    )
}

function ShiftRow({ shift, deleting, onDelete }: {
    shift: Shift
    deleting: boolean
    onDelete: (id: string) => void
}) {
    const [confirming, setConfirming] = useState(false)
    const isPast = new Date(shift.startTime) <= new Date()

    return (
        <div className={styles.card}>
            <div className={styles.cardLeft}>
                <span className={styles.cardTitle}>{shift.clinicName}</span>
                <span className={styles.cardMeta}>{fmtRange(shift.startTime, shift.endTime)}</span>
            </div>
            {!isPast && (
                <div className={styles.cardActions}>
                    {!confirming && (
                        <button className={styles.deleteBtn} onClick={() => setConfirming(true)}>
                            Remove
                        </button>
                    )}
                    {confirming && (
                        <div className={styles.confirm}>
                            <span className={styles.confirmText}>Remove?</span>
                            <button
                                className={styles.confirmYes}
                                onClick={() => { setConfirming(false); onDelete(shift.id) }}
                                disabled={deleting}
                            >
                                {deleting ? '…' : 'Yes'}
                            </button>
                            <button
                                className={styles.confirmNo}
                                onClick={() => setConfirming(false)}
                                disabled={deleting}
                            >
                                No
                            </button>
                        </div>
                    )}
                </div>
            )}
        </div>
    )
}
