# Minimal API - Sistema de Gerenciamento de Veículos

Este projeto é uma API desenvolvida utilizando a técnica de **Minimal APIs** do ASP.NET Core para gerenciamento de veículos, com sistema de autenticação JWT e controle de acesso baseado em perfis de usuário.

## 🚀 Características Principais

- **Minimal APIs**: Implementação moderna e eficiente usando .NET 7
- **Autenticação JWT**: Sistema completo de autenticação com tokens JWT
- **Controle de Acesso**: Diferentes níveis de permissão (Administrador, Editor)
- **Swagger UI**: Documentação interativa da API
- **Entity Framework Core**: ORM para acesso a dados com MySQL
- **Testes Completos**: Testes unitários e de integração
- **Validação de Dados**: Validação robusta dos DTOs
- **Paginação**: Sistema de paginação para listagens

## 🏗️ Arquitetura do Projeto

```
Api/
├── Dominio/
│   ├── DTOs/                    # Data Transfer Objects
│   ├── Entidades/               # Entidades do domínio
│   ├── Enuns/                   # Enumerações
│   ├── Interfaces/              # Interfaces de serviços
│   ├── ModelViews/              # ViewModels para respostas
│   └── Servicos/                # Implementação dos serviços
├── Infraestrutura/
│   └── Db/                      # Contexto do banco de dados
├── Migrations/                  # Migrações do Entity Framework
└── Properties/                  # Configurações de launch
Test/
├── Domain/
│   └── Servicos/                # Testes unitários dos serviços
├── Requests/                    # Testes de integração dos endpoints
├── Helpers/                     # Utilitários para testes
└── Mocks/                       # Objetos mock para testes
```

## 🔐 Sistema de Autenticação

### Perfis de Usuário
- **Administrador (Adm)**: Acesso completo a todos os endpoints
- **Editor**: Pode criar, listar e visualizar veículos

### Endpoints de Autenticação
- `POST /administradores/login` - Autenticação de usuário
- `GET /administradores` - Listar administradores (Adm apenas)
- `GET /administradores/{id}` - Obter administrador por ID (Adm apenas)
- `POST /administradores` - Criar novo administrador (Adm apenas)

## 🚗 Endpoints de Veículos

### Operações CRUD Completas
- `POST /veiculos` - Criar veículo (Adm/Editor)
- `GET /veiculos` - Listar veículos com filtros e paginação (Autenticado)
- `GET /veiculos/{id}` - Obter veículo por ID (Adm/Editor)
- `PUT /veiculos/{id}` - Atualizar veículo (Adm apenas)
- `DELETE /veiculos/{id}` - Excluir veículo (Adm apenas)

### Filtros Disponíveis
- **Nome**: Busca parcial no nome do veículo
- **Marca**: Busca parcial na marca do veículo
- **Paginação**: Controle de página (padrão: 10 itens por página)

## 📊 Modelos de Dados

### Veículo
```json
{
  "id": 1,
  "nome": "Civic",
  "marca": "Honda",
  "ano": 2020
}
```

### Administrador
```json
{
  "id": 1,
  "email": "admin@exemplo.com",
  "perfil": "Adm"
}
```

## 🔧 Configuração e Execução

### Pré-requisitos
- .NET 7.0 SDK
- MySQL Server
- Visual Studio Code ou Visual Studio

### Configuração do Banco de Dados
1. Configure a string de conexão no `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "MySql": "server=localhost;database=minimal_api;user=root;password=suasenha"
  }
}
```

2. Execute as migrações:
```bash
dotnet ef database update
```

### Executando a Aplicação
```bash
cd Api
dotnet run
```

A aplicação estará disponível em:
- API: `http://localhost:5004`
- Swagger UI: `http://localhost:5004/swagger`

## 🧪 Executando Testes

### Todos os Testes
```bash
dotnet test
```

### Testes Específicos
```bash
# Apenas testes unitários de veículos
dotnet test --filter "FullyQualifiedName~VeiculoServicoTest"

# Apenas testes de integração
dotnet test --filter "FullyQualifiedName~RequestTest"
```

## 📝 Exemplo de Uso

### 1. Autenticação
```bash
POST /administradores/login
Content-Type: application/json

{
  "email": "administrador@teste.com",
  "senha": "123456"
}
```

Resposta:
```json
{
  "email": "administrador@teste.com",
  "perfil": "Adm",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### 2. Criar Veículo
```bash
POST /veiculos
Authorization: Bearer seu_token_jwt
Content-Type: application/json

{
  "nome": "Civic",
  "marca": "Honda",
  "ano": 2020
}
```

### 3. Listar Veículos com Filtros
```bash
GET /veiculos?nome=civic&marca=honda&pagina=1
Authorization: Bearer seu_token_jwt
```

## ✅ Validações Implementadas

### Veículos
- Nome: Obrigatório
- Marca: Obrigatória
- Ano: Deve ser superior a 1950

### Administradores
- Email: Obrigatório e formato válido
- Senha: Obrigatória
- Perfil: Deve ser "Adm" ou "Editor"

## 🔒 Segurança

- **JWT Tokens**: Expiração configurada para 1 dia
- **CORS**: Configurado para desenvolvimento (ajustar para produção)
- **Autorização por Roles**: Controle granular de acesso aos endpoints
- **Validação de Entrada**: Todas as entradas são validadas

## 🧪 Cobertura de Testes

### Testes Unitários
- ✅ Serviços de Veículo (CRUD completo)
- ✅ Serviços de Administrador
- ✅ Validações de DTOs

### Testes de Integração
- ✅ Endpoints de Veículos
- ✅ Autenticação JWT
- ✅ Autorização por perfis
- ✅ Validação de entrada
- ✅ Cenários de erro

## 📋 Próximas Melhorias

- [ ] Implementar logging estruturado
- [ ] Adicionar rate limiting
- [ ] Implementar cache Redis
- [ ] Adicionar métricas e health checks
- [ ] Dockerização da aplicação
- [ ] Pipeline CI/CD

## 🤝 Contribuição

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

---

**Desenvolvido com ❤️ usando ASP.NET Core Minimal APIs**
