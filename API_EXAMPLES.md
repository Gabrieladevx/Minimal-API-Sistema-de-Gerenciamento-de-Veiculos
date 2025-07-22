# Exemplos de Teste da API

Este arquivo contém exemplos práticos de como testar todos os endpoints da API usando curl.

## 🔐 Autenticação

### 1. Login de Administrador
```bash
curl -X POST "http://localhost:5004/administradores/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "administrador@teste.com",
    "senha": "123456"
  }'
```

**Resposta esperada:**
```json
{
  "email": "administrador@teste.com",
  "perfil": "Adm",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

*Salve o token retornado para usar nos próximos exemplos*

## 🏠 Endpoint Home

### 2. Verificar Status da API
```bash
curl -X GET "http://localhost:5004/"
```

## 👥 Administradores

### 3. Listar Administradores (Apenas Adm)
```bash
curl -X GET "http://localhost:5004/administradores?pagina=1" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

### 4. Obter Administrador por ID (Apenas Adm)
```bash
curl -X GET "http://localhost:5004/administradores/1" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

### 5. Criar Novo Administrador (Apenas Adm)
```bash
curl -X POST "http://localhost:5004/administradores" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -d '{
    "email": "editor@teste.com",
    "senha": "123456",
    "perfil": "Editor"
  }'
```

## 🚗 Veículos

### 6. Criar Veículo (Adm/Editor)
```bash
curl -X POST "http://localhost:5004/veiculos" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -d '{
    "nome": "Civic",
    "marca": "Honda",
    "ano": 2020
  }'
```

**Resposta esperada:**
```json
{
  "id": 1,
  "nome": "Civic",
  "marca": "Honda",
  "ano": 2020
}
```

### 7. Listar Todos os Veículos (Autenticado)
```bash
curl -X GET "http://localhost:5004/veiculos" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

### 8. Listar Veículos com Paginação
```bash
curl -X GET "http://localhost:5004/veiculos?pagina=1" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

### 9. Buscar Veículos por Nome
```bash
curl -X GET "http://localhost:5004/veiculos?nome=civic" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

### 10. Buscar Veículos por Marca
```bash
curl -X GET "http://localhost:5004/veiculos?marca=honda" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

### 11. Buscar com Múltiplos Filtros
```bash
curl -X GET "http://localhost:5004/veiculos?nome=civic&marca=honda&pagina=1" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

### 12. Obter Veículo por ID (Adm/Editor)
```bash
curl -X GET "http://localhost:5004/veiculos/1" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

### 13. Atualizar Veículo (Apenas Adm)
```bash
curl -X PUT "http://localhost:5004/veiculos/1" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -d '{
    "nome": "Civic Si",
    "marca": "Honda",
    "ano": 2021
  }'
```

### 14. Excluir Veículo (Apenas Adm)
```bash
curl -X DELETE "http://localhost:5004/veiculos/1" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

## ❌ Testando Cenários de Erro

### 15. Acesso Não Autorizado
```bash
curl -X GET "http://localhost:5004/veiculos"
```
*Retorna: 401 Unauthorized*

### 16. Token Inválido
```bash
curl -X GET "http://localhost:5004/veiculos" \
  -H "Authorization: Bearer token_invalido"
```
*Retorna: 401 Unauthorized*

### 17. Dados Inválidos para Veículo
```bash
curl -X POST "http://localhost:5004/veiculos" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -d '{
    "nome": "",
    "marca": "",
    "ano": 1940
  }'
```
*Retorna: 400 Bad Request com mensagens de erro*

### 18. Veículo Não Encontrado
```bash
curl -X GET "http://localhost:5004/veiculos/999" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```
*Retorna: 404 Not Found*

### 19. Acesso Editor Tentando Atualizar (Proibido)
Primeiro, faça login como Editor:
```bash
curl -X POST "http://localhost:5004/administradores/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "editor@teste.com",
    "senha": "123456"
  }'
```

Então tente atualizar (deve falhar):
```bash
curl -X PUT "http://localhost:5004/veiculos/1" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TOKEN_DO_EDITOR" \
  -d '{
    "nome": "Civic Si",
    "marca": "Honda",
    "ano": 2021
  }'
```
*Retorna: 403 Forbidden*

## 🔧 Script de Teste Completo

Aqui está um script bash que testa todos os cenários:

```bash
#!/bin/bash

# Variáveis
BASE_URL="http://localhost:5004"
ADMIN_EMAIL="administrador@teste.com"
ADMIN_PASSWORD="123456"

echo "🔐 Fazendo login como administrador..."
LOGIN_RESPONSE=$(curl -s -X POST "$BASE_URL/administradores/login" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"$ADMIN_EMAIL\",\"senha\":\"$ADMIN_PASSWORD\"}")

TOKEN=$(echo $LOGIN_RESPONSE | grep -o '"token":"[^"]*' | grep -o '[^"]*$')

if [ -z "$TOKEN" ]; then
  echo "❌ Falha na autenticação"
  exit 1
fi

echo "✅ Login realizado com sucesso"
echo "Token: ${TOKEN:0:50}..."

echo ""
echo "🚗 Criando um veículo de teste..."
CREATE_RESPONSE=$(curl -s -X POST "$BASE_URL/veiculos" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"nome":"Civic Teste","marca":"Honda","ano":2020}')

echo "Resposta: $CREATE_RESPONSE"

echo ""
echo "📋 Listando veículos..."
LIST_RESPONSE=$(curl -s -X GET "$BASE_URL/veiculos" \
  -H "Authorization: Bearer $TOKEN")

echo "Veículos encontrados: $LIST_RESPONSE"

echo ""
echo "✅ Testes concluídos!"
```

Salve este script como `test_api.sh`, torne-o executável (`chmod +x test_api.sh`) e execute (`./test_api.sh`).

## 📝 Notas Importantes

1. **Substitua `SEU_TOKEN_AQUI`** pelo token JWT real obtido no endpoint de login
2. **URL Base**: Certifique-se de que a API está rodando em `http://localhost:5004`
3. **Usuário Padrão**: O administrador padrão é criado automaticamente pela migração
4. **Headers**: Sempre inclua o `Content-Type: application/json` para requisições POST/PUT
5. **Autorização**: O header `Authorization: Bearer TOKEN` é obrigatório para endpoints protegidos

## 🔍 Dicas para Debugging

- Use `-v` no curl para ver headers de resposta: `curl -v ...`
- Para formatar JSON responses: `curl ... | jq .`
- Para ver apenas o status code: `curl -w "%{http_code}" -o /dev/null -s ...`
