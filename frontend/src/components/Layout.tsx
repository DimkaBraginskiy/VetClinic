import type { ReactNode } from 'react';
import Header from './Header';
import Footer from './Footer';

interface ContainerProps {
    title?: string;
    children: ReactNode;
}

export default function Layout({ children }: ContainerProps) {
    return (
        <div style={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
            {/* Self-close the Header here so it does not expect children */}
            <Header />

            <main style={{ flex: 1, padding: 0, display: 'flex', flexDirection: 'column' }}>
                {children}
            </main>

            <Footer />
        </div>
    );
}