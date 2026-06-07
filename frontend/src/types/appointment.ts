export interface VetAvailability {
    veterinarianId: string
    firstName: string
    lastName: string
    middleName: string | null
    type: string
    clinicVetId: string
    appointmentDurationMinutes: number
    offeringPrice: number | null
    offeringType: string | null
    availableHours: string[]  // ISO datetime strings from backend
}