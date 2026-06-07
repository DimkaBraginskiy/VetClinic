import { useState } from 'react'
import type { Animal } from '../types/animal'
import styles from './AnimalCard.module.css'

interface Props {
    animal: Animal
    onDelete: (id: string) => Promise<void>
}

export default function AnimalCard({ animal, onDelete }: Props) {
    const [confirming, setConfirming] = useState(false)
    const [deleting, setDeleting]     = useState(false)

    const handleDelete = async () => {
        setDeleting(true)
        await onDelete(animal.id)
        setDeleting(false)
        setConfirming(false)
    }

    return (
        <div className={styles.card}>
            <div className={styles.info}>
                <span className={styles.name}>{animal.name}</span>
                <span className={styles.meta}>
                    {[animal.species, animal.breed].filter(Boolean).join(' · ') || 'Unknown'}
                </span>
                {animal.weight != null && (
                    <span className={styles.weight}>{animal.weight} kg</span>
                )}
            </div>

            <div className={styles.actions}>
                {!confirming && (
                    <button
                        className={styles.deleteBtn}
                        onClick={() => setConfirming(true)}
                    >
                        Delete
                    </button>
                )}

                {confirming && (
                    <div className={styles.confirm}>
                        <span className={styles.confirmText}>Delete {animal.name}?</span>
                        <button
                            className={styles.confirmYes}
                            onClick={handleDelete}
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
        </div>
    )
}