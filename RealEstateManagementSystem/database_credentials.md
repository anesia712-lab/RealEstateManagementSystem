# Skyline Realtor — database access and credentials (EAD)

Markers get a printable copy: **`uploads/DATABASE_CREDENTIALS.pdf`**. This Markdown file is the living reference for your team while you develop.

Share enough detail here (and in **`appsettings.json`**) that a lecturer can point the **web app** and **desktop app** at the right SQL Server without guessing.

---

## 1. Web application (`RealEstateManagementSystem`)

Connection string name: **`DefaultConnection`**  
Configuration file: **`appsettings.json`**

### Current configuration (Docker SQL Server — local development)

Example as committed for local grading:

```
Server=localhost,1433;Initial Catalog=RealEstateDB;User Id=sa;Password=RealEstate_local_1;Encrypt=True;TrustServerCertificate=True;
```

| Field | Value |
| :--- | :--- |
| **Server** | `localhost,1433` (host port mapped from Docker; see repo root **`docker-compose.yml`**) |
| **Database** | `RealEstateDB` |
| **Authentication** | SQL login |
| **User** | `sa` |
| **Password** | `RealEstate_local_1` (change in Compose + appsettings together if needed) |

**First-time Docker setup (optional):**

1. Repo root: `docker compose up -d`  
2. Run `./scripts/init-docker-db.sh` (creates database + tables).  
3. Run web app: `dotnet run` inside `RealEstateManagementSystem/` (folder with the web `.csproj`).

### Alternative — Remote SQL Server (lab / VPS)

If you deploy to instructor-provided or cloud SQL Server, paste the **`Server`** (or endpoint), **`Initial Catalog`**, **`User Id`**, and **`Password`** your host gave you here and mirror them into **`appsettings.json`**.

> **Submission tip:** Prefer a **temporary account** dedicated to grading, not your personal passwords, if uploading to shared drives.

---

## 2. Desktop application (`DesktopApp`)

Connection strings are **hardcoded per form / login** under `DesktopApp/*.cs` (and may match `appsettings.json` if you standardized on Docker).

- **Production-style submission:** Update those strings to the same **`Server`** / **`RealEstateDB`** / **`User Id`** / **`Password`** the marker should use.

### Demo / fallback behaviour (desktop)

When the desktop app cannot reach SQL Server (or for module demonstration), **`LoginForm`** may still authenticate using the hardcoded fallback listed in course materials / your Module 1 spec. Confirm what your lecturer expects for **final** submission (**database-backed login** is typically required).

---

## 3. Backup & schema artifacts

| File | Purpose |
| :--- | :--- |
| **`RealEstateDB_Backup.bak`** | Full SQL Server backup for restore in SSMS |
| **`database_schema.sql`** | Script reference (historical `/ Windows path create DB` segments may need trimming on Linux/docker; prefer **`.bak` restore** + **`docker/init/01-realestate.sql`** for container dev) |

---

## 4. Technology

- **Provider:** SQL Server (`Microsoft.Data.SqlClient`)  
- **Web stack:** ASP.NET Core (see web `.csproj` `TargetFramework`)  
- **Desktop:** Windows Forms (see `DesktopApp/DesktopApp.csproj`)

---

## 5. Windows Authentication (only if you submit that setup)

If the marker should use **Integrated Security** on a named SQL instance, document the live string here **only for that environment**, for example:

```
Data Source=YOUR_SERVER_NAME;Initial Catalog=RealEstateDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;
```

*(Integrated Security requires the marker’s Windows identity to have rights on SQL Server—not suitable for anonymous cloud hosts.)*
