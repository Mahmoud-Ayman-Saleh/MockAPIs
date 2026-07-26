# MockAPIs - API Design Specification

This document provides a comprehensive design specification for all REST API endpoints provided by the **MockAPIs** platform.

---

## Global API Conventions

### Base URLs
- **Management API Base URL**: `/api`
- **Dynamic Mock Runtime API Base URL**: `/{token}/api/v1/{resourceSlug}`

### Authentication Scheme
Management API endpoints require HTTP Bearer Authentication. Include the JWT token obtained from `/api/Auth/Login` or `/api/Auth/Register` in the request header:
```http
Authorization: Bearer <your_jwt_access_token>
```
Dynamic Mock Runtime endpoints do not require JWT Bearer headers; access control is scoped via the unique project `token` in the path URL.

---

## Table of Contents
1. [Authentication API (`/api/Auth`)](#1-authentication-api-apiauth)
2. [Project Management API (`/api/Project`)](#2-project-management-api-apiproject)
3. [Resource Management API (`/api/Resource`)](#3-resource-management-api-apiresource)
4. [Field Management API (`/api/resources/{resourceId}/fields`)](#4-field-management-api-apiresourcesresourceidfields)
5. [Endpoint Configuration API (`/api/resources/{resourceId}/endpoint-config`)](#5-endpoint-configuration-api-apiresourcesresourceidendpoint-config)
6. [Data Generation & Preview API (`/api/resources/{resourceId}`)](#6-data-generation--preview-api-apiresourcesresourceid)
7. [Dynamic Mock Runtime Engine API (`/{token}/api/v1/{resourceSlug}`)](#7-dynamic-mock-runtime-engine-api-tokenapiv1resourceslug)

---

## 1. Authentication API (`/api/Auth`)

### 1.1 Register User
Registers a new user account and returns access/refresh tokens.

- **HTTP Method**: `POST`
- **Path**: `/api/Auth/Register`
- **Auth Required**: No
- **Request Body**: `application/json`
  ```json
  {
    "userName": "johndoe",
    "email": "johndoe@example.com",
    "password": "StrongPassword123!"
  }
  ```
- **Success Response**: `200 OK`
  ```json
  {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "d8f3b2a1c9e4...",
    "expiresAt": "2026-07-27T23:20:00Z",
    "user": {
      "userId": "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
      "email": "johndoe@example.com",
      "userName": "johndoe",
      "role": "User",
      "createdAt": "2026-07-26T23:20:00Z"
    }
  }
  ```
- **Error Responses**:
  - `400 Bad Request`: `{"error": "Username or Email already exists"}`

---

### 1.2 Login User
Authenticates a user with username and password.

- **HTTP Method**: `POST`
- **Path**: `/api/Auth/Login`
- **Auth Required**: No
- **Request Body**: `application/json`
  ```json
  {
    "userName": "johndoe",
    "password": "StrongPassword123!"
  }
  ```
- **Success Response**: `200 OK`
  ```json
  {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "d8f3b2a1c9e4...",
    "expiresAt": "2026-07-27T23:20:00Z",
    "user": {
      "userId": "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
      "email": "johndoe@example.com",
      "userName": "johndoe",
      "role": "User",
      "createdAt": "2026-07-26T23:20:00Z"
    }
  }
  ```
- **Error Responses**:
  - `401 Unauthorized`: `{"error": "Invalid username or password"}`

---

### 1.3 Refresh Access Token
Generates a fresh JWT access token using a valid refresh token.

- **HTTP Method**: `POST`
- **Path**: `/api/Auth/RefreshToken`
- **Auth Required**: No
- **Request Body**: `application/json`
  ```json
  {
    "refreshToken": "d8f3b2a1c9e4..."
  }
  ```
- **Success Response**: `200 OK`
  ```json
  {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "f9e8d7c6b5a4...",
    "expiresAt": "2026-07-27T23:20:00Z",
    "user": {
      "userId": "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
      "email": "johndoe@example.com",
      "userName": "johndoe",
      "role": "User",
      "createdAt": "2026-07-26T23:20:00Z"
    }
  }
  ```
- **Error Responses**:
  - `401 Unauthorized`: `{"error": "Invalid or expired refresh token"}`

---

## 2. Project Management API (`/api/Project`)

### 2.1 Get All Projects
Lists all projects owned by the currently authenticated user.

- **HTTP Method**: `GET`
- **Path**: `/api/Project`
- **Auth Required**: Yes (`Bearer <token>`)
- **Success Response**: `200 OK`
  ```json
  [
    {
      "id": "11111111-2222-3333-4444-555555555555",
      "name": "E-Commerce Mock API",
      "slug": "e-commerce-mock-api",
      "token": "tok_sec_9876543210",
      "baseUrl": "http://localhost:5000/tok_sec_9876543210/api/v1",
      "isActive": true,
      "createdAt": "2026-07-26T20:00:00Z",
      "resourceCount": 2
    }
  ]
  ```

---

### 2.2 Get Project by ID
Retrieves details of a single project including summaries of its resources.

- **HTTP Method**: `GET`
- **Path**: `/api/Project/{id}`
- **Auth Required**: Yes (`Bearer <token>`)
- **Path Parameters**:
  - `id` (Guid, required): Project ID.
- **Success Response**: `200 OK`
  ```json
  {
    "id": "11111111-2222-3333-4444-555555555555",
    "name": "E-Commerce Mock API",
    "slug": "e-commerce-mock-api",
    "token": "tok_sec_9876543210",
    "baseUrl": "http://localhost:5000/tok_sec_9876543210/api/v1",
    "isActive": true,
    "createdAt": "2026-07-26T20:00:00Z",
    "resources": [
      {
        "id": "22222222-3333-4444-5555-666666666666",
        "name": "Products",
        "slug": "products",
        "count": 50
      },
      {
        "id": "33333333-4444-5555-6666-777777777777",
        "name": "Users",
        "slug": "users",
        "count": 100
      }
    ]
  }
  ```
- **Error Responses**:
  - `404 Not Found`: `{"message": "Project not found"}`

---

### 2.3 Create Project
Creates a new project owned by the current user.

- **HTTP Method**: `POST`
- **Path**: `/api/Project`
- **Auth Required**: Yes (`Bearer <token>`)
- **Request Body**: `application/json` (Raw string representing project name)
  ```json
  "E-Commerce Mock API"
  ```
- **Success Response**: `201 Created`
  - **Headers**: `Location: /api/Project/11111111-2222-3333-4444-555555555555`
  - **Body**: Same schema as [Get Project by ID](#22-get-project-by-id).

---

### 2.4 Rename Project
Renames an existing project.

- **HTTP Method**: `PUT`
- **Path**: `/api/Project/{id}/rename`
- **Auth Required**: Yes (`Bearer <token>`)
- **Path Parameters**:
  - `id` (Guid, required): Project ID.
- **Request Body**: `application/json` (Raw string representing new name)
  ```json
  "Updated E-Commerce API"
  ```
- **Success Response**: `200 OK` (Returns updated project object).
- **Error Responses**:
  - `404 Not Found`: `{"message": "Project not found"}`

---

### 2.5 Delete Project
Deletes a project and all associated resources, fields, endpoint configurations, and dynamic data.

- **HTTP Method**: `DELETE`
- **Path**: `/api/Project/{id}`
- **Auth Required**: Yes (`Bearer <token>`)
- **Path Parameters**:
  - `id` (Guid, required): Project ID.
- **Success Response**: `200 OK`
  ```json
  {
    "message": "Project deleted successfully"
  }
  ```
- **Error Responses**:
  - `404 Not Found`: `{"message": "Project not found"}`

---

## 3. Resource Management API (`/api/Resource`)

### 3.1 Create Resource
Creates a new resource under the specified project.

- **HTTP Method**: `POST`
- **Path**: `/api/Resource/{id}`
- **Auth Required**: Yes (`Bearer <token>`)
- **Path Parameters**:
  - `id` (Guid, required): Parent Project ID.
- **Request Body**: `application/json` (Raw string representing resource name)
  ```json
  "Products"
  ```
- **Success Response**: `201 Created`
  ```json
  {
    "id": "22222222-3333-4444-5555-666666666666",
    "name": "Products",
    "slug": "products",
    "count": 0,
    "endpointConfig": {
      "getList": true,
      "getById": true,
      "post": true,
      "put": true,
      "delete": true,
      "enablePagination": true,
      "enableSearch": true
    }
  }
  ```

---

### 3.2 Delete Resource
Deletes a resource by its ID along with its schema fields and mock data records.

- **HTTP Method**: `DELETE`
- **Path**: `/api/Resource/{id}`
- **Auth Required**: Yes (`Bearer <token>`)
- **Path Parameters**:
  - `id` (Guid, required): Resource ID.
- **Success Response**: `200 OK`
  ```json
  {
    "message": "Resource deleted successfully"
  }
  ```
- **Error Responses**:
  - `404 Not Found`: `{"message": "Resource not found"}`

---

## 4. Field Management API (`/api/resources/{resourceId}/fields`)

### 4.1 Get All Fields
Lists all field definitions configured for a resource.

- **HTTP Method**: `GET`
- **Path**: `/api/resources/{resourceId}/fields`
- **Auth Required**: Yes (`Bearer <token>`)
- **Path Parameters**:
  - `resourceId` (Guid, required): Resource ID.
- **Success Response**: `200 OK`
  ```json
  [
    {
      "id": "44444444-5555-6666-7777-888888888888",
      "name": "title",
      "dataType": "String",
      "fakerHint": "Commerce.ProductName",
      "isRequired": true
    },
    {
      "id": "55555555-6666-7777-8888-999999999999",
      "name": "price",
      "dataType": "Decimal",
      "fakerHint": "Commerce.Price",
      "isRequired": true
    }
  ]
  ```

---

### 4.2 Create Field
Adds a new field definition to a resource.

- **HTTP Method**: `POST`
- **Path**: `/api/resources/{resourceId}/fields`
- **Auth Required**: Yes (`Bearer <token>`)
- **Path Parameters**:
  - `resourceId` (Guid, required): Resource ID.
- **Request Body**: `application/json`
  ```json
  {
    "name": "title",
    "dataType": "String",
    "fakerHint": "Commerce.ProductName",
    "isRequired": true
  }
  ```
  *Supported `dataType` values*: `String`, `Integer`, `Decimal`, `Boolean`, `DateTime`, `Guid`.
- **Success Response**: `201 Created`
  ```json
  {
    "id": "44444444-5555-6666-7777-888888888888",
    "name": "title",
    "dataType": "String",
    "fakerHint": "Commerce.ProductName",
    "isRequired": true
  }
  ```

---

### 4.3 Update Field
Updates an existing field definition.

- **HTTP Method**: `PUT`
- **Path**: `/api/resources/{resourceId}/fields/{fieldId}`
- **Auth Required**: Yes (`Bearer <token>`)
- **Path Parameters**:
  - `resourceId` (Guid, required): Resource ID.
  - `fieldId` (Guid, required): Field ID.
- **Request Body**: `application/json`
  ```json
  {
    "name": "productTitle",
    "dataType": "String",
    "fakerHint": "Commerce.ProductName",
    "isRequired": false
  }
  ```
- **Success Response**: `200 OK` (Returns updated `FieldDto`).

---

### 4.4 Delete Field
Deletes a field definition from a resource.

- **HTTP Method**: `DELETE`
- **Path**: `/api/resources/{resourceId}/fields/{fieldId}`
- **Auth Required**: Yes (`Bearer <token>`)
- **Path Parameters**:
  - `resourceId` (Guid, required): Resource ID.
  - `fieldId` (Guid, required): Field ID.
- **Success Response**: `200 OK`
  ```json
  {
    "message": "Field deleted successfully"
  }
  ```

---

## 5. Endpoint Configuration API (`/api/resources/{resourceId}/endpoint-config`)

### 5.1 Update Endpoint Configuration
Configures feature flags and HTTP verbs allowed on the runtime mock endpoint for a resource.

- **HTTP Method**: `PUT`
- **Path**: `/api/resources/{resourceId}/endpoint-config`
- **Auth Required**: Yes (`Bearer <token>`)
- **Path Parameters**:
  - `resourceId` (Guid, required): Resource ID.
- **Request Body**: `application/json`
  ```json
  {
    "getList": true,
    "getById": true,
    "post": true,
    "put": true,
    "delete": true,
    "enablePagination": true,
    "enableSearch": true
  }
  ```
- **Success Response**: `200 OK`
  ```json
  {
    "getList": true,
    "getById": true,
    "post": true,
    "put": true,
    "delete": true,
    "enablePagination": true,
    "enableSearch": true
  }
  ```

---

## 6. Data Generation & Preview API (`/api/resources/{resourceId}`)

### 6.1 Preview Mock Data
Generates an in-memory sample preview of mock data based on the resource's defined fields without persisting to the database.

- **HTTP Method**: `GET`
- **Path**: `/api/resources/{resourceId}/preview`
- **Auth Required**: Yes (`Bearer <token>`)
- **Path Parameters**:
  - `resourceId` (Guid, required): Resource ID.
- **Success Response**: `200 OK`
  ```json
  {
    "resourceId": "22222222-3333-4444-5555-666666666666",
    "preview": [
      {
        "id": "66666666-7777-8888-9999-000000000000",
        "title": "Rustic Wooden Chair",
        "price": 129.99
      },
      {
        "id": "77777777-8888-9999-0000-111111111111",
        "title": "Sleek Leather Shoes",
        "price": 89.50
      }
    ]
  }
  ```

---

### 6.2 Bulk Generate Data
Generates fake mock records using Faker engine and persists them in the database for runtime querying.

- **HTTP Method**: `POST`
- **Path**: `/api/resources/{resourceId}/generate`
- **Auth Required**: Yes (`Bearer <token>`)
- **Path Parameters**:
  - `resourceId` (Guid, required): Resource ID.
- **Request Body**: `application/json`
  ```json
  {
    "count": 20
  }
  ```
- **Success Response**: `200 OK`
  ```json
  {
    "resourceId": "22222222-3333-4444-5555-666666666666",
    "generatedCount": 20,
    "message": "Successfully generated 20 records."
  }
  ```

---

## 7. Dynamic Mock Runtime Engine API (`/{token}/api/v1/{resourceSlug}`)

These endpoints expose public, dynamic CRUD access to mock data based on project settings and resource endpoint configurations.

### 7.1 Query / Get List of Mock Records
Retrieves records for a given resource slug under a project token. Supports pagination and searching when enabled.

- **HTTP Method**: `GET`
- **Path**: `/{token}/api/v1/{resourceSlug}`
- **Auth Required**: No (Uses URL `token`)
- **Path Parameters**:
  - `token` (string, required): Project secret token.
  - `resourceSlug` (string, required): Resource slug (e.g., `products`).
- **Query Parameters**:
  - `search` (string, optional): Search keyword matching string field values.
  - `page` (integer, optional): Page number (1-indexed). Default: `1`.
  - `limit` (integer, optional): Items per page. Default: `10`.

- **Success Response (Pagination Enabled)**: `200 OK`
  ```json
  {
    "data": [
      {
        "id": "66666666-7777-8888-9999-000000000000",
        "title": "Rustic Wooden Chair",
        "price": 129.99
      }
    ],
    "page": 1,
    "limit": 10,
    "total": 50,
    "totalPages": 5
  }
  ```
- **Success Response (Pagination Disabled)**: `200 OK`
  ```json
  [
    {
      "id": "66666666-7777-8888-9999-000000000000",
      "title": "Rustic Wooden Chair",
      "price": 129.99
    }
  ]
  ```

---

### 7.2 Get Mock Record by ID
Retrieves a specific mock record by its Guid ID.

- **HTTP Method**: `GET`
- **Path**: `/{token}/api/v1/{resourceSlug}/{id}`
- **Auth Required**: No (Uses URL `token`)
- **Path Parameters**:
  - `token` (string, required): Project secret token.
  - `resourceSlug` (string, required): Resource slug.
  - `id` (Guid, required): Record Guid ID.
- **Success Response**: `200 OK`
  ```json
  {
    "id": "66666666-7777-8888-9999-000000000000",
    "title": "Rustic Wooden Chair",
    "price": 129.99
  }
  ```
- **Error Responses**:
  - `404 Not Found`: Record not found or endpoint method disabled in `EndpointConfig`.

---

### 7.3 Create Mock Record
Creates a new mock record dynamically.

- **HTTP Method**: `POST`
- **Path**: `/{token}/api/v1/{resourceSlug}`
- **Auth Required**: No (Uses URL `token`)
- **Path Parameters**:
  - `token` (string, required): Project secret token.
  - `resourceSlug` (string, required): Resource slug.
- **Request Body**: `application/json` (Dynamic key-value dictionary)
  ```json
  {
    "title": "Ergonomic Desk",
    "price": 249.99
  }
  ```
- **Success Response**: `201 Created`
  ```json
  {
    "id": "88888888-9999-0000-1111-222222222222",
    "title": "Ergonomic Desk",
    "price": 249.99
  }
  ```

---

### 7.4 Update Mock Record
Updates an existing mock record by Guid ID.

- **HTTP Method**: `PUT`
- **Path**: `/{token}/api/v1/{resourceSlug}/{id}`
- **Auth Required**: No (Uses URL `token`)
- **Path Parameters**:
  - `token` (string, required): Project secret token.
  - `resourceSlug` (string, required): Resource slug.
  - `id` (Guid, required): Record Guid ID.
- **Request Body**: `application/json` (Dynamic key-value dictionary)
  ```json
  {
    "title": "Executive Ergonomic Desk",
    "price": 299.99
  }
  ```
- **Success Response**: `200 OK`
  ```json
  {
    "id": "88888888-9999-0000-1111-222222222222",
    "title": "Executive Ergonomic Desk",
    "price": 299.99
  }
  ```

---

### 7.5 Delete Mock Record
Deletes a specific mock record by Guid ID.

- **HTTP Method**: `DELETE`
- **Path**: `/{token}/api/v1/{resourceSlug}/{id}`
- **Auth Required**: No (Uses URL `token`)
- **Path Parameters**:
  - `token` (string, required): Project secret token.
  - `resourceSlug` (string, required): Resource slug.
  - `id` (Guid, required): Record Guid ID.
- **Success Response**: `200 OK`
  ```json
  {
    "message": "Record deleted successfully"
  }
  ```