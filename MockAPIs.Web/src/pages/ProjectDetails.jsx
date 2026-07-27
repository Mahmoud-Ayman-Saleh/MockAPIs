import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { api } from '../services/api';

export function ProjectDetails() {
  const { projectId } = useParams();
  const navigate = useNavigate();

  const [project, setProject] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Rename state
  const [isRenaming, setIsRenaming] = useState(false);
  const [newName, setNewName] = useState('');

  // Create Resource state
  const [showResourceModal, setShowResourceModal] = useState(false);
  const [resourceName, setResourceName] = useState('');
  const [creatingResource, setCreatingResource] = useState(false);

  const fetchProject = async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await api.get(`/api/Project/${projectId}`);
      setProject(res.data);
      setNewName(res.data.name);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to load project details.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (projectId) fetchProject();
  }, [projectId]);

  const handleRename = async (e) => {
    e.preventDefault();
    if (!newName.trim()) return;

    try {
      const res = await api.put(`/api/Project/${projectId}/rename`, newName.trim());
      setProject((prev) => ({ ...prev, ...res.data }));
      setIsRenaming(false);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to rename project.');
    }
  };

  const handleDeleteProject = async () => {
    if (!window.confirm('Are you sure you want to delete this project and all its resources?')) return;

    try {
      await api.delete(`/api/Project/${projectId}`);
      navigate('/projects');
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to delete project.');
    }
  };

  const handleCreateResource = async (e) => {
    e.preventDefault();
    if (!resourceName.trim()) return;

    setCreatingResource(true);
    try {
      await api.post(`/api/Resource/${projectId}`, resourceName.trim());
      setResourceName('');
      setShowResourceModal(false);
      fetchProject();
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to create resource.');
    } finally {
      setCreatingResource(false);
    }
  };

  const handleDeleteResource = async (resourceId) => {
    if (!window.confirm('Are you sure you want to delete this resource?')) return;

    try {
      await api.delete(`/api/Resource/${resourceId}`);
      fetchProject();
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to delete resource.');
    }
  };

  if (loading) return <div className="loading-spinner">Loading project...</div>;
  if (!project && error) return <div className="alert alert-danger">{error}</div>;

  return (
    <div>
      <div style={{ marginBottom: '16px' }}>
        <Link to="/projects" style={{ fontSize: '13px', color: 'var(--text-muted)' }}>
          ← Back to Projects
        </Link>
      </div>

      {error && <div className="alert alert-danger">{error}</div>}

      {/* Project Header */}
      <div className="card">
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
          <div>
            {isRenaming ? (
              <form onSubmit={handleRename} style={{ display: 'flex', gap: '8px', marginBottom: '8px' }}>
                <input
                  type="text"
                  className="form-control"
                  value={newName}
                  onChange={(e) => setNewName(e.target.value)}
                  required
                />
                <button type="submit" className="btn btn-primary btn-sm">
                  Save
                </button>
                <button type="button" onClick={() => setIsRenaming(false)} className="btn btn-secondary btn-sm">
                  Cancel
                </button>
              </form>
            ) : (
              <div style={{ display: 'flex', alignItems: 'center', gap: '12px', marginBottom: '8px' }}>
                <h1 style={{ fontSize: '20px', fontWeight: 600 }}>{project.name}</h1>
                <button onClick={() => setIsRenaming(true)} className="btn btn-secondary btn-sm">
                  Rename
                </button>
              </div>
            )}
            <p style={{ fontSize: '13px', color: 'var(--text-muted)' }}>
              Project Token: <code>{project.token}</code>
            </p>
          </div>

          <button onClick={handleDeleteProject} className="btn btn-danger btn-sm">
            Delete Project
          </button>
        </div>

        <div style={{ marginTop: '16px', paddingTop: '16px', borderTop: '1px solid var(--border-color)', fontSize: '13px' }}>
          <strong>Mock API Base URL:</strong>
          <pre style={{ marginTop: '6px', display: 'inline-block', width: '100%', overflowX: 'auto' }}>
            {project.baseUrl}
          </pre>
        </div>
      </div>

      {/* Resources Header */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', margin: '24px 0 16px' }}>
        <h2 style={{ fontSize: '16px', fontWeight: 600 }}>Resources</h2>
        <button onClick={() => setShowResourceModal(true)} className="btn btn-primary btn-sm">
          + Add Resource
        </button>
      </div>

      {/* Create Resource Inline Card */}
      {showResourceModal && (
        <div className="card" style={{ borderLeft: '4px solid var(--accent)' }}>
          <h3 style={{ fontSize: '14px', fontWeight: 600, marginBottom: '12px' }}>New Resource</h3>
          <form onSubmit={handleCreateResource} style={{ display: 'flex', gap: '12px' }}>
            <input
              type="text"
              className="form-control"
              placeholder="e.g. Products, Users, Orders"
              value={resourceName}
              onChange={(e) => setResourceName(e.target.value)}
              autoFocus
              required
            />
            <button type="submit" className="btn btn-primary" disabled={creatingResource}>
              {creatingResource ? 'Saving...' : 'Save'}
            </button>
            <button type="button" onClick={() => setShowResourceModal(false)} className="btn btn-secondary">
              Cancel
            </button>
          </form>
        </div>
      )}

      {/* Resources Table */}
      {project.resources.length === 0 ? (
        <div className="card empty-state">
          <p style={{ fontWeight: 500, marginBottom: '8px' }}>No resources configured</p>
          <p style={{ fontSize: '13px' }}>Click "+ Add Resource" above to add your first resource schema.</p>
        </div>
      ) : (
        <div className="table-container">
          <table className="table">
            <thead>
              <tr>
                <th>Resource Name</th>
                <th>Slug</th>
                <th>Records Count</th>
                <th>Endpoint URL</th>
                <th style={{ textAlign: 'right' }}>Actions</th>
              </tr>
            </thead>
            <tbody>
              {project.resources.map((res) => (
                <tr key={res.id}>
                  <td>
                    <Link to={`/projects/${project.id}/resources/${res.id}`} style={{ fontWeight: 600 }}>
                      {res.name}
                    </Link>
                  </td>
                  <td><code>{res.slug}</code></td>
                  <td>
                    <span className="badge badge-secondary">{res.count} records</span>
                  </td>
                  <td>
                    <code>{project.baseUrl}/{res.slug}</code>
                  </td>
                  <td style={{ textAlign: 'right', display: 'flex', gap: '8px', justifyContent: 'flex-end' }}>
                    <Link to={`/projects/${project.id}/resources/${res.id}`} className="btn btn-secondary btn-sm">
                      Configure Fields & Config →
                    </Link>
                    <button onClick={() => handleDeleteResource(res.id)} className="btn btn-danger btn-sm">
                      Delete
                    </button>
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
