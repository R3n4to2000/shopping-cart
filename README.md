# Carrinho de Compras

Aplicação full-stack para gerenciamento de um carrinho de compras.

- Backend: ASP.NET Core com Entity Framework Core e SQL Server;
- Frontend: React, TypeScript e Vite;
- Banco de dados executado em container Docker.

## Estrutura

```text
src/
├── ShoppingCart.Domain
├── ShoppingCart.Application
├── ShoppingCart.Infrastructure
└── ShoppingCart.Api

tests/
├── Domain.Tests
└── Application.Tests

frontend/
└── Aplicação React
```

O backend utiliza uma Clean Architecture reduzida:

- `Domain`: entidades e regras de negócio;
- `Application`: casos de uso e interfaces dos repositórios;
- `Infrastructure`: Entity Framework Core, SQL Server, migrations, seed e repositórios;
- `Api`: Controllers, contratos HTTP, Swagger e tratamento de erros;
- `frontend`: interface que consome a API.

## Pré-requisitos

- .NET SDK 8;
- Node.js e npm;
- Docker Desktop.

## Banco de dados

Crie o container SQL Server:

```bash
docker run \
  --name shoppingcart-sqlserver \
  -e 'ACCEPT_EULA=Y' \
  -e 'MSSQL_PID=Developer' \
  -e 'MSSQL_SA_PASSWORD=ShoppingCart@2026Db' \
  -p 1433:1433 \
  -v shoppingcart_sqlserver_data:/var/opt/mssql \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

Nas próximas execuções:

```bash
docker start shoppingcart-sqlserver
```

Configure a connection string.

### Git Bash

```bash
export ConnectionStrings__DefaultConnection='Server=localhost,1433;Database=ShoppingCartDb;User Id=sa;Password=ShoppingCart@2026Db;TrustServerCertificate=True;Encrypt=False;'
```

### PowerShell

```powershell
$env:ConnectionStrings__DefaultConnection='Server=localhost,1433;Database=ShoppingCartDb;User Id=sa;Password=ShoppingCart@2026Db;TrustServerCertificate=True;Encrypt=False;'
```

Aplique as migrations:

```bash
dotnet tool restore

dotnet ef database update \
  --project src/ShoppingCart.Infrastructure/ShoppingCart.Infrastructure.csproj \
  --startup-project src/ShoppingCart.Api/ShoppingCart.Api.csproj \
  --context ShoppingCartDbContext
```

A migration cria as tabelas e popula os 10 produtos e os cupons `10OFF` e `15OFF`.

## Executando o backend

Na raiz do repositório:

```bash
dotnet restore

dotnet run \
  --project src/ShoppingCart.Api/ShoppingCart.Api.csproj \
  --urls http://localhost:5000
```

A API estará disponível em:

```text
http://localhost:5000
```

## Swagger e exemplos HTTP

Com a API em execução, a documentação interativa pode ser acessada pelo Swagger:

```text
http://localhost:5000/swagger
```

Caso a API seja executada pelo perfil padrão do `launchSettings.json`, utilize a porta exibida no terminal, por exemplo:

```text
http://localhost:5048/swagger
```

O arquivo:

```text
src/ShoppingCart.Api/ShoppingCart.Api.http
```

contém exemplos do fluxo completo e de cenários de erro da API.

Ele pode ser executado pelo Visual Studio, Rider ou VS Code com uma extensão compatível com arquivos `.http`.

A variável `baseUrl` do arquivo deve corresponder à porta usada pela API.

## Executando o frontend

Entre na pasta:

```bash
cd frontend
```

Instale as dependências:

```bash
npm install
```

Crie o arquivo local de ambiente:

```bash
cp .env.example .env.development
```

Configure a URL da API:

```env
VITE_API_URL=http://localhost:5000
```

Execute:

```bash
npm run dev
```

O frontend estará disponível em:

```text
http://localhost:5173
```

## Build do frontend

```bash
cd frontend
npm run build
```

## Testes do backend

Na raiz do repositório:

```bash
dotnet build
dotnet test
```

## Endpoints principais

| Método | Endpoint                                | Descrição               |
| ------ | --------------------------------------- | ----------------------- |
| GET    | `/api/products`                         | Lista os produtos       |
| POST   | `/api/carts`                            | Cria um carrinho        |
| GET    | `/api/carts/{cartId}`                   | Consulta um carrinho    |
| POST   | `/api/carts/{cartId}/items`             | Adiciona um produto     |
| PUT    | `/api/carts/{cartId}/items/{productId}` | Altera a quantidade     |
| DELETE | `/api/carts/{cartId}/items/{productId}` | Remove um produto       |
| PUT    | `/api/carts/{cartId}/coupon`            | Aplica ou troca o cupom |
| DELETE | `/api/carts/{cartId}/coupon`            | Remove o cupom          |
| POST   | `/api/carts/{cartId}/checkout`          | Finaliza o carrinho     |

## Decisões e premissas

- O carrinho é a raiz do agregado e controla itens, cupom, cálculos e status.
- Valores monetários utilizam `decimal` e arredondamento para duas casas decimais.
- Adicionar novamente o mesmo produto soma a quantidade informada.
- Alterar quantidade substitui o valor atual pela quantidade exata.
- A quantidade não pode ser menor ou igual a zero nem ultrapassar o estoque.
- Apenas um cupom pode estar ativo; aplicar outro substitui o anterior.
- O backend é responsável por subtotal, desconto e total.
- Um carrinho finalizado não pode mais ser alterado.
- O checkout não reduz o estoque, pois o requisito solicita validação de disponibilidade, mas não movimentação de estoque.
- O frontend armazena apenas o identificador do carrinho no `localStorage`.
- A API retorna erros padronizados utilizando `ProblemDetails`.
