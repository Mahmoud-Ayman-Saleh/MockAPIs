### Auth
- `POST` `/auth/register`
- `POST` `/auth/login`
- `POST` `/auth/refresh-token`

### Projects
- `GET` `/projects`
- `GET` `/projects/{id}`
- `POST` `/projects`
- `PUT` `/projects/{id}/rename`
- `DELETE` `/projects/{id}`

### Resources
- `POST` `/resources`
- `DELETE` `/resources/{id}`

### Fields
- `GET` `/resources/{resourceId}/fields`
- `POST` `/resources/{resourceId}/fields`
- `PUT` `/resources/{resourceId}/fields/{fieldId}`
- `DELETE` `/resources/{resourceId}/fields/{fieldId}`

### Endpoint Config
- `PUT` `/resources/{resourceId}/endpoint-config`

### Data
- `GET` `/resources/{id}/preview`
- `POST` `/resources/{id}/generate`

---

### Mock Runtime *(different controller)*
- `GET` `/{token}/api/v1/{resource}`
- `GET` `/{token}/api/v1/{resource}/{id}`
- `POST` `/{token}/api/v1/{resource}`
- `PUT` `/{token}/api/v1/{resource}/{id}`
- `DELETE` `/{token}/api/v1/{resource}/{id}`