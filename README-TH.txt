LOGBOOK + SQL SERVER (ASP.NET Core .NET 8)

สิ่งที่ต้องมี: .NET 8 SDK และ SQL Server (Express/Developer ก็ได้)

1) เปิด appsettings.json และแก้ DefaultConnection ให้ตรงกับ SQL Server ของคุณ
SQL Server ปกติ: Server=localhost;Database=LogbookDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True
SQL Express: Server=localhost\SQLEXPRESS;Database=LogbookDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True
LocalDB: Server=(localdb)\MSSQLLocalDB;Database=LogbookDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True

2) เปิด Terminal ในโฟลเดอร์โปรเจกต์ แล้วตั้งบัญชี Admin ใน PowerShell (ใช้หน้าต่างเดียวกับที่รัน):
$env:LOGBOOK_ADMIN_USERNAME="admin"
$env:LOGBOOK_ADMIN_PASSWORD="ใส่รหัสผ่านที่เดายากอย่างน้อย 12 ตัวอักษร"
dotnet restore
dotnet run

3) เปิด URL ที่แสดงใน Terminal แล้วระบบจะพาไปหน้า Login หากยังไม่ได้เข้าสู่ระบบ

ระบบสร้างฐานข้อมูล/ตารางด้วย EnsureCreated และสร้าง Admin ครั้งแรกเมื่อกำหนด environment variables หากไม่ได้ตั้งตัวแปร จะยังไม่มีบัญชี Login

มี Login/Logout, เพิ่ม/ดู/แก้ไข/ลบข้อมูลผ่าน API ที่ต้อง Login, Dashboard, รายงาน และส่งออก CSV ข้อมูลอยู่ใน SQL Server ไม่ใช่ localStorage

ข้อควรระวัง: ตัวอย่างนี้เหมาะกับการเริ่มต้นพัฒนา ก่อนใช้งานจริงควรใช้ HTTPS, เพิ่ม CSRF protection และ rate limiting, จัดการสิทธิ์ผู้ใช้/การเปลี่ยนรหัสผ่าน, ทำ migrations/backup และจำกัดการเข้าถึงข้อมูลส่วนบุคคล

ฟีเจอร์หน้าข้อมูลครอบครัวและรูปภาพ
1. ก่อนเปิดระบบ ให้เปิดไฟล์ sql/AddBeneficiaryMedia.sql ใน SQL Server Management Studio และ Execute กับฐานข้อมูล LogbookDb เพียงครั้งเดียว เพื่อสร้างตารางเก็บรูปภาพ
2. เปิดระบบและเข้าสู่ระบบตามปกติ จากหน้า ข้อมูลผู้รับความช่วยเหลือ ให้คลิกชื่อบุคคลเพื่อเปิดหน้า family.html
3. ในหน้าข้อมูลครอบครัว สามารถอัปโหลดรูปบ้าน และอัปโหลดรูปหลักฐานการช่วยเหลือ พร้อมชื่อหน่วยงาน วันที่ และรายละเอียดได้
4. รองรับรูป JPG, PNG และ WEBP ขนาดไม่เกิน 10 MB ต่อรูป รูปจะถูกเก็บใน wwwroot/uploads/beneficiaries แยกตามรหัสบุคคล
5. หากไม่มีรูป ระบบจะแสดงข้อความว่าไม่มีรูป และรูปภาพ/ข้อมูลของแต่ละบุคคลจะถูกแยกกัน

เพิ่มระบบข้อมูลครอบครัว (family pages)
1) สำรองโปรเจกต์เดิมก่อนแตก ZIP นี้ทับไฟล์
2) เปิด SQL Server Management Studio และเลือกฐานข้อมูล LogbookDb
3) เปิดและ Execute ไฟล์ sql/AddBeneficiaryMedia.sql เพื่อสร้างตารางรูปภาพ และ sql/AddHouseholdMembers.sql เพื่อสร้างตารางสมาชิกครอบครัว (ทำครั้งเดียวต่อฐานข้อมูล) โดยไม่ลบข้อมูลเดิม
4) เปิดโฟลเดอร์โปรเจกต์ใน Visual Studio แล้ว Run
5) เมนู “ข้อมูลใหม่ครอบครัว” ใช้เพิ่มครอบครัวใหม่; หลังบันทึกจะไปหน้ารายละเอียดครอบครัวเพื่ออัปโหลดรูปบ้าน/หลักฐาน
6) เมนู “อัปเดตข้อมูลครอบครัว” ใช้ค้นหาและแก้ไขข้อมูลครอบครัวเดิม รวมถึงเพิ่ม/ลบรูป
7) ในหน้า family.html จะแสดงหัวหน้าครอบครัวและรายชื่อสมาชิกทุกคนที่บันทึกไว้ สามารถเพิ่มหรือลบสมาชิกได้ รายชื่อสมาชิกแยกจากหัวหน้าครอบครัวซึ่งเป็นข้อมูลหลักของผู้รับความช่วยเหลือ
8) รูปภาพรองรับ JPG/JPEG, PNG, WEBP ขนาดไม่เกิน 10 MB ต่อรูป

หมายเหตุเรื่องรูปภาพ: ข้อมูลกำกับรูป (ประเภทภาพ หน่วยงาน รายละเอียด วันที่ และพาธ) เก็บใน SQL Server ส่วนไฟล์รูปจริงเก็บใน wwwroot/uploads/beneficiaries/ ตามรหัสครอบครัว ดังนั้นควรสำรองทั้งฐานข้อมูล LogbookDb และโฟลเดอร์ uploads ด้วย
