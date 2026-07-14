# UnitTests_ExpenseAPI

A small ASP.NET Core Web API for managing expenses and exporting Excel reports using ClosedXML. Stores data in SQLite via Entity Framework Core and exposes endpoints to manage expenses and categories and to import/export Excel reports.

## Features
- CRUD for expenses and categories
- Summary DTO endpoints for reporting
- Export monthly and yearly Excel workbooks (ClosedXML)
- Excel import scaffold (TODO)
- SQLite via EF Core
- Swagger UI for interactive API exploration

## Tech stack
- .NET 10
- C# 13
- ASP.NET Core Web API
- Entity Framework Core (SQLite)
- ClosedXML

## Prerequisites
- .NET 10 SDK
- __Visual Studio 2022__ (recommended) or VS Code

## Quick start (local)
1. Clone:
   git clone https://github.com/LucasNoffsMotta/UnitTests_ExpenseAPI.git
2. Build:
   dotnet restore
   dotnet build
3. Run:
   dotnet run --project API
4. Open the API in a browser:
   - Swagger UI is available in Development at `/swagger`
   - In __Visual Studio 2022__ press __F5__ or use __Debug > Start Debugging__.

## Configuration
Key settings in `API/appsettings.json`:
- `ConnectionStrings:DefaultConnection` — SQLite DB path (default: `Application.db`)
- `BasicReportFilePath`, `FullReportFilePath` — sample output paths used in development

Update paths to suit your environment.

## API Endpoints (summary)
Base path: `/api`

Expenses:
- `GET /api/expenses` — list all expenses
- `GET /api/expenses/summary` — list summary DTOs
- `GET /api/expenses/byMonth/{month}` — list by month (1-12)
- `GET /api/expenses/{id}` — get by id
- `POST /api/expenses` — create (`CreateExpenseDTO`)
- `DELETE /api/expenses/{id}` — delete

Categories:
- `GET /api/category`
- `POST /api/category` — create (`CreateCategoryDTO`)
- `POST /api/category/delete/{ID}` — delete by id

Excel (controller: `ExcelController`):
- `POST /api/excel/exportMonthReport?month={month}` (body: `ExportFolderDTO`) — saves month report to specified folder
- `POST /api/excel/exportYearReport` (body: `ExportFolderDTO`) — saves full-year workbook
- `POST /api/excel/import` (body: `ImportExcelDTO`) — import (work in progress)

## DTO examples
`CreateExpenseDTO`:
{
  "CategoryID": 1,
  "Value": 123.45,
  "Date": "2025-03-01"
}

`CreateCategoryDTO`:
{
  "Description": "Groceries",
  "HexadecimalColor": "#FF0000"
}

`ExportFolderDTO`:
{ "Path": "C:\\temp" }

`ImportExcelDTO`:
{ "DataFile": "C:\\temp\\ToImport.xlsx" }

## Important notes & TODOs
- Excel import (`GetObjectsFromExcel`) is not implemented — returns an empty list (see `API/Services/Excel/ExcelService.cs`).
- No authentication/authorization — do not expose to untrusted networks without adding auth.
- No unit tests included. Consider adding tests for controllers and Excel generation.

## Project layout
- `API/` — Web API project
  - `Controllers/` — API controllers
  - `Services/Excel/` — Excel service and interface
  - `DTO/` — request/response DTOs
  - `Program.cs`, `appsettings.json`

## Contributing
Follow repository coding style and open PRs for new features or fixes. See `CONTRIBUTING.md` (create or update it if missing).

## Troubleshooting
- If the SQLite file cannot be created, ensure the running user has write permission to the folder referenced in `DefaultConnection`.

## License
Open source — check repository for the chosen license or add one if missing.
