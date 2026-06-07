import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import AppLayout from '../../components/AppLayout'
import { getAppointmentById } from '../../api/appointment'
import { getCustomerId } from '../../api/auth'
import type { Appointment } from '../../types/appointment'
import styles from './AppointmentDetailPage.module.css'

function fmtDateTime(iso: string) {
    const d = new Date(iso)
    return d.toLocaleDateString('en-US', { weekday: 'long', month: 'long', day: 'numeric' })
        + ' at '
        + d.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', hour12: false })
}

function Row({ label, value }: { label: string; value: string }) {
    return (
        <div className={styles.row}>
            <span className={styles.rowLabel}>{label}</span>
            <span className={styles.rowValue}>{value}</span>
        </div>
    )
}

const STATUS_CLASS: Record<string, string> = {
    Scheduled:  styles.statusScheduled,
    InProgress: styles.statusInProgress,
    Completed:  styles.statusCompleted,
    Cancelled:  styles.statusCancelled,
}

export default function AppointmentDetailPage() {
    const { id }     = useParams<{ id: string }>()
    const navigate   = useNavigate()
    const [appt, setAppt]       = useState<Appointment | null>(null)
    const [loading, setLoading] = useState(true)
    const [error, setError]     = useState<string | null>(null)

    useEffect(() => {
        const customerId = getCustomerId()
        if (!id || !customerId) { setLoading(false); return }
        getAppointmentById(id, customerId)
            .then(setAppt)
            .catch(() => setError('Could not load appointment details.'))
            .finally(() => setLoading(false))
    }, [id])

    if (loading) return <AppLayout><p className={styles.hint}>Loading…</p></AppLayout>
    if (error || !appt) return <AppLayout><p className={styles.errorMsg}>{error ?? 'Not found.'}</p></AppLayout>

    const discountPct = appt.discounts.reduce((s, d) => s + d.percentage, 0)
    const discountAmt = appt.basePrice * (discountPct / 100)

    return (
        <AppLayout>
            <div className={styles.page}>

                <div className={styles.pageHeader}>
                    <button className={styles.backBtn} onClick={() => navigate('/appointments')}>← Back</button>
                    <span className={`${styles.statusBadge} ${STATUS_CLASS[appt.status] ?? ''}`}>
                        {appt.status}
                    </span>
                </div>

                <h2 className={styles.title}>
                    {appt.treatment
                        ? `${appt.treatment.type} for ${appt.animals[0]?.name ?? 'Unknown'}`
                        : `Consultation for ${appt.animals[0]?.name ?? 'Unknown'}`
                    }
                </h2>

                {/* Overview */}
                <div className={styles.section}>
                    <h3 className={styles.sectionTitle}>Overview</h3>
                    <Row label="Type"   value={appt.type} />
                    <Row label="Mode"   value={appt.mode} />
                    <Row label="Start"  value={fmtDateTime(appt.startDate)} />
                    <Row label="End"    value={fmtDateTime(appt.endDate)} />
                </div>

                {/* Veterinarian */}
                <div className={styles.section}>
                    <h3 className={styles.sectionTitle}>Veterinarian</h3>
                    <Row label="Name" value={appt.veterinarian.fullName} />
                    <Row label="Type" value={appt.veterinarian.type} />
                    <Row label="ID"   value={appt.veterinarian.clinicVetId} />
                </div>

                {/* Animals */}
                <div className={styles.section}>
                    <h3 className={styles.sectionTitle}>Animals</h3>
                    {appt.animals.map(a => (
                        <Row key={a.id}
                             label={a.name}
                             value={[a.species, a.breed].filter(Boolean).join(' · ') || 'Unknown'} />
                    ))}
                </div>

                {/* Location */}
                <div className={styles.section}>
                    <h3 className={styles.sectionTitle}>Location</h3>
                    {appt.mode === 'Online' && (
                        <>
                            <Row label="Format" value="Video call" />
                            {appt.meetingLink && (
                                <div className={styles.row}>
                                    <span className={styles.rowLabel}>Meeting link</span>
                                    <a href={appt.meetingLink} target="_blank" rel="noreferrer"
                                       className={styles.link}>{appt.meetingLink}</a>
                                </div>
                            )}
                        </>
                    )}
                    {appt.mode === 'Home' && appt.homeAddress && (
                        <>
                            <Row label="Street"  value={`${appt.homeAddress.street} ${appt.homeAddress.building}`} />
                            <Row label="City"    value={appt.homeAddress.city} />
                            <Row label="Country" value={appt.homeAddress.country} />
                            {appt.homeAddress.postalCode && <Row label="Postal" value={appt.homeAddress.postalCode} />}
                        </>
                    )}
                    {appt.mode === 'Clinic' && (
                        <>
                            {appt.cabinetNumber != null && <Row label="Cabinet" value={`#${appt.cabinetNumber}`} />}
                            {appt.arriveTime && <Row label="Arrive by" value={fmtDateTime(appt.arriveTime)} />}
                        </>
                    )}
                </div>

                {/* Treatment */}
                {appt.treatment && (
                    <div className={styles.section}>
                        <h3 className={styles.sectionTitle}>Treatment</h3>
                        <Row label="Type"     value={appt.treatment.type} />
                        <Row label="Duration" value={`${appt.treatment.duration} min`} />
                        <Row label="Price"    value={`€${appt.treatment.price.toFixed(2)}`} />
                    </div>
                )}

                {/* Price */}
                <div className={styles.section}>
                    <h3 className={styles.sectionTitle}>Payment</h3>
                    <Row label="Base price" value={`€${appt.basePrice.toFixed(2)}`} />
                    {appt.discounts.map(d => (
                        <Row key={d.id}
                             label={`Promo: ${d.promoCode} (${d.percentage}%)`}
                             value={`−€${(appt.basePrice * d.percentage / 100).toFixed(2)}`} />
                    ))}
                    {discountAmt > 0 && (
                        <Row label="After discount" value={`€${(appt.basePrice - discountAmt).toFixed(2)}`} />
                    )}
                    <div className={`${styles.row} ${styles.rowTotal}`}>
                        <span className={styles.rowLabel}>Total paid</span>
                        <span className={styles.rowValue}>€{appt.totalPrice.toFixed(2)}</span>
                    </div>
                </div>

            </div>
        </AppLayout>
    )
}