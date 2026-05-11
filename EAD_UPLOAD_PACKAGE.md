# EAD solution — submission package (Skyline Realtor)

**Enterprise Application Development (F-VTDI-ACAD-AST-1.1)**

Use this page when you pack your **soft copy** beside the assignment PDF (**Enterprise Application Development – Project.pdf**). Written phase work (Modules 1–4 narratives, diagrams, validations, mockups) stays **outside** this repo unless your lecturer asked for specific filenames—follow the brief.

**Marker-friendly PDFs** plus the **`.bak`** and **`database_schema.sql`** are in **`uploads/`**. Read **`uploads/README.md`**. After you change wording in **`uploads/_pdf_source/*.md`**, rebuild PDFs with:

```
python3 scripts/build_upload_pdfs.py
```

If a lecturer hits tooling issues, they should **contact the developers** (your group).

---

## 1) Desktop application — Module 1

| Item | Location |
| :--- | :--- |
| Folder | `RealEstateManagementSystem/DesktopApp/` |
| Project | `DesktopApp/DesktopApp.csproj` |
| Solution | `RealEstateManagementSystem.sln` → **DesktopApp** |

Run the GUI on **Windows** only (**WinForms**, `net8.0-windows`). The project can **compile** on macOS/Linux thanks to **`EnableWindowsTargeting`**; execution of the desktop UI is Windows-only.

---

## 2) Class library — Module 2

| Item | Location |
| :--- | :--- |
| Folder | `RealEstateManagementSystem/Module2_ClassLibrary/` |
| Project | `Module2_ClassLibrary/Module2_ClassLibrary.csproj` |

The MVC site mirrors some types under **`Models/`**. Mention **both** the library and web models if your lecturer wants a full Module 2 map.

---

## 3) Web application — Module 3

| Item | Location |
| :--- | :--- |
| Folder | `RealEstateManagementSystem/` |
| Project | `RealEstateManagementSystem.csproj` |
| Run | `dotnet run` from that folder (**.NET 8 SDK**) |

Configure **`DefaultConnection`** in **`appsettings.json`** to match your SQL Server (Docker, lab VM, etc.).

**Optional:** `docker-compose.yml`, `docker/init/`, **`scripts/init-docker-db.sh`** for local Docker SQL.

---

## 4) Database backup and schema

| Item | Location |
| :--- | :--- |
| **`.bak`** | **`uploads/RealEstateDB_Backup.bak`** |
| **DDL script** | **`uploads/database_schema.sql`** |

**Restore in SSMS:** Databases → Restore Database → Device → choose the `.bak`.

---

## 5) Credentials and group roster

| Topic | Markdown (source of truth while developing) | PDF (markers) |
| :--- | :--- | :--- |
| SQL / connections | **`RealEstateManagementSystem/database_credentials.md`** | **`uploads/DATABASE_CREDENTIALS.pdf`** |
| Group members | **`RealEstateManagementSystem/group_members.md`** | **`uploads/GROUP_MEMBERS.pdf`** |

Before you submit, make sure **`group_members.md`** and **`GROUP_MEMBERS.pdf`** carry the correct **student IDs** (replace **`--`** when known).

---

## 6) One solution file for Visual Studio / VS Code

| File | Opens |
| :--- | :--- |
| **`RealEstateManagementSystem.sln`** | Web + DesktopApp + Module2_ClassLibrary |

---

## Suggested ZIP layout (example)

```text
YourGroupName_EAD_Solution/
├── RealEstateManagementSystem.sln
├── EAD_UPLOAD_PACKAGE.md
├── README.md
├── docker-compose.yml              optional
├── docker/
├── scripts/
├── uploads/                        lecturer PDFs, .bak, .sql
│   └── _pdf_source/                Markdown sources for PDF build
├── RealEstateManagementSystem/
│   ├── RealEstateManagementSystem.csproj
│   ├── DesktopApp/
│   ├── Module2_ClassLibrary/
│   ├── Models/, Views/, Controllers/, …
│   ├── appsettings.json
│   ├── database_credentials.md
│   └── group_members.md
└── Your_Module1_to_4_Documentation.pdf
```

---

**Lecturer:** Ssyade Gayle  
**Sample due date on brief:** April 26, 2026 — confirm on Canvas.
