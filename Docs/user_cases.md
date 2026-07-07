# MockAPIs — User Cases

## Who Uses MockAPIs

| User | Role | Goal |
|---|---|---|
| **Ahmed** | Full-stack developer | Build a personal project without writing backend first |
| **Sara** | Frontend developer at a startup | Start building UI before the backend team is done |
| **Karim** | Team lead | Give his 3 frontend devs a shared consistent mock API |
| **Nour** | Junior developer | Learn how REST APIs work by calling real endpoints |

---

## Use Case 1 — First Time User (Ahmed)

**Situation:** Ahmed wants to build an e-commerce frontend but has no backend yet.

```mermaid
journey
    title Ahmed's Journey — First Time User
    section Discovery
      Visit homepage: 5: Ahmed
      Read the hero section: 4: Ahmed
      Click Get Started Free: 5: Ahmed
    section Onboarding
      Login with Google: 5: Ahmed
      Redirected to Dashboard: 4: Ahmed
    section Building
      Create E-Commerce API project: 5: Ahmed
      Create Products resource: 5: Ahmed
      Add fields title price imageUrl: 4: Ahmed
      Check Data Preview: 5: Ahmed
      Generate 20 records: 5: Ahmed
    section Using
      Copy API URL: 5: Ahmed
      Call endpoint in React app: 5: Ahmed
      Frontend works immediately: 5: Ahmed
```

### Step-by-step flow

```mermaid
flowchart TD
    A([Ahmed visits mocknest.io]) --> B[Clicks Get Started Free]
    B --> C[Login with Google OAuth]
    C --> D{New user?}
    D -->|Yes| E[Create AppUser\nEmail: ahmed@gmail.com\nPlan: Free]
    D -->|No| F[Load existing user]
    E & F --> G[Set session cookie\nRedirect to Dashboard]
    G --> H[Dashboard empty\nNo projects yet]
    H --> I[Click + New Project\nType: E-Commerce API]
    I --> J["POST /api/projects\n{ name: E-Commerce API }"]
    J --> K["System generates:\nslug: e-commerce-api\ntoken: 69daa954...\nbaseUrl: mockapis.io/69daa954.../api/v1"]
    K --> L[Project card appears\non dashboard]
    L --> M[Open project → workspace]
    M --> N[Click + Add Resource\nType: Products]
    N --> O["POST /api/resources\n{ projectId, name: Products }"]
    O --> P["Resource created\nslug: products\nAll endpoints enabled by default"]
    P --> Q[Add 4 fields\ntitle · price · imageUrl · inStock]
    Q --> R[Click Data Preview tab]
    R --> S["GET /resources/id/preview\nFakerEngine generates 3 samples\nnot saved to DB"]
    S --> T{Looks good?}
    T -->|No| Q
    T -->|Yes| U["Generate Data\nPOST /resources/id/generate\n{ count: 20 }"]
    U --> V[20 records saved to DB\nResource.Count = 20]
    V --> W[Copy URL from API Access tab]
    W --> X[Paste URL into React app]
    X --> Y([Frontend works immediately\nno backend code written])
```

---

## Use Case 2 — Frontend Developer Using a Shared URL (Sara)

**Situation:** Sara's team lead sends her a MockAPIs base URL. She uses it directly — no account needed.

```mermaid
sequenceDiagram
    participant K as Karim (Lead)
    participant S as Sara (Frontend Dev)
    participant API as MockAPIs Runtime

    K->>S: Share base URL\nhttps://mockapis.io/91fbb.../api/v1

    S->>API: GET /91fbb.../api/v1/users
    API-->>S: [{ id, name, email }, ...]

    S->>API: GET /91fbb.../api/v1/users/uuid-1
    API-->>S: { id, name, email }

    S->>API: POST /91fbb.../api/v1/users\n{ name: Sara Ali, email: sara@example.com }
    API-->>S: 201 { id: uuid-new, name: Sara Ali, email: ... }

    S->>API: GET /91fbb.../api/v1/users?page=1&limit=5
    API-->>S: { data: [...], page: 1, limit: 5, total: 30, totalPages: 6 }

    S->>API: GET /91fbb.../api/v1/users?search=sara
    API-->>S: [{ id: uuid-new, name: Sara Ali }]

    note over S: Sara never logs in to MockAPIs\nJust uses the URLs directly
```

---

## Use Case 3 — Updating the Schema Mid-Project (Ahmed)

**Situation:** Ahmed realizes he needs a `category` field on products after already generating data.

```mermaid
flowchart TD
    A[Ahmed opens Products → Schema tab] --> B["Click + Add Field\nName: category\nDataType: String\nFakerHint: commerce.department"]
    B --> C["POST /resources/id/fields\n{ name: category, dataType: String }"]
    C --> D[Field saved to DB]
    D --> E[Switch to Data Preview tab]
    E --> F["GET /resources/id/preview\nFakerEngine re-runs with new field"]
    F --> G["Preview now shows:\n{ title, price, imageUrl, inStock, category: Electronics }"]
    G --> H{Looks good?}
    H -->|No| B
    H -->|Yes| I["Click Generate Data\nPOST /resources/id/generate\n{ count: 20 }"]
    I --> J[Delete old 20 records\nwithout category field]
    J --> K[Insert 20 new records\nwith category field]
    K --> L["All future GET /products\nnow include category field"]
```

---

## Use Case 4 — Restricting Endpoints (Karim the Team Lead)

**Situation:** Karim wants his frontend team to only GET data, not POST or DELETE.

```mermaid
flowchart TD
    A[Karim opens Orders resource\nEndpoints tab] --> B["Toggle settings:\nGET /orders ✅\nGET /orders/:id ✅\nPOST /orders ❌\nPUT /orders/:id ❌\nDELETE /orders/:id ❌\nPagination ✅\nSearch ❌"]
    B --> C["PUT /resources/id/endpoint-config\n{ getList: true, getById: true,\npost: false, put: false, delete: false,\nenablePagination: true }"]
    C --> D[Config saved to DB]

    D --> E[Frontend dev tries\nPOST /orders]
    E --> F[MockRuntimeService\nchecks EndpointConfig.Post]
    F --> G{Post enabled?}
    G -->|No| H["throw MethodNotAllowedException\n→ ExceptionMiddleware\n→ 405 + message"]
    G -->|Yes| I[would proceed normally]
```

---

## Use Case 5 — Renaming and Deleting (Ahmed)

```mermaid
flowchart TD
    subgraph Rename["Renaming the project"]
        A[Three-dot menu → Rename] --> B["Type: Shop API"]
        B --> C["PUT /api/projects/id/rename\n{ name: Shop API }"]
        C --> D["System generates new slug:\nshop-api\nChecks global uniqueness"]
        D --> E{Slug unique?}
        E -->|No| F[shop-api-1]
        F --> E
        E -->|Yes| G[Update project\nname + slug in DB]
        G --> H[Dashboard card shows\nShop API]
    end

    subgraph DeleteResource["Deleting a Resource"]
        I[Right-click Reviews → Delete] --> J[Confirmation modal]
        J --> K["DELETE /api/resources/id"]
        K --> L["IsFieldOwnedByUserAsync\nAnyAsync EXISTS query"]
        L --> M{Owned?}
        M -->|No| N[404 Not Found]
        M -->|Yes| O[Delete resource from DB]
        O --> P["PostgreSQL cascade:\n→ DELETE Fields\n→ DELETE MockRecords\n→ DELETE EndpointConfig"]
        P --> Q[Sidebar no longer\nshows Reviews]
    end
```

---

## Use Case 6 — Deleting an Entire Project (Ahmed)

```mermaid
flowchart TD
    A[Dashboard → three-dot menu\n→ Delete Shop API] --> B[Confirmation modal\nThis deletes everything]
    B --> C["DELETE /api/projects/id"]
    C --> D[Verify Ahmed owns project\nAnyAsync EXISTS query]
    D --> E{Owned?}
    E -->|No| F[404 Not Found]
    E -->|Yes| G[Delete project]
    G --> H["PostgreSQL cascade:\nResources deleted\n→ Fields deleted\n→ MockRecords deleted\n→ EndpointConfigs deleted"]
    H --> I[Dashboard empty again]
    I --> J["Any call to\nhttps://mockapis.io/69daa95.../api/v1/products\n→ 404 token not found"]
```

---

## Use Case 7 — Junior Dev Tests a Disabled Endpoint (Nour)

```mermaid
sequenceDiagram
    participant N as Nour (Junior Dev)
    participant CTR as MockRuntimeController
    participant SVC as MockRuntimeService
    participant MW as ExceptionMiddleware

    N->>CTR: DELETE /{token}/api/v1/products/uuid-1
    CTR->>SVC: DeleteRecordAsync(token, products, uuid-1)
    SVC->>SVC: ResolveResourceAsync()\nfinds project + resource ✅
    SVC->>SVC: check EndpointConfig.Delete
    note over SVC: Delete == false
    SVC->>MW: throw MethodNotAllowedException\n"DELETE method is not enabled"
    MW->>N: 405 Method Not Allowed\n{ "message": "DELETE method is not enabled for this resource" }
    note over N: Nour learns endpoints\ncan be configured per resource
```

---

## Error Scenarios Summary

```mermaid
flowchart TD
    REQ[Incoming Request] --> T1{Token valid?}
    T1 -->|No| E404A[404 Project not found]
    T1 -->|Yes| T2{Resource slug\nexists?}
    T2 -->|No| E404B[404 Resource not found]
    T2 -->|Yes| T3{Method\nenabled?}
    T3 -->|No| E405[405 Method not enabled]
    T3 -->|Yes| T4{Record id\nexists?}
    T4 -->|No| E404C[404 Record not found]
    T4 -->|Yes| T5{Input\nvalid?}
    T5 -->|No| E400[400 Validation error]
    T5 -->|Yes| SUCCESS[200 / 201 Success]
    T6[Unexpected error] --> E500[500 Something went wrong\nFull error logged]
```

| Scenario | HTTP Code | Message |
|---|---|---|
| Wrong token in URL | 404 | Project not found |
| Wrong resource slug | 404 | Resource not found |
| Record id not found | 404 | Record not found |
| Accessing another user's project | 404 | Project not found (hides existence) |
| Method disabled | 405 | DELETE method is not enabled for this resource |
| Empty field name | 400 | Field name is required |
| Invalid DataType | 400 | Invalid data type 'Xyz' |
| Count > 1000 | 400 | Count cannot exceed 1000 records |
| Unexpected server error | 500 | Something went wrong |
