import { useState, useRef, useEffect } from 'react';

const FAKER_CATEGORIES = [
  {
    name: 'Commerce',
    options: [
      { label: 'Commerce.ProductName', value: 'Commerce.ProductName', desc: 'Product title (e.g. Ergonomic Leather Chair)' },
      { label: 'Commerce.Department', value: 'Commerce.Department', desc: 'Department (e.g. Electronics, Clothing)' },
      { label: 'Commerce.ProductAdjective', value: 'Commerce.ProductAdjective', desc: 'Adjective (e.g. Sleek, Durable)' },
      { label: 'Commerce.ProductMaterial', value: 'Commerce.ProductMaterial', desc: 'Material (e.g. Wooden, Plastic, Steel)' },
      { label: 'Commerce.Categories', value: 'Commerce.Categories', desc: 'Single category name' },
      { label: 'Commerce.Price', value: 'Commerce.Price', desc: 'Price amount (e.g. 149.99)' },
      { label: 'Commerce.Color', value: 'Commerce.Color', desc: 'Color name (e.g. Magenta, Teal)' },
      { label: 'Commerce.Isbn', value: 'Commerce.Isbn', desc: '13-digit EAN/ISBN barcode' },
    ]
  },
  {
    name: 'Finance',
    options: [
      { label: 'Finance.Amount', value: 'Finance.Amount', desc: 'Financial monetary amount' },
      { label: 'Finance.Currency', value: 'Finance.Currency', desc: 'Currency code (USD, EUR)' },
      { label: 'Finance.AccountName', value: 'Finance.AccountName', desc: 'Account type label' },
      { label: 'Finance.AccountNumber', value: 'Finance.AccountNumber', desc: 'Account number string' },
      { label: 'Finance.Bic', value: 'Finance.Bic', desc: 'SWIFT/BIC code' },
      { label: 'Finance.CreditCard', value: 'Finance.CreditCard', desc: 'Card number string' },
      { label: 'Finance.Iban', value: 'Finance.Iban', desc: 'International IBAN string' },
      { label: 'Finance.BitcoinAddress', value: 'Finance.BitcoinAddress', desc: 'Crypto BTC address' },
    ]
  },
  {
    name: 'Internet',
    options: [
      { label: 'Internet.Email', value: 'Internet.Email', desc: 'User email address' },
      { label: 'Internet.Username', value: 'Internet.Username', desc: 'User nickname' },
      { label: 'Internet.Url', value: 'Internet.Url', desc: 'Full website URL' },
      { label: 'Internet.Ip', value: 'Internet.Ip', desc: 'IPv4 address string' },
      { label: 'Internet.Ipv6', value: 'Internet.Ipv6', desc: 'IPv6 address string' },
      { label: 'Internet.Mac', value: 'Internet.Mac', desc: 'Hardware MAC address' },
      { label: 'Internet.UserAgent', value: 'Internet.UserAgent', desc: 'Browser user-agent string' },
      { label: 'Internet.Password', value: 'Internet.Password', desc: 'Random hashed password string' },
      { label: 'Internet.Domain', value: 'Internet.Domain', desc: 'Domain name' },
      { label: 'Internet.Avatar', value: 'Internet.Avatar', desc: 'Profile avatar image URL' },
    ]
  },
  {
    name: 'Name',
    options: [
      { label: 'Name.FullName', value: 'Name.FullName', desc: 'First + Last name' },
      { label: 'Name.FirstName', value: 'Name.FirstName', desc: 'First name' },
      { label: 'Name.LastName', value: 'Name.LastName', desc: 'Last name' },
      { label: 'Name.Prefix', value: 'Name.Prefix', desc: 'Prefix (Dr., Ms., Mr.)' },
      { label: 'Name.Suffix', value: 'Name.Suffix', desc: 'Suffix (Jr., PhD)' },
      { label: 'Name.JobTitle', value: 'Name.JobTitle', desc: 'Professional job title' },
      { label: 'Name.JobDescriptor', value: 'Name.JobDescriptor', desc: 'Job descriptor' },
    ]
  },
  {
    name: 'Address',
    options: [
      { label: 'Address.City', value: 'Address.City', desc: 'City name' },
      { label: 'Address.Country', value: 'Address.Country', desc: 'Country name' },
      { label: 'Address.CountryCode', value: 'Address.CountryCode', desc: 'Country 2-letter code' },
      { label: 'Address.StreetAddress', value: 'Address.StreetAddress', desc: 'Street address' },
      { label: 'Address.ZipCode', value: 'Address.ZipCode', desc: 'Postal zip code' },
      { label: 'Address.State', value: 'Address.State', desc: 'State / Province' },
      { label: 'Address.Latitude', value: 'Address.Latitude', desc: 'GPS Latitude' },
      { label: 'Address.Longitude', value: 'Address.Longitude', desc: 'GPS Longitude' },
      { label: 'Address.BuildingNumber', value: 'Address.BuildingNumber', desc: 'Building / Suite number' },
    ]
  },
  {
    name: 'Company',
    options: [
      { label: 'Company.CompanyName', value: 'Company.CompanyName', desc: 'Business company name' },
      { label: 'Company.CatchPhrase', value: 'Company.CatchPhrase', desc: 'Company slogan' },
      { label: 'Company.Bs', value: 'Company.Bs', desc: 'Business jargon phrase' },
      { label: 'Company.Suffix', value: 'Company.Suffix', desc: 'Inc, LLC, Group' },
    ]
  },
  {
    name: 'System & Database',
    options: [
      { label: 'System.FileName', value: 'System.FileName', desc: 'Filename with extension' },
      { label: 'System.Mime', value: 'System.Mime', desc: 'MIME content-type' },
      { label: 'System.FileType', value: 'System.FileType', desc: 'Extension or filetype' },
      { label: 'System.Semver', value: 'System.Semver', desc: 'Semantic version (1.2.4)' },
      { label: 'Database.Column', value: 'Database.Column', desc: 'Database column name' },
      { label: 'Database.Type', value: 'Database.Type', desc: 'SQL column type' },
      { label: 'Database.Engine', value: 'Database.Engine', desc: 'DB storage engine' },
    ]
  },
  {
    name: 'Hacker & Vehicle',
    options: [
      { label: 'Hacker.Phrase', value: 'Hacker.Phrase', desc: 'Tech jargon sentence' },
      { label: 'Hacker.Noun', value: 'Hacker.Noun', desc: 'Tech noun (protocol, matrix)' },
      { label: 'Vehicle.Vin', value: 'Vehicle.Vin', desc: 'Vehicle VIN string' },
      { label: 'Vehicle.Manufacturer', value: 'Vehicle.Manufacturer', desc: 'Car brand' },
      { label: 'Vehicle.Model', value: 'Vehicle.Model', desc: 'Car model' },
      { label: 'Vehicle.Type', value: 'Vehicle.Type', desc: 'Car body type (SUV, Sedan)' },
    ]
  },
  {
    name: 'Date & Time',
    options: [
      { label: 'Date.Past', value: 'Date.Past', desc: 'Past timestamp' },
      { label: 'Date.Future', value: 'Date.Future', desc: 'Future timestamp' },
      { label: 'Date.Recent', value: 'Date.Recent', desc: 'Recent timestamp' },
      { label: 'Date.Month', value: 'Date.Month', desc: 'Month name' },
      { label: 'Date.Weekday', value: 'Date.Weekday', desc: 'Day of week' },
    ]
  },
  {
    name: 'Lorem',
    options: [
      { label: 'Lorem.Word', value: 'Lorem.Word', desc: 'Single lorem word' },
      { label: 'Lorem.Sentence', value: 'Lorem.Sentence', desc: 'Short sentence' },
      { label: 'Lorem.Paragraph', value: 'Lorem.Paragraph', desc: 'Paragraph block' },
      { label: 'Lorem.Slug', value: 'Lorem.Slug', desc: 'URL slug string' },
    ]
  }
];

export function FakerHintSelect({ value, onChange }) {
  const [isOpen, setIsOpen] = useState(false);
  const [search, setSearch] = useState('');
  const containerRef = useRef(null);

  // Close dropdown on outside click
  useEffect(() => {
    function handleClickOutside(event) {
      if (containerRef.current && !containerRef.current.contains(event.target)) {
        setIsOpen(false);
      }
    }
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const handleSelect = (val) => {
    onChange(val);
    setIsOpen(false);
    setSearch('');
  };

  const handleClear = (e) => {
    e.stopPropagation();
    onChange('');
    setSearch('');
  };

  // Filter categories based on search input
  const filteredCategories = FAKER_CATEGORIES.map((cat) => ({
    name: cat.name,
    options: cat.options.filter(
      (opt) =>
        opt.label.toLowerCase().includes(search.toLowerCase()) ||
        opt.value.toLowerCase().includes(search.toLowerCase()) ||
        (opt.desc && opt.desc.toLowerCase().includes(search.toLowerCase()))
    ),
  })).filter((cat) => cat.options.length > 0);

  const isCustomValue =
    search.trim().length > 0 &&
    !FAKER_CATEGORIES.some((cat) =>
      cat.options.some((opt) => opt.value.toLowerCase() === search.trim().toLowerCase())
    );

  return (
    <div ref={containerRef} style={{ position: 'relative', width: '100%' }}>
      {/* Input / Control Field */}
      <div
        onClick={() => setIsOpen(!isOpen)}
        style={{
          display: 'flex',
          alignItems: 'center',
          justify: 'space-between',
          background: '#ffffff',
          border: isOpen ? '1px solid var(--accent)' : '1px solid var(--border-color)',
          boxShadow: isOpen ? '0 0 0 2px rgba(37, 99, 235, 0.1)' : 'none',
          borderRadius: 'var(--radius)',
          padding: '7px 12px',
          cursor: 'pointer',
          userSelect: 'none',
          fontSize: '13px',
          transition: 'all 0.15s ease',
        }}
      >
        <span style={{ color: value ? 'var(--text-main)' : '#94a3b8', fontFamily: value ? 'var(--font-mono)' : 'inherit', fontSize: value ? '13px' : '13px' }}>
          {value || 'Select or type a Faker generator...'}
        </span>
        <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
          {value && (
            <button
              type="button"
              onClick={handleClear}
              style={{
                border: 'none',
                background: 'transparent',
                color: 'var(--text-muted)',
                cursor: 'pointer',
                fontSize: '14px',
                padding: '0 2px',
                lineHeight: 1,
              }}
              title="Clear selection"
            >
              ✕
            </button>
          )}
          <span style={{ color: 'var(--text-muted)', fontSize: '10px' }}>{isOpen ? '▲' : '▼'}</span>
        </div>
      </div>

      {/* Custom Dropdown Modal */}
      {isOpen && (
        <div
          style={{
            position: 'absolute',
            top: 'calc(100% + 4px)',
            left: 0,
            right: 0,
            zIndex: 100,
            background: '#ffffff',
            border: '1px solid var(--border-color)',
            borderRadius: 'var(--radius)',
            boxShadow: '0 10px 25px -5px rgba(0, 0, 0, 0.1), 0 8px 10px -6px rgba(0, 0, 0, 0.05)',
            maxHeight: '320px',
            display: 'flex',
            flexDirection: 'column',
            overflow: 'hidden',
          }}
        >
          {/* Search Header */}
          <div style={{ padding: '8px', borderBottom: '1px solid var(--border-color)', background: '#f8fafc' }}>
            <input
              type="text"
              autoFocus
              placeholder="Search or type custom hint (e.g. Commerce.ProductName)..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              style={{
                width: '100%',
                padding: '6px 10px',
                fontSize: '13px',
                border: '1px solid var(--border-color)',
                borderRadius: '4px',
                outline: 'none',
              }}
            />
          </div>

          {/* List Content */}
          <div style={{ flex: 1, overflowY: 'auto', padding: '6px 0' }}>
            {/* Custom search option */}
            {isCustomValue && (
              <div
                onClick={() => handleSelect(search.trim())}
                style={{
                  padding: '8px 12px',
                  cursor: 'pointer',
                  fontSize: '13px',
                  background: '#eff6ff',
                  borderBottom: '1px solid #dbeafe',
                  color: '#1d4ed8',
                  fontWeight: 500,
                }}
              >
                ➕ Use custom hint: <code style={{ color: '#1d4ed8', background: '#dbeafe' }}>{search.trim()}</code>
              </div>
            )}

            {filteredCategories.length === 0 && !isCustomValue ? (
              <div style={{ padding: '16px', textAlign: 'center', color: 'var(--text-muted)', fontSize: '13px' }}>
                No matching generator hints found. Type a custom value above.
              </div>
            ) : (
              filteredCategories.map((cat) => (
                <div key={cat.name} style={{ marginBottom: '6px' }}>
                  <div
                    style={{
                      padding: '4px 12px',
                      fontSize: '11px',
                      fontWeight: 700,
                      color: 'var(--text-muted)',
                      textTransform: 'uppercase',
                      letterSpacing: '0.05em',
                      background: '#f8fafc',
                    }}
                  >
                    {cat.name}
                  </div>
                  {cat.options.map((opt) => (
                    <div
                      key={opt.value}
                      onClick={() => handleSelect(opt.value)}
                      style={{
                        padding: '6px 12px',
                        cursor: 'pointer',
                        display: 'flex',
                        flexDirection: 'column',
                        gap: '2px',
                        background: value === opt.value ? '#f1f5f9' : 'transparent',
                        transition: 'background 0.1s ease',
                      }}
                      onMouseEnter={(e) => (e.currentTarget.style.background = '#f8fafc')}
                      onMouseLeave={(e) => (e.currentTarget.style.background = value === opt.value ? '#f1f5f9' : 'transparent')}
                    >
                      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                        <code style={{ fontSize: '12px', fontWeight: 600, color: 'var(--text-main)' }}>{opt.label}</code>
                        {value === opt.value && <span style={{ color: 'var(--accent)', fontSize: '12px' }}>✓</span>}
                      </div>
                      {opt.desc && <span style={{ fontSize: '11px', color: 'var(--text-muted)' }}>{opt.desc}</span>}
                    </div>
                  ))}
                </div>
              ))
            )}
          </div>
        </div>
      )}
    </div>
  );
}
