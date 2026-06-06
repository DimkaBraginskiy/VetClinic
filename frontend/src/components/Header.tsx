import { useLocation, useNavigate } from 'react-router-dom'

export default function Header() {
    const { pathname } = useLocation()
    const navigate     = useNavigate()
    const showUser     = pathname !== '/'

    return (
        <header>
            <h1
                onClick={() => showUser && navigate('/home')}
                style={{ cursor: showUser ? 'pointer' : 'default' }}
            >
                Ilan's Vet Clinic
            </h1>
            {showUser && (
                <button className="header-user-btn" title="Account">
                    <svg xmlns="http://www.w3.org/2000/svg" width="22" height="22"
                         viewBox="0 0 24 24" fill="none" stroke="currentColor"
                         strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                        <circle cx="12" cy="8" r="4" />
                        <path d="M4 20c0-4 3.6-7 8-7s8 3 8 7" />
                    </svg>
                </button>
            )}
        </header>
    )
}