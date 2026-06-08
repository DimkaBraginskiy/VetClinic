const BASE = 'http://localhost:5258/api'

export interface Shift {
    id: string
    startTime: string
    endTime: string
    clinicId: string
    clinicName: string
}

export interface VetProfile {
    id: string
    fullName: string
    type: string
    clinicVetId: string
    clinicId: string
    clinicName: string
}

export async function getVetProfile(vetId: string): Promise<VetProfile> {
    const res = await fetch(`${BASE}/veterinarians/${vetId}`)
    if (!res.ok) throw new Error('Failed to fetch vet profile')
    return res.json()
}

export async function getVetShifts(vetId: string): Promise<Shift[]> {
    const res = await fetch(`${BASE}/shifts?veterinarianId=${vetId}`)
    if (!res.ok) throw new Error('Failed to fetch shifts')
    return res.json()
}

export async function createShift(dto: {
    veterinarianId: string
    clinicId: string
    startTime: string
    endTime: string
}): Promise<string> {
    const res = await fetch(`${BASE}/shifts`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(dto),
    })
    if (!res.ok) {
        const data = await res.json().catch(() => ({}))
        throw new Error((data as { message?: string }).message ?? 'Failed to create shift')
    }
    const data = await res.json()
    return data.id
}

export async function deleteShift(id: string): Promise<void> {
    const res = await fetch(`${BASE}/shifts/${id}`, { method: 'DELETE' })
    if (!res.ok) {
        const text = await res.text().catch(() => '')
        throw new Error(text || 'Failed to delete shift')
    }
}