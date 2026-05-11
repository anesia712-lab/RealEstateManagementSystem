# Teacher and marker folder (`uploads/`)

Everything here is meant to be easy to skim: PDFs first, then the database files. If something does not match your lab setup, please **contact the developers** (the student group) with your OS and the error message.

---

## What to read first

1. **`GET_STARTED.pdf`** — Install **.NET 8**, set up SQL (Docker or restore), run the web app on **Windows** or **Mac**  
2. **`DATABASE_CREDENTIALS.pdf`** — Connection string names and examples  
3. **`EAD_SUBMISSION_CHECKLIST.pdf`** — Where Module 1 / 2 / 3 projects sit in the zip  
4. **`GROUP_MEMBERS.pdf`** — Who submitted  

---

## Files in this folder

| File | What it is |
| :--- | :--- |
| **GET_STARTED.pdf** | Plain-language setup for Windows and macOS |
| **EAD_SUBMISSION_CHECKLIST.pdf** | Repo map for EAD markers |
| **GROUP_MEMBERS.pdf** | Names and student IDs |
| **DATABASE_CREDENTIALS.pdf** | SQL connection guidance |
| **RealEstateDB_Backup.bak** | Full backup (restore in SSMS) |
| **database_schema.sql** | DDL reference |

Editable Markdown sources (for students who need to regenerate PDFs) live in **`_pdf_source/`**.

---

## Regenerating PDFs (optional)

Requires Python 3 and **fpdf2** (`pip install fpdf2` if needed). From the **repository root**:

```bash
python3 scripts/build_upload_pdfs.py
```

Same idea works on Windows if Python and fpdf2 are installed.

---

## Zipping for submission

Include **`RealEstateManagementSystem.sln`**, this **`uploads/`** folder, **`EAD_UPLOAD_PACKAGE.md`** (repo root checklist), **`docker-compose.yml`** if you rely on Docker, and the **`RealEstateManagementSystem/`** source tree—the layout in **`EAD_SUBMISSION_CHECKLIST.pdf`** matches how we expect lecturers to unpack the work.
