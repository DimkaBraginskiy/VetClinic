import type { Animal } from '../types/animal'


const BASE = 'http://localhost:5258/api'

export async function getLoyaltyPoints(customerId: string): Promise<number> {
    const res = await fetch(`${BASE}/customers/${customerId}/loyalty-points`)
    if (!res.ok) return 0
    const data = await res.json()
    return (data as { loyaltyPoints: number }).loyaltyPoints ?? 0
}

export async function getAppointmentCount(customerId: string): Promise<number> {
    const res = await fetch(`${BASE}/customers/${customerId}/appointments/count`)
    if (!res.ok) return 0
    const data = await res.json()
    return data.count ?? 0
}

export async function getAnimals(customerId: string): Promise<Animal[]> {
    const res = await fetch(`${BASE}/customers/${customerId}/animals`)
    if (!res.ok) throw new Error('Failed to fetch animals.')
    return res.json()
}

export async function deleteAnimal(customerId: string, animalId: string): Promise<void> {
    const res = await fetch(`${BASE}/customers/${customerId}/animals/${animalId}`, {
        method: 'DELETE',
    })
    if (!res.ok) throw new Error('Failed to delete animal.')
}

export async function getAnimalById(customerId: string, animalId: string): Promise<Animal> {
    const res = await fetch(`${BASE}/customers/${customerId}/animals/${animalId}`)
    if (!res.ok) throw new Error('Failed to fetch animal.')
    return res.json()
}

export async function addAnimal(
    customerId: string,
    data: {
        name: string
        dateOfBirth: string
        weight?: number | null
        species?: string | null
        breed?: string | null
    }
): Promise<string> {
    const res = await fetch(`${BASE}/customers/${customerId}/animals`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
    })
    if (!res.ok) throw new Error('Failed to add animal.')
    const result = await res.json()
    return result.id
}