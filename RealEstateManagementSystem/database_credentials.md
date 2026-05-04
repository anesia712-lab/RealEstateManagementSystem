# Real Estate Management System - Database Credentials

**Connection String (Windows Authentication / SQL Server):**
```
Data Source=KHRISEAN;Initial Catalog=RealEstateDB;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name="SQL Server Management Studio";Command Timeout=0
```

**Database Provider:** SQL Server (`Microsoft.Data.SqlClient`)
**Server Name:** KHRISEAN
**Database Name:** RealEstateDB

**Authentication Type:** Windows Authentication (Integrated Security)
*No explicit username or password is required to connect to the database locally, as it uses your Windows credentials.*

### Hardcoded Application Fallback Credentials
In the event the database cannot be reached from the Desktop Application, the following fallback credentials will grant access:
- **Email/Username**: `admin@admin.com`
- **Password**: `admin123`
