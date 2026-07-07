# MockAPIs — System Design

## High Level Architecture

```mermaid
graph TD
    Browser["Client Browser\nReact / Blazor Frontend"]

    Browser -->|Authenticated requests| MgmtAPI
    Browser -->|Public requests no auth| RuntimeAPI

    subgraph DotNet[".NET 9 Web API"]
        MgmtAPI["Management API\n/api/projects\n/api/resources\n/api/fields"]
        RuntimeAPI["Mock Runtime API\n/{token}/api/v1/{resource}"]
    end

    subgraph BLL["MockAPIs.BLL"]
        Services["Services\nProjectService\nResourceService\nFieldService\nDataService\nMockRuntimeService"]
        FakerEngine["FakerEngine\nBogus library"]
        Exceptions["Exceptions\nNotFoundException\nValidationException\nMethodNotAllowedException"]
    end

    subgraph DAL["MockAPIs.DAL"]
        Repos["Repositories\nProjectRepository\nResourceRepository\nFieldRepository\nMockRuntimeRepository"]
        EF["EF Core DbContext"]
    end

    DB[("PostgreSQL\nJSONB support")]

    MgmtAPI --> Services
    RuntimeAPI --> Services
    Services --> FakerEngine
    Services --> Repos
    Repos --> EF
    EF --> DB
```

---

## 3-Tier Layer Breakdown

```mermaid
graph LR
    subgraph API["MockAPIs.API"]
        direction TB
        ProjectsController
        ResourcesController
        FieldsController
        EndpointConfigsController
        DataController
        MockRuntimeController["MockRuntimeController\ncatch-all dynamic"]
        ExceptionMiddleware
    end

    subgraph BLL["MockAPIs.BLL"]
        direction TB
        IProjectService --> ProjectService
        IResourceService --> ResourceService
        IFieldService --> FieldService
        IDataService --> DataService
        IMockRuntimeService --> MockRuntimeService
        FakerEngine
        NotFoundException
        ValidationException
        MethodNotAllowedException
        UnauthorizedException
    end

    subgraph DAL["MockAPIs.DAL"]
        direction TB
        AppDbContext
        IProjectRepository --> ProjectRepository
        IResourceRepository --> ResourceRepository
        IFieldRepository --> FieldRepository
        IDataRepository --> DataRepository
        IMockRuntimeRepository --> MockRuntimeRepository
        AppUser
        Project
        Resource
        Field
        MockRecord
        EndpointConfig
    end

    API -->|calls interfaces| BLL
    BLL -->|calls interfaces| DAL
    DAL --> PostgreSQL[("PostgreSQL")]
```

---

## Database Schema (ERD)

```mermaid
erDiagram
    AspNetUsers {
        uuid Id PK
        string Email
        string PasswordHash
        datetime CreatedAt
        string Plan
    }
    Projects {
        uuid Id PK
        uuid UserId FK
        string Name
        string Slug
        string Token
        string BaseUrl
        datetime CreatedAt
        bool IsActive
    }
    Resources {
        uuid Id PK
        uuid ProjectId FK
        string Name
        string Slug
        int Count
    }
    Fields {
        uuid Id PK
        uuid ResourceId FK
        string Name
        string DataType
        string FakerHint
        bool IsRequired
    }
    MockRecords {
        uuid Id PK
        uuid ResourceId FK
        jsonb Data
        datetime CreatedAt
    }
    EndpointConfigs {
        uuid Id PK
        uuid ResourceId FK
        bool GetList
        bool GetById
        bool Post
        bool Put
        bool Delete
        bool EnablePagination
        bool EnableSearch
    }

    AspNetUsers ||--o{ Projects : "owns"
    Projects ||--o{ Resources : "contains"
    Resources ||--o{ Fields : "has"
    Resources ||--o{ MockRecords : "stores"
    Resources ||--|| EndpointConfigs : "configured by"
```

---

## Cascade Delete Chain

```mermaid
graph TD
    A[DELETE Project] --> B[cascade]
    B --> C[DELETE all Resources]
    C --> D[cascade]
    D --> E[DELETE all Fields]
    D --> F[DELETE all MockRecords]
    D --> G[DELETE EndpointConfig]

    style A fill:#cc3333,color:#fff
    style C fill:#cc6633,color:#fff
    style E fill:#cc9933,color:#fff
    style F fill:#cc9933,color:#fff
    style G fill:#cc9933,color:#fff
```

Configured in `OnModelCreating` via `OnDelete(DeleteBehavior.Cascade)` on all FK relationships.

---

## Dynamic Runtime URL Resolution

```mermaid
flowchart LR
    URL["GET /69daa954.../api/v1/products/uuid-1"]
    URL --> T[Extract token\n69daa954...]
    URL --> R[Extract resource slug\nproducts]
    URL --> I[Extract record id\nuuid-1]

    T --> P["Projects\nWHERE Token = 69daa954...\nAND IsActive = true"]
    P --> RC["Resources JOIN EndpointConfigs\nWHERE ProjectId = p.Id\nAND Slug = products"]
    RC --> M{Method\nenabled?}
    M -->|Yes| SERVE[Serve MockRecord]
    M -->|No| E405[405 Method Not Allowed]
```

---

## Ownership Validation — Query Depth Per Operation

```mermaid
graph TD
    subgraph Op1["Create Resource\nhave: projectId + userId"]
        Q1["Projects.AnyAsync\np.Id == projectId\nAND p.UserId == userId"]
        Q1 --> R1["1 query · 0 JOINs"]
    end

    subgraph Op2["Create Field\nhave: resourceId + userId"]
        Q2["Resources.AnyAsync\nr.Id == resourceId\nAND r.Project.UserId == userId"]
        Q2 --> R2["1 query · 1 JOIN"]
    end

    subgraph Op3["Delete Field\nhave: fieldId + userId"]
        Q3["Fields.AnyAsync\nf.Id == fieldId\nAND f.Resource.Project.UserId == userId"]
        Q3 --> R3["1 query · 2 JOINs"]
    end

    note["No entities loaded into memory\nEF Core translates navigation\nproperties to SQL JOINs automatically"]
```

---

## Exception Middleware Pipeline

```mermaid
graph LR
    REQ[HTTP Request] --> MW[ExceptionMiddleware]
    MW --> CTR[Controller]
    CTR --> SVC[Service]
    SVC --> REPO[Repository]

    REPO -->|throws| EX{Exception type}
    SVC -->|throws| EX
    CTR -->|throws| EX

    EX -->|NotFoundException| E404[404 Not Found]
    EX -->|UnauthorizedException| E403[403 Forbidden]
    EX -->|ValidationException| E400[400 Bad Request]
    EX -->|MethodNotAllowedException| E405[405 Method Not Allowed]
    EX -->|Exception| E500[500 Internal Server Error]

    E404 & E403 & E400 & E405 & E500 --> JSON["JSON response\n{ message: ... }"]
```

---

## FakerEngine — Data Generation Logic

```mermaid
flowchart TD
    A[FakerEngine.Generate\nfields list + count] --> B[Loop 1 to count]
    B --> C[record id = newGuid]
    C --> D[For each Field]
    D --> E{FakerHint\nset?}
    E -->|Yes| F[ResolveHint\ndot-notation lookup]
    F --> G{Hint\nrecognized?}
    G -->|Yes| H["faker.Commerce.ProductName()\nfaker.Finance.Amount()\nfaker.Internet.Email() ..."]
    G -->|No| I[fallback to DataType]
    E -->|No| I
    I --> J{DataType switch}
    J -->|String| K[faker.Lorem.Word]
    J -->|Number| L[faker.Random.Int]
    J -->|Boolean| M[faker.Random.Bool]
    J -->|Date| N[faker.Date.Past]
    J -->|Image| O[faker.Image.LoremFlickrUrl]
    J -->|Email| P[faker.Internet.Email]
    J -->|Name| Q[faker.Name.FullName]
    J -->|Price| R[faker.Finance.Amount]
    H & K & L & M & N & O & P & Q & R --> S[Add to record dictionary]
    S --> T{More fields?}
    T -->|Yes| D
    T -->|No| U{More records?}
    U -->|Yes| B
    U -->|No| V[Return List of Dictionaries]
```

---

## Serialize / Deserialize Cycle

```mermaid
sequenceDiagram
    participant E as FakerEngine
    participant S as DataService
    participant DB as PostgreSQL (JSONB)
    participant R as MockRuntimeService
    participant F as Frontend

    E->>S: Dictionary { title, price, imageUrl }
    S->>S: JsonSerializer.Serialize(dict)\n→ JSON string
    S->>DB: INSERT MockRecords.Data = JSON string
    DB-->>DB: stored as JSONB

    F->>R: GET /{token}/api/v1/products
    R->>DB: SELECT Data FROM MockRecords
    DB-->>R: JSON string per row
    R->>R: JsonSerializer.Deserialize(string)\n→ Dictionary
    R->>R: inject real DB id into dict
    R-->>F: clean JSON array response
```

---

## URL Design

### Management API

```mermaid
graph LR
    Auth["Auth\nPOST /auth/login/google\nGET /auth/callback/google\nPOST /auth/logout\nGET /auth/me"]
    Projects["Projects\nGET /api/projects\nGET /api/projects/id\nPOST /api/projects\nPUT /api/projects/id/rename\nDELETE /api/projects/id"]
    Resources["Resources\nPOST /api/resources\nDELETE /api/resources/id"]
    Fields["Fields\nGET /api/resources/rid/fields\nPOST /api/resources/rid/fields\nPUT /api/resources/rid/fields/fid\nDELETE /api/resources/rid/fields/fid"]
    Config["Endpoint Config\nPUT /api/resources/rid/endpoint-config"]
    Data["Data\nGET /api/resources/rid/preview\nPOST /api/resources/rid/generate"]
```

### Mock Runtime API (Public)

```mermaid
graph LR
    Token["/{token}"] --> Base["/api/v1/"]
    Base --> List["{resource}\nGET — list\nPOST — create"]
    Base --> Single["{resource}/{id}\nGET — one record\nPUT — update\nDELETE — delete"]
    List --> QP["Query Params\n?page=1&limit=10\n?search=chair"]
```

---

## Dependency Registration (Program.cs)

```mermaid
graph LR
    subgraph DI["Scoped Registrations"]
        direction TB
        R1[IProjectRepository → ProjectRepository]
        R2[IResourceRepository → ResourceRepository]
        R3[IFieldRepository → FieldRepository]
        R4[IEndpointConfigRepository → EndpointConfigRepository]
        R5[IDataRepository → DataRepository]
        R6[IMockRuntimeRepository → MockRuntimeRepository]
        S1[IProjectService → ProjectService]
        S2[IResourceService → ResourceService]
        S3[IFieldService → FieldService]
        S4[IEndpointConfigService → EndpointConfigService]
        S5[IDataService → DataService]
        S6[IMockRuntimeService → MockRuntimeService]
    end
```
