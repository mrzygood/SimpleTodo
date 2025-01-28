### Roadmap
- [x] Create simple TODO app's domain logic
- [x] API endpoints *.rest file
- [x] Database integration - PostgreSQL + auto-migration on app start
- [x] Logging wih serilog
- [ ] Entities not found handling (replacement for null-forgiving operator)
- [ ] Exception filter
- [ ] UI - Vue + TS
- [ ] API Versioning

TODO:
- Generate migrations locally and paste to the script
- Add azure resource group
- Create resources in azure portal: postgres, app service, container registry
- Build image locally and push to azure container registry
- Generate service principal and save as secret in Github to use later in github actions
- Generate connection string for the app with lower permissions
- Prepare github action


### EF Core commands
```
dotnet ef migrations add <migration_name> -o ./Persistence/Migrations
dotnet ef database update
dotnet ef migrations remove

dotnet ef migrations script # Generate SQL script for migrations
```

Article steps:

Generate SQL script for migrations:
```bash
dotnet ef migrations script
```

Remove redundant code and leave only this which create schema and table.
Prepare catalogs consistent with "grate" default catalog structure.
Applied naming convention for migration files is taken from the liquid base because seems to be readable.

TODO what is naming convention recommended by "grate".

# Create resource group

```bash
az group create -l polandcentral -n simple-todo
```

# Create container registry

```bash
az acr create --name simpletodo `
              --resource-group simple-todo `
              --location polandcentral `
              --sku Basic `
              --public-network-enabled true  
```


