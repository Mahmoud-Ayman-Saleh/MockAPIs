import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { api } from '../services/api';

export function Projects() {
  const [projects, setProjects] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [newProjectName, setNewProjectName] = useState('');
  const [creating, setCreating] = useState(false);
  const [showModal, setShowModal] = useState(false);

  const fetchProjects = async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await api.get('/api/Project');
      setProjects(res.data);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to load projects.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchProjects();
  }, []);

  const handleCreate = async (e) => {
    e.preventDefault();
    if (!newProjectName.trim()) return;

    setCreating(true);
    try {
      await api.post('/api/Project', newProjectName.trim());
      setNewProjectName('');
      setShowModal(false);
      fetchProjects();
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to create project.');
    } finally {
      setCreating(false);
    }
  };

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
        <div>
          <h1 style={{ fontSize: '20px', fontWeight: 600 }}>Projects</h1>
          <p style={{ fontSize: '13px', color: 'var(--text-muted)' }}>Manage your mock API projects and resource configurations</p>
        </div>
        <button onClick={() => setShowModal(true)} className="btn btn-primary">
          + Create Project
        </button>
      </div>

      {error && <div className="alert alert-danger">{error}</div>}

      {/* Modal / Inline Create Form */}
      {showModal && (
        <div className="card" style={{ borderLeft: '4px solid var(--accent)' }}>
          <h3 style={{ fontSize: '14px', fontWeight: 600, marginBottom: '12px' }}>New Project</h3>
          <form onSubmit={handleCreate} style={{ display: 'flex', gap: '12px' }}>
            <input
              type="text"
              className="form-control"
              placeholder="e.g. E-Commerce Mock API"
              value={newProjectName}
              onChange={(e) => setNewProjectName(e.target.value)}
              autoFocus
              required
            />
            <button type="submit" className="btn btn-primary" disabled={creating}>
              {creating ? 'Creating...' : 'Save'}
            </button>
            <button type="button" onClick={() => setShowModal(false)} className="btn btn-secondary">
              Cancel
            </button>
          </form>
        </div>
      )}

      {loading ? (
        <div className="loading-spinner">Loading projects...</div>
      ) : projects.length === 0 ? (
        <div className="card empty-state">
          <p style={{ fontWeight: 500, marginBottom: '8px' }}>No projects created yet</p>
          <p style={{ fontSize: '13px' }}>Click "+ Create Project" above to build your first mock API.</p>
        </div>
      ) : (
        <div className="table-container">
          <table className="table">
            <thead>
              <tr>
                <th>Project Name</th>
                <th>Slug</th>
                <th>Project Token</th>
                <th>Resources</th>
                <th>Created At</th>
                <th style={{ textAlign: 'right' }}>Actions</th>
              </tr>
            </thead>
            <tbody>
              {projects.map((proj) => (
                <tr key={proj.id}>
                  <td>
                    <Link to={`/projects/${proj.id}`} style={{ fontWeight: 600 }}>
                      {proj.name}
                    </Link>
                  </td>
                  <td><code>{proj.slug}</code></td>
                  <td><code>{proj.token}</code></td>
                  <td>
                    <span className="badge badge-secondary">{proj.resourceCount} resources</span>
                  </td>
                  <td style={{ color: 'var(--text-muted)' }}>
                    {new Date(proj.createdAt).toLocaleDateString()}
                  </td>
                  <td style={{ textAlign: 'right' }}>
                    <Link to={`/projects/${proj.id}`} className="btn btn-secondary btn-sm">
                      Manage →
                    </Link>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
