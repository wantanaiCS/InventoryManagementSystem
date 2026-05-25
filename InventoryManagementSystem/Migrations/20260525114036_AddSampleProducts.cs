using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InventoryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddSampleProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "Color", "CreatedDate", "CurrentStock", "DepthCm", "Description", "HeightCm", "Material", "Price", "ProductCode", "ProductName", "Unit", "WarehouseLocation", "WidthCm" },
                values: new object[,]
                {
                    { 1, 1, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1087), 15, null, "โซฟาหนังสังเคราะห์ 3 ที่นั่ง สีเทาเข้ม", null, "", 12500m, "LR-001", "โซฟา 3 ที่นั่ง สีเทา", "ชิ้น", "", null },
                    { 2, 1, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1093), 8, null, "โซฟามุมขาอลูมิเนียม สีดำ", null, "", 18900m, "LR-002", "โซฟามุม 2 ส่วน", "ชิ้น", "", null },
                    { 3, 1, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1096), 12, null, "โต๊ะกลางไม้สักแท้ ขนาด 100x50 ซม.", null, "", 5500m, "LR-003", "โต๊ะกลางไม้สัก", "ชิ้น", "", null },
                    { 4, 1, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1099), 20, null, "ชั้นวางกำแพง 3 ชั้น สีไวท์โอ๊ค", null, "", 3200m, "LR-004", "ชั้นวางกำแพงไม้", "ชิ้น", "", null },
                    { 5, 1, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1102), 10, null, "เก้าอี้ยาว 2 ที่นั่ง ลูกฟูก", null, "", 4800m, "LR-005", "เก้าอี้ยาว", "ชิ้น", "", null },
                    { 6, 1, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1105), 6, null, "ตู้ TV สีบีช ความกว้าง 120 ซม.", null, "", 8900m, "LR-006", "ตู้ TV ไม้อัด", "ชิ้น", "", null },
                    { 7, 1, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1108), 25, null, "โคมไฟโมเดิร์น โลหะสีดำ", null, "", 1500m, "LR-007", "โคมไฟตั้งพื้น", "ชิ้น", "", null },
                    { 8, 1, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1111), 14, null, "เก้าอี้นวม หนนุนสั่ง", null, "", 3500m, "LR-008", "เก้าอี้นวม", "ชิ้น", "", null },
                    { 9, 1, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1114), 5, null, "โต๊ะเกมลาดสีคอนกรีต", null, "", 6800m, "LR-009", "โต๊ะเกม", "ชิ้น", "", null },
                    { 10, 2, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1117), 4, null, "เตียงนอน ขนาด 150x200 ซม. ไม้สัก", null, "", 22000m, "BR-001", "เตียงนอน ขนาด Queen", "ชิ้น", "", null },
                    { 11, 2, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1120), 7, null, "เตียงนอน ขนาด 90x200 ซม. ไม้ยาง", null, "", 9500m, "BR-002", "เตียงนอน ขนาด Single", "ชิ้น", "", null },
                    { 12, 2, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1123), 5, null, "ตู้เสื้อผ้า 2 บาน สีขาว", null, "", 7800m, "BR-003", "ตู้เสื้อผ้า 2 บาน", "ชิ้น", "", null },
                    { 13, 2, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1126), 3, null, "ตู้เสื้อผ้า 3 บาน กระจก ไม้ลัดดา", null, "", 12500m, "BR-004", "ตู้เสื้อผ้า 3 บาน", "ชิ้น", "", null },
                    { 14, 2, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1129), 8, null, "โต๊ะเครื่องแป้งพร้อมกระจก สีเคลือบ", null, "", 5200m, "BR-005", "โต๊ะเครื่องแป้ง", "ชิ้น", "", null },
                    { 15, 2, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1132), 18, null, "ที่นอน ฟอมความหนาแน่นสูง 5 นิ้ว", null, "", 3500m, "BR-006", "ที่นอน ฟอม 5 นิ้ว", "ชิ้น", "", null },
                    { 16, 2, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1135), 50, null, "หมอนข้าง หมอกฟอมยาง", null, "", 600m, "BR-007", "หมอนข้าง", "ชิ้น", "", null },
                    { 17, 2, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1138), 12, null, "ผ้าม่านกันแสง สีเบจ", null, "", 2800m, "BR-008", "ผ้าม่านห้องนอน", "ชิ้น", "", null },
                    { 18, 2, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1140), 6, null, "ตะแกรงหัวเตียง สีทอง", null, "", 4500m, "BR-009", "ตะแกรงหัวเตียง", "ชิ้น", "", null },
                    { 19, 3, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1144), 3, null, "โต๊ะอาหาร ไม้สักแท้ 6 ที่นั่ง", null, "", 16800m, "DR-001", "โต๊ะอาหาร ไม้สัก 6 ที่", "ชิ้น", "", null },
                    { 20, 3, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1147), 5, null, "โต๊ะอาหาร ไม้ยาง 4 ที่นั่ง", null, "", 8900m, "DR-002", "โต๊ะอาหาร 4 ที่", "ชิ้น", "", null },
                    { 21, 3, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1150), 24, null, "เก้าอี้อาหาร หนังสังเคราะห์ สีดำ", null, "", 1850m, "DR-003", "เก้าอี้อาหาร หนัง", "ชิ้น", "", null },
                    { 22, 3, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1152), 20, null, "เก้าอี้อาหาร ไม้บีช สีธรรมชาติ", null, "", 2200m, "DR-004", "เก้าอี้อาหาร ไม้", "ชิ้น", "", null },
                    { 23, 3, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1155), 4, null, "ตู้เก็บจาน 2 บาน กระจก", null, "", 6500m, "DR-005", "ตู้เก็บจาน", "ชิ้น", "", null },
                    { 24, 3, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1158), 8, null, "โคมไฟแขวนทรงสี่เหลี่ยม", null, "", 3200m, "DR-006", "โคมไฟห้องทานอาหาร", "ชิ้น", "", null },
                    { 25, 3, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1161), 6, null, "พรมสำหรับห้องทานอาหาร", null, "", 4500m, "DR-007", "พรม 150x200 ซม.", "ชิ้น", "", null },
                    { 26, 3, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1164), 3, null, "ตู้บาร์ 1 บาน สีวอลนัท", null, "", 5800m, "DR-008", "ตู้บาร์ไม้", "ชิ้น", "", null },
                    { 27, 3, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1167), 2, null, "โต๊ะขยายไม้สัก ขนาด 120-160", null, "", 18500m, "DR-009", "โต๊ะขยาย ไม้สัก", "ชิ้น", "", null },
                    { 28, 4, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1208), 10, null, "โต๊ะทำงาน ไม้อัด 120 ซม.", null, "", 5200m, "OF-001", "โต๊ะทำงาน 120 ซม.", "ชิ้น", "", null },
                    { 29, 4, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1211), 7, null, "โต๊ะทำงาน ไม้ลัดดา 150 ซม.", null, "", 6800m, "OF-002", "โต๊ะทำงาน 150 ซม.", "ชิ้น", "", null },
                    { 30, 4, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1214), 12, null, "เก้าอี้สำนักงาน ปรับความสูงได้", null, "", 4200m, "OF-003", "เก้าอี้สำนักงาน ปรับความสูง", "ชิ้น", "", null },
                    { 31, 4, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1217), 5, null, "เก้าอี้บอส หนัง ห้อแขนปรับได้", null, "", 6500m, "OF-004", "เก้าอี้บอสสีดำ", "ชิ้น", "", null },
                    { 32, 4, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1220), 8, null, "ชั้นหนังสือ 5 ชั้น ไม้ลัดดา", null, "", 3800m, "OF-005", "ชั้นหนังสือ 5 ชั้น", "ชิ้น", "", null },
                    { 33, 4, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1223), 6, null, "ตู้ลิ้นชัก ลูกลื่น สีเทา", null, "", 4500m, "OF-006", "ตู้ลิ้นชัก 3 ชั้น", "ชิ้น", "", null },
                    { 34, 4, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1226), 20, null, "แท่นวางจอคอมพิวเตอร์ ปรับความสูงได้", null, "", 1200m, "OF-007", "แท่นวางจอ", "ชิ้น", "", null },
                    { 35, 4, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1229), 30, null, "ไฟจอปืนกลาง LED ขาว", null, "", 800m, "OF-008", "ไฟจอปืนกลาง", "ชิ้น", "", null },
                    { 36, 4, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1232), 9, null, "โต๊ะแล็ปทอปแบบยางสด", null, "", 3500m, "OF-009", "โต๊ะแล็ป", "ชิ้น", "", null },
                    { 37, 5, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1235), 14, null, "ชั้นวาง 5 ชั้น เหล็กสีดำ", null, "", 2500m, "ST-001", "ชั้นวาง 5 ชั้น เหล็ก", "ชิ้น", "", null },
                    { 38, 5, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1238), 10, null, "ชั้นวาง 4 ชั้น ไม้บีช", null, "", 3500m, "ST-002", "ชั้นวาง 4 ชั้น ไม้", "ชิ้น", "", null },
                    { 39, 5, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1241), 5, null, "ตู้ลิ้นชัก 6 ชั้น ลูกลื่น", null, "", 5800m, "ST-003", "ตู้ลิ้นชัก 6 ชั้น", "ชิ้น", "", null },
                    { 40, 5, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1244), 6, null, "ตู้เก็บสิ่งของ 2 บาน 3 ชั้น", null, "", 4200m, "ST-004", "ตู้เก็บสิ่งของ", "ชิ้น", "", null },
                    { 41, 5, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1247), 16, null, "ลังเก็บสิ่งของ ไม้สน", null, "", 2200m, "ST-005", "ลังเก็บสิ่งของ", "ชิ้น", "", null },
                    { 42, 5, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1250), 40, null, "บล็อกเก็บสิ่งของ ผ้าใบ", null, "", 800m, "ST-006", "บล็อกเก็บสิ่งของ", "ชิ้น", "", null },
                    { 43, 5, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1252), 100, null, "กล่องเก็บของ PP ขนาดใหญ่", null, "", 350m, "ST-007", "กล่องเก็บของ PP", "ชิ้น", "", null },
                    { 44, 5, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1255), 12, null, "ชั้นวางกำแพง 3 ชั้น สีขาว", null, "", 2800m, "ST-008", "ชั้นวางกำแพง 3 ชั้น", "ชิ้น", "", null },
                    { 45, 5, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1258), 20, null, "ตะกร้าเก็บของ เหล็กสีดำ", null, "", 1500m, "ST-009", "ตะกร้าเก็บของ", "ชิ้น", "", null },
                    { 46, 6, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1261), 4, null, "โต๊ะสนามเหล็กเกาะลม", null, "", 4500m, "OD-001", "โต๊ะสนามเหล็ก", "ชิ้น", "", null },
                    { 47, 6, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1264), 18, null, "เก้าอี้สนาม พลาสติก สีส้ม", null, "", 1200m, "OD-002", "เก้าอี้สนาม", "ชิ้น", "", null },
                    { 48, 6, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1267), 5, null, "ร่มสนาม ขนาด 2.5 ม. สีทอง", null, "", 3500m, "OD-003", "ร่มสนาม", "ชิ้น", "", null },
                    { 49, 6, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1270), 8, null, "เก้าอี้พักผ่อนกลางแจ้ง", null, "", 2800m, "OD-004", "เก้าอี้พักผ่อน", "ชิ้น", "", null },
                    { 50, 6, "", new DateTime(2026, 5, 25, 18, 40, 31, 704, DateTimeKind.Local).AddTicks(1273), 3, null, "ชุดโต๊ะเก้าอี้บาร์ 3 ชิ้น", null, "", 6500m, "OD-005", "ชุดโต๊ะเก้าอี้บาร์", "ชิ้น", "", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 50);
        }
    }
}
