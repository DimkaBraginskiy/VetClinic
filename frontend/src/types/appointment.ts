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
    availableHours: string[]
}

export interface Appointment {
    id: string
    status: string
    type: string
    mode: string
    startDate: string
    endDate: string
    basePrice: number
    totalPrice: number
    meetingLink: string | null
    arriveTime: string | null
    cabinetNumber: number | null
    homeAddress: {
        country: string
        city: string
        street: string
        building: number
        flat: number | null
        postalCode: string | null
    } | null
    veterinarian: {
        id: string
        fullName: string
        type: string
        clinicVetId: string
    }
    animals: { id: string; name: string; species: string | null; breed: string | null }[]
    treatment: { id: string; type: string; duration: number; price: number } | null
    discounts: { id: string; percentage: number; promoCode: string }[]
}