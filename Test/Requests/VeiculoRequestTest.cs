using System.Net;
using System.Text;
using System.Text.Json;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.DTOs;
using Test.Helpers;

namespace Test.Requests;

[TestClass]
public class VeiculoRequestTest
{
    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        Setup.ClassInit(testContext);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }

    [TestMethod]
    public async Task TestarPostVeiculo()
    {
        // Arrange
        var token = await ObterTokenJWT();
        var veiculoDTO = new VeiculoDTO
        {
            Nome = "Civic",
            Marca = "Honda",
            Ano = 2020
        };

        var content = new StringContent(JsonSerializer.Serialize(veiculoDTO), Encoding.UTF8, "application/json");
        
        Setup.client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await Setup.client.PostAsync("/veiculos", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var veiculo = JsonSerializer.Deserialize<Veiculo>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(veiculo);
        Assert.AreEqual("Civic", veiculo.Nome);
        Assert.AreEqual("Honda", veiculo.Marca);
        Assert.AreEqual(2020, veiculo.Ano);
        Assert.IsTrue(veiculo.Id > 0);
    }

    [TestMethod]
    public async Task TestarPostVeiculoSemAutenticacao()
    {
        // Arrange
        var veiculoDTO = new VeiculoDTO
        {
            Nome = "Civic",
            Marca = "Honda",
            Ano = 2020
        };

        var content = new StringContent(JsonSerializer.Serialize(veiculoDTO), Encoding.UTF8, "application/json");

        // Act
        var response = await Setup.client.PostAsync("/veiculos", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task TestarGetVeiculos()
    {
        // Arrange
        var token = await ObterTokenJWT();
        Setup.client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await Setup.client.GetAsync("/veiculos");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var veiculos = JsonSerializer.Deserialize<List<Veiculo>>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(veiculos);
    }

    [TestMethod]
    public async Task TestarGetVeiculoPorId()
    {
        // Arrange
        var token = await ObterTokenJWT();
        Setup.client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Primeiro, criar um veículo
        var veiculoDTO = new VeiculoDTO
        {
            Nome = "Corolla",
            Marca = "Toyota",
            Ano = 2021
        };

        var content = new StringContent(JsonSerializer.Serialize(veiculoDTO), Encoding.UTF8, "application/json");
        var postResponse = await Setup.client.PostAsync("/veiculos", content);
        var postResult = await postResponse.Content.ReadAsStringAsync();
        var veiculoCriado = JsonSerializer.Deserialize<Veiculo>(postResult, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Act
        var response = await Setup.client.GetAsync($"/veiculos/{veiculoCriado!.Id}");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var veiculo = JsonSerializer.Deserialize<Veiculo>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(veiculo);
        Assert.AreEqual(veiculoCriado.Id, veiculo.Id);
        Assert.AreEqual("Corolla", veiculo.Nome);
    }

    [TestMethod]
    public async Task TestarPutVeiculo()
    {
        // Arrange
        var token = await ObterTokenJWT();
        Setup.client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Primeiro, criar um veículo
        var veiculoDTO = new VeiculoDTO
        {
            Nome = "Fusca",
            Marca = "Volkswagen",
            Ano = 1980
        };

        var content = new StringContent(JsonSerializer.Serialize(veiculoDTO), Encoding.UTF8, "application/json");
        var postResponse = await Setup.client.PostAsync("/veiculos", content);
        var postResult = await postResponse.Content.ReadAsStringAsync();
        var veiculoCriado = JsonSerializer.Deserialize<Veiculo>(postResult, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Atualizar o veículo
        var veiculoAtualizado = new VeiculoDTO
        {
            Nome = "Fusca Personalizado",
            Marca = "Volkswagen",
            Ano = 1985
        };

        var updateContent = new StringContent(JsonSerializer.Serialize(veiculoAtualizado), Encoding.UTF8, "application/json");

        // Act
        var response = await Setup.client.PutAsync($"/veiculos/{veiculoCriado!.Id}", updateContent);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var veiculo = JsonSerializer.Deserialize<Veiculo>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(veiculo);
        Assert.AreEqual("Fusca Personalizado", veiculo.Nome);
        Assert.AreEqual(1985, veiculo.Ano);
    }

    [TestMethod]
    public async Task TestarDeleteVeiculo()
    {
        // Arrange
        var token = await ObterTokenJWT();
        Setup.client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Primeiro, criar um veículo
        var veiculoDTO = new VeiculoDTO
        {
            Nome = "Gol",
            Marca = "Volkswagen",
            Ano = 2010
        };

        var content = new StringContent(JsonSerializer.Serialize(veiculoDTO), Encoding.UTF8, "application/json");
        var postResponse = await Setup.client.PostAsync("/veiculos", content);
        var postResult = await postResponse.Content.ReadAsStringAsync();
        var veiculoCriado = JsonSerializer.Deserialize<Veiculo>(postResult, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Act
        var response = await Setup.client.DeleteAsync($"/veiculos/{veiculoCriado!.Id}");

        // Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);

        // Verificar se foi realmente deletado
        var getResponse = await Setup.client.GetAsync($"/veiculos/{veiculoCriado.Id}");
        Assert.AreEqual(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [TestMethod]
    public async Task TestarValidacaoVeiculoComDadosInvalidos()
    {
        // Arrange
        var token = await ObterTokenJWT();
        Setup.client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var veiculoDTO = new VeiculoDTO
        {
            Nome = "", // Nome vazio - inválido
            Marca = "",  // Marca vazia - inválida
            Ano = 1940   // Ano muito antigo - inválido
        };

        var content = new StringContent(JsonSerializer.Serialize(veiculoDTO), Encoding.UTF8, "application/json");

        // Act
        var response = await Setup.client.PostAsync("/veiculos", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var erros = JsonSerializer.Deserialize<ErrosDeValidacao>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(erros);
        Assert.IsTrue(erros.Mensagens.Count >= 3); // Pelo menos 3 erros de validação
    }

    private async Task<string> ObterTokenJWT()
    {
        var loginDTO = new LoginDTO
        {
            Email = "adm@teste.com",
            Senha = "123456"
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "application/json");
        var response = await Setup.client.PostAsync("/administradores/login", content);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var admLogado = JsonSerializer.Deserialize<AdministradorLogado>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return admLogado?.Token ?? "";
    }
}
