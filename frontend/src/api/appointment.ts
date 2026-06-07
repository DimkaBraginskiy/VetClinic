import type { Appointment, VetAvailability } from '../types/appointment'

const BASE = 'http://localhost:5258/api'

export async function getAvailableVets(
    date: Date,
    clinicId: string | null,
    treatmentType: string | null
): Promise<VetAvailability[]> {
    const params = new URLSearchParams()
    params.set('date', date.toISOString().slice(0, 10))
    if (clinicId) params.set('clinicId', clinicId)
    if (treatmentType) params.set('treatmentType', treatmentType)

    const res = await fetch(`${BASE}/veterinarians/available?${params}`)
    if (!res.ok) throw new Error('Failed to fetch veterinarians')
    const data = await res.json()
    return Array.isArray(data) ? data : (data.vets ?? [])
}

export async function validateDiscount(code: string): Promise<{ promoCode: string; percentage: number }> {
    const res = await fetch(`${BASE}/appointments/validate-discount?code=${encodeURIComponent(code)}`)
    if (!res.ok) throw new Error('Invalid or unknown promo code')
    return res.json()
}

export async function getAppointments(customerId: string): Promise<Appointment[]> {
    const res = await fetch(`${BASE}/appointments?customerId=${customerId}`)
    if (!res.ok) throw new Error('Failed to fetch appointments')
    return res.json()
}

export async function cancelAppointment(appointmentId: string): Promise<void> {
    const res = await fetch(`${BASE}/appointments/cancel/${appointmentId}`, {
        method: 'PUT',
    })
    if (!res.ok) throw new Error('Failed to cancel appointment')
}

export async function bookAppointment(mode: string, dto: Record<string, unknown>): Promise<void> {
    const endpoint = mode === 'Online' ? 'online' : mode === 'Home' ? 'home' : 'clinic'
    const res = await fetch(`${BASE}/appointments/${endpoint}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(dto),
    })
    if (!res.ok) {
        const data = await res.json().catch(() => ({}))
        throw new Error((data as { message?: string }).message ?? 'Failed to book appointment')
    }
}