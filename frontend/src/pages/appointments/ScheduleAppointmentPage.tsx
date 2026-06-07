import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import AppLayout from '../../components/AppLayout'
import { getAnimals, getLoyaltyPoints } from '../../api/customer'
import { getClinics } from '../../api/clinics'
import { getAvailableVets, validateDiscount, bookAppointment } from '../../api/appointment'
import { getCustomerId } from '../../api/auth'
import type { Animal } from '../../types/animal'
import type { ClinicOption } from '../../types/clinic'
import type { VetAvailability } from '../../types/appointment'
import styles from './ScheduleAppointmentPage.module.css'

type ApptType = 'Consultation' | 'Treatment'
type ApptMode = 'Online' | 'Home' | 'Clinic'
type StepKey  = 'animal' | 'type' | 'treatmentType' | 'mode' | 'clinic' | 'address' | 'day' | 'vet' | 'summary'

const TREATMENT_TYPES = ['Checkup', 'Vaccine', 'Surgery', 'Eyesight', 'Chiropractic']

const ALL_MODES = {
    Online: { value: 'Online' as ApptMode, label: 'Online',    desc: 'Video call with a vet' },
    Home:   { value: 'Home'   as ApptMode, label: 'At home',   desc: 'Vet visits your place' },
    Clinic: { value: 'Clinic' as ApptMode, label: 'In clinic', desc: 'Visit the clinic' },
}

function getAvailableModes(type: ApptType, treatmentType: string | null) {
    if (type === 'Consultation') return [ALL_MODES.Online, ALL_MODES.Home, ALL_MODES.Clinic]
    switch (treatmentType) {
        case 'Vaccine':
        case 'Surgery':
        case 'Eyesight':
            return [ALL_MODES.Clinic]
        case 'Checkup':
        case 'Chiropractic':
            return [ALL_MODES.Home, ALL_MODES.Clinic]
        default:
            return [ALL_MODES.Home, ALL_MODES.Clinic]
    }
}

function buildSequence(type: ApptType | null, _treatmentType: string | null, effectiveMode: ApptMode | null): StepKey[] {
    const steps: StepKey[] = ['animal', 'type']
    if (type === 'Treatment') steps.push('treatmentType')
    steps.push('mode')
    if (effectiveMode === 'Clinic') steps.push('clinic')
    if (effectiveMode === 'Home')   steps.push('address')
    steps.push('day', 'vet', 'summary')
    return steps
}

function nextDays(count: number) {
    return Array.from({ length: count }, (_, i) => {
        const d = new Date()
        d.setDate(d.getDate() + i + 1)
        return d
    })
}

function fmtTime(isoStr: string) {
    return new Date(isoStr).toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', hour12: false })
}

function fmtDate(d: Date) {
    return d.toLocaleDateString('en-US', { weekday: 'long', month: 'long', day: 'numeric' })
}

const DAYS   = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']
const MONTHS = ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec']

export default function ScheduleAppointmentPage() {
    const navigate = useNavigate()

    // wizard state
    const [stepIdx,       setStepIdx]       = useState(0)
    const [animalId,      setAnimalId]       = useState<string | null>(null)
    const [apptType,      setApptType]       = useState<ApptType | null>(null)
    const [treatmentType, setTreatmentType]  = useState<string | null>(null)
    const [mode,          setMode]           = useState<ApptMode | null>(null)
    const [clinicId,      setClinicId]       = useState<string | null>(null)
    const [selectedDay,   setSelectedDay]    = useState<Date | null>(null)
    const [selectedVetId, setSelectedVetId]  = useState<string | null>(null)
    const [selectedSlot,  setSelectedSlot]   = useState<string | null>(null)
    const [address, setAddress] = useState({ country: '', city: '', street: '', building: '', flat: '', postalCode: '' })

    // data
    const [animals,       setAnimals]       = useState<Animal[]>([])
    const [clinics,       setClinics]       = useState<ClinicOption[]>([])
    const [availableVets, setAvailableVets] = useState<VetAvailability[]>([])
    const [vetsLoading,   setVetsLoading]   = useState(false)
    const [vetsError,     setVetsError]     = useState<string | null>(null)

    // summary state
    const [promoInput,      setPromoInput]      = useState('')
    const [appliedDiscount, setAppliedDiscount] = useState<{ promoCode: string; percentage: number } | null>(null)
    const [discountError,   setDiscountError]   = useState<string | null>(null)
    const [discountLoading, setDiscountLoading] = useState(false)
    const [loyaltyAvail,    setLoyaltyAvail]    = useState<number | null>(null)
    const [loyaltyInput,    setLoyaltyInput]    = useState('')
    const [isBooking,       setIsBooking]       = useState(false)
    const [bookingError,    setBookingError]    = useState<string | null>(null)

    useEffect(() => {
        const id = getCustomerId()
        if (id) getAnimals(id).then(setAnimals).catch(() => {})
    }, [])

    // auto-select when only one mode is available
    useEffect(() => {
        if (!apptType) return
        const modes = getAvailableModes(apptType, treatmentType)
        if (modes.length === 1) setMode(modes[0].value)
        else setMode(null)
    }, [apptType, treatmentType])

    // fetch available vets or loyalty points when we enter those steps
    useEffect(() => {
        const currentStepKey = sequence[stepIdx]

        if (currentStepKey === 'vet' && selectedDay) {
            setVetsLoading(true)
            setVetsError(null)
            setAvailableVets([])
            getAvailableVets(selectedDay, clinicId, treatmentType)
                .then(setAvailableVets)
                .catch(() => setVetsError('Could not load veterinarians. Please go back and try again.'))
                .finally(() => setVetsLoading(false))
        }

        if (currentStepKey === 'summary') {
            const customerId = getCustomerId()
            if (customerId) {
                getLoyaltyPoints(customerId).then(setLoyaltyAvail).catch(() => setLoyaltyAvail(0))
            }
        }
    }, [stepIdx]) // eslint-disable-line react-hooks/exhaustive-deps

    const availableModes = apptType ? getAvailableModes(apptType, treatmentType) : []
    const effectiveMode  = availableModes.length === 1 ? availableModes[0].value : mode

    const sequence    = buildSequence(apptType, treatmentType, effectiveMode)
    const currentStep = sequence[stepIdx]
    const totalSteps  = sequence.length

    const canProceed = (): boolean => {
        switch (currentStep) {
            case 'animal':        return !!animalId
            case 'type':          return !!apptType
            case 'treatmentType': return !!treatmentType
            case 'mode':          return !!effectiveMode
            case 'clinic':        return !!clinicId
            case 'address':       return !!(address.country && address.city && address.street && address.building)
            case 'day':           return !!selectedDay
            case 'vet':           return !!selectedVetId && !!selectedSlot
            case 'summary':       return !isBooking
            default:              return false
        }
    }

    const goBack = () => {
        if (stepIdx === 0) { navigate('/appointments'); return }
        if (currentStep === 'mode')    { setMode(null); setClinicId(null); setSelectedDay(null) }
        if (currentStep === 'clinic')  { setClinicId(null) }
        if (currentStep === 'address') { setAddress({ country:'', city:'', street:'', building:'', flat:'', postalCode:'' }) }
        if (currentStep === 'vet')     { setSelectedVetId(null); setSelectedSlot(null); setAvailableVets([]) }
        if (currentStep === 'summary') { setAppliedDiscount(null); setDiscountError(null); setPromoInput(''); setLoyaltyInput(''); setBookingError(null) }
        setStepIdx(i => i - 1)
    }

    const goNext = async () => {
        if (currentStep === 'summary') {
            await handlePay()
            return
        }

        const nextStep = sequence[stepIdx + 1]
        if (nextStep === 'clinic' && clinics.length === 0) {
            getClinics().then(setClinics).catch(() => {})
        }
        setStepIdx(i => i + 1)
    }

    const handleApplyPromo = async () => {
        const code = promoInput.trim()
        if (!code) return
        setDiscountLoading(true)
        setDiscountError(null)
        setAppliedDiscount(null)
        try {
            const result = await validateDiscount(code)
            setAppliedDiscount(result)
        } catch {
            setDiscountError('Promo code not found or invalid.')
        } finally {
            setDiscountLoading(false)
        }
    }

    const handlePay = async () => {
        const customerId = getCustomerId()
        if (!customerId || !selectedVetId || !animalId || !selectedSlot || !effectiveMode) return

        const loyaltyToRedeem = Math.min(
            Math.max(0, parseFloat(loyaltyInput) || 0),
            loyaltyAvail ?? 0
        )
        const basePrice = selectedVet?.offeringPrice ?? 0

        const base = {
            veterinarianId: selectedVetId,
            customerId,
            animalId,
            startDate: selectedSlot,
            basePrice,
            promoCode: appliedDiscount?.promoCode ?? null,
            loyaltyPointsToRedeem: loyaltyToRedeem,
        }

        let dto: Record<string, unknown>

        if (effectiveMode === 'Online') {
            dto = base
        } else if (effectiveMode === 'Home') {
            dto = {
                ...base,
                type: apptType,
                treatmentType: treatmentType ?? null,
                address: {
                    country: address.country,
                    city: address.city,
                    street: address.street,
                    building: parseInt(address.building) || 0,
                    flat: address.flat ? parseInt(address.flat) : null,
                    postalCode: address.postalCode || null,
                },
            }
        } else {
            const arriveTime = new Date(new Date(selectedSlot).getTime() - 10 * 60 * 1000).toISOString()
            dto = {
                ...base,
                type: apptType,
                treatmentType: treatmentType ?? null,
                clinicId,
                arriveTime,
            }
        }

        setIsBooking(true)
        setBookingError(null)
        try {
            await bookAppointment(effectiveMode, dto)
            navigate('/appointments')
        } catch (e) {
            setBookingError((e as Error).message ?? 'Something went wrong. Please try again.')
        } finally {
            setIsBooking(false)
        }
    }

    const selectedVet    = availableVets.find(v => v.veterinarianId === selectedVetId) ?? null
    const selectedAnimal = animals.find(a => a.id === animalId) ?? null
    const selectedClinic = clinics.find(c => c.id === clinicId) ?? null

    // price calculation for summary
    const basePrice      = selectedVet?.offeringPrice ?? 0
    const discountPct    = appliedDiscount?.percentage ?? 0
    const discountAmt    = basePrice * (discountPct / 100)
    const loyaltyNum     = Math.min(Math.max(0, parseFloat(loyaltyInput) || 0), loyaltyAvail ?? 0)
    const totalPrice     = Math.max(0, basePrice - discountAmt - loyaltyNum)

    return (
        <AppLayout>
            <div className={styles.page}>

                <div className={styles.stepHeader}>
                    <p className={styles.stepLabel}>Step {stepIdx + 1} of {totalSteps}</p>
                    <h2 className={styles.heading}>{stepHeading(currentStep)}</h2>
                </div>

                {/* ── Animal selection ── */}
                {currentStep === 'animal' && (
                    <div className={styles.rowList}>
                        {animals.length === 0 && <p className={styles.empty}>No animals found. Add one first.</p>}
                        {animals.map(a => (
                            <button key={a.id}
                                 className={`${styles.rowItem} ${animalId === a.id ? styles.selected : ''}`}
                                 onClick={() => setAnimalId(a.id)}
                            >
                                <span className={styles.rowItemMain}>{a.name}</span>
                                <span className={styles.rowItemSub}>{[a.species, a.breed].filter(Boolean).join(' · ') || 'Unknown'}</span>
                            </button>
                        ))}
                    </div>
                )}

                {/* ── Appointment type ── */}
                {currentStep === 'type' && (
                    <div className={styles.rowList}>
                        {([
                            { value: 'Consultation' as const, desc: 'Talk to a vet online, at home or in clinic.' },
                            { value: 'Treatment'    as const, desc: 'A specific procedure performed by a specialist.' },
                        ]).map(t => (
                            <button key={t.value}
                                 className={`${styles.rowItem} ${apptType === t.value ? styles.selected : ''}`}
                                 onClick={() => { setApptType(t.value); setMode(null); setTreatmentType(null) }}
                            >
                                <span className={styles.rowItemMain}>{t.value}</span>
                                <span className={styles.rowItemSub}>{t.desc}</span>
                            </button>
                        ))}
                    </div>
                )}

                {/* ── Treatment type ── */}
                {currentStep === 'treatmentType' && (
                    <div className={styles.rowList}>
                        {TREATMENT_TYPES.map(t => (
                            <button key={t}
                                 className={`${styles.rowItem} ${treatmentType === t ? styles.selected : ''}`}
                                 onClick={() => setTreatmentType(t)}
                            >
                                <span className={styles.rowItemMain}>{t}</span>
                            </button>
                        ))}
                    </div>
                )}

                {/* ── Mode ── */}
                {currentStep === 'mode' && (
                    <div className={styles.rowList}>
                        {availableModes.map(m => (
                            <button key={m.value}
                                 className={`${styles.rowItem} ${mode === m.value ? styles.selected : ''}`}
                                 onClick={() => { setMode(m.value); setClinicId(null); setSelectedDay(null) }}
                            >
                                <span className={styles.rowItemMain}>{m.label}</span>
                                <span className={styles.rowItemSub}>{m.desc}</span>
                            </button>
                        ))}
                    </div>
                )}

                {/* ── Clinic selection ── */}
                {currentStep === 'clinic' && (
                    <div className={styles.rowList}>
                        {clinics.length === 0 && <p className={styles.empty}>No clinics available.</p>}
                        {clinics.map(c => (
                            <button key={c.id}
                                 className={`${styles.rowItem} ${clinicId === c.id ? styles.selected : ''}`}
                                 onClick={() => setClinicId(c.id)}
                            >
                                <span className={styles.rowItemMain}>{c.name}</span>
                                <span className={styles.rowItemSub}>{c.city}, {c.street} {c.building}</span>
                            </button>
                        ))}
                    </div>
                )}

                {/* ── Home address ── */}
                {currentStep === 'address' && (
                    <div className={styles.addressForm}>
                        <div className={styles.formRow}>
                            <div className={styles.field}>
                                <label className={styles.fieldLabel}>Country *</label>
                                <input className={styles.input} value={address.country}
                                       onChange={e => setAddress(a => ({...a, country: e.target.value}))} />
                            </div>
                            <div className={styles.field}>
                                <label className={styles.fieldLabel}>City *</label>
                                <input className={styles.input} value={address.city}
                                       onChange={e => setAddress(a => ({...a, city: e.target.value}))} />
                            </div>
                        </div>
                        <div className={styles.field}>
                            <label className={styles.fieldLabel}>Street *</label>
                            <input className={styles.input} value={address.street}
                                   onChange={e => setAddress(a => ({...a, street: e.target.value}))} />
                        </div>
                        <div className={styles.formRow}>
                            <div className={styles.field}>
                                <label className={styles.fieldLabel}>Building *</label>
                                <input className={styles.input} type="number" value={address.building}
                                       onChange={e => setAddress(a => ({...a, building: e.target.value}))} />
                            </div>
                            <div className={styles.field}>
                                <label className={styles.fieldLabel}>Flat</label>
                                <input className={styles.input} value={address.flat}
                                       onChange={e => setAddress(a => ({...a, flat: e.target.value}))} />
                            </div>
                        </div>
                        <div className={styles.field}>
                            <label className={styles.fieldLabel}>Postal code</label>
                            <input className={styles.input} value={address.postalCode}
                                   onChange={e => setAddress(a => ({...a, postalCode: e.target.value}))} />
                        </div>
                    </div>
                )}

                {/* ── Day selection ── */}
                {currentStep === 'day' && (
                    <div className={styles.dayGrid}>
                        {nextDays(28).map(d => (
                            <div key={d.toISOString()}
                                 className={`${styles.dayCard} ${selectedDay?.toDateString() === d.toDateString() ? styles.selected : ''}`}
                                 onClick={() => setSelectedDay(d)}
                            >
                                <span className={styles.dayName}>{DAYS[d.getDay()]}</span>
                                <span className={styles.dayNum}>{d.getDate()}</span>
                                <span className={styles.dayMon}>{MONTHS[d.getMonth()]}</span>
                            </div>
                        ))}
                    </div>
                )}

                {/* ── Veterinarian selection ── */}
                {currentStep === 'vet' && (
                    <div className={styles.vetList}>
                        {vetsLoading && (
                            <p className={styles.empty}>Loading available veterinarians…</p>
                        )}
                        {vetsError && (
                            <p className={styles.errorMsg}>{vetsError}</p>
                        )}
                        {!vetsLoading && !vetsError && availableVets.length === 0 && (
                            <div className={styles.noVets}>
                                <p>No veterinarians are available on this day.</p>
                                <p className={styles.noVetsSub}>Please go back and select a different day.</p>
                            </div>
                        )}
                        {availableVets.map(vet => (
                            <div key={vet.veterinarianId}
                                 className={`${styles.vetCard} ${selectedVetId === vet.veterinarianId ? styles.vetCardSelected : ''}`}
                            >
                                <div className={styles.vetHeader}>
                                    <div className={styles.vetInfo}>
                                        <span className={styles.vetName}>
                                            {vet.firstName} {vet.lastName}
                                        </span>
                                        <span className={styles.vetType}>
                                            {vet.type} · {vet.clinicVetId}
                                        </span>
                                        <span className={styles.vetOffering}>
                                            {vet.offeringType
                                                ? `${vet.offeringType} · ${vet.appointmentDurationMinutes} min · €${vet.offeringPrice?.toFixed(2)}`
                                                : `Consultation · ${vet.appointmentDurationMinutes} min`
                                            }
                                        </span>
                                    </div>
                                </div>

                                <div className={styles.slotGrid}>
                                    {vet.availableHours.map(slot => {
                                        const isSelected = selectedVetId === vet.veterinarianId && selectedSlot === slot
                                        return (
                                            <button
                                                key={slot}
                                                className={`${styles.slotBtn} ${isSelected ? styles.slotSelected : ''}`}
                                                onClick={() => {
                                                    setSelectedVetId(vet.veterinarianId)
                                                    setSelectedSlot(slot)
                                                }}
                                            >
                                                {fmtTime(slot)}
                                            </button>
                                        )
                                    })}
                                </div>
                            </div>
                        ))}
                    </div>
                )}

                {/* ── Summary ── */}
                {currentStep === 'summary' && (
                    <div className={styles.summaryPage}>

                        {/* Section 1: Appointment details */}
                        <div className={styles.summarySection}>
                            <h3 className={styles.sectionTitle}>Appointment Details</h3>
                            <div className={styles.summaryRows}>
                                <SummaryRow label="Animal"    value={selectedAnimal ? `${selectedAnimal.name} (${selectedAnimal.species})` : '—'} />
                                <SummaryRow label="Type"      value={apptType ?? '—'} />
                                {treatmentType && <SummaryRow label="Treatment" value={treatmentType} />}
                                <SummaryRow label="Mode"      value={effectiveMode ?? '—'} />
                                {effectiveMode === 'Clinic' && selectedClinic && (
                                    <SummaryRow label="Clinic" value={`${selectedClinic.name} — ${selectedClinic.city}`} />
                                )}
                                {effectiveMode === 'Home' && (
                                    <SummaryRow label="Address"
                                        value={`${address.street} ${address.building}, ${address.city}`} />
                                )}
                                {effectiveMode === 'Online' && (
                                    <SummaryRow label="Meeting" value="Video call link will be sent" />
                                )}
                                <SummaryRow label="Date" value={selectedDay ? fmtDate(selectedDay) : '—'} />
                                <SummaryRow label="Time" value={selectedSlot ? fmtTime(selectedSlot) : '—'} />
                            </div>
                        </div>

                        {/* Section 2: Veterinarian */}
                        <div className={styles.summarySection}>
                            <h3 className={styles.sectionTitle}>Veterinarian</h3>
                            {selectedVet ? (
                                <div className={styles.summaryRows}>
                                    <SummaryRow label="Name"     value={`${selectedVet.firstName} ${selectedVet.lastName}`} />
                                    <SummaryRow label="Type"     value={selectedVet.type} />
                                    <SummaryRow label="ID"       value={selectedVet.clinicVetId} />
                                    <SummaryRow label="Duration" value={`${selectedVet.appointmentDurationMinutes} min`} />
                                    <SummaryRow label="Price"    value={selectedVet.offeringPrice != null ? `€${selectedVet.offeringPrice.toFixed(2)}` : 'Included'} />
                                </div>
                            ) : (
                                <p className={styles.empty}>No veterinarian selected.</p>
                            )}
                        </div>

                        {/* Section 3: Promo code */}
                        <div className={styles.summarySection}>
                            <h3 className={styles.sectionTitle}>Promo Code</h3>
                            {appliedDiscount ? (
                                <div className={styles.discountApplied}>
                                    <span className={styles.discountBadge}>
                                        {appliedDiscount.promoCode} — {appliedDiscount.percentage}% off
                                    </span>
                                    <button className={styles.removeBtn}
                                        onClick={() => { setAppliedDiscount(null); setPromoInput('') }}>
                                        Remove
                                    </button>
                                </div>
                            ) : (
                                <div className={styles.promoRow}>
                                    <input
                                        className={styles.input}
                                        placeholder="Enter promo code"
                                        value={promoInput}
                                        onChange={e => { setPromoInput(e.target.value.toUpperCase()); setDiscountError(null) }}
                                        onKeyDown={e => e.key === 'Enter' && handleApplyPromo()}
                                    />
                                    <button
                                        className={styles.applyBtn}
                                        onClick={handleApplyPromo}
                                        disabled={discountLoading || !promoInput.trim()}
                                    >
                                        {discountLoading ? '…' : 'Apply'}
                                    </button>
                                </div>
                            )}
                            {discountError && <p className={styles.errorMsg}>{discountError}</p>}
                        </div>

                        {/* Section 4: Loyalty points + Total */}
                        <div className={styles.summarySection}>
                            <h3 className={styles.sectionTitle}>Loyalty Points & Total</h3>

                            <div className={styles.loyaltyRow}>
                                <span className={styles.loyaltyLabel}>
                                    Available: <strong>{loyaltyAvail?.toFixed(2) ?? '…'} pts</strong>
                                </span>
                                <div className={styles.loyaltyInputRow}>
                                    <label className={styles.fieldLabel}>Redeem points</label>
                                    <input
                                        className={styles.input}
                                        type="number"
                                        min="0"
                                        max={loyaltyAvail ?? 0}
                                        step="1"
                                        placeholder="0"
                                        value={loyaltyInput}
                                        onChange={e => setLoyaltyInput(e.target.value)}
                                    />
                                </div>
                            </div>

                            <div className={styles.priceBreakdown}>
                                <div className={styles.priceRow}>
                                    <span>Base price</span>
                                    <span>€{basePrice.toFixed(2)}</span>
                                </div>
                                {discountPct > 0 && (
                                    <div className={`${styles.priceRow} ${styles.priceDiscount}`}>
                                        <span>Discount ({discountPct}%)</span>
                                        <span>−€{discountAmt.toFixed(2)}</span>
                                    </div>
                                )}
                                {loyaltyNum > 0 && (
                                    <div className={`${styles.priceRow} ${styles.priceDiscount}`}>
                                        <span>Loyalty points</span>
                                        <span>−€{loyaltyNum.toFixed(2)}</span>
                                    </div>
                                )}
                                <div className={styles.priceDivider} />
                                <div className={`${styles.priceRow} ${styles.priceTotal}`}>
                                    <span>Total</span>
                                    <span>€{totalPrice.toFixed(2)}</span>
                                </div>
                            </div>

                            {bookingError && <p className={styles.errorMsg}>{bookingError}</p>}

                            <button
                                className={styles.payBtn}
                                onClick={handlePay}
                                disabled={isBooking}
                            >
                                {isBooking ? 'Booking…' : `Pay €${totalPrice.toFixed(2)}`}
                            </button>
                        </div>

                    </div>
                )}

            </div>

            {currentStep !== 'summary' && (
                <>
                    <button className={styles.backBtn} onClick={goBack}>Back</button>
                    <button className={styles.nextBtn} onClick={goNext} disabled={!canProceed()}>
                        {nextBtnLabel(currentStep)}
                    </button>
                </>
            )}
            {currentStep === 'summary' && (
                <button className={styles.backBtn} onClick={goBack}>Back</button>
            )}
        </AppLayout>
    )
}

function SummaryRow({ label, value }: { label: string; value: string }) {
    return (
        <div className={styles.summaryRow}>
            <span className={styles.summaryRowLabel}>{label}</span>
            <span className={styles.summaryRowValue}>{value}</span>
        </div>
    )
}

function stepHeading(step: StepKey): string {
    switch (step) {
        case 'animal':        return 'Which animal is this for?'
        case 'type':          return 'What kind of appointment?'
        case 'treatmentType': return 'Select treatment type'
        case 'mode':          return 'How would you like to meet?'
        case 'clinic':        return 'Choose a clinic'
        case 'address':       return 'Where should the vet come?'
        case 'day':           return 'Pick a day'
        case 'vet':           return 'Choose a veterinarian & time'
        case 'summary':       return 'Confirm your appointment'
        default:              return ''
    }
}

function nextBtnLabel(step: StepKey): string {
    switch (step) {
        case 'day':     return 'Find veterinarians'
        case 'vet':     return 'Review'
        default:        return 'Next'
    }
}