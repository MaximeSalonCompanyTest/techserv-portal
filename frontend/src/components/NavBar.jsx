import { Link, useNavigate } from 'react-router-dom';
import { getName, getRole, logout } from '../api';

export default function NavBar() {
  const navigate = useNavigate();

  // Logout only happens in the browser: delete the token, then go to the login page.
  function handleLogout() {
    logout();
    navigate('/login');
  }

  return (
    <nav className="navbar">
      <strong>TechServ</strong>
      {/* Agents and customers see different links, based on the role saved at login. */}
      {getRole() === 'Agent' ? (
        <Link to="/agent">Todas as solicitações</Link>
      ) : (
        <>
          <Link to="/requests">Minhas solicitações</Link>
          <Link to="/requests/new">Nova solicitação</Link>
        </>
      )}
      <Link to="/profile">Meu perfil</Link>
      <span className="navbar-user">{getName()}</span>
      <button onClick={handleLogout}>Sair</button>
    </nav>
  );
}
