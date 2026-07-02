<div align="center">

# 📦 Inventory Management System

**ระบบจัดการคลังสินค้าครบวงจร** สำหรับธุรกิจเฟอร์นิเจอร์  
สร้างด้วย ASP.NET Core MVC + Entity Framework Core บน .NET 10

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-Latest-239120?style=for-the-badge&logo=csharp)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-LocalDB-CC2927?style=for-the-badge&logo=microsoftsqlserver)](https://www.microsoft.com/en-us/sql-server)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5-7952B3?style=for-the-badge&logo=bootstrap)](https://getbootstrap.com/)
[![EF Core](https://img.shields.io/badge/EF_Core-8.0-512BD4?style=for-the-badge&logo=dotnet)](https://learn.microsoft.com/en-us/ef/core/)

</div>

---

## ✨ ภาพรวมโปรเจค

ระบบจัดการคลังสินค้าที่สร้างขึ้นเพื่อจัดการสินค้า พนักงาน และการเคลื่อนไหวของสต็อกในองค์กร  
ออกแบบด้วย **Clean Architecture** ตาม Pattern ของ .NET จริง มี Role-based Access Control, Audit Logging, Real-time Notifications และ Excel Export ครบครัน

> 🎯 โปรเจคนี้สร้างขึ้นเพื่อเรียนรู้ C# / .NET และใช้เป็น Portfolio สำหรับสมัครงาน Junior .NET Developer

---

## 🚀 ฟีเจอร์หลัก

<table>
<tr>
<td width="50%">

### 🔐 Authentication & Authorization
- Login / Register ด้วย Session
- Role-based: **Admin** และ **Employee**
- Password hashing ด้วย BCrypt
- Custom `[Authorize]` Attribute
- หน้า Profile พร้อมประวัติธุรกรรม

</td>
<td width="50%">

### 📊 Dashboard & Notifications
- สรุป metrics ภาพรวมองค์กร
- In-app Notifications (Low Stock / Onboarding)
- ธุรกรรมล่าสุดแบบ Real-time
- รายการสต็อกต่ำ (≤ 5 ชิ้น) พร้อมแจ้งเตือน

</td>
</tr>
<tr>
<td width="50%">

### 📦 Product Management
- CRUD สินค้าสำหรับ Admin
- ค้นหาและกรองตามหมวดหมู่
- รหัสสินค้า Unique ป้องกันซ้ำ
- Audit Log บันทึกทุกการเปลี่ยนแปลง

</td>
<td width="50%">

### 🔄 Inventory Transactions (Stock IN/OUT)
- รับสินค้าเข้า (IN) / จ่ายสินค้าออก (OUT)
- ตรวจสต็อกก่อนจ่ายอัตโนมัติ
- ประวัติธุรกรรมพร้อม Pagination + Filter
- Business Rules: ตรวจสิทธิ์พนักงานต่อหมวดสินค้า

</td>
</tr>
<tr>
<td width="50%">

### 👥 Employee Management
- CRUD พนักงาน + Soft Delete
- Self-Registration → Pending Approval Flow
- Admin อนุมัติ/ปฏิเสธคำขอ
- Timeline การทำงานและ Audit Trail

</td>
<td width="50%">

### 📁 Export & Reporting
- Export พนักงานและประวัติสต็อก → `.xlsx`
- สร้างด้วย ClosedXML
- รองรับเฉพาะ Admin

</td>
</tr>
</table>

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| **Framework** | ASP.NET Core MVC (.NET 10) |
| **Language** | C# |
| **ORM** | Entity Framework Core 8.0 |
| **Database** | SQL Server (LocalDB / SQL Server Express) |
| **Frontend** | Razor Views + Bootstrap 5 |
| **Auth** | Session-based + BCrypt.Net |
| **Excel Export** | ClosedXML |
| **Localization** | th-TH / en-US |

---

## 🏗️ สถาปัตยกรรม

```
Controllers  ──▶  Services (Business Logic)  ──▶  Repositories (Data Access)  ──▶  EF Core  ──▶  SQL Server
```

```
InventoryManagementSystem/
├── 📁 Controllers/          # MVC + REST API Controllers
│   └── Api/                 # Products, Employees, Inventory API
├── 📁 Models/               # Domain entities (User, Product, Employee ...)
├── 📁 Services/             # Business logic layer (IService + Service)
├── 📁 Repositories/         # Generic Repository pattern
├── 📁 Data/                 # ApplicationDbContext + Migrations
├── 📁 ViewModels/           # DTOs สำหรับ View
├── 📁 Views/                # Razor views (.cshtml)
├── 📁 Attributes/           # Custom [Authorize] attribute
├── 📁 Helpers/              # Session helpers
├── 📁 Resources/            # Localization strings
└── 📁 wwwroot/              # Static files (CSS, JS, Images)
```

---

## 🗄️ Database Schema

```
Roles (1) ────── (N) Users
                      │
              ┌───────┴────────┐
              │                │
           Employee      InventoryTransactions ── Products ── Categories
              │                                        │
              └─── EmployeeCategoryAssignment ─────────┘

AuditLogs (บันทึกทุก Action ของ User)
AppNotifications (แจ้งเตือน In-app)
Departments (แผนก)
```

---

## ⚡ เริ่มต้นใช้งาน

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) หรือ SQL Server Express
- Visual Studio 2022+ หรือ VS Code

### 1. Clone โปรเจค

```bash
git clone https://github.com/wantanaiCS/InventoryManagementSystem.git
cd InventoryManagementSystem
```

### 2. ตั้งค่า Connection String

แก้ไขไฟล์ `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=InventoryManagementDb;Integrated Security=true;TrustServerCertificate=True;"
  }
}
```

### 3. รัน Database Migration

```bash
cd InventoryManagementSystem
dotnet ef database update
```

### 4. รันแอปพลิเคชัน

```bash
dotnet run
```

เปิดเบราว์เซอร์ที่ `https://localhost:PORT`

---

## 🔌 REST API Endpoints

> API ทั้งหมดต้อง Login ด้วย Session ก่อน

| Method | Endpoint | คำอธิบาย |
|--------|----------|----------|
| `GET` | `/api/Products?search=...` | ค้นหาสินค้า |
| `GET` | `/api/Products/{id}` | ดูรายละเอียดสินค้า |
| `GET` | `/api/Employees` | รายชื่อพนักงาน (filter ได้) |
| `GET` | `/api/Employees/{id}` | ดูข้อมูลพนักงาน |
| `GET` | `/api/InventoryApi/recent?take=20` | ธุรกรรมล่าสุด |
| `GET` | `/api/InventoryApi/history?page=1&type=IN` | ประวัติสต็อก |
| `POST` | `/api/InventoryApi/receive` | รับสินค้าเข้า |
| `POST` | `/api/InventoryApi/dispense` | จ่ายสินค้าออก |

---

## 👤 บทบาทผู้ใช้

| ฟีเจอร์ | Admin | Employee |
|---------|:-----:|:--------:|
| จัดการสินค้า (CRUD) | ✅ | ❌ |
| ดูรายการสินค้า | ✅ | ✅ |
| Stock IN/OUT | ✅ | ✅ (ตามหมวดที่ได้รับ) |
| จัดการพนักงาน | ✅ | ❌ |
| อนุมัติ/ปฏิเสธ | ✅ | ❌ |
| Export Excel | ✅ | ❌ |
| Dashboard | ✅ (ทั้งหมด) | ✅ (ของตัวเอง) |
| Notifications | ✅ | ❌ |

---

## 🧩 Design Patterns ที่ใช้

- **Repository Pattern** — Abstract data access ออกจาก Business Logic
- **Service Layer Pattern** — แยก Business Logic จาก Controller
- **Dependency Injection** — Loosely coupled, testable code
- **MVC Pattern** — Separation of concerns ใน Presentation layer

---

## 🌐 Localization

รองรับ 2 ภาษา:
- 🇹🇭 ภาษาไทย (`th-TH`) — ค่า default
- 🇺🇸 ภาษาอังกฤษ (`en-US`)

เปลี่ยนภาษาได้จาก Navbar หรือผ่าน Cookie

---

## 📈 Roadmap

- [ ] Unit Tests ด้วย xUnit + Moq
- [ ] JWT Authentication สำหรับ API
- [ ] PDF Report ด้วย QuestPDF
- [ ] Concurrency Token สำหรับ Stock Update
- [ ] Pagination ในทุก List View
- [ ] Docker Support

---

## 📚 Learning Goals

โปรเจคนี้ครอบคลุมการเรียนรู้:

- ✅ Clean Architecture ใน .NET
- ✅ Entity Framework Core + Migrations
- ✅ Authentication ด้วย Session + BCrypt
- ✅ Role-based Authorization
- ✅ REST API development
- ✅ Repository & Service Pattern
- ✅ LINQ queries & aggregations
- ✅ Audit Logging & Soft Delete
- ✅ Excel Export ด้วย ClosedXML
- ✅ Localization (Multi-language)

---

<div align="center">

สร้างด้วย ❤️ เพื่อเรียนรู้ .NET และเตรียมพร้อมสู่การเป็น Junior .NET Developer

**[GitHub](https://github.com/wantanaiCS/InventoryManagementSystem)**

</div>
