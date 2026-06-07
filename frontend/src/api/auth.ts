const BASE = 'http://localhost:5258/api'

export async function login(email: string, password: string): Promise<void> {
    const res = await fetch(`${BASE}/auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password }),
    })

    if (!res.ok) {
        const data = await res.json().catch(() => ({}))
        throw new Error(data?.message ?? 'Invalid email or password.')
    }

    const data = await res.json()
    localStorage.setItem('loggedIn', 'true')
    if (data.customerId) localStorage.setItem('customerId', data.customerId)
}

export function getCustomerId(): string | null {
    return localStorage.getItem('customerId')
}

export function logout(): void {
    localStorage.removeItem('loggedIn')
    localStorage.removeItem('customerId')
}