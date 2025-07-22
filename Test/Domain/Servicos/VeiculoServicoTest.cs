using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

namespace Test.Domain.Servicos;

[TestClass]
public class VeiculoServicoTest
{
    private DbContexto CriarContextoDeTeste()
    {
        var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        var options = new DbContextOptionsBuilder<DbContexto>()
            .UseInMemoryDatabase(databaseName: $"{assemblyName}_{Guid.NewGuid()}")
            .Options;

        return new DbContexto(options);
    }

    [TestMethod]
    public void TestarIncluirVeiculo()
    {
        // Arrange
        using var context = CriarContextoDeTeste();
        var veiculoServico = new VeiculoServico(context);

        var veiculo = new Veiculo
        {
            Nome = "Civic",
            Marca = "Honda", 
            Ano = 2020
        };

        // Act
        veiculoServico.Incluir(veiculo);

        // Assert
        Assert.AreEqual(1, context.Veiculos.Count());
        var veiculoSalvo = context.Veiculos.First();
        Assert.AreEqual("Civic", veiculoSalvo.Nome);
        Assert.AreEqual("Honda", veiculoSalvo.Marca);
        Assert.AreEqual(2020, veiculoSalvo.Ano);
    }

    [TestMethod]
    public void TestarBuscarVeiculoPorId()
    {
        // Arrange
        using var context = CriarContextoDeTeste();
        var veiculoServico = new VeiculoServico(context);

        var veiculo = new Veiculo
        {
            Nome = "Corolla",
            Marca = "Toyota",
            Ano = 2021
        };

        context.Veiculos.Add(veiculo);
        context.SaveChanges();

        // Act
        var veiculoEncontrado = veiculoServico.BuscaPorId(veiculo.Id);

        // Assert
        Assert.IsNotNull(veiculoEncontrado);
        Assert.AreEqual("Corolla", veiculoEncontrado.Nome);
        Assert.AreEqual("Toyota", veiculoEncontrado.Marca);
        Assert.AreEqual(2021, veiculoEncontrado.Ano);
    }

    [TestMethod]
    public void TestarBuscarVeiculoPorIdInexistente()
    {
        // Arrange
        using var context = CriarContextoDeTeste();
        var veiculoServico = new VeiculoServico(context);

        // Act
        var veiculoEncontrado = veiculoServico.BuscaPorId(999);

        // Assert
        Assert.IsNull(veiculoEncontrado);
    }

    [TestMethod]
    public void TestarListarTodosOsVeiculos()
    {
        // Arrange
        using var context = CriarContextoDeTeste();
        var veiculoServico = new VeiculoServico(context);

        var veiculos = new List<Veiculo>
        {
            new Veiculo { Nome = "Civic", Marca = "Honda", Ano = 2020 },
            new Veiculo { Nome = "Corolla", Marca = "Toyota", Ano = 2021 },
            new Veiculo { Nome = "Gol", Marca = "Volkswagen", Ano = 2019 }
        };

        foreach (var v in veiculos)
        {
            context.Veiculos.Add(v);
        }
        context.SaveChanges();

        // Act
        var resultado = veiculoServico.Todos();

        // Assert
        Assert.AreEqual(3, resultado.Count);
        Assert.IsTrue(resultado.Any(v => v.Nome == "Civic"));
        Assert.IsTrue(resultado.Any(v => v.Nome == "Corolla"));
        Assert.IsTrue(resultado.Any(v => v.Nome == "Gol"));
    }

    [TestMethod]
    public void TestarListarVeiculosComPaginacao()
    {
        // Arrange
        using var context = CriarContextoDeTeste();
        var veiculoServico = new VeiculoServico(context);

        // Criando 15 veículos para testar paginação
        for (int i = 1; i <= 15; i++)
        {
            context.Veiculos.Add(new Veiculo 
            { 
                Nome = $"Veiculo {i}", 
                Marca = "Marca", 
                Ano = 2020 + i 
            });
        }
        context.SaveChanges();

        // Act
        var primeiraPagina = veiculoServico.Todos(1);
        var segundaPagina = veiculoServico.Todos(2);

        // Assert
        Assert.AreEqual(10, primeiraPagina.Count); // Primeira página deve ter 10 itens
        Assert.AreEqual(5, segundaPagina.Count);   // Segunda página deve ter 5 itens
    }

    [TestMethod]
    public void TestarAtualizarVeiculo()
    {
        // Arrange
        using var context = CriarContextoDeTeste();
        var veiculoServico = new VeiculoServico(context);

        var veiculo = new Veiculo
        {
            Nome = "Fusca",
            Marca = "Volkswagen",
            Ano = 1980
        };

        context.Veiculos.Add(veiculo);
        context.SaveChanges();

        // Act
        veiculo.Nome = "Fusca Personalizado";
        veiculo.Ano = 1985;
        veiculoServico.Atualizar(veiculo);

        // Assert
        var veiculoAtualizado = context.Veiculos.Find(veiculo.Id);
        Assert.IsNotNull(veiculoAtualizado);
        Assert.AreEqual("Fusca Personalizado", veiculoAtualizado.Nome);
        Assert.AreEqual(1985, veiculoAtualizado.Ano);
        Assert.AreEqual("Volkswagen", veiculoAtualizado.Marca); // Não foi alterada
    }

    [TestMethod]
    public void TestarApagarVeiculo()
    {
        // Arrange
        using var context = CriarContextoDeTeste();
        var veiculoServico = new VeiculoServico(context);

        var veiculo = new Veiculo
        {
            Nome = "Palio",
            Marca = "Fiat",
            Ano = 2018
        };

        context.Veiculos.Add(veiculo);
        context.SaveChanges();

        Assert.AreEqual(1, context.Veiculos.Count());

        // Act
        veiculoServico.Apagar(veiculo);

        // Assert
        Assert.AreEqual(0, context.Veiculos.Count());
        var veiculoRemovido = context.Veiculos.Find(veiculo.Id);
        Assert.IsNull(veiculoRemovido);
    }

    [TestMethod]
    public void TestarFiltrarVeiculosPorNome()
    {
        // Arrange
        using var context = CriarContextoDeTeste();
        var veiculoServico = new VeiculoServico(context);

        var veiculos = new List<Veiculo>
        {
            new Veiculo { Nome = "Honda Civic", Marca = "Honda", Ano = 2020 },
            new Veiculo { Nome = "Honda Accord", Marca = "Honda", Ano = 2021 },
            new Veiculo { Nome = "Toyota Corolla", Marca = "Toyota", Ano = 2019 }
        };

        foreach (var v in veiculos)
        {
            context.Veiculos.Add(v);
        }
        context.SaveChanges();

        // Act
        var resultado = veiculoServico.Todos(1, "honda");

        // Assert
        Assert.AreEqual(2, resultado.Count);
        Assert.IsTrue(resultado.All(v => v.Nome.ToLower().Contains("honda")));
    }
}
