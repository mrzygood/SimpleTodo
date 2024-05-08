### Roadmap
- [x] Create simple TODO app's domain logic
- [x] API endpoints *.rest file
- [x] Database integration - PostgreSQL + auto-migration on app start
- [x] Logging wih serilog
- [ ] Entities not found handling (replacement for null-forgiving operator)
- [ ] Exception filter
- [ ] UI - Vue + TS
- [ ] API Versioning
- [ ] Auth

### EF Core commands
```
dotnet ef migrations add <migration_name> -o ./Persistence/Migrations
dotnet ef database update
dotnet ef migrations remove
```

### Keycloak

#### Keycloak setup
1. Create database named _keycloak_ in postgres.
2. Add _silent-check-sso.html_ and _keycloak.json_ files in public catalog.
3. Configure keycloak (look at a next section).
4. Set config in _keycloak.json_ for created client.

#### Keycloak configuration
1. Create new _realm_ dedicated to frontend app.
Next steps will be executed in context of newly created _realm_.
2. Create _client_. Select only _Standard flow_ in _Authentication flow_ section.
_Client authentication_ leave as _Off_ to allow for public access.
3. Set URLs.
   - _Root URL_ as `http://localhost:8080`
   - _Home URL_ as `http://localhost:8080`
   - _Valid redirect URIs_ as `http://localhost:8080/*`
   - _Valid post logout redirect URIs_ as `+`
   - _Web origins_ as `+`
4. Create new user.
