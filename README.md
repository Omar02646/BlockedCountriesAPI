# 🌍 Country Blocking API

## 📌 عن المشروع
API لحظر الدول بناءً على الـ IP، مع إمكانية الحظر الدائم أو المؤقت، والبحث عن موقع الـ IP، وتسجيل محاولات الوصول المرفوضة.

## 🛠️ التقنيات المستخدمة
- **.NET 8 Web API**
- **ip-api.com** (خدمة مجانية لتحديد موقع الـ IP)
- **In-Memory Storage** باستخدام ConcurrentDictionary
- **Background Service** لإزالة الحظر المؤقت
- **Swagger** لتوثيق الـ API

## 🚀 طريقة التشغيل

```bash
dotnet restore
dotnet run
