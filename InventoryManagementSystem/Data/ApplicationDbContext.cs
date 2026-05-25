using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<EmployeeCategoryAssignment> EmployeeCategoryAssignments { get; set; }
        public DbSet<AppNotification> AppNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AuditLog>().HasKey(al => al.AuditId);
            modelBuilder.Entity<InventoryTransaction>().HasKey(it => it.TransactionId);
            modelBuilder.Entity<AppNotification>().HasKey(n => n.NotificationId);

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.ProductCode)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<EmployeeCategoryAssignment>()
                .HasKey(x => new { x.EmployeeId, x.CategoryId });

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.ReportsTo)
                .WithMany(e => e.DirectReports)
                .HasForeignKey(e => e.ReportsToEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeCategoryAssignment>()
                .HasOne(x => x.Employee)
                .WithMany(e => e.CategoryAssignments)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmployeeCategoryAssignment>()
                .HasOne(x => x.Category)
                .WithMany(c => c.EmployeeAssignments)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(it => it.Product)
                .WithMany(p => p.InventoryTransactions)
                .HasForeignKey(it => it.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(it => it.CreatedByUser)
                .WithMany(u => u.InventoryTransactions)
                .HasForeignKey(it => it.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AuditLog>()
                .HasOne(al => al.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(al => al.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AppNotification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin", Description = "Administrator" },
                new Role { RoleId = 2, RoleName = "Employee", Description = "Employee" }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "ห้องนั่งเล่น", Description = "โซฟา โต๊ะกลาง ชั้นวาง", ProductLine = "Furniture" },
                new Category { CategoryId = 2, CategoryName = "ห้องนอน", Description = "เตียง ตู้เสื้อผ้า โต๊ะเครื่องแป้ง", ProductLine = "Furniture" },
                new Category { CategoryId = 3, CategoryName = "ห้องทานอาหาร", Description = "โต๊ะอาหาร เก้าอี้", ProductLine = "Furniture" },
                new Category { CategoryId = 4, CategoryName = "ห้องทำงาน", Description = "โต๊ะทำงาน เก้าอี้สำนักงาน", ProductLine = "Furniture" },
                new Category { CategoryId = 5, CategoryName = "จัดเก็บ", Description = "ชั้นวาง ตู้ลิ้นชัก", ProductLine = "Furniture" },
                new Category { CategoryId = 6, CategoryName = "กลางแจ้ง", Description = "เฟอร์นิเจอร์ลาน", ProductLine = "Furniture" }
            );

            modelBuilder.Entity<Department>().HasData(
                new Department { DepartmentId = 1, DepartmentName = "Warehouse", Description = "Main warehouse operations" },
                new Department { DepartmentId = 2, DepartmentName = "Procurement", Description = "Purchasing and receiving" },
                new Department { DepartmentId = 3, DepartmentName = "Sales", Description = "Outbound and customer orders" },
                new Department { DepartmentId = 4, DepartmentName = "Administration", Description = "Office and HR support" }
            );

            // Seed 50 sample products
            modelBuilder.Entity<Product>().HasData(
                // Category 1: ห้องนั่งเล่น (Living Room) - 9 products
                new Product { ProductId = 1, ProductCode = "LR-001", ProductName = "โซฟา 3 ที่นั่ง สีเทา", Description = "โซฟาหนังสังเคราะห์ 3 ที่นั่ง สีเทาเข้ม", CategoryId = 1, Price = 12500, CurrentStock = 15, CreatedDate = DateTime.Now },
                new Product { ProductId = 2, ProductCode = "LR-002", ProductName = "โซฟามุม 2 ส่วน", Description = "โซฟามุมขาอลูมิเนียม สีดำ", CategoryId = 1, Price = 18900, CurrentStock = 8, CreatedDate = DateTime.Now },
                new Product { ProductId = 3, ProductCode = "LR-003", ProductName = "โต๊ะกลางไม้สัก", Description = "โต๊ะกลางไม้สักแท้ ขนาด 100x50 ซม.", CategoryId = 1, Price = 5500, CurrentStock = 12, CreatedDate = DateTime.Now },
                new Product { ProductId = 4, ProductCode = "LR-004", ProductName = "ชั้นวางกำแพงไม้", Description = "ชั้นวางกำแพง 3 ชั้น สีไวท์โอ๊ค", CategoryId = 1, Price = 3200, CurrentStock = 20, CreatedDate = DateTime.Now },
                new Product { ProductId = 5, ProductCode = "LR-005", ProductName = "เก้าอี้ยาว", Description = "เก้าอี้ยาว 2 ที่นั่ง ลูกฟูก", CategoryId = 1, Price = 4800, CurrentStock = 10, CreatedDate = DateTime.Now },
                new Product { ProductId = 6, ProductCode = "LR-006", ProductName = "ตู้ TV ไม้อัด", Description = "ตู้ TV สีบีช ความกว้าง 120 ซม.", CategoryId = 1, Price = 8900, CurrentStock = 6, CreatedDate = DateTime.Now },
                new Product { ProductId = 7, ProductCode = "LR-007", ProductName = "โคมไฟตั้งพื้น", Description = "โคมไฟโมเดิร์น โลหะสีดำ", CategoryId = 1, Price = 1500, CurrentStock = 25, CreatedDate = DateTime.Now },
                new Product { ProductId = 8, ProductCode = "LR-008", ProductName = "เก้าอี้นวม", Description = "เก้าอี้นวม หนนุนสั่ง", CategoryId = 1, Price = 3500, CurrentStock = 14, CreatedDate = DateTime.Now },
                new Product { ProductId = 9, ProductCode = "LR-009", ProductName = "โต๊ะเกม", Description = "โต๊ะเกมลาดสีคอนกรีต", CategoryId = 1, Price = 6800, CurrentStock = 5, CreatedDate = DateTime.Now },

                // Category 2: ห้องนอน (Bedroom) - 9 products
                new Product { ProductId = 10, ProductCode = "BR-001", ProductName = "เตียงนอน ขนาด Queen", Description = "เตียงนอน ขนาด 150x200 ซม. ไม้สัก", CategoryId = 2, Price = 22000, CurrentStock = 4, CreatedDate = DateTime.Now },
                new Product { ProductId = 11, ProductCode = "BR-002", ProductName = "เตียงนอน ขนาด Single", Description = "เตียงนอน ขนาด 90x200 ซม. ไม้ยาง", CategoryId = 2, Price = 9500, CurrentStock = 7, CreatedDate = DateTime.Now },
                new Product { ProductId = 12, ProductCode = "BR-003", ProductName = "ตู้เสื้อผ้า 2 บาน", Description = "ตู้เสื้อผ้า 2 บาน สีขาว", CategoryId = 2, Price = 7800, CurrentStock = 5, CreatedDate = DateTime.Now },
                new Product { ProductId = 13, ProductCode = "BR-004", ProductName = "ตู้เสื้อผ้า 3 บาน", Description = "ตู้เสื้อผ้า 3 บาน กระจก ไม้ลัดดา", CategoryId = 2, Price = 12500, CurrentStock = 3, CreatedDate = DateTime.Now },
                new Product { ProductId = 14, ProductCode = "BR-005", ProductName = "โต๊ะเครื่องแป้ง", Description = "โต๊ะเครื่องแป้งพร้อมกระจก สีเคลือบ", CategoryId = 2, Price = 5200, CurrentStock = 8, CreatedDate = DateTime.Now },
                new Product { ProductId = 15, ProductCode = "BR-006", ProductName = "ที่นอน ฟอม 5 นิ้ว", Description = "ที่นอน ฟอมความหนาแน่นสูง 5 นิ้ว", CategoryId = 2, Price = 3500, CurrentStock = 18, CreatedDate = DateTime.Now },
                new Product { ProductId = 16, ProductCode = "BR-007", ProductName = "หมอนข้าง", Description = "หมอนข้าง หมอกฟอมยาง", CategoryId = 2, Price = 600, CurrentStock = 50, CreatedDate = DateTime.Now },
                new Product { ProductId = 17, ProductCode = "BR-008", ProductName = "ผ้าม่านห้องนอน", Description = "ผ้าม่านกันแสง สีเบจ", CategoryId = 2, Price = 2800, CurrentStock = 12, CreatedDate = DateTime.Now },
                new Product { ProductId = 18, ProductCode = "BR-009", ProductName = "ตะแกรงหัวเตียง", Description = "ตะแกรงหัวเตียง สีทอง", CategoryId = 2, Price = 4500, CurrentStock = 6, CreatedDate = DateTime.Now },

                // Category 3: ห้องทานอาหาร (Dining Room) - 9 products
                new Product { ProductId = 19, ProductCode = "DR-001", ProductName = "โต๊ะอาหาร ไม้สัก 6 ที่", Description = "โต๊ะอาหาร ไม้สักแท้ 6 ที่นั่ง", CategoryId = 3, Price = 16800, CurrentStock = 3, CreatedDate = DateTime.Now },
                new Product { ProductId = 20, ProductCode = "DR-002", ProductName = "โต๊ะอาหาร 4 ที่", Description = "โต๊ะอาหาร ไม้ยาง 4 ที่นั่ง", CategoryId = 3, Price = 8900, CurrentStock = 5, CreatedDate = DateTime.Now },
                new Product { ProductId = 21, ProductCode = "DR-003", ProductName = "เก้าอี้อาหาร หนัง", Description = "เก้าอี้อาหาร หนังสังเคราะห์ สีดำ", CategoryId = 3, Price = 1850, CurrentStock = 24, CreatedDate = DateTime.Now },
                new Product { ProductId = 22, ProductCode = "DR-004", ProductName = "เก้าอี้อาหาร ไม้", Description = "เก้าอี้อาหาร ไม้บีช สีธรรมชาติ", CategoryId = 3, Price = 2200, CurrentStock = 20, CreatedDate = DateTime.Now },
                new Product { ProductId = 23, ProductCode = "DR-005", ProductName = "ตู้เก็บจาน", Description = "ตู้เก็บจาน 2 บาน กระจก", CategoryId = 3, Price = 6500, CurrentStock = 4, CreatedDate = DateTime.Now },
                new Product { ProductId = 24, ProductCode = "DR-006", ProductName = "โคมไฟห้องทานอาหาร", Description = "โคมไฟแขวนทรงสี่เหลี่ยม", CategoryId = 3, Price = 3200, CurrentStock = 8, CreatedDate = DateTime.Now },
                new Product { ProductId = 25, ProductCode = "DR-007", ProductName = "พรม 150x200 ซม.", Description = "พรมสำหรับห้องทานอาหาร", CategoryId = 3, Price = 4500, CurrentStock = 6, CreatedDate = DateTime.Now },
                new Product { ProductId = 26, ProductCode = "DR-008", ProductName = "ตู้บาร์ไม้", Description = "ตู้บาร์ 1 บาน สีวอลนัท", CategoryId = 3, Price = 5800, CurrentStock = 3, CreatedDate = DateTime.Now },
                new Product { ProductId = 27, ProductCode = "DR-009", ProductName = "โต๊ะขยาย ไม้สัก", Description = "โต๊ะขยายไม้สัก ขนาด 120-160", CategoryId = 3, Price = 18500, CurrentStock = 2, CreatedDate = DateTime.Now },

                // Category 4: ห้องทำงาน (Office) - 9 products
                new Product { ProductId = 28, ProductCode = "OF-001", ProductName = "โต๊ะทำงาน 120 ซม.", Description = "โต๊ะทำงาน ไม้อัด 120 ซม.", CategoryId = 4, Price = 5200, CurrentStock = 10, CreatedDate = DateTime.Now },
                new Product { ProductId = 29, ProductCode = "OF-002", ProductName = "โต๊ะทำงาน 150 ซม.", Description = "โต๊ะทำงาน ไม้ลัดดา 150 ซม.", CategoryId = 4, Price = 6800, CurrentStock = 7, CreatedDate = DateTime.Now },
                new Product { ProductId = 30, ProductCode = "OF-003", ProductName = "เก้าอี้สำนักงาน ปรับความสูง", Description = "เก้าอี้สำนักงาน ปรับความสูงได้", CategoryId = 4, Price = 4200, CurrentStock = 12, CreatedDate = DateTime.Now },
                new Product { ProductId = 31, ProductCode = "OF-004", ProductName = "เก้าอี้บอสสีดำ", Description = "เก้าอี้บอส หนัง ห้อแขนปรับได้", CategoryId = 4, Price = 6500, CurrentStock = 5, CreatedDate = DateTime.Now },
                new Product { ProductId = 32, ProductCode = "OF-005", ProductName = "ชั้นหนังสือ 5 ชั้น", Description = "ชั้นหนังสือ 5 ชั้น ไม้ลัดดา", CategoryId = 4, Price = 3800, CurrentStock = 8, CreatedDate = DateTime.Now },
                new Product { ProductId = 33, ProductCode = "OF-006", ProductName = "ตู้ลิ้นชัก 3 ชั้น", Description = "ตู้ลิ้นชัก ลูกลื่น สีเทา", CategoryId = 4, Price = 4500, CurrentStock = 6, CreatedDate = DateTime.Now },
                new Product { ProductId = 34, ProductCode = "OF-007", ProductName = "แท่นวางจอ", Description = "แท่นวางจอคอมพิวเตอร์ ปรับความสูงได้", CategoryId = 4, Price = 1200, CurrentStock = 20, CreatedDate = DateTime.Now },
                new Product { ProductId = 35, ProductCode = "OF-008", ProductName = "ไฟจอปืนกลาง", Description = "ไฟจอปืนกลาง LED ขาว", CategoryId = 4, Price = 800, CurrentStock = 30, CreatedDate = DateTime.Now },
                new Product { ProductId = 36, ProductCode = "OF-009", ProductName = "โต๊ะแล็ป", Description = "โต๊ะแล็ปทอปแบบยางสด", CategoryId = 4, Price = 3500, CurrentStock = 9, CreatedDate = DateTime.Now },

                // Category 5: จัดเก็บ (Storage) - 9 products
                new Product { ProductId = 37, ProductCode = "ST-001", ProductName = "ชั้นวาง 5 ชั้น เหล็ก", Description = "ชั้นวาง 5 ชั้น เหล็กสีดำ", CategoryId = 5, Price = 2500, CurrentStock = 14, CreatedDate = DateTime.Now },
                new Product { ProductId = 38, ProductCode = "ST-002", ProductName = "ชั้นวาง 4 ชั้น ไม้", Description = "ชั้นวาง 4 ชั้น ไม้บีช", CategoryId = 5, Price = 3500, CurrentStock = 10, CreatedDate = DateTime.Now },
                new Product { ProductId = 39, ProductCode = "ST-003", ProductName = "ตู้ลิ้นชัก 6 ชั้น", Description = "ตู้ลิ้นชัก 6 ชั้น ลูกลื่น", CategoryId = 5, Price = 5800, CurrentStock = 5, CreatedDate = DateTime.Now },
                new Product { ProductId = 40, ProductCode = "ST-004", ProductName = "ตู้เก็บสิ่งของ", Description = "ตู้เก็บสิ่งของ 2 บาน 3 ชั้น", CategoryId = 5, Price = 4200, CurrentStock = 6, CreatedDate = DateTime.Now },
                new Product { ProductId = 41, ProductCode = "ST-005", ProductName = "ลังเก็บสิ่งของ", Description = "ลังเก็บสิ่งของ ไม้สน", CategoryId = 5, Price = 2200, CurrentStock = 16, CreatedDate = DateTime.Now },
                new Product { ProductId = 42, ProductCode = "ST-006", ProductName = "บล็อกเก็บสิ่งของ", Description = "บล็อกเก็บสิ่งของ ผ้าใบ", CategoryId = 5, Price = 800, CurrentStock = 40, CreatedDate = DateTime.Now },
                new Product { ProductId = 43, ProductCode = "ST-007", ProductName = "กล่องเก็บของ PP", Description = "กล่องเก็บของ PP ขนาดใหญ่", CategoryId = 5, Price = 350, CurrentStock = 100, CreatedDate = DateTime.Now },
                new Product { ProductId = 44, ProductCode = "ST-008", ProductName = "ชั้นวางกำแพง 3 ชั้น", Description = "ชั้นวางกำแพง 3 ชั้น สีขาว", CategoryId = 5, Price = 2800, CurrentStock = 12, CreatedDate = DateTime.Now },
                new Product { ProductId = 45, ProductCode = "ST-009", ProductName = "ตะกร้าเก็บของ", Description = "ตะกร้าเก็บของ เหล็กสีดำ", CategoryId = 5, Price = 1500, CurrentStock = 20, CreatedDate = DateTime.Now },

                // Category 6: กลางแจ้ง (Outdoor) - 5 products
                new Product { ProductId = 46, ProductCode = "OD-001", ProductName = "โต๊ะสนามเหล็ก", Description = "โต๊ะสนามเหล็กเกาะลม", CategoryId = 6, Price = 4500, CurrentStock = 4, CreatedDate = DateTime.Now },
                new Product { ProductId = 47, ProductCode = "OD-002", ProductName = "เก้าอี้สนาม", Description = "เก้าอี้สนาม พลาสติก สีส้ม", CategoryId = 6, Price = 1200, CurrentStock = 18, CreatedDate = DateTime.Now },
                new Product { ProductId = 48, ProductCode = "OD-003", ProductName = "ร่มสนาม", Description = "ร่มสนาม ขนาด 2.5 ม. สีทอง", CategoryId = 6, Price = 3500, CurrentStock = 5, CreatedDate = DateTime.Now },
                new Product { ProductId = 49, ProductCode = "OD-004", ProductName = "เก้าอี้พักผ่อน", Description = "เก้าอี้พักผ่อนกลางแจ้ง", CategoryId = 6, Price = 2800, CurrentStock = 8, CreatedDate = DateTime.Now },
                new Product { ProductId = 50, ProductCode = "OD-005", ProductName = "ชุดโต๊ะเก้าอี้บาร์", Description = "ชุดโต๊ะเก้าอี้บาร์ 3 ชิ้น", CategoryId = 6, Price = 6500, CurrentStock = 3, CreatedDate = DateTime.Now }
            );
        }
    }
}
