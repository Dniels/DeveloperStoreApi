# Sales API - Ambev.DeveloperEvaluation

Uma API RESTful completa para gerenciamento de registros de vendas, implementada seguindo os princípios de Domain-Driven Design (DDD) e o padrão External Identities.

##  Funcionalidades

A API permite gerenciar registros de vendas com as seguintes informações:

- Número da venda
- Data da venda
- Cliente
- Valor total da venda
- Filial onde a venda foi realizada
- Produtos vendidos
- Quantidades
- Preços unitários
- Descontos aplicados
- Valor total de cada item
- Status (Cancelada/Não Cancelada)

### Eventos de Domínio

O sistema publica os seguintes eventos de domínio:
- `SaleCreated` - Quando uma venda é criada
- `SaleModified` - Quando uma venda é modificada
- `SaleCancelled` - Quando uma venda é cancelada
- `ItemCancelled` - Quando um item específico é cancelado

##  Tecnologias

### Backend
- **.NET 8.0** - Framework principal
- **C#** - Linguagem de programação
- **EF Core** - ORM para acesso a dados
- **MediatR** - Implementação do padrão Mediator
- **AutoMapper** - Mapeamento de objetos
- **Rebus** - Service bus para messaging

### Bancos de Dados
- **PostgreSQL** - Banco relacional principal

### Testes
- **xUnit** - Framework de testes unitários
- **NSubstitute** - Biblioteca de mocking
- **Bogus (Faker)** - Geração de dados para testes

##  Estrutura do Projeto

```
root/
├── src/
│   ├── Ambev.DeveloperEvaluation.WebApi/       # Camada de apresentação (API)
│   ├── Ambev.DeveloperEvaluation.Application/  # Camada de aplicação
│   ├── Ambev.DeveloperEvaluation.Domain/       # Camada de domínio
│   └── Ambev.DeveloperEvaluation.ORM/          # Camada de infraestrutura
├── tests/
│   ├── Ambev.DeveloperEvaluation.Unit/    # Testes unitários
└── README.md
```

##  Como Executar

### Pré-requisitos

- .NET 8.0 SDK
- PostgreSQL

### Configuração

1. Clone o repositório:
```bash
git clone https://github.com/Dniels/DeveloperStoreApi.git
cd DeveloperStoreApi
```

2. Configure as strings de conexão no `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Database=DeveloperStore;Username=user;Password=password",
    "MongoDB": "mongodb://localhost:27017/DeveloperStore"
  }
}
```

3. Execute as migrações do banco:
```bash
dotnet ef database update
```

4. Execute a aplicação:
```bash
dotnet run --project src/DeveloperStore.Sales.API
```

### Executando os Testes

```bash
# Todos os testes
dotnet test
```

##  API Endpoints

### Vendas

#### GET /api/sales
Recupera uma lista de vendas com suporte a paginação, filtros e ordenação.

**Parâmetros de Query:**
- `_page` (opcional): Número da página (padrão: 1)
- `_size` (opcional): Itens por página (padrão: 10)
- `_order` (opcional): Ordenação (ex: "saleNumber desc, date asc")
- `customerName` (opcional): Filtro por nome do cliente
- `branchName` (opcional): Filtro por nome da filial
- `_minDate` (opcional): Data mínima
- `_maxDate` (opcional): Data máxima
- `_minAmount` (opcional): Valor mínimo
- `_maxAmount` (opcional): Valor máximo

**Resposta:**
```json
{
  "data": [
    {
      "id": "uuid",
      "saleNumber": "string",
      "date": "2024-01-01T00:00:00Z",
      "customer": {
        "id": "uuid",
        "name": "string",
        "email": "string"
      },
      "branch": {
        "id": "uuid",
        "name": "string",
        "address": "string"
      },
      "items": [
        {
          "productId": "uuid",
          "productName": "string",
          "quantity": 5,
          "unitPrice": 100.00,
          "discount": 10.00,
          "totalAmount": 450.00,
          "cancelled": false
        }
      ],
      "totalAmount": 450.00,
      "cancelled": false
    }
  ],
  "totalItems": 100,
  "currentPage": 1,
  "totalPages": 10
}
```

#### POST /api/sales
Cria uma nova venda.

**Body da Requisição:**
```json
{
  "customer": {
    "id": "uuid",
    "name": "string",
    "email": "string"
  },
  "branch": {
    "id": "uuid",
    "name": "string",
    "address": "string"
  },
  "items": [
    {
      "productId": "uuid",
      "productName": "string",
      "quantity": 5,
      "unitPrice": 100.00
    }
  ]
}
```

#### GET /api/sales/{id}
Recupera uma venda específica pelo ID.

#### PUT /api/sales/{id}
Atualiza uma venda existente.

#### DELETE /api/sales/{id}
Cancela uma venda (soft delete).

#### POST /api/sales/{id}/cancel
Cancela uma venda específica.

#### POST /api/sales/{saleId}/items/{itemId}/cancel
Cancela um item específico de uma venda.

##  Regras de Negócio

### Descontos por Quantidade

1. **4 a 9 itens idênticos**: 10% de desconto
2. **10 a 20 itens idênticos**: 20% de desconto
3. **Máximo de 20 itens** por produto por venda
4. **Menos de 4 itens**: Sem desconto permitido

### Validações

- Não é possível vender mais de 20 itens idênticos
- Descontos só são aplicados automaticamente conforme as regras acima
- Vendas canceladas não podem ser modificadas
- Itens cancelados não afetam o cálculo de desconto dos demais itens

##  Paginação, Filtros e Ordenação

### Paginação
```
GET /api/sales?_page=2&_size=20
```

### Ordenação
```
GET /api/sales?_order="date desc, totalAmount asc"
```

### Filtros
```
# Por nome do cliente
GET /api/sales?customerName=João*

# Por faixa de valores
GET /api/sales?_minAmount=100&_maxAmount=1000

# Por período
GET /api/sales?_minDate=2024-01-01&_maxDate=2024-12-31

# Combinado
GET /api/sales?customerName=João*&_minAmount=100&_order="date desc"
```

##  Tratamento de Erros

A API utiliza códigos de status HTTP convencionais:

- **2xx**: Sucesso
- **4xx**: Erro do cliente
- **5xx**: Erro do servidor

### Formato de Resposta de Erro

```json
{
  "type": "ValidationError",
  "error": "Dados de entrada inválidos",
  "detail": "A quantidade deve ser maior que 0 e menor ou igual a 20"
}
```

## 🏗 Arquitetura

O projeto segue os princípios de **Domain-Driven Design (DDD)**:

- **API**: Controllers e configuração da API
- **Application**: Casos de uso, DTOs e mapeamentos
- **Domain**: Entidades, value objects, agregados e regras de negócio
- **Infrastructure**: Repositórios, contexto de dados e serviços externos

### Padrões Utilizados

- **CQRS**: Separação de comandos e queries
- **Mediator**: Desacoplamento entre camadas
- **Repository**: Abstração do acesso a dados
- **External Identities**: Referência a entidades de outros domínios
- **Domain Events**: Comunicação entre agregados

