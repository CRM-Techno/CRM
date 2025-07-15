using FinancialAgent.Models;
using MongoDB.Driver;

namespace FinancialAgent.Data;

public class DataGenerator
{
    private readonly MongoDbContext _context;
    private readonly Random _random = new Random();
    private readonly string[] _modules = { "Vendas", "Atendimento", "Financeiro", "Estoque" };
    private readonly string[] _clients = { "Cliente A", "Cliente B", "Cliente C", "Cliente D", "Cliente E" };
    private readonly string[] _products = { "Produto 1", "Produto 2", "Produto 3", "Produto 4", "Produto 5" };
    private readonly string[] _salespersons = { "Vendedor 1", "Vendedor 2", "Vendedor 3" };

    public DataGenerator(MongoDbContext context)
    {
        _context = context;
    }

    public async Task GenerateTestDataAsync(int numberOfRecords)
    {
        var records = new List<FinancialRecord>();
        var startDate = new DateTime(2025, 1, 1);
        var endDate = new DateTime(2025, 12, 31);

        for (int i = 0; i < numberOfRecords; i++)
        {
            var daysOffset = _random.Next(0, (endDate - startDate).Days + 1);
            var recordDate = startDate.AddDays(daysOffset);

            records.Add(new FinancialRecord
            {
                Date = recordDate,
                Revenue = (decimal)(_random.NextDouble() * 10000 + 500), // Receita entre 500 e 10500
                Cost = (decimal)(_random.NextDouble() * 8000 + 300),    // Custo entre 300 e 8300
                Category = _modules[_random.Next(_modules.Length)],
                Client = _clients[_random.Next(_clients.Length)],
                Product = _products[_random.Next(_products.Length)],
                Salesperson = _salespersons[_random.Next(_salespersons.Length)]
            });
        }

        await _context.FinancialRecords.InsertManyAsync(records);
        Console.WriteLine($"{numberOfRecords} registros inseridos na coleção FinancialRecords.");
    }

    public async Task ClearTestDataAsync()
    {
        await _context.FinancialRecords.DeleteManyAsync(Builders<FinancialRecord>.Filter.Empty);
        await _context.DashboardData.DeleteManyAsync(Builders<DashboardData>.Filter.Empty);
        Console.WriteLine("Coleções FinancialRecords e DashboardData limpas.");
    }
}