import { useState } from 'react';
import axios from 'axios';

export function MockExplorer() {
  const [token, setToken] = useState('');
  const [resourceSlug, setResourceSlug] = useState('');
  const [recordId, setRecordId] = useState('');

  // Query parameters
  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  const [limit, setLimit] = useState(10);

  // Request Body
  const [jsonBody, setJsonBody] = useState('{\n  "title": "Sample Item",\n  "price": 29.99\n}');

  // Output State
  const [loading, setLoading] = useState(false);
  const [status, setStatus] = useState(null);
  const [responseOutput, setResponseOutput] = useState(null);
  const [error, setError] = useState(null);

  const getBaseUrl = () => {
    const apiBase = import.meta.env.VITE_API_BASE_URL || window.location.origin;
    return `${apiBase}/${token}/api/v1/${resourceSlug}`;
  };

  const handleExecute = async (method) => {
    if (!token.trim() || !resourceSlug.trim()) {
      setError('Please provide both Project Token and Resource Slug.');
      return;
    }

    setError(null);
    setStatus(null);
    setResponseOutput(null);
    setLoading(true);

    const baseUrl = getBaseUrl();
    let url = baseUrl;

    try {
      let res;
      if (method === 'GET_LIST') {
        const params = new URLSearchParams();
        if (search) params.append('search', search);
        if (page) params.append('page', page);
        if (limit) params.append('limit', limit);

        const queryString = params.toString();
        url = queryString ? `${baseUrl}?${queryString}` : baseUrl;
        res = await axios.get(url);
      } else if (method === 'GET_BY_ID') {
        if (!recordId) throw new Error('Record ID is required for GET by ID.');
        url = `${baseUrl}/${recordId}`;
        res = await axios.get(url);
      } else if (method === 'POST') {
        const parsed = JSON.parse(jsonBody);
        res = await axios.post(baseUrl, parsed);
      } else if (method === 'PUT') {
        if (!recordId) throw new Error('Record ID is required for PUT update.');
        url = `${baseUrl}/${recordId}`;
        const parsed = JSON.parse(jsonBody);
        res = await axios.put(url, parsed);
      } else if (method === 'DELETE') {
        if (!recordId) throw new Error('Record ID is required for DELETE.');
        url = `${baseUrl}/${recordId}`;
        res = await axios.delete(url);
      }

      setStatus(res.status);
      setResponseOutput(res.data);
    } catch (err) {
      if (err.response) {
        setStatus(err.response.status);
        setResponseOutput(err.response.data);
      } else {
        setError(err.message || 'Request failed.');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <h1 style={{ fontSize: '20px', fontWeight: 600, marginBottom: '4px' }}>Dynamic Mock Runtime Explorer</h1>
      <p style={{ fontSize: '13px', color: 'var(--text-muted)', marginBottom: '20px' }}>
        Test dynamic mock runtime endpoints directly using your project token and resource slug.
      </p>

      {error && <div className="alert alert-danger">{error}</div>}

      <div className="card">
        <h3 style={{ fontSize: '14px', fontWeight: 600, marginBottom: '14px' }}>Request Configuration</h3>

        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px', marginBottom: '12px' }}>
          <div>
            <label className="form-label">Project Token</label>
            <input
              type="text"
              className="form-control"
              placeholder="e.g. tok_sec_123456"
              value={token}
              onChange={(e) => setToken(e.target.value)}
            />
          </div>
          <div>
            <label className="form-label">Resource Slug</label>
            <input
              type="text"
              className="form-control"
              placeholder="e.g. products, users"
              value={resourceSlug}
              onChange={(e) => setResourceSlug(e.target.value)}
            />
          </div>
        </div>

        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr 1fr', gap: '12px', marginBottom: '16px' }}>
          <div>
            <label className="form-label">Record ID (Guid)</label>
            <input
              type="text"
              className="form-control"
              placeholder="Record Guid for GET/PUT/DELETE"
              value={recordId}
              onChange={(e) => setRecordId(e.target.value)}
            />
          </div>
          <div>
            <label className="form-label">Search Query</label>
            <input
              type="text"
              className="form-control"
              placeholder="Search keyword"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
          </div>
          <div>
            <label className="form-label">Page Number</label>
            <input
              type="number"
              className="form-control"
              value={page}
              onChange={(e) => setPage(e.target.value)}
              min="1"
            />
          </div>
          <div>
            <label className="form-label">Limit per Page</label>
            <input
              type="number"
              className="form-control"
              value={limit}
              onChange={(e) => setLimit(e.target.value)}
              min="1"
            />
          </div>
        </div>

        <div className="form-group" style={{ marginBottom: '16px' }}>
          <label className="form-label">JSON Payload (for POST / PUT)</label>
          <textarea
            className="form-control"
            rows="4"
            value={jsonBody}
            onChange={(e) => setJsonBody(e.target.value)}
            style={{ fontFamily: 'var(--font-mono)' }}
          />
        </div>

        <div style={{ display: 'flex', gap: '8px', flexWrap: 'wrap' }}>
          <button onClick={() => handleExecute('GET_LIST')} className="btn btn-primary" disabled={loading}>
            GET List
          </button>
          <button onClick={() => handleExecute('GET_BY_ID')} className="btn btn-secondary" disabled={loading}>
            GET by ID
          </button>
          <button onClick={() => handleExecute('POST')} className="btn btn-secondary" disabled={loading}>
            POST Create
          </button>
          <button onClick={() => handleExecute('PUT')} className="btn btn-secondary" disabled={loading}>
            PUT Update
          </button>
          <button onClick={() => handleExecute('DELETE')} className="btn btn-danger" disabled={loading}>
            DELETE
          </button>
        </div>
      </div>

      {/* Response Box */}
      {(status !== null || responseOutput !== null || loading) && (
        <div className="card">
          <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '12px' }}>
            <h3 style={{ fontSize: '14px', fontWeight: 600 }}>Response</h3>
            {status && (
              <span className={`badge ${status >= 200 && status < 300 ? 'badge-success' : 'badge-warning'}`}>
                Status: {status}
              </span>
            )}
          </div>

          {loading ? (
            <div className="loading-spinner">Executing request...</div>
          ) : (
            <pre style={{ maxHeight: '400px', overflowY: 'auto', background: '#0f172a', color: '#e2e8f0', padding: '14px', borderRadius: '6px' }}>
              {JSON.stringify(responseOutput, null, 2)}
            </pre>
          )}
        </div>
      )}
    </div>
  );
}
