import { useState, type ReactNode } from 'react'
import { useNavigate, useLocation } from 'react-router-dom'
import styles from './AppLayout.module.css'

interface Props { children: ReactNode }

export default function AppLayout({ children }: Props) {
    const [open, setOpen] = useState(false)
    const navigate        = useNavigate()
    const { pathname }    = useLocation()

    const go = (path: string) => {
        navigate(path)
        setOpen(false)
    }

    return (
        <div className={styles.layout}>

            {/* ── Sidebar ── */}
            <nav className={`${styles.sidebar} ${open ? styles.open : ''}`}>
                <div className={styles.sidebarInner}>

                    <button className={styles.hamburger} onClick={() => setOpen(o => !o)}>
                        <span className={styles.bar} />
                        <span className={styles.bar} />
                        <span className={styles.bar} />
                    </button>

                    <div className={styles.divider} />

                    <button
                        className={`${styles.navBtn} ${pathname === '/home' ? styles.active : ''}`}
                        onClick={() => go('/home')}
                    >
                        Home
                    </button>
                    <button
                        className={`${styles.navBtn} ${pathname === '/appointments' ? styles.active : ''}`}
                        onClick={() => go('/appointments')}
                    >
                        My Appointments
                    </button>
                    <button
                        className={`${styles.navBtn} ${pathname === '/animals' ? styles.active : ''}`}
                        onClick={() => go('/animals')}
                    >
                        My Animals
                    </button>

                </div>
            </nav>

            {/* ── Page content ── */}
            <div className={styles.content}>
                {children}
            </div>

        </div>
    )
}