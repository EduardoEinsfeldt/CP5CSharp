# Loggu API

Este projeto é uma API RESTful desenvolvida em .NET 8 para o gerenciamento de motos, usuários e movimentos de pátio, utilizando MongoDB como banco de dados.

## Integrantes:

- Vicenzo Massao de Oliveira --> 554833
- Eduardo Augusto Pelegrino Einsfeldt --> 556460

_(Nós não fizemos a CP4, e meu grupo do challenge decidiu não fazer essa CP, então me juntei com o Eduardo, espero que não tenha problema, obrigado!)_ ~~ Vicenzo

## Tecnologias Utilizadas

- .NET 8 (SDK)

- ASP.NET Core 8 (Web API)

- MongoDB (Banco de Dados NoSQL)

- Swagger (OpenAPI) (Documentação e Teste de API)

- ASP.NET Core Health Checks (Monitoramento de Saúde da API)

- ASP.NET Core API Versioning (Versionamento de API)

## Estrutura do Projeto

O projeto segue uma arquitetura limpa (Clean Architecture), dividida nas seguintes camadas:

- Domain: Contém as entidades de negócio (Moto, Usuario, etc.) e as interfaces dos repositórios (IMotoRepository, etc.).

- Application: Contém os DTOs (Data Transfer Objects) usados para a comunicação da API.

- Infrastructure: Contém a implementação de acesso a dados (Contexto do MongoDB, Repositórios e Mapeamentos/Validadores de schema).

- Controllers: Contém os endpoints da API (MotosController, UsuariosController).

## Configuração e Execução

#### 1. Pré-requisitos

- .NET 8 SDK

- Um servidor MongoDB (local ou Atlas).

#### 2. Configuração do Banco de Dados

- A string de conexão do MongoDB deve ser configurada no arquivo appsettings.json.

```json
{
  "Mongo": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "loggu_cp5"
  }
}
```

#### 3. Executando a Aplicação

1. Certifique-se de que seu servidor MongoDB está em execução.

2. Restaure as dependências do .NET: (Eu pessoalmente rodo o projeto no VSCode pelo bash)

```Bash
dotnet restore
```

3. Execute o projeto:

```Bash
dotnet build
dotnet run
```

A aplicação estará disponível em http://localhost:5243 (ou https://localhost:7141, dependendo do seu perfil de inicialização).

## Endpoints da API

A documentação completa da API (Swagger) está disponível para testes e visualização na rota base da aplicação:

- **Swagger UI**: http://localhost:5243/swagger

### Endpoints Principais

- **Health Check:** `GET /health`

  - Verifica a saúde da aplicação e a conexão com o MongoDB.

- **Motos:** `GET, POST, PUT, DELETE /api/motos`

  - Gerenciamento completo de motocicletas.

- **Usuários:** `GET, POST, PUT, DELETE /api/usuarios`

  - Gerenciamento completo de usuários.

- **Movimentos de Pátio:** `POST /api/movimentos`
  e
  `GET /api/movimentos/moto/{motoId}`
  - Registra entradas e saídas de motos do pátio e lista movimentos por moto.
