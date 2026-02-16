# Redshift Guardian .NET 4.0

**תאריך:** 15 בפברואר 2026
**פלטפורמה:** .NET Framework 4.0 + Visual Studio 2010
**ארכיטקטורה:** WinForms + ODBC + ADO.NET

---

## מבנה הפרויקט

```
RedshiftGuardianNET/
├── RedshiftGuardianNET.csproj    ← קובץ הפרויקט לVS 2010
├── App.config                     ← הגדרות ו-connection strings
├── Models/
│   ├── Cluster.cs                ← אובייקט cluster
│   ├── RedshiftUser.cs           ← משתמש Redshift
│   ├── TablePermission.cs        ← הרשאת טבלה
│   ├── RoleLineage.cs            ← ירושת תפקידים
│   └── ScanResult.cs             ← תוצאת סריקה
├── DataAccess/
│   ├── RedshiftConnectionFactory.cs  ← יצירת חיבורי ODBC ⭐
│   ├── RedshiftRepository.cs         ← שאילתות ל-Redshift ⭐
│   ├── DatabaseContext.cs            ← (לייצר)
│   └── ClusterRepository.cs          ← (לייצר)
├── Services/
│   ├── ClusterService.cs         ← (לייצר)
│   └── ScannerService.cs         ← (לייצר)
└── Forms/
    ├── DashboardForm.cs          ← (לייצר)
    └── ClusterEditForm.cs        ← (לייצר)
```

---

## ✅ מה סיימנו עד כה

### 1. מבנה פרויקט ✅
- קובץ .csproj מוכן לVS 2010
- App.config עם הגדרות
- מבנה תיקיות

### 2. Models Layer ✅
- **Cluster.cs** - מחזיק פרטי cluster (host, port, region, etc.)
- **RedshiftUser.cs** - משתמש Redshift
- **TablePermission.cs** - הרשאה על טבלה
- **RoleLineage.cs** - ירושת תפקידים
- **ScanResult.cs** - תוצאת סריקה

### 3. ODBC Integration ✅ ⭐
- **RedshiftConnectionFactory.cs**
  - יוצר חיבור ODBC ל-Redshift
  - תומך ב-IAM authentication
  - תומך גם ב-username/password

- **RedshiftRepository.cs**
  - שאילתות מלאות:
    - `GetAllUsers()` - כל המשתמשים
    - `GetTablePermissions()` - הרשאות טבלאות
    - `GetRoleLineage()` - ירושת roles
    - `GetSchemaPermissions()` - הרשאות schemas
    - `TestConnection()` - בדיקת חיבור
    - `ExecuteQuery()` - שאילתה חופשית

---

## 🔧 מה נשאר לעשות

### Phase 2: Local Database (SQL Server Compact)
- [ ] `DatabaseContext.cs` - חיבור ל-DB מקומי
- [ ] `ClusterRepository.cs` - CRUD של clusters
- [ ] CREATE TABLE scripts

### Phase 3: Business Logic
- [ ] `ClusterService.cs` - ניהול clusters
- [ ] `ScannerService.cs` - סריקת permissions

### Phase 4: WinForms UI
- [ ] `Program.cs` - entry point
- [ ] `DashboardForm.cs` - מסך ראשי
- [ ] `ClusterEditForm.cs` - עריכת cluster
- [ ] Forms designers (.Designer.cs)
- [ ] Resource files (.resx)

### Phase 5: Build & Test
- [ ] Build ב-VS 2010
- [ ] Test connection ל-Redshift
- [ ] Package כ-EXE

---

## 🚀 איך לפתוח ב-VS 2010

### שלב 1: העתק קבצים ל-Windows

העתק את כל התיקייה:
```
RedshiftGuardianNET/ → C:\Projects\RedshiftGuardianNET\
```

### שלב 2: פתח ב-Visual Studio 2010

1. פתח VS 2010
2. File → Open → Project/Solution
3. בחר: `C:\Projects\RedshiftGuardianNET\RedshiftGuardianNET.csproj`
4. אמור להיטען הפרויקט!

### שלב 3: התקן תלויות

צריך להתקין ב-Windows:

1. **Amazon Redshift ODBC Driver (64-bit)**
   ```
   https://docs.aws.amazon.com/redshift/latest/mgmt/odbc20-install.html
   ```

2. **SQL Server Compact 4.0**
   ```
   https://www.microsoft.com/en-us/download/details.aspx?id=17876
   ```

### שלב 4: הגדר AWS Credentials

צור קובץ: `C:\Users\<Username>\.aws\credentials`

```ini
[default]
aws_access_key_id = YOUR_ACCESS_KEY
aws_secret_access_key = YOUR_SECRET_KEY
```

או השתמש ב-AWS CLI:
```cmd
aws configure
```

---

## 💡 דוגמאות שימוש

### חיבור ODBC ידני (בשביל לבדוק)

```csharp
using RedshiftGuardianNET.DataAccess;

// Create connection with IAM
var conn = RedshiftConnectionFactory.CreateConnection(
    host: "my-cluster.abc123.us-east-1.redshift.amazonaws.com",
    port: 5439,
    database: "dev",
    region: "us-east-1",
    awsProfile: "default"
);

// Test it
bool success = RedshiftConnectionFactory.TestConnection(conn);
Console.WriteLine("Connection: " + (success ? "OK" : "Failed"));
```

### שליפת Users

```csharp
using RedshiftGuardianNET.DataAccess;
using RedshiftGuardianNET.Models;

var cluster = new Cluster
{
    Host = "my-cluster.abc123.us-east-1.redshift.amazonaws.com",
    Port = 5439,
    Database = "dev",
    Region = "us-east-1",
    AwsProfile = "default",
    UseIAM = true
};

var repo = new RedshiftRepository(cluster);
var users = repo.GetAllUsers();

foreach (var user in users)
{
    Console.WriteLine("User: {0}, Superuser: {1}",
        user.Username, user.IsSuperuser);
}
```

### שליפת Permissions

```csharp
var repo = new RedshiftRepository(cluster);
var permissions = repo.GetTablePermissions();

foreach (var perm in permissions)
{
    Console.WriteLine("{0} has {1} on {2}.{3}",
        perm.Username,
        perm.PermissionType,
        perm.SchemaName,
        perm.TableName);
}
```

---

## ⚠️ בעיות נפוצות

### 1. "Driver not found"

**בעיה:** ODBC Driver לא מותקן או לא נמצא.

**פתרון:**
- התקן Amazon Redshift ODBC Driver
- בדוק ב-ODBC Data Source Administrator (64-bit):
  - Control Panel → Administrative Tools → ODBC Data Sources (64-bit)
  - Drivers tab → צריך לראות "Amazon Redshift (x64)"

### 2. "IAM authentication failed"

**בעיה:** AWS credentials לא נמצאו או לא תקינים.

**פתרון:**
- בדוק שקיים `~/.aws/credentials`
- בדוק שה-profile name נכון (default)
- נסה להריץ: `aws sts get-caller-identity` כדי לאמת credentials

### 3. "SSL/TLS error"

**בעיה:** SSL certificate validation נכשלה.

**פתרון:**
- הוסף `SSLMode=require` ל-connection string (כבר קיים)
- עדכן .NET 4.0 ל-latest service pack
- עדכן Windows root certificates

### 4. Project won't build in VS 2010

**בעיה:** קבצים חסרים או references שגויים.

**פתרון:**
- בדוק שכל קבצי .cs נוצרו
- Right-click על הפרויקט → Properties → Build
- Target Framework צריך להיות: .NET Framework 4.0

---

## 📚 קבצים חשובים

### App.config

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <add key="RedshiftOdbcDriver" value="Amazon Redshift (x64)" />
    <add key="DefaultAwsProfile" value="default" />
    <add key="DefaultRegion" value="us-east-1" />
  </appSettings>
</configuration>
```

### RedshiftConnectionFactory - Connection String

```csharp
// עם IAM:
Driver={Amazon Redshift (x64)};
Server=my-cluster.abc123.us-east-1.redshift.amazonaws.com;
Port=5439;
Database=dev;
Region=us-east-1;
IAM=1;                     ← מאפשר IAM
Profile=default;           ← AWS profile
SSL=1;
SSLMode=require;

// עם username/password:
Driver={Amazon Redshift (x64)};
Server=...;
Port=5439;
Database=dev;
UID=myuser;                ← username
PWD=mypassword;            ← password
SSL=1;
SSLMode=require;
```

---

## 🎯 הצעדים הבאים

אחרי שתפתח ב-VS 2010, צריך להשלים:

1. **DatabaseContext.cs** - ניהול local DB (SQL Server Compact)
2. **ClusterRepository.cs** - שמירת clusters ב-local DB
3. **Services** - ClusterService, ScannerService
4. **Forms** - UI WinForms
5. **Program.cs** - entry point

אבל הליבה - **ODBC Integration** - **כבר מוכנה!** ⭐

---

## 📞 תמיכה

**אם יש בעיות:**

1. בדוק ODBC Driver מותקן:
   ```cmd
   odbcad32.exe
   ```

2. בדוק AWS credentials:
   ```cmd
   aws sts get-caller-identity
   ```

3. Test connection string ידנית ב-ODBC Data Source Administrator

4. בדוק logs ב-Event Viewer (אם ODBC נכשל)

---

**נוצר:** 15 בפברואר 2026
**סטטוס:** ✅ Phase 1 הושלמה - ODBC Integration מוכנה!
**הצעד הבא:** פתח ב-VS 2010 והשלם את ה-Forms!

**קובץ ZIP יצוא:** יש ליצור ארכיון להעברה ל-Windows.
