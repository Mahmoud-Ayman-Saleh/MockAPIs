import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { api } from '../services/api';

const API_BASE = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5167';

export function ResourceDetails() {
  const { projectId, resourceId } = useParams();

  const [fields, setFields] = useState([]);
  const [loadingFields, setLoadingFields] = useState(true);
  const [error, setError] = useState(null);
  const [successMsg, setSuccessMsg] = useState(null);

  // Project/Resource metadata for endpoint examples
  const [projectToken, setProjectToken] = useState('');
  const [resourceSlug, setResourceSlug] = useState('');

  // Field Form Modal State
  const [showFieldModal, setShowFieldModal] = useState(false);
  const [editingFieldId, setEditingFieldId] = useState(null);
  const [fieldName, setFieldName] = useState('');
  const [dataType, setDataType] = useState('String');
  const [fakerHint, setFakerHint] = useState('');
  const [isRequired, setIsRequired] = useState(false);

  // Endpoint Config State
  const [config, setConfig] = useState({
    getList: true,
    getById: true,
    post: true,
    put: true,
    delete: true,
    enablePagination: true,
    enableSearch: true,
  });

  // Data Generation & Preview State
  const [previewData, setPreviewData] = useState(null);
  const [loadingPreview, setLoadingPreview] = useState(false);
  const [generateCount, setGenerateCount] = useState(20);
  const [generating, setGenerating] = useState(false);

  // Fetch project token and resource slug for endpoint examples
  useEffect(() => {
    const fetchMeta = async () => {
      try {
        const [projRes, resRes] = await Promise.all([
          api.get(`/api/Project/${projectId}`),
          api.get(`/api/resources/${resourceId}/details`),
        ]);
        setProjectToken(projRes.data.token);
        setResourceSlug(resRes.data.slug);
      } catch {
        // Non-critical — endpoint examples will fall back to placeholders
      }
    };
    fetchMeta();
  }, [projectId, resourceId]);

  const fetchFields = async () => {
    setLoadingFields(true);
    try {
      const res = await api.get(`/api/resources/${resourceId}/fields`);
      setFields(res.data);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to fetch resource fields.');
    } finally {
      setLoadingFields(false);
    }
  };

  useEffect(() => {
    if (resourceId) fetchFields();
  }, [resourceId]);

  const openCreateModal = () => {
    setEditingFieldId(null);
    setFieldName('');
    setDataType('String');
    setFakerHint('');
    setIsRequired(false);
    setShowFieldModal(true);
  };

  const openEditModal = (field) => {
    setEditingFieldId(field.id);
    setFieldName(field.name);
    setDataType(field.dataType);
    setFakerHint(field.fakerHint || '');
    setIsRequired(field.isRequired);
    setShowFieldModal(true);
  };

  const handleDataTypeChange = (newType) => {
    setDataType(newType);
    // Only String type supports faker hints
    if (newType !== 'String') {
      setFakerHint('');
    }
  };

  const handleSaveField = async (e) => {
    e.preventDefault();
    setError(null);
    const dto = { name: fieldName, dataType, fakerHint: fakerHint || null, isRequired };

    try {
      if (editingFieldId) {
        await api.put(`/api/resources/${resourceId}/fields/${editingFieldId}`, dto);
      } else {
        await api.post(`/api/resources/${resourceId}/fields`, dto);
      }
      setShowFieldModal(false);
      fetchFields();
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to save field definition.');
    }
  };

  const handleDeleteField = async (fieldId) => {
    if (!window.confirm('Delete this field definition?')) return;
    try {
      await api.delete(`/api/resources/${resourceId}/fields/${fieldId}`);
      fetchFields();
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to delete field.');
    }
  };

  const handleSaveConfig = async () => {
    setError(null);
    try {
      const res = await api.put(`/api/resources/${resourceId}/endpoint-config`, config);
      setConfig(res.data);
      setSuccessMsg('Endpoint configuration saved successfully.');
      setTimeout(() => setSuccessMsg(null), 3000);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to update endpoint configuration.');
    }
  };

  const handleFetchPreview = async () => {
    setLoadingPreview(true);
    try {
      const res = await api.get(`/api/resources/${resourceId}/preview`);
      setPreviewData(res.data.preview);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to generate preview data.');
    } finally {
      setLoadingPreview(false);
    }
  };

  const handleGenerateData = async () => {
    setGenerating(true);
    setError(null);
    try {
      const res = await api.post(`/api/resources/${resourceId}/generate`, { count: parseInt(generateCount, 10) });
      setSuccessMsg(`Successfully generated ${res.data.generatedCount} records.`);
      setTimeout(() => setSuccessMsg(null), 3000);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to bulk generate data.');
    } finally {
      setGenerating(false);
    }
  };

  return (
    <div>
      <div style={{ marginBottom: '16px' }}>
        <Link to={`/projects/${projectId}`} style={{ fontSize: '13px', color: 'var(--text-muted)' }}>
          ← Back to Project Details
        </Link>
      </div>

      {error && <div className="alert alert-danger">{error}</div>}
      {successMsg && <div className="alert alert-info">{successMsg}</div>}

      {/* Fields Section */}
      <div className="card">
        <div className="card-header">
          <h2 className="card-title">Schema Fields</h2>
          <button onClick={openCreateModal} className="btn btn-primary btn-sm">
            + Add Field
          </button>
        </div>

        {showFieldModal && (
          <form onSubmit={handleSaveField} style={{ background: '#f8fafc', padding: '16px', borderRadius: '6px', marginBottom: '16px', border: '1px solid var(--border-color)' }}>
            <h4 style={{ marginBottom: '12px', fontSize: '14px' }}>{editingFieldId ? 'Edit Field' : 'Add New Field'}</h4>
            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px', marginBottom: '12px' }}>
              <div>
                <label className="form-label">Field Name</label>
                <input type="text" className="form-control" value={fieldName} onChange={(e) => setFieldName(e.target.value)} required />
              </div>
              <div>
                <label className="form-label">Data Type</label>
                <select className="form-control" value={dataType} onChange={(e) => handleDataTypeChange(e.target.value)}>
                  <option value="String">String</option>
                  <option value="Number">Number</option>
                  <option value="Boolean">Boolean</option>
                  <option value="Date">Date</option>
                  <option value="Image">Image</option>
                  <option value="UUID">UUID</option>
                  <option value="Email">Email</option>
                  <option value="Name">Name</option>
                  <option value="Price">Price</option>
                  <option value="Paragraph">Paragraph</option>
                </select>
              </div>
            </div>
            {dataType === 'String' && (
              <div className="form-group">
                <label className="form-label">Faker Hint</label>
                <select className="form-control" value={fakerHint} onChange={(e) => setFakerHint(e.target.value)}>
                  <option value="">— None (random word) —</option>
                  <optgroup label="Commerce">
                    <option value="Commerce.ProductName">Commerce.ProductName</option>
                    <option value="Commerce.Department">Commerce.Department</option>
                    <option value="Commerce.ProductAdjective">Commerce.ProductAdjective</option>
                    <option value="Commerce.ProductMaterial">Commerce.ProductMaterial</option>
                    <option value="Commerce.Categories">Commerce.Categories</option>
                  </optgroup>
                  <optgroup label="Finance">
                    <option value="Finance.Amount">Finance.Amount</option>
                    <option value="Finance.Currency">Finance.Currency</option>
                    <option value="Finance.AccountName">Finance.AccountName</option>
                  </optgroup>
                  <optgroup label="Internet">
                    <option value="Internet.Email">Internet.Email</option>
                    <option value="Internet.Username">Internet.Username</option>
                    <option value="Internet.Url">Internet.Url</option>
                    <option value="Internet.Ip">Internet.Ip</option>
                    <option value="Internet.UserAgent">Internet.UserAgent</option>
                  </optgroup>
                  <optgroup label="Image">
                    <option value="Image.Url">Image.Url</option>
                  </optgroup>
                  <optgroup label="Name">
                    <option value="Name.FullName">Name.FullName</option>
                    <option value="Name.FirstName">Name.FirstName</option>
                    <option value="Name.LastName">Name.LastName</option>
                    <option value="Name.Prefix">Name.Prefix</option>
                  </optgroup>
                  <optgroup label="Address">
                    <option value="Address.City">Address.City</option>
                    <option value="Address.Country">Address.Country</option>
                    <option value="Address.StreetAddress">Address.StreetAddress</option>
                    <option value="Address.ZipCode">Address.ZipCode</option>
                    <option value="Address.State">Address.State</option>
                  </optgroup>
                  <optgroup label="Phone">
                    <option value="Phone.PhoneNumber">Phone.PhoneNumber</option>
                  </optgroup>
                  <optgroup label="Company">
                    <option value="Company.CompanyName">Company.CompanyName</option>
                    <option value="Company.CatchPhrase">Company.CatchPhrase</option>
                    <option value="Company.Bs">Company.Bs</option>
                  </optgroup>
                  <optgroup label="Lorem">
                    <option value="Lorem.Word">Lorem.Word</option>
                    <option value="Lorem.Sentence">Lorem.Sentence</option>
                    <option value="Lorem.Paragraph">Lorem.Paragraph</option>
                  </optgroup>
                  <optgroup label="Date">
                    <option value="Date.Past">Date.Past</option>
                    <option value="Date.Future">Date.Future</option>
                    <option value="Date.Recent">Date.Recent</option>
                  </optgroup>
                  <optgroup label="Random">
                    <option value="Random.Number">Random.Number</option>
                    <option value="Random.Bool">Random.Bool</option>
                    <option value="Random.UUID">Random.UUID</option>
                  </optgroup>
                </select>
              </div>
            )}
            <div className="form-group" style={{ marginBottom: '12px' }}>
              <label className="form-checkbox">
                <input type="checkbox" checked={isRequired} onChange={(e) => setIsRequired(e.target.checked)} />
                Is Required
              </label>
            </div>
            <div style={{ display: 'flex', gap: '8px' }}>
              <button type="submit" className="btn btn-primary btn-sm">Save Field</button>
              <button type="button" onClick={() => setShowFieldModal(false)} className="btn btn-secondary btn-sm">Cancel</button>
            </div>
          </form>
        )}

        {loadingFields ? (
          <div className="loading-spinner">Loading fields...</div>
        ) : fields.length === 0 ? (
          <div className="empty-state">No fields defined yet. Click "+ Add Field" to configure the resource schema.</div>
        ) : (
          <div className="table-container">
            <table className="table">
              <thead>
                <tr>
                  <th>Field Name</th>
                  <th>Data Type</th>
                  <th>Faker Hint</th>
                  <th>Required</th>
                  <th style={{ textAlign: 'right' }}>Actions</th>
                </tr>
              </thead>
              <tbody>
                {fields.map((f) => (
                  <tr key={f.id}>
                    <td><strong>{f.name}</strong></td>
                    <td><code>{f.dataType}</code></td>
                    <td>{f.fakerHint ? <code>{f.fakerHint}</code> : <span style={{ color: 'var(--text-muted)' }}>-</span>}</td>
                    <td>{f.isRequired ? <span className="badge badge-warning">Required</span> : <span className="badge badge-secondary">Optional</span>}</td>
                    <td style={{ textAlign: 'right' }}>
                      <button onClick={() => openEditModal(f)} className="btn btn-secondary btn-sm" style={{ marginRight: '6px' }}>Edit</button>
                      <button onClick={() => handleDeleteField(f.id)} className="btn btn-danger btn-sm">Delete</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Endpoint Settings */}
      <div className="card">
        <h2 className="card-title" style={{ marginBottom: '4px' }}>Endpoint Configuration</h2>
        <p style={{ fontSize: '12px', color: 'var(--text-muted)', marginBottom: '16px' }}>
          Toggle which HTTP methods are available on your mock API.
        </p>

        <div style={{ display: 'flex', flexDirection: 'column', gap: '10px', marginBottom: '16px' }}>
          {[
            { key: 'getList',  label: 'GET List',    method: 'GET',    path: `${API_BASE}/${projectToken || '{token}'}/api/v1/${resourceSlug || '{resource}'}` },
            { key: 'getById',  label: 'GET by ID',   method: 'GET',    path: `${API_BASE}/${projectToken || '{token}'}/api/v1/${resourceSlug || '{resource}'}/{id}` },
            { key: 'post',     label: 'POST Create', method: 'POST',   path: `${API_BASE}/${projectToken || '{token}'}/api/v1/${resourceSlug || '{resource}'}` },
            { key: 'put',      label: 'PUT Update',  method: 'PUT',    path: `${API_BASE}/${projectToken || '{token}'}/api/v1/${resourceSlug || '{resource}'}/{id}` },
            { key: 'delete',   label: 'DELETE',      method: 'DELETE', path: `${API_BASE}/${projectToken || '{token}'}/api/v1/${resourceSlug || '{resource}'}/{id}` },
          ].map(({ key, label, method, path }) => (
            <div key={key} style={{ background: '#f8fafc', borderRadius: '6px', border: '1px solid var(--border-color)', padding: '10px 12px' }}>
              <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: '6px' }}>
                <label className="form-checkbox" style={{ marginBottom: 0 }}>
                  <input type="checkbox" checked={config[key]} onChange={(e) => setConfig({ ...config, [key]: e.target.checked })} /> {label}
                </label>
                <span style={{ fontSize: '11px', fontWeight: 600, color: method === 'GET' ? '#16a34a' : method === 'POST' ? '#2563eb' : method === 'PUT' ? '#d97706' : '#dc2626', textTransform: 'uppercase' }}>{method}</span>
              </div>
              <code style={{ fontSize: '11px', color: 'var(--text-secondary)', wordBreak: 'break-all', display: 'block' }}>{path}</code>
            </div>
          ))}

          <div style={{ display: 'flex', gap: '10px' }}>
            <div style={{ flex: 1, background: '#f8fafc', borderRadius: '6px', border: '1px solid var(--border-color)', padding: '10px 12px' }}>
              <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: '6px' }}>
                <label className="form-checkbox" style={{ marginBottom: 0 }}>
                  <input type="checkbox" checked={config.enablePagination} onChange={(e) => setConfig({ ...config, enablePagination: e.target.checked })} /> Pagination
                </label>
              </div>
              <code style={{ fontSize: '11px', color: 'var(--text-secondary)', display: 'block' }}>?page=1&limit=10</code>
            </div>
            <div style={{ flex: 1, background: '#f8fafc', borderRadius: '6px', border: '1px solid var(--border-color)', padding: '10px 12px' }}>
              <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: '6px' }}>
                <label className="form-checkbox" style={{ marginBottom: 0 }}>
                  <input type="checkbox" checked={config.enableSearch} onChange={(e) => setConfig({ ...config, enableSearch: e.target.checked })} /> Search
                </label>
              </div>
              <code style={{ fontSize: '11px', color: 'var(--text-secondary)', display: 'block' }}>?search=keyword</code>
            </div>
          </div>
        </div>

        <button onClick={handleSaveConfig} className="btn btn-primary btn-sm">Save Config</button>
      </div>

      {/* Data Generator & Preview */}
      <div className="card">
        <h2 className="card-title" style={{ marginBottom: '16px' }}>Data Generation & Live Preview</h2>
        <div style={{ display: 'flex', gap: '16px', alignItems: 'center', marginBottom: '16px' }}>
          <button onClick={handleFetchPreview} className="btn btn-secondary" disabled={loadingPreview}>
            {loadingPreview ? 'Loading Preview...' : '🔍 Live Sample Preview'}
          </button>

          <div style={{ display: 'flex', gap: '8px', alignItems: 'center' }}>
            <input type="number" className="form-control" style={{ width: '90px' }} value={generateCount} onChange={(e) => setGenerateCount(e.target.value)} min="1" max="500" />
            <button onClick={handleGenerateData} className="btn btn-primary" disabled={generating}>
              {generating ? 'Generating...' : '⚡ Bulk Generate Records'}
            </button>
          </div>
        </div>

        {previewData && (
          <div style={{ marginTop: '12px' }}>
            <h4 style={{ fontSize: '13px', fontWeight: 600, marginBottom: '8px' }}>Sample Output JSON:</h4>
            <pre style={{ maxHeight: '300px', overflowY: 'auto', background: '#0f172a', color: '#e2e8f0', padding: '12px', borderRadius: '6px' }}>
              {JSON.stringify(previewData, null, 2)}
            </pre>
          </div>
        )}
      </div>
    </div>
  );
}
