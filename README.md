### Roadmap
- [x] Create simple TODO app's domain logic
- [x] API endpoints *.rest file
- [x] Database integration - PostgreSQL + auto-migration on app start
- [x] Logging wih serilog
- [ ] Entities not found handling (replacement for null-forgiving operator)
- [ ] Exception filter
- [ ] UI - Vue + TS
- [ ] API Versioning

### EF Core commands
```
dotnet ef migrations add <migration_name> -o ./Persistence/Migrations
dotnet ef database update
dotnet ef migrations remove
```
