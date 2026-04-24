# HamburguerDev

Aplicação para gestão de pedidos de hamburgueria com backend ASP.NET Core (arquitetura em camadas), banco SQLite e frontend em Blazor.

## Estrutura da solução

- src/HamburguerDev.Api: API ASP.NET Core (controllers, configurações, view models e mapeamentos)
- src/HamburguerDev.Business: regras de negócio, serviços, modelos de domínio e notificações
- src/HamburguerDev.Data: repositórios, contexto EF Core, mapeamentos e migrações
- src/HamburguerDev.Shared: configurações compartilhadas (incluindo caminho do SQLite)
- src/HamburguerDev.Frontend: frontend Blazor Server
- tests/HamburguerDev.Business.Tests: testes unitários da camada de negócio

## Requisitos

- .NET SDK 8.0+
- Windows, Linux ou macOS

## Banco de dados

- Banco: SQLite
- Arquivo de configuração compartilhada: src/HamburguerDev.Shared/CommonConfigurations/sharedsettings.json
- As migrações são aplicadas automaticamente na inicialização da API.

## Como executar

1. Restaurar pacotes

```bash
dotnet restore HamburguerDev.sln
```

2. Subir a API

```bash
dotnet run --project src/HamburguerDev.Api/HamburguerDev.Api.csproj
```

Ambiente de desenvolvimento (padrão do launchSettings):

- API HTTP: http://localhost:5196
- API HTTPS: https://localhost:7113
- Swagger: https://localhost:7113/swagger

3. Subir o frontend Blazor (em outro terminal)

```bash
dotnet run --project src/HamburguerDev.Frontend/HamburguerDev.Frontend.csproj
```

Por padrão, o frontend usa a base da API em https://localhost:7113 (configurável em src/HamburguerDev.Frontend/appsettings*.json).

## Testes

Executar todos os testes da solução:

```bash
dotnet test HamburguerDev.sln
```

Executar somente os testes de negócio:

```bash
dotnet test tests/HamburguerDev.Business.Tests/HamburguerDev.Business.Tests.csproj
```

## Decisões de arquitetura

1. Arquitetura em camadas
- Separação clara entre API, domínio (Business), dados (Data) e frontend.
- Objetivo: reduzir acoplamento, facilitar manutenção e evoluções por módulo.

2. API fina e regras no domínio
- Controllers concentram orquestração HTTP.
- Regras de negócio ficam em services da camada Business.
- Objetivo: manter comportamento centralizado e testável fora da camada web.

3. Persistência via repositórios
- A camada Data abstrai EF Core por meio de repositórios.
- Objetivo: isolar acesso a dados e manter serviços de negócio independentes de detalhes de infraestrutura.

4. Notificações de domínio para erros de regra
- Uso de INotificador/Notificador para acumular erros de validação e negócio.
- Objetivo: padronizar resposta de erro sem lançar exceções para cenários esperados de regra.

5. DTOs/ViewModels e AutoMapper
- Contratos de entrada/saída da API são separados dos modelos de domínio.
- Objetivo: desacoplar modelo interno da API pública e facilitar evolução de contrato.

6. Frontend Blazor Server com serviço de API
- O frontend concentra chamadas HTTP em IPedidoApiService/PedidoApiService.
- Objetivo: evitar espalhamento de chamadas HTTP nos componentes e simplificar manutenção.

7. Testes unitários focados em regras
- Cobertura principal em PedidoService e ProdutoService com xUnit + Moq.
- Objetivo: proteger regras críticas (validações, desconto, finalização, atualização e exclusão) contra regressões.
