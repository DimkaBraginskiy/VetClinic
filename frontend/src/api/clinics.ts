import type { ClinicOption } from '../types/clinic'

const BASE = 'http://localhost:5258/api'

export async function getClinics(): Promise<ClinicOption[]> {
    const res = await fetch(`${BASE}/clinics`)
    if (!res.ok) throw new Error('Failed to fetch clinics.')
    return res.json()
}