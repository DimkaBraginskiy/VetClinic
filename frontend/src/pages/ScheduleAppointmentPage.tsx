import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import AppLayout from '../components/AppLayout'
import { getAnimals } from '../api/customer'
import { getClinics } from '../api/clinics'
import { getCustomerId } from '../api/auth'
import type { Animal } from '../types/animal'
import type { ClinicOption } from '../types/clinic'
import styles from './ScheduleAppointmentPage.module.css'

type ApptType    = 'Consultation' | 'Treatment'
type ApptMode    = 'Online' | 'Home' | 'Clinic'
type StepKey     = 'animal' | 'type' | 'treatmentType' | 'mode' | 'clinic' | 'address' | 'day'

const TREATMENT_TYPES = [
    { value: 'Checkup',      icon: '🩺', label: 'Checkup' },
    { value: 'Vaccine',      icon: '💉', label: 'Vaccine' },
    { value: 'Surgery',      icon: '🔪', label: 'Surgery' },
    { value: 'Eyesight',     icon: '👁',  label: 'Eyesight' },
    { value: 'Chiropractic', icon: '🦴', label: 'Chiropractic' },
]

const ALL_MODES = {
    Online: { value: 'Online' as ApptMode, icon: '💻', label: 'Online',    desc: 'Video call with a vet' },
    Home:   { value: 'Home'   as ApptMode, icon: '🏠', label: 'At home',   desc: 'Vet visits your place' },
    Clinic: { value: 'Clinic' as ApptMode, icon: '🏥', label: 'In clinic', desc: 'Visit the clinic' },
}

function getAvailableModes(type: ApptType, treatmentType: string | null) {
    if (type === 'Consultation') return [ALL_MODES.Online, ALL_MODES.Home, ALL_MODES.Clinic]
    switch (treatmentType) {
        case 'Vaccine':
        case 'Surgery':
        case 'Eyesight':
            return [ALL_MODES.Clinic]                          // clinic only — no choice
        case 'Checkup':
        case 'Chiropractic':
            return [ALL_MODES.Home, ALL_MODES.Clinic]          // home or clinic
        default:
            return [ALL_MODES.Home, ALL_MODES.Clinic]
    }
}

function buildSequence(type: ApptType | null, _treatmentType: string | null, effectiveMode: ApptMode | null): StepKey[] {
    const steps: StepKey[] = ['animal', 'type']
    if (type === 'Treatment') steps.push('treatmentType')
    steps.push('mode')   // always shown — single option is auto-selected but still visible
    if (effectiveMode === 'Clinic') steps.push('clinic')
    if (effectiveMode === 'Home')   steps.push('address')
    steps.push('day')
    return steps
}

function nextDays(count: number) {
    return Array.from({ length: count }, (_, i) => {
        const d = new Date()
        d.setDate(d.getDate() + i + 1)
        return d
    })
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
    const [address, setAddress] = useState({ country: '', city: '', street: '', building: '', flat: '', postalCode: '' })

    // data
    const [animals,  setAnimals]  = useState<Animal[]>([])
    const [clinics,  setClinics]  = useState<ClinicOption[]>([])

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

    // if only one mode is available for this treatment, it's auto-determined
    const availableModes  = apptType ? getAvailableModes(apptType, treatmentType) : []
    const effectiveMode   = availableModes.length === 1 ? availableModes[0].value : mode

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
            default:              return false
        }
    }

    const goBack = () => {
        if (stepIdx === 0) { navigate('/appointments'); return }
        // reset downstream state when going back
        if (currentStep === 'mode')   { setMode(null); setClinicId(null); setSelectedDay(null) }
        if (currentStep === 'clinic') { setClinicId(null) }
        if (currentStep === 'address'){ setAddress({ country:'', city:'', street:'', building:'', flat:'', postalCode:'' }) }
        setStepIdx(i => i - 1)
    }

    const goNext = () => {
        const nextStep = sequence[stepIdx + 1]
        if (nextStep === 'clinic' && clinics.length === 0) {
            getClinics().then(setClinics).catch(() => {})
        }
        if (stepIdx === totalSteps - 1) {
            // TODO: navigate to vet selection
            alert('Day selected! Vet selection coming next.')
            return
        }
        setStepIdx(i => i + 1)
    }

    const animalSpeciesIcon = (species: string | null) =>
        species === 'Dog' ? '🐕' : species === 'Cat' ? '🐈' : species === 'Rabbit' ? '🐇' :
        species === 'Bird' ? '🐦' : '🐾'

    return (
        <AppLayout>
            <div className={styles.page}>

                <div className={styles.stepHeader}>
                    <p className={styles.stepLabel}>Step {stepIdx + 1} of {totalSteps}</p>
                    <h2 className={styles.heading}>{stepHeading(currentStep)}</h2>
                </div>

                {/* ── Animal selection ── */}
                {currentStep === 'animal' && (
                    <div className={styles.cardGrid}>
                        {animals.length === 0 && <p className={styles.empty}>No animals found. Add one first.</p>}
                        {animals.map(a => (
                            <div key={a.id}
                                 className={`${styles.animalCard} ${animalId === a.id ? styles.selected : ''}`}
                                 onClick={() => setAnimalId(a.id)}
                            >
                                <span className={styles.animalAvatar}>{animalSpeciesIcon(a.species)}</span>
                                <span className={styles.animalName}>{a.name}</span>
                                <span className={styles.animalMeta}>{[a.species, a.breed].filter(Boolean).join(' · ') || 'Unknown'}</span>
                            </div>
                        ))}
                    </div>
                )}

                {/* ── Appointment type ── */}
                {currentStep === 'type' && (
                    <div className={styles.cardGrid}>
                        {([
                            { value: 'Consultation', icon: '💬', desc: 'Talk to a vet online, at home or in clinic.' },
                            { value: 'Treatment',    icon: '🩺', desc: 'A specific procedure performed by a specialist.' },
                        ] as const).map(t => (
                            <div key={t.value}
                                 className={`${styles.bigCard} ${apptType === t.value ? styles.selected : ''}`}
                                 onClick={() => { setApptType(t.value); setMode(null); setTreatmentType(null) }}
                            >
                                <div className={styles.bigCardImage}>{t.icon}</div>
                                <span className={styles.bigCardLabel}>{t.value}</span>
                                <span className={styles.bigCardDesc}>{t.desc}</span>
                            </div>
                        ))}
                    </div>
                )}

                {/* ── Treatment type ── */}
                {currentStep === 'treatmentType' && (
                    <div className={styles.cardGrid}>
                        {TREATMENT_TYPES.map(t => (
                            <div key={t.value}
                                 className={`${styles.optionCard} ${treatmentType === t.value ? styles.selected : ''}`}
                                 onClick={() => setTreatmentType(t.value)}
                            >
                                <span className={styles.optionIcon}>{t.icon}</span>
                                <span className={styles.optionLabel}>{t.label}</span>
                            </div>
                        ))}
                    </div>
                )}

                {/* ── Mode ── */}
                {currentStep === 'mode' && (
                    <div className={styles.cardGrid}>
                        {availableModes.map(m => (
                            <div key={m.value}
                                 className={`${styles.optionCard} ${mode === m.value ? styles.selected : ''}`}
                                 onClick={() => { setMode(m.value); setClinicId(null); setSelectedDay(null) }}
                            >
                                <span className={styles.optionIcon}>{m.icon}</span>
                                <span className={styles.optionLabel}>{m.label}</span>
                                <span className={styles.optionSub}>{m.desc}</span>
                            </div>
                        ))}
                    </div>
                )}

                {/* ── Clinic selection ── */}
                {currentStep === 'clinic' && (
                    <div className={styles.cardGrid}>
                        {clinics.length === 0 && <p className={styles.empty}>No clinics available.</p>}
                        {clinics.map(c => (
                            <div key={c.id}
                                 className={`${styles.optionCard} ${clinicId === c.id ? styles.selected : ''}`}
                                 onClick={() => setClinicId(c.id)}
                            >
                                <span className={styles.optionIcon}>🏥</span>
                                <span className={styles.optionLabel}>{c.name}</span>
                                <span className={styles.optionSub}>{c.city}, {c.street} {c.building}</span>
                            </div>
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

            </div>

            <button className={styles.backBtn} onClick={goBack}>← Back</button>
            <button className={styles.nextBtn} onClick={goNext} disabled={!canProceed()}>
                {stepIdx === totalSteps - 1 ? 'Confirm day →' : 'Next →'}
            </button>
        </AppLayout>
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
        default:              return ''
    }
}