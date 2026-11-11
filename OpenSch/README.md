# OpenSch

The project was generated using the [Clean.Architecture.Solution.Template](https://github.com/jasontaylordev/CleanArchitecture) version 10.8.0.

## Build

Run `dotnet build` to build the solution.

## Run

To run the application:

```bash
dotnet run --project .\src\AppHost
```

The Aspire dashboard will open automatically, showing the application URLs and logs.

## School data and admin import

A fresh database is seeded with 50 demo schools so every read endpoint returns data.
Seeding only backfills missing columns on existing rows; it never overwrites values an
administrator has imported.

School reads (`GET /api/schools`, `POST /api/compare`) always come from the database.

### Create the first administrator

Set the bootstrap credentials with `dotnet user-secrets` (never commit them):

```bash
dotnet user-secrets set "AdminBootstrap:Email" "admin@example.com" --project .\src\Web\Web.csproj
dotnet user-secrets set "AdminBootstrap:Password" "<a-strong-password>" --project .\src\Web\Web.csproj
```

On the next start the app creates that user (if needed) and assigns the `Administrator`
role. Remove the two secrets afterwards if you no longer want startup to manage the account.

### Import schools from CSV

Sign in as the administrator, then open `/admin/impor` (the **Impor Data** navbar link).
A template is available at `samples/template-impor-sekolah.csv`.

Required columns: `npsn,nama,jenjang,wilayah,literasi,numerasi`

Optional columns: `status,kecamatan,murid,guru,ruang_kelas,tanggal_data`

Rules:

- `npsn` must be 8 digits and unique within the file; rows are upserted by NPSN.
- `jenjang` is `SD`, `SMP`, or `SMK`; `status` is `Negeri` or `Swasta`.
- `literasi` and `numerasi` are numbers between 0 and 100.
- `tanggal_data` uses `yyyy-MM-dd`.
- Maximum file size is 2 MB.

The import is all-or-nothing: if any row is invalid, nothing is written and the portal
shows a row-level error report. Successful and failed runs are recorded in `impor_data`.
The feature is only reachable through the administrator-only `/api/admin/*` endpoints;
public school reads are never affected by an import.

## Code Styles & Formatting

The template includes [EditorConfig](https://editorconfig.org/) support to help maintain consistent coding styles for multiple developers working on the same project across various editors and IDEs. The **.editorconfig** file defines the coding styles applicable to this solution.

## Code Scaffolding

The template includes support to scaffold new commands and queries.

Start in the `.\src\Application\` folder.

Create a new command:

```
dotnet new ca-usecase --name CreateTodoList --feature-name TodoLists --usecase-type command --return-type int
```

Create a new query:

```
dotnet new ca-usecase -n GetTodos -fn TodoLists -ut query -rt TodosVm
```

If you encounter the error *"No templates or subcommands found matching: 'ca-usecase'."*, install the template and try again:

```bash
dotnet new install Clean.Architecture.Solution.Template::10.8.0
```

## Test

The solution contains unit, integration, and functional tests.

To run the tests:
```bash
dotnet test
```

## Help
To learn more about the template go to the [project website](https://cleanarchitecture.jasontaylor.dev). Here you can find additional guidance, request new features, report a bug, and discuss the template with other users.