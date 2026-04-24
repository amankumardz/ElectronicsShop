# Electronics Shop

ASP.NET Core MVC + Razor Pages application for product browsing, repair requests, and admin dashboards.

## Architecture at a glance

- **Frontend framework:** ASP.NET Core MVC views + Razor Pages (Identity UI).
- **Styling system:** Bootstrap 5 + custom theme in `wwwroot/css/site.css`.
- **Routing pattern:** conventional MVC route + `MapRazorPages()` for Identity/account flows.
- **Auth/session:** ASP.NET Core Identity cookie auth with EF Core stores.
- **Database:** SQL Server via Entity Framework Core.

## Google Authentication Setup

This project supports email/password and Google OAuth login/signup through ASP.NET Core Identity external providers.

### 1) Create OAuth credentials in Google Cloud Console

1. Open **Google Cloud Console** → **APIs & Services** → **Credentials**.
2. Create an **OAuth 2.0 Client ID** (Web application).
3. Add authorized redirect URI:
   - `https://localhost:5001/signin-google` (or your local HTTPS URL)
4. For production, add your production callback URI as well.

### 2) Configure environment variables

Copy `.env.example` values into your environment/user-secrets:

- `AUTHENTICATION__GOOGLE__CLIENTID`
- `AUTHENTICATION__GOOGLE__CLIENTSECRET`
- `AUTHENTICATION__GOOGLE__CALLBACKPATH` (default `/signin-google`)

> ASP.NET Core maps `__` to nested config keys (`Authentication:Google:*`).

### 3) Run locally

```bash
dotnet restore
dotnet ef database update
dotnet run
```

Then open the app and use **Continue with Google** on Login/Register.

## Notes for production

- Ensure callback path and host exactly match Google console settings.
- Use secure secret storage (environment variables, Key Vault, etc.).
- Keep HTTPS enabled and verify reverse-proxy headers if applicable.
