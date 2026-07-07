# MockAPIs — Detailed Workflow

## How a Request Flows Through the System

Every request follows this path through the 3-tier architecture:

```mermaid
sequenceDiagram
    participant C as Client
    participant MW as ExceptionMiddleware
    participant CTR as Controller (API)
    participant SVC as Service (BLL)
    participant REPO as Repository (DAL)
    participant DB as PostgreSQL

    C->>MW: HTTP Request
    MW->>CTR: passes through (no error)
    CTR->>SVC: calls service method
    SVC->>REPO: calls repository method
    REPO->>DB: SQL query via EF Core
    DB-->>REPO: returns rows
    REPO-->>SVC: returns entity
    SVC-->>CTR: returns DTO (FromEntity)
    CTR-->>C: IActionResult (200/201/etc)

    note over MW: catches any exception<br/>returns JSON error response
```

---

## Layer Responsibilities

```mermaid
graph TD
    subgraph API["MockAPIs.API — Presentation Layer"]
        A1[Extract userId from claims]
        A2[Call BLL service]
        A3[Return IActionResult]
        A4[No business logic here]
    end

    subgraph BLL["MockAPIs.BLL — Business Logic Layer"]
        B1[Validate inputs]
        B2[Check ownership via AnyAsync]
        B3[Orchestrate repository calls]
        B4[Generate slugs and tokens]
        B5[Run FakerEngine]
        B6[Map entity to DTO via FromEntity]
    end

    subgraph DAL["MockAPIs.DAL — Data Access Layer"]
        D1[EF Core DbContext]
        D2[Repository methods]
        D3[No business logic]
        D4[Never catches exceptions]
    end

    API --> BLL --> DAL
```

---

## Authentication Flow (OAuth)

```mermaid
sequenceDiagram
    participant U as User
    participant App as MockAPIs
    participant G as Google OAuth
    participant DB as PostgreSQL

    U->>App: Click "Login with Google"
    App->>G: GET /auth/login/google (redirect)
    G->>U: Show consent screen
    U->>G: Approve
    G->>App: GET /auth/callback/google?code=...
    App->>G: Exchange code for token
    G-->>App: User info (email, name)
    App->>DB: Does user exist?
    alt New User
        DB-->>App: No
        App->>DB: INSERT AspNetUsers (Email, Plan=Free, CreatedAt=now)
    else Existing User
        DB-->>App: Yes → load user
    end
    App-->>U: Set session cookie → redirect to Dashboard
```

---

## Project Creation Flow

```mermaid
flowchart TD
    A[POST /api/projects\nBody: name] --> B[Extract userId from claims]
    B --> C{Name empty?}
    C -->|Yes| D[throw ValidationException 400]
    C -->|No| E[GenerateSlug\nE-Commerce API → e-commerce-api]
    E --> F{Slug unique globally?}
    F -->|No| G[append counter\ne-commerce-api-1]
    G --> F
    F -->|Yes| H[GenerateUniqueToken\n24-char hex]
    H --> I{Token unique?}
    I -->|No| J[generate new token]
    J --> I
    I -->|Yes| K[Build Project entity\nBaseUrl = mockapis.io/token/api/v1]
    K --> L[AddAsync + SaveChangesAsync]
    L --> M[ProjectCreatedDto.FromEntity]
    M --> N[201 Response]
```

---

## Resource Creation Flow

```mermaid
flowchart TD
    A[POST /api/resources\nBody: projectId + name] --> B[Extract userId from claims]
    B --> C{Name empty?}
    C -->|Yes| D[throw ValidationException 400]
    C -->|No| E[Projects.AnyAsync\np.Id == projectId AND p.UserId == userId]
    E --> F{Owned?}
    F -->|No| G[throw NotFoundException 404]
    F -->|Yes| H[GenerateSlug\nProducts → products]
    H --> I{Slug unique in project?}
    I -->|No| J[append counter\nproducts-1]
    J --> I
    I -->|Yes| K[Build Resource entity\nCount = 0]
    K --> L[Build EndpointConfig\nall methods enabled by default]
    L --> M[Attach config to resource]
    M --> N[AddAsync + SaveChangesAsync\nboth saved in one DB call]
    N --> O[ResourceCreatedDto.FromEntity]
    O --> P[201 Response]
```

---

## Field Creation Flow

```mermaid
flowchart TD
    A[POST /api/resources/resourceId/fields] --> B{Name empty?}
    B -->|Yes| C[throw ValidationException]
    B -->|No| D{Valid DataType enum?}
    D -->|No| E[throw ValidationException\nInvalid data type]
    D -->|Yes| F[Resources.AnyAsync\nr.Id == resourceId AND r.Project.UserId == userId]
    F --> G{Owned?}
    G -->|No| H[throw NotFoundException]
    G -->|Yes| I[Build Field entity]
    I --> J[AddAsync + SaveChangesAsync]
    J --> K[FieldCreatedDto.FromEntity]
    K --> L[201 Response]
```

---

## Data Generation Flow

```mermaid
flowchart TD
    A[POST /resources/id/generate\nBody: count] --> B{count valid?\n0 < count <= 1000}
    B -->|No| C[throw ValidationException]
    B -->|Yes| D[IsResourceOwnedByUserAsync]
    D --> E{Owned?}
    E -->|No| F[throw NotFoundException]
    E -->|Yes| G[GetResourceWithFieldsAsync\nload Resource + all Fields]
    G --> H{Has fields?}
    H -->|No| I[throw ValidationException\nNo fields defined]
    H -->|Yes| J[FakerEngine.Generate fields count]

    subgraph Bogus["FakerEngine loop — for each record"]
        J --> K[record id = newGuid]
        K --> L{FakerHint set?}
        L -->|Yes| M[ResolveHint\ncommerce.productName\n→ faker.Commerce.ProductName]
        L -->|No| N[fallback to DataType\nString → faker.Lorem.Word]
        M --> O[add to record dict]
        N --> O
    end

    O --> P[JsonSerializer.Serialize each dict\n→ JSON string]
    P --> Q[DeleteExistingRecordsAsync]
    Q --> R[AddRecordsAsync]
    R --> S[UpdateResourceCountAsync]
    S --> T[SaveChangesAsync\nsingle transaction]
    T --> U[200 Response\ngeneratedCount: 20]
```

---

## Mock Runtime — GET List Flow

```mermaid
sequenceDiagram
    participant FE as Frontend Dev
    participant CTR as MockRuntimeController
    participant SVC as MockRuntimeService
    participant DB as PostgreSQL

    FE->>CTR: GET /{token}/api/v1/products?page=1&limit=10
    CTR->>SVC: GetListAsync(token, products, page=1, limit=10)

    SVC->>DB: SELECT * FROM Projects WHERE Token=token AND IsActive=true
    alt Project not found
        DB-->>SVC: null
        SVC-->>FE: 404 Project not found
    else Found
        DB-->>SVC: Project entity
    end

    SVC->>DB: SELECT r.*, ec.* FROM Resources JOIN EndpointConfigs\nWHERE ProjectId=id AND Slug=products
    alt Resource not found
        DB-->>SVC: null
        SVC-->>FE: 404 Resource not found
    else Found
        DB-->>SVC: Resource + EndpointConfig
    end

    alt GetList disabled
        SVC-->>FE: 405 Method not enabled
    end

    SVC->>DB: SELECT * FROM MockRecords WHERE ResourceId=id
    DB-->>SVC: List of MockRecord rows

    note over SVC: Deserialize each Data JSON string\nto Dictionary

    alt Pagination enabled AND page param provided
        SVC->>SVC: Skip + Take slice\ncalculate totalPages
        SVC-->>FE: 200 PaginatedResponseDto
    else No pagination
        SVC-->>FE: 200 flat list
    end
```

---

## Mock Runtime — POST (Create Record) Flow

```mermaid
sequenceDiagram
    participant FE as Frontend Dev
    participant CTR as MockRuntimeController
    participant SVC as MockRuntimeService
    participant DB as PostgreSQL

    FE->>CTR: POST /{token}/api/v1/products\nBody: title, price (as JsonElement)
    CTR->>CTR: ConvertJsonElement on each value\nJsonElement String → plain string\nJsonElement Number → plain double
    CTR->>SVC: CreateRecordAsync(token, products, converted dict)
    SVC->>DB: Resolve token → project → resource
    SVC->>SVC: check EndpointConfig.Post == true
    SVC->>SVC: newId = Guid.NewGuid()\nbody[id] = newId
    SVC->>SVC: JsonSerializer.Serialize(body)\n→ JSON string
    SVC->>DB: INSERT MockRecords (Id, ResourceId, Data, CreatedAt)
    DB-->>SVC: saved
    SVC-->>CTR: return body dict (with id injected)
    CTR-->>FE: 201 { id, title, price }
```

---

## Ownership Validation Strategy

```mermaid
graph TD
    subgraph CreateResource["Creating a Resource\nhave: projectId + userId"]
        CR[Projects.AnyAsync\np.Id == projectId\nAND p.UserId == userId]
        CR --> CR2[1 query · 0 JOINs]
    end

    subgraph CreateField["Creating a Field\nhave: resourceId + userId"]
        CF[Resources.AnyAsync\nr.Id == resourceId\nAND r.Project.UserId == userId]
        CF --> CF2[1 query · 1 JOIN]
    end

    subgraph DeleteField["Deleting a Field\nhave: fieldId + userId"]
        DF[Fields.AnyAsync\nf.Id == fieldId\nAND f.Resource.Project.UserId == userId]
        DF --> DF2[1 query · 2 JOINs]
    end

    note1[No entities loaded into memory\nJust a boolean back from DB]
```

---

## Error Handling Flow

```mermaid
flowchart TD
    A[Any layer throws exception] --> B[Bubbles up untouched\nthrough all layers]
    B --> C[ExceptionMiddleware.InvokeAsync catches it]
    C --> D{Exception type?}
    D -->|NotFoundException| E[404 + message]
    D -->|UnauthorizedException| F[403 + message]
    D -->|ValidationException| G[400 + message]
    D -->|MethodNotAllowedException| H[405 + message]
    D -->|Exception unexpected| I[500 + Something went wrong\nfull error logged]
    E & F & G & H & I --> J[WriteErrorResponse\nContent-Type: application/json\nStatusCode set\nJSON body written]
```

---

## Data Storage — Serialize / Deserialize Cycle

```mermaid
flowchart LR
    A[User defines fields\ntitle: String\nprice: Price] --> B[FakerEngine generates\nDictionary]
    B --> C["JsonSerializer.Serialize\n→ JSON string"]
    C --> D[("MockRecords.Data\nJSONB column")]
    D --> E["JsonSerializer.Deserialize\n→ Dictionary"]
    E --> F[Returned as\nclean JSON to frontend]

    style C fill:#f0a500,color:#000
    style E fill:#f0a500,color:#000
    style D fill:#336699,color:#fff
```

---

## Slug Uniqueness Rules

```mermaid
graph TD
    subgraph Global["Project Slug — GLOBALLY unique"]
        PS1[e-commerce-api] --> PS2{exists anywhere\nin platform?}
        PS2 -->|Yes| PS3[e-commerce-api-1]
        PS3 --> PS2
        PS2 -->|No| PS4[use it ✅]
    end

    subgraph PerProject["Resource Slug — unique PER PROJECT"]
        RS1[products in Project A ✅]
        RS2[products in Project B ✅]
        RS3[no conflict — different projectId]
        RS1 & RS2 --> RS3
    end
```
