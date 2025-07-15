using FinancialAgent.Models;
using MongoDB.Driver;

namespace FinancialAgent.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetConnectionString("MongoDbConnection"));
        _database = client.GetDatabase("FinancialAgentDb");

        // Verificar e criar coleções, se necessário
        CreateCollectionsIfNotExist();
    }

    public IMongoCollection<FinancialRecord> FinancialRecords => 
        _database.GetCollection<FinancialRecord>("FinancialRecords");
    
    public IMongoCollection<DashboardData> DashboardData => 
        _database.GetCollection<DashboardData>("DashboardData");

    private void CreateCollectionsIfNotExist()
    {
        var collectionNames = _database.ListCollectionNames().ToList();
        
        if (!collectionNames.Contains("FinancialRecords"))
        {
            _database.CreateCollection("FinancialRecords");
        }
        
        if (!collectionNames.Contains("DashboardData"))
        {
            _database.CreateCollection("DashboardData");
        }
    }
}