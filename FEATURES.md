# Inventory Management System — Features Overview

เอกสารนี้สรุปว่า **ระบบมีฟีเจอร์อะไรบ้าง** และ **แต่ละฟีเจอร์ทำงานอย่างไร** (อ้างอิงจากโค้ดในโปรเจกต์ปัจจุบัน)

---

### บทบาทผู้ใช้งาน (Roles)

- **Admin**
  - จัดการสินค้า (CRUD)
  - จัดการพนักงาน (CRUD + Onboarding + Approve/Reject)
  - Export รายงาน (Excel)
  - เห็น Dashboard แบบผู้ดูแล (ข้อมูลรวม/แจ้งเตือน)
- **Employee**
  - ดูรายการสินค้า/รายละเอียดสินค้า
  - ทำรายการสต็อก (IN/OUT) หากผ่านเงื่อนไขพนักงาน
  - สมัครโปรไฟล์พนักงาน (Self registration) แล้วรออนุมัติ
  - ดู Dashboard และประสิทธิภาพของตนเอง

> การยืนยันตัวตนใช้ **Session** และ `[Authorize]` (ยังไม่ใช้ JWT)

---

### 1) Authentication & Session (เข้าสู่ระบบ/สมัครสมาชิก/โปรไฟล์)

- **Login**: `/Auth/Login` (GET/POST)
  - ตรวจ username/password ผ่าน `AuthService.AuthenticateAsync`
  - เก็บ Session: `UserId`, `Username`, `UserRole`
  - สำเร็จแล้วไป `/Dashboard/Index`
- **Register**: `/Auth/Register` (GET/POST)
  - สร้างผู้ใช้ Role = Employee
  - สำเร็จแล้วไปสมัครโปรไฟล์พนักงานที่ `/Employee/ApplyProfile`
- **Logout**: `/Auth/Logout` (GET)
  - ล้าง Session
- **My Profile**: `/Auth/Profile` (GET, ต้อง Login)
  - แสดงข้อมูลผู้ใช้/บทบาท, ข้อมูลพนักงาน (ถ้ามี), และธุรกรรมล่าสุดของผู้ใช้

**Service ที่เกี่ยวข้อง**
- `AuthService`: hash/verify password ด้วย BCrypt, ตรวจ username/email ซ้ำ

---

### 2) Dashboard (ภาพรวมระบบ + แจ้งเตือน)

- **Dashboard**: `/Dashboard/Index` (GET, ต้อง Login)
  - สรุปจำนวนสินค้า/สต็อกรวม/พนักงาน
  - จำนวนธุรกรรมวันนี้ (ทั้งหมด/ของฉัน)
  - รายการสต็อกต่ำ (<= 5)
  - ธุรกรรมล่าสุด
  - แจ้งเตือน (Notifications) ที่ยังไม่อ่าน
- **Mark Notification Read**: `/Dashboard/MarkNotificationRead` (POST)

**Service ที่เกี่ยวข้อง**
- `DashboardService`: คำนวณ metrics ด้วย EF Core (LINQ)
- `NotificationService`: สร้าง/ดึง/mark read/ทำ system checks

---

### 3) Products (Furniture Catalog) — ดูรายการ/ค้นหา/CRUD

- **รายการสินค้า**: `/Product/Index` (GET, ต้อง Login)
  - ค้นหาด้วย `searchTerm`
  - filter ด้วย `categoryId`
  - แสดงหมวดเฉพาะ ProductLine = `"Furniture"`
  - ปุ่มจัดการ (Create/Edit/Delete) แสดง/ใช้งานได้สำหรับ Admin
- **รายละเอียดสินค้า**: `/Product/Details/{id}` (GET)
- **สร้างสินค้า**: `/Product/Create` (GET/POST, Admin เท่านั้น)
  - ตรวจรหัสสินค้าซ้ำ (`ProductCode`)
  - เขียน AuditLog: `CREATE` ตาราง `Products`
- **แก้ไขสินค้า**: `/Product/Edit/{id}` (GET/POST, Admin เท่านั้น)
  - เขียน AuditLog: `UPDATE`
- **ลบสินค้า**: `/Product/Delete/{id}` (GET/POST, Admin เท่านั้น)
  - เขียน AuditLog: `DELETE`

**Service ที่เกี่ยวข้อง**
- `ProductService` ผ่าน `IProductService`
- `AuditService` สำหรับบันทึกกิจกรรม

---

### 4) Inventory Transactions (Stock IN/OUT + History)

เป้าหมายคือ **บันทึกประวัติการเคลื่อนไหวสต็อก** และ **ปรับ `CurrentStock` ของสินค้า** ให้สอดคล้องกับการรับเข้า/จ่ายออก

- **History**: `/Inventory/Index` (GET, ต้อง Login)
  - filter ด้วย `type = IN/OUT`
  - pagination (page + pageSize ภายใน service)
- **Receive (IN)**: `/Inventory/Receive` (GET/POST)
  - เพิ่มสต็อกและบันทึก `InventoryTransactions`
  - เขียน AuditLog: `STOCK_IN`
- **Dispense (OUT)**: `/Inventory/Dispense` (GET/POST)
  - ลดสต็อกและบันทึก `InventoryTransactions`
  - เขียน AuditLog: `STOCK_OUT`

**กฎธุรกิจหลัก (ใน `InventoryService.RecordTransactionAsync`)**
- `transactionType` ต้องเป็น `IN` หรือ `OUT`
- `quantity > 0`
- `OUT` ห้ามมากกว่าสต็อกที่มี
- ถ้ามี Employee และยังไม่ `Approved` → ทำรายการไม่ได้
- ถ้า Employee ถูก assign หมวดสินค้า → ทำได้เฉพาะหมวดที่ได้รับมอบหมาย

---

### 5) Employee Management (HR/Onboarding/Approval)

- **รายชื่อพนักงาน**: `/Employee/Index` (GET, ต้อง Login)
  - รองรับ filter/pagination ผ่าน `EmployeeFilterViewModel`
  - สำหรับ Admin จะเห็นจำนวน pending approvals
- **รายละเอียดพนักงาน**: `/Employee/Details/{id}` (GET)
  - สรุปธุรกรรมล่าสุด, audit trail, timeline
- **Create / Edit / Delete (soft delete)**: (Admin เท่านั้น)
  - `/Employee/Create` (GET/POST)
  - `/Employee/Edit/{id}` (GET/POST)
  - `/Employee/Delete/{id}` (GET/POST)
  - `/Employee/ToggleActive` (POST) เปิด/ปิดการใช้งาน
- **Onboarding (Admin)**: `/Employee/Onboard` (GET/POST)
  - สร้างทั้ง User + Employee (flow สำหรับ Admin)
- **Self-registration (Employee)**: `/Employee/ApplyProfile` (GET/POST)
  - ส่งคำขอสมัครพนักงาน → สถานะ `Pending` รอ Admin
- **Approvals (Admin)**:
  - `/Employee/PendingApprovals` (GET)
  - `/Employee/Approve` (POST)
  - `/Employee/Reject` (POST)

**Service ที่เกี่ยวข้อง**
- `EmployeeService` (ผูกกับ Audit + Notification)
- `NotificationService` (แจ้งเตือนระบบ เช่น onboarding ไม่ครบ)

---

### 6) Export / Reporting (Excel)

สำหรับ **Admin เท่านั้น**

- **Export Employees**: `/Export/Employees` → ไฟล์ `.xlsx`
- **Export Inventory History**: `/Export/InventoryHistory` → ไฟล์ `.xlsx`

**Service ที่เกี่ยวข้อง**
- `ExportService` ใช้ `ClosedXML` สร้าง Excel

---

### 7) Notifications (In-app)

แจ้งเตือนถูกแสดงบน Dashboard และสามารถ mark read ได้

**System checks ที่ทำอยู่**
- **Low stock alert**: ถ้ามีสินค้า `CurrentStock <= 5` → แจ้งเตือน Admin (ป้องกันการสร้างซ้ำเมื่อยังไม่อ่าน)
- **Incomplete onboarding**: ถ้ามี User (Employee role) ที่ยังไม่มี Employee profile → แจ้งเตือน Admin

---

### 8) Audit Logs (บันทึกกิจกรรม)

ระบบบันทึกกิจกรรมสำคัญลง `AuditLogs` เช่น:
- Product CRUD: `CREATE/UPDATE/DELETE`
- Inventory: `STOCK_IN/STOCK_OUT`
- Employee actions (ใน service บางส่วน)

สามารถดึง audit trail ตาม record หรือผู้ใช้ได้ใน `AuditService`

---

### 9) REST API (ภายใน/ใช้ร่วมกับ UI session)

> API ทั้งหมดต้อง Login (Session) เช่นเดียวกับเว็บ

- **Products API**
  - `GET /api/Products?search=...`
  - `GET /api/Products/{id}`
- **Employees API**
  - `GET /api/Employees` (รองรับ filter)
  - `GET /api/Employees/{id}`
- **Inventory API**
  - `GET /api/InventoryApi/recent?take=20`
  - `GET /api/InventoryApi/history?page=1&type=IN|OUT`
  - `POST /api/InventoryApi/receive` (JSON: productId, quantity, notes, shift)
  - `POST /api/InventoryApi/dispense` (JSON: productId, quantity, notes, shift)

---

### หมายเหตุด้านขอบเขต/ข้อจำกัด (ตามโค้ดปัจจุบัน)

- **Concurrency ของสต็อก**: การลด/เพิ่มสต็อกทำแบบ read แล้ว update (SaveChanges ครั้งเดียว) ยังไม่มี concurrency token/transaction isolation สำหรับกรณีหลายคนจ่ายพร้อมกัน
- **Auth สำหรับ API**: ยังพึ่ง Session (ไม่ใช่ JWT) เหมาะกับ internal usage มากกว่า public API

