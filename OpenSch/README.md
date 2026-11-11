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

## Bot protection (Cloudflare Turnstile)

Registration requires a Cloudflare Turnstile token. The browser renders the widget and
sends the token in the `X-Turnstile-Token` header; the API verifies it with Cloudflare
before the Identity `/register` endpoint runs. Login is intentionally not protected so
existing demo accounts keep working if the Turnstile script is blocked.

Development uses Cloudflare's public test keys from `appsettings.Development.json`:

- Site key: `1x00000000000000000000AA`
- Secret key: `1x0000000000000000000000000000000AA`

These keys always pass and are safe to publish. For production, create a Turnstile
widget for your domain and set real values through environment variables or
`dotnet user-secrets`:

```bash
dotnet user-secrets set "Turnstile:Enabled" "true" --project .\src\Web\Web.csproj
dotnet user-secrets set "Turnstile:SiteKey" "<site-key>" --project .\src\Web\Web.csproj
dotnet user-secrets set "Turnstile:SecretKey" "<secret-key>" --project .\src\Web\Web.csproj
```

`Turnstile:Enabled=false` disables verification (for example, at an offline demo). The
public site key is exposed through `GET /api/config/public`; the secret key never leaves
the server.

## ERD-aligned pengguna view

ASP.NET Identity stores users in `"AspNetUsers"`. For ERD and reporting purposes the
database also exposes a read-only view named `pengguna` with `id`, `nama`, `email`,
`peran`, and `email_terkonfirmasi`. Writes still go through Identity, and password
hashes are not exposed by the view. PostgreSQL creates it with the `AddPenggunaView`
migration; SQLite creates it during startup after `EnsureCreated`.

## Ranking page and deterministic summary

The `/peringkat` page compares schools only inside the same education level and city.
`GET /api/schools` returns each school's rank inside its own `(Level, City)` group, and
the page shows the Top N list plus a rule-based summary:

```http
GET /api/schools/ranking/summary?level=SMP&city=Jakarta%20Selatan&limit=10
```

- `level` and `city` are required and validated against the persisted data.
- `limit` is clamped to 1..50 (default 10).
- The response contains `level`, `city`, `totalSchools`, the ranked `schools`, and a
  `narrative` with `headline`, `points`, and `scopeNote`.
- The narrative is deterministic and rule-based. It never calls an AI provider; the AI
  comparison feature remains separate on `/api/compare/ai`.

## Deploy with Railway (Docker)

The repository contains a multi-stage `Dockerfile`, `.dockerignore`, and `railway.json`.
The Dockerfile builds the React SPA with Node, publishes the .NET app with
`/p:SkipClientBuild=true`, and runs it on port 8080. Local `dotnet publish` still builds
the SPA because `PublishRunWebpack` is skipped only when that property is set.

1. Create a Railway service from the GitHub repository.
2. Set **Root Directory** to `OpenSch`.
3. Set **Config File Path** to `/OpenSch/railway.json` (Railway config files do not
   follow the root directory automatically). Railway currently marks config-as-code as
   deprecated; if the file is no longer applied, use the same values in the service
   settings instead.
4. Choose the **Singapore** region and keep **one replica** (migrations run at startup).
5. Generate a public domain and set its target port to `8080`.
6. Configure the variables below in the Railway service.

| Variable | Purpose |
| --- | --- |
| `ASPNETCORE_FORWARDEDHEADERS_ENABLED` | `true`; Railway terminates TLS, so forwarded headers must be trusted. |
| `DatabaseProvider` | `Postgres`. |
| `ConnectionStrings__OpenSchDb` | Supabase PostgreSQL connection string. |
| `Cors__AllowedOrigins__0` | `https://<your-railway-domain>` (same-origin requests work without CORS, but the policy requires an explicit origin). |
| `Turnstile__Enabled` | `true` to protect registration. |
| `Turnstile__SiteKey` / `Turnstile__SecretKey` | Real Cloudflare keys for the deployed hostname. |
| `Email__FromAddress` | Sender address for confirmation and password-reset mail. |
| `Email__Smtp__Host` / `__Port` / `__UseSsl` / `__UserName` / `__Password` | SMTP credentials. Without SMTP, production registration fails while sending the confirmation email. |
| `AdminBootstrap__Email` / `__Password` | First-boot administrator only; remove the password afterwards. |
| `AiComparison__Enabled` / `__Endpoint` / `__Model` / `__ApiKey` | Optional AI comparison provider. |

Use Supabase's **session pooler on port 5432** (not the transaction pooler on 6543)
because the app applies EF Core migrations during startup. A typical value looks like:

```text
Host=aws-0-<region>.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.<project-ref>;Password=<password>;SSL Mode=Require
```

Add the deployed hostname (the Railway domain or your custom domain) to the Turnstile
widget in Cloudflare. The public test keys work on any hostname but should not be used as
production security. `GET /health` is mapped outside Development for Railway's
healthcheck.

The Docker image has not been built on this machine because Docker is not installed;
Railway builds it on the first deploy. Verify the first deployment's logs for the
`dotnet restore`, `npm ci`, and `dotnet publish` stages.

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