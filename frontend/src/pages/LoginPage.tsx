import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import styles from './LoginPage.module.css'

export default function LoginPage() {
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const navigate = useNavigate()

    const handleLogin = async (e: React.SyntheticEvent) => {
        e.preventDefault()
        navigate('/home')
    }

    return (
        <div className={styles.page}>
            <div className={styles.card}>
                <h2 className={styles.title}>Welcome back</h2>
                <p className={styles.subtitle}>Sign in to your account</p>

                <form className={styles.form} onSubmit={handleLogin}>
                    <input
                        className={styles.input}
                        type="email"
                        placeholder="you@example.com"
                        value={email}
                        onChange={e => setEmail(e.target.value)}
                        required
                    />
                    <input
                        className={styles.input}
                        type="password"
                        placeholder="Password"
                        value={password}
                        onChange={e => setPassword(e.target.value)}
                        required
                    />
                    <button className={styles.button} type="submit">
                        Sign in
                    </button>
                </form>
            </div>
        </div>
    )
}