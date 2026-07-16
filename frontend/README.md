# Carrinho de Compras - Frontend

Frontend React com TypeScript e Vite para consumir a API REST do teste técnico Shopping Cart.

## Requisitos

- Node.js compatível com o projeto.
- SQL Server em execução para a API.
- Backend `ShoppingCart.Api` configurado e com banco/migrations aplicados.

## Configuração

Instale as dependências do frontend:

```bash
npm install
```

Crie o arquivo `.env` a partir do exemplo:

```bash
cp .env.example .env
```

Configure a URL da API:

```env
VITE_API_URL=http://localhost:5000
```

Se executar a API pelo perfil padrão `http` do `launchSettings.json`, ajuste para `http://localhost:5048` ou suba a API explicitamente em `http://localhost:5000`.

## Executando a API

A partir da raiz do repositório:

```bash
dotnet run --project src/ShoppingCart.Api/ShoppingCart.Api.csproj --urls http://localhost:5000
```

A API depende do SQL Server configurado no projeto backend.

## Executando o frontend

A partir da pasta `frontend`:

```bash
npm run dev
```

O Vite sobe por padrão em:

```text
http://localhost:5173
```

## Build

Para validar TypeScript e gerar o build de produção:

```bash
npm run build
```
