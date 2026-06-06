import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import AppLayout from '../components/AppLayout'
import AnimalCard from '../components/AnimalCard'
import { getAnimals, deleteAnimal } from '../api/customer'
import { getCustomerId } from '../api/auth'
import type { Animal } from '../types/animal'
import styles from './AnimalsPage.module.css'

export default function AnimalsPage() {
    const [animals, setAnimals] = useState<Animal[]>([])
    const [loading, setLoading] = useState(true)
    const [error, setError]     = useState('')
    const navigate              = useNavigate()

    useEffect(() => {
        const customerId = getCustomerId()
        if (!customerId) {
            setError('No customer account linked to this login.')
            setLoading(false)
            return
        }
        getAnimals(customerId)
            .then(setAnimals)
            .catch(() => setError('Failed to load animals.'))
            .finally(() => setLoading(false))
    }, [])

    const handleDelete = async (animalId: string) => {
        const customerId = getCustomerId()
        if (!customerId) return
        await deleteAnimal(customerId, animalId)
        setAnimals(prev => prev.filter(a => a.id !== animalId))
    }

    return (
        <AppLayout>
            <div className={styles.panel}>
                <div className={styles.panelHeader}>
                    <h2>My Animals</h2>
                </div>

                {loading && <p className={styles.empty}>Loading…</p>}
                {!loading && error && <p className={styles.empty}>{error}</p>}

                {!loading && !error && animals.length === 0 && (
                    <div className={styles.emptyState}>
                        <span className={styles.emptyText}>You have no animals registered yet.</span>
                    </div>
                )}

                {!loading && !error && animals.length > 0 && (
                    <div className={styles.list}>
                        {animals.map(a => (
                            <AnimalCard key={a.id} animal={a} onDelete={handleDelete} />
                        ))}
                    </div>
                )}
            </div>

            <button className={styles.fab} onClick={() => navigate('/animals/add')}>+ Add animal</button>
        </AppLayout>
    )
}