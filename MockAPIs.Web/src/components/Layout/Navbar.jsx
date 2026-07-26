import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

export function Navbar() {
  const { user, isAuthenticated, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <header style={{ background: '#ffffff', borderBottom: '1px solid var(--border-color)', padding: '12px 24px' }}>
      <div style={{ maxWidth: '1200px', margin: '0 auto', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '20px' }}>
          <Link to="/" style={{ fontWeight: 700, fontSize: '16px', color: 'var(--primary)', textDecoration: 'none' }}>
            MockAPIs Admin
          </Link>
          {isAuthenticated && (
            <nav style={{ display: 'flex', gap: '16px', fontSize: '13px' }}>
              <Link to="/projects" style={{ color: 'var(--text-main)', textDecoration: 'none' }}>
                Projects
              </Link>
            </nav>
          )}
        </div>

        <div>
          {isAuthenticated ? (
            <div style={{ display: 'flex', alignItems: 'center', gap: '16px' }}>
              <span style={{ fontSize: '13px', color: 'var(--text-muted)' }}>
                User: <strong>{user?.userName || user?.email}</strong>
              </span>
              <button onClick={handleLogout} className="btn btn-secondary btn-sm">
                Sign Out
              </button>
            </div>
          ) : (
            <div style={{ display: 'flex', gap: '10px' }}>
              <Link to="/login" className="btn btn-secondary btn-sm">
                Sign In
              </Link>
              <Link to="/register" className="btn btn-primary btn-sm">
                Register
              </Link>
            </div>
          )}
        </div>
      </div>
    </header>
  );
}
