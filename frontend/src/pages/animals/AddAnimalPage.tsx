import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import AppLayout from '../../components/AppLayout'
import { addAnimal } from '../../api/customer'
import { getCustomerId } from '../../api/auth'
import styles from './AddAnimalPage.module.css'

const SPECIES = ['Dog', 'Cat', 'Rabbit', 'Bird', 'Other']

export default function AddAnimalPage() {
    const navigate = useNavigate()

    const [name, setName]               = useState('')
    const [dob, setDob]                 = useState('')
    const [weight, setWeight]           = useState('')
    const [species, setSpecies]         = useState('')
    const [breed, setBreed]             = useState('')
    const [error, setError]             = useState('')
    const [loading, setLoading]         = useState(false)

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault()
        setError('')
        setLoading(true)

        const customerId = getCustomerId()
        if (!customerId) {
            setError('No customer account found. Please log in again.')
            setLoading(false)
            return
        }

        try {
            await addAnimal(customerId, {
                name,
                dateOfBirth: dob,
                weight:  weight  ? parseFloat(weight)  : null,
                species: species || null,
                breed:   breed   || null,
            })
            navigate('/animals')
        } catch {
            setError('Failed to add animal. Please try again.')
        } finally {
            setLoading(false)
        }
    }

    return (
        <AppLayout>
            <div className={styles.panel}>
                <div className={styles.panelHeader}>
                    <button className={styles.backBtn} onClick={() => navigate('/animals')}>←</button>
                    <h2>Add animal</h2>
                </div>

                <form className={styles.form} onSubmit={handleSubmit}>
                    <div className={styles.field}>
                        <label className={styles.label}>Name *</label>
                        <input
                            className={styles.input}
                            placeholder="e.g. Buddy"
                            value={name}
                            onChange={e => setName(e.target.value)}
                            required
                        />
                    </div>

                    <div className={styles.row}>
                        <div className={styles.field}>
                            <label className={styles.label}>Date of birth *</label>
                            <input
                                className={styles.input}
                                type="date"
                                value={dob}
                                onChange={e => setDob(e.target.value)}
                                required
                            />
                        </div>
                        <div className={styles.field}>
                            <label className={styles.label}>Weight (kg)</label>
                            <input
                                className={styles.input}
                                type="number"
                                min="0"
                                step="0.1"
                                placeholder="e.g. 12.5"
                                value={weight}
                                onChange={e => setWeight(e.target.value)}
                            />
                        </div>
                    </div>

                    <div className={styles.row}>
                        <div className={styles.field}>
                            <label className={styles.label}>Species</label>
                            <select
                                className={styles.select}
                                value={species}
                                onChange={e => { setSpecies(e.target.value); setBreed('') }}
                            >
                                <option value="">— none —</option>
                                {SPECIES.map(s => <option key={s} value={s}>{s}</option>)}
                            </select>
                        </div>
                        <div className={styles.field}>
                            <label className={styles.label}>Breed</label>
                            <input
                                className={styles.input}
                                placeholder="e.g. Labrador"
                                value={breed}
                                onChange={e => setBreed(e.target.value)}
                                disabled={!species}
                            />
                        </div>
                    </div>

                    {error && <p className={styles.error}>{error}</p>}

                    <button className={styles.submitBtn} type="submit" disabled={loading}>
                        {loading ? 'Saving…' : 'Add animal'}
                    </button>
                </form>
            </div>
        </AppLayout>
    )
}