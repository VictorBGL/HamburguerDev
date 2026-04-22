# HamburguerDev

Estrutura inicial da solução baseada na arquitetura em camadas do projeto `gestao-contas-mba`, com nomenclatura adaptada para `HamburguerDev`.

## Estrutura

- `src/HamburguerDev.Api` - API ASP.NET Core
- `src/HamburguerDev.Business` - Camada de negócio (vazia)
- `src/HamburguerDev.Data` - Camada de dados (EF Core + SQLite)
- `src/HamburguerDev.Shared` - Configurações compartilhadas

## Banco de dados

O SQLite está configurado em:

- `src/HamburguerDev.Shared/CommonConfigurations/sharedsettings.json`

Ao iniciar a API, as migrações são aplicadas automaticamente e as tabelas do Entity Framework são criadas.

## Como executar

```bash
dotnet restore HamburguerDev.slnx
dotnet run --project src/HamburguerDev.Api/HamburguerDev.Api.csproj
```

Swagger disponível em ambiente de desenvolvimento.
