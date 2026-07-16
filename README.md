# Carrinho de Compras

Aplicação full-stack para gerenciamento de um carrinho de compras.

- Backend em ASP.NET Core;
- persistência com Entity Framework Core e SQL Server;
- frontend em React, TypeScript e Vite;
- ambiente containerizado com Docker Compose.

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
- `Application`: casos de uso, modelos e interfaces dos repositórios;
- `Infrastructure`: Entity Framework Core, migrations, seed e repositórios;
- `Api`: Controllers, contratos HTTP, Swagger e tratamento de erros;
- `frontend`: interface React que consome a API.

---

## Execução com Docker Compose

Esta é a forma mais simples de executar o projeto completo.

### Pré-requisito

- Docker Desktop.

### Configuração inicial

Na raiz do repositório, crie o arquivo local de ambiente:

```bash
cp .env.example .env
```

Esse comando precisa ser executado apenas na primeira configuração ou após clonar novamente o repositório.

O arquivo `.env` deve possuir:

```env
MSSQL_SA_PASSWORD=ShoppingCart@2026Db
```

### Iniciar a aplicação

```bash
docker compose up --build -d
```

O comando inicia:

- SQL Server;
- API;
- frontend;
- migrations e seed do banco de dados.

Acesse:

```text
Frontend: http://localhost:5173
API: http://localhost:5000
Swagger: http://localhost:5000/swagger
```

Para acompanhar os logs:

```bash
docker compose logs -f
```

Para conferir os serviços:

```bash
docker compose ps
```

### Parar os serviços

Preservando os dados do banco:

```bash
docker compose down
```

Para iniciar novamente:

```bash
docker compose up -d
```

### Recriar o banco do zero

O comando abaixo remove os containers e o volume do SQL Server:

```bash
docker compose down -v
```

Depois, suba novamente:

```bash
docker compose up --build -d
```

Um novo banco será criado e as migrations serão aplicadas automaticamente.

---

## Execução manual

Nesta opção, o SQL Server continua sendo executado pelo Docker, enquanto backend e frontend são iniciados localmente.

### Pré-requisitos

- .NET SDK 8;
- Node.js e npm;
- Docker Desktop.

### 1. Iniciar somente o SQL Server

Na raiz:

```bash
docker compose up -d sqlserver
```

### 2. Configurar a connection string

#### Git Bash ou Linux

```bash
export ConnectionStrings__DefaultConnection='Server=localhost,1433;Database=ShoppingCartDb;User Id=sa;Password=ShoppingCart@2026Db;TrustServerCertificate=True;Encrypt=False;'
```

#### PowerShell

```powershell
$env:ConnectionStrings__DefaultConnection='Server=localhost,1433;Database=ShoppingCartDb;User Id=sa;Password=ShoppingCart@2026Db;TrustServerCertificate=True;Encrypt=False;'
```

### 3. Aplicar as migrations

```bash
dotnet tool restore

dotnet ef database update \
  --project src/ShoppingCart.Infrastructure/ShoppingCart.Infrastructure.csproj \
  --startup-project src/ShoppingCart.Api/ShoppingCart.Api.csproj \
  --context ShoppingCartDbContext
```

A migration cria as tabelas e popula o catálogo com os 10 produtos e os cupons `10OFF` e `15OFF`.

### 4. Executar o backend

Na raiz:

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

### 5. Configurar o frontend

Entre na pasta:

```bash
cd frontend
```

Instale as dependências:

```bash
npm install
```

Crie o arquivo local:

```bash
cp .env.example .env.development
```

Configure:

```env
VITE_API_URL=http://localhost:5000
```

### 6. Executar o frontend

```bash
npm run dev
```

Acesse:

```text
http://localhost:5173
```

---

## Swagger

Com a API em execução, a documentação interativa está disponível em:

```text
http://localhost:5000/swagger
```

Caso a API seja iniciada sem fixar a porta, utilize a URL exibida no terminal, por exemplo:

```text
http://localhost:5048/swagger
```

O Swagger permite visualizar e executar todos os endpoints da API.

---

## Exemplos de chamadas HTTP

O arquivo:

```text
src/ShoppingCart.Api/ShoppingCart.Api.http
```

contém exemplos do fluxo completo e de cenários de erro.

Ele pode ser executado pelo Visual Studio, Rider ou VS Code com uma extensão compatível com arquivos `.http`.

A variável `baseUrl` deve apontar para a porta da API:

```http
@baseUrl = http://localhost:5000
```

---

## Testes e builds

### Backend

Na raiz:

```bash
dotnet build
dotnet test
```

### Frontend

```bash
cd frontend
npm run build
```

---

## Endpoints principais

| Método | Endpoint | Descrição |
|---|---|---|
| GET | `/api/products` | Lista os produtos |
| POST | `/api/carts` | Cria um carrinho |
| GET | `/api/carts/{cartId}` | Consulta um carrinho |
| POST | `/api/carts/{cartId}/items` | Adiciona um produto |
| PUT | `/api/carts/{cartId}/items/{productId}` | Altera a quantidade |
| DELETE | `/api/carts/{cartId}/items/{productId}` | Remove um produto |
| PUT | `/api/carts/{cartId}/coupon` | Aplica ou troca o cupom |
| DELETE | `/api/carts/{cartId}/coupon` | Remove o cupom |
| POST | `/api/carts/{cartId}/checkout` | Finaliza o carrinho |

---

## Decisões e premissas

- O carrinho é a raiz do agregado e controla itens, cupom, cálculos e status.
- Valores monetários utilizam `decimal` e arredondamento para duas casas decimais.
- Adicionar novamente o mesmo produto soma a quantidade informada à existente.
- Alterar quantidade substitui o valor atual pela quantidade exata.
- Quantidades menores ou iguais a zero não são permitidas.
- A quantidade final não pode ultrapassar o estoque disponível.
- Apenas um cupom pode permanecer ativo; aplicar outro substitui o anterior.
- O backend é a fonte da verdade para subtotal, desconto e total.
- Um carrinho finalizado não pode mais ser alterado.
- O checkout não reduz o estoque, pois o requisito solicita validação de disponibilidade, mas não movimentação de estoque.
- Produtos e cupons são carregados no banco por migration e seed a partir dos arquivos JSON fornecidos.
- O frontend armazena apenas o identificador do carrinho no `localStorage`.
- A API retorna erros padronizados utilizando `ProblemDetails`.
- Na execução manual, as migrations são aplicadas pelo comando `dotnet ef database update`.
- Na execução com Docker Compose, as migrations são aplicadas automaticamente pela API.