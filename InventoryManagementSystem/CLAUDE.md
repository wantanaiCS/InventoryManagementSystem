# Inventory Management System - Project Guidelines

## 📋 Project Overview

**Project Name:** Inventory Management System  
**Purpose:** Learning C# and .NET development with real-world application  
**Target Career:** Junior .NET Developer  
**Start Date:** 2025  
**Target Completion:** 2026

---

## 🎯 Project Goals

### Primary Objectives
- ✅ Learn C# fundamentals and advanced concepts
- ✅ Understand ASP.NET Core MVC architecture
- ✅ Master Entity Framework Core and database design
- ✅ Build a production-ready inventory system
- ✅ Create a portfolio project for job applications

### Learning Outcomes
- Clean Code principles and SOLID
- Design Patterns (Repository, Service, DI)
- Database modeling and relationships
- Authentication & Authorization
- REST API development
- Unit Testing

---

## 🏗️ Architecture

### Technology Stack
```
Frontend:      ASP.NET Core MVC with Bootstrap
Backend:       C# (.NET 10)
Database:      SQL Server (LocalDB / SQL Server Express)
ORM:           Entity Framework Core 8.0
Dependency:    AutoMapper
Version Control: Git + GitHub
IDE:           Visual Studio Community 2026
```

### Project Structure
```
InventoryManagementSystem/
├── Models/                 # Domain entities
│   ├── Role.cs
│   ├── User.cs
│   ├── Employee.cs
│   ├── Category.cs
│   ├── Product.cs
│   ├── InventoryTransaction.cs
│   └── AuditLog.cs
├── Data/                   # Database context & migrations
│   ├── ApplicationDbContext.cs
│   └── Migrations/
├── Controllers/            # MVC Controllers
│   ├── HomeController.cs
│   ├── ProductController.cs
│   ├── EmployeeController.cs
│   ├── InventoryController.cs
│   └── AuthController.cs
├── Services/              # Business logic layer
│   ├── IProductService.cs
│   ├── ProductService.cs
│   ├── IEmployeeService.cs
│   ├── EmployeeService.cs
│   ├── IInventoryService.cs
│   ├── InventoryService.cs
│   ├── IAuthService.cs
│   └── AuthService.cs
├── Repositories/          # Data access layer
│   ├── IRepository.cs
│   └── Repository.cs
├── Views/                 # Razor views
│   ├── Product/
│   ├── Employee/
│   ├── Inventory/
│   └── Auth/
├── Program.cs
├── appsettings.json
└── CLAUDE.md             # This file
```

---

## 🗄️ Database Design

### Tables & Relationships

```
Roles (1) ──────────── (N) Users
                         │
                         ├──── (1) Employee
                         ├──── (N) InventoryTransactions (CreatedBy)
                         └──── (N) AuditLogs

Categories (1) ──────────── (N) Products
                               │
                               └──── (N) InventoryTransactions
```

### Core Tables

| Table | Purpose | Key Fields |
|-------|---------|-----------|
| **Roles** | User roles | RoleId, RoleName |
| **Users** | System users | UserId, Username, PasswordHash, RoleId |
| **Employees** | Employee info | EmployeeId, UserId, FullName, Position |
| **Categories** | Product categories | CategoryId, CategoryName |
| **Products** | Inventory items | ProductId, ProductCode (UNIQUE), ProductName, Price, CurrentStock |
| **InventoryTransactions** | Stock movements | TransactionId, ProductId, TransactionType (IN/OUT), Quantity |
| **AuditLogs** | System audit trail | AuditId, UserId, Action, TableName, Timestamp |

---

## 📦 Development Phases

### Phase 1: ✅ COMPLETE - Foundation Setup
- [x] Project structure created
- [x] NuGet packages added (EF Core, SQL Server)
- [x] Database models designed
- [x] DbContext configured
- [x] Migrations created & applied
- [x] Dependency Injection setup

### Phase 2: ✅ IN PROGRESS - Product Management CRUD
- [x] ProductService created
- [x] ProductController created
- [x] Views: Index, Create, Edit, Details, Delete
- [x] Search functionality
- [x] Validation & error handling
- [ ] Unit tests for ProductService
- [ ] Pagination

### Phase 3: TODO - Core Features

#### 3.1 Employee Management
- [ ] EmployeeService & Controller
- [ ] Employee CRUD views
- [ ] Link to User authentication

#### 3.2 Authentication System
- [x] Role model created
- [x] User model created
- [ ] AuthService implementation
- [ ] Login page
- [ ] Register page
- [ ] Logout functionality
- [ ] Session management

#### 3.3 Stock/Inventory Management
- [ ] InventoryTransactionService
- [ ] InventoryController
- [ ] Receive stock view
- [ ] Dispense stock view
- [ ] Stock history view
- [ ] Automatic stock update on transaction
- [ ] Low stock alerts

#### 3.4 Dashboard
- [ ] Dashboard page
- [ ] Key metrics (total products, total stock, etc.)
- [ ] Low stock items list
- [ ] Recent transactions
- [ ] LINQ aggregations & queries

### Phase 4: TODO - Advanced Features

#### 4.1 REST API
- [ ] API controllers for Products
- [ ] API controllers for Employees
- [ ] API controllers for Inventory
- [ ] Proper HTTP status codes
- [ ] Error handling

#### 4.2 Security & Authentication
- [ ] JWT implementation
- [ ] Bearer token validation
- [ ] API authentication

#### 4.3 Reporting
- [ ] Export to Excel (ClosedXML)
- [ ] Export to PDF (QuestPDF)
- [ ] Stock reports
- [ ] Transaction history reports

#### 4.4 Audit & Logging
- [ ] Audit trail implementation
- [ ] Change tracking
- [ ] User action logging
- [ ] Serilog integration

#### 4.5 Testing
- [ ] Unit tests (xUnit)
- [ ] Repository pattern tests
- [ ] Service layer tests
- [ ] Controller tests

---

## 🔑 Code Standards & Best Practices

### Naming Conventions
- **Classes/Interfaces:** PascalCase (e.g., `ProductService`, `IProductService`)
- **Methods:** PascalCase (e.g., `GetProductById`, `CreateProduct`)
- **Variables/Properties:** camelCase (e.g., `productId`, `productName`)
- **Constants:** UPPER_SNAKE_CASE (e.g., `DEFAULT_PAGE_SIZE`)
- **Private fields:** _camelCase (e.g., `_context`, `_logger`)

### Architecture Principles
```
Controllers
    ↓ (calls)
Services (Business Logic)
    ↓ (calls)
Repositories (Data Access)
    ↓ (calls)
DbContext (EF Core)
    ↓ (maps to)
Database
```

### Design Patterns Used
- **Repository Pattern:** Abstraction over data access
- **Service Pattern:** Business logic separation
- **Dependency Injection:** Loose coupling
- **Factory Pattern:** Object creation (may use later)

### Code Quality Rules
- ✅ Use async/await for all I/O operations
- ✅ Validate input before processing
- ✅ Use exceptions for error handling
- ✅ Add meaningful comments only (code should be self-documenting)
- ✅ Keep methods small and focused (Single Responsibility)
- ✅ Use LINQ for data queries
- ✅ Avoid N+1 query problems (use `.Include()`)

---

## 🔌 Connection Strings

### Development (LocalDB)
```
Server=(localdb)\mssqllocaldb;
Database=InventoryManagementDb;
Integrated Security=true;
TrustServerCertificate=True;
```

### Production (SQL Server Express)
```
Server=YOUR_SERVER_NAME;
Database=InventoryManagementDb;
User Id=sa;
Password=YOUR_PASSWORD;
TrustServerCertificate=True;
```

---

## 📝 Common Commands

### Entity Framework Core
```bash
# Create migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove migration
dotnet ef migrations remove

# Drop database
dotnet ef database drop
```

### Git Workflow
```bash
# Create feature branch
git checkout -b feature/feature-name

# Commit changes
git commit -m "feat: add feature description"

# Push to remote
git push origin feature/feature-name

# Create pull request on GitHub
# Merge to develop branch
```

### Running the Application
```bash
# Development
dotnet run

# With watch mode
dotnet watch run

# Build
dotnet build

# Test
dotnet test
```

---

## 🧪 Testing Strategy

### Unit Tests
- **Framework:** xUnit (add later)
- **Mocking:** Moq (add later)
- **Coverage Goal:** 80% of services

### Test Files Location
```
Tests/
├── Services/
│   ├── ProductServiceTests.cs
│   ├── EmployeeServiceTests.cs
│   └── InventoryServiceTests.cs
└── Controllers/
    ├── ProductControllerTests.cs
    └── EmployeeControllerTests.cs
```

---

## 📚 Learning Resources

### C# Fundamentals
- OOP principles
- LINQ queries
- Async/await pattern
- Exception handling
- Delegates & Events

### ASP.NET Core
- MVC pattern
- Routing
- Middleware
- Dependency Injection
- Authentication/Authorization

### Entity Framework Core
- DbContext
- CRUD operations
- Relationships (1:1, 1:N, N:N)
- Migrations
- Query optimization

### SQL Server
- Table design
- Indexes
- Foreign keys
- Transactions
- JOINS (INNER, LEFT, RIGHT)

---

## 🎓 Interview Points

### What to highlight in interviews:
1. **Clean Architecture:** Separation of concerns (Controllers → Services → Repositories)
2. **Design Patterns:** Repository and Service patterns for maintainability
3. **Database Design:** Proper relationships, keys, and normalization
4. **Error Handling:** Validation and exception management
5. **Async/Await:** Proper use for I/O operations
6. **Git Workflow:** Professional version control practices
7. **SOLID Principles:** Especially Dependency Injection and Interface segregation
8. **Testing:** Unit tests for critical business logic (later)

### Key Features to Showcase:
- ✅ CRUD operations with validation
- ✅ Search and filtering
- ✅ Role-based access control
- ✅ Transaction management (stock in/out)
- ✅ Audit trail
- ✅ RESTful API (Phase 4)
- ✅ Error handling & logging

---

## 🚀 Current Status

**Last Updated:** 2025

### Completed Milestones
- ✅ Project setup and structure
- ✅ Database models and DbContext
- ✅ SQL Server LocalDB configuration
- ✅ Product CRUD (Phase 2)

### Next Tasks (Priority Order)
1. Unit tests for ProductService
2. Employee Management (Phase 3.1)
3. Authentication System (Phase 3.2)
4. Inventory Transactions (Phase 3.3)
5. Dashboard (Phase 3.4)

### Known Issues / TODO
- [ ] Add product price decimal precision (HasPrecision)
- [ ] Add pagination to product list
- [ ] Add product images (future)
- [ ] Add notifications (future)
- [ ] Performance optimization for large datasets

---

## 📞 Notes for Future Reference

### Performance Considerations
- Use `.AsNoTracking()` for read-only queries
- Index frequently searched columns (ProductCode, Username)
- Implement pagination for large datasets
- Cache frequently accessed data (Categories)

### Security Considerations
- Hash passwords with bcrypt or similar
- Validate all user inputs
- Use HTTPS in production
- Implement CORS properly for API
- SQL injection prevention (use EF Core parameterized queries)
- XSS prevention in views

### Scalability
- Prepare for multi-user transactions
- Implement proper locking for stock updates
- Consider caching strategies
- Database indexing strategy

---

## 📞 Contact & Resources

**GitHub Repository:** https://github.com/wantanaiCS/InventoryManagementSystem

**Learning Goals:**
- Understand professional .NET development patterns
- Build portfolio-ready projects
- Prepare for junior developer interviews
- Establish good coding practices from the start

---

**Last Modified:** 2025
**Maintained By:** Developer (Learning)
**Version:** 1.0
