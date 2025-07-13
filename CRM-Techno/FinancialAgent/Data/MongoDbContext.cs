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
    }

    public IMongoCollection<FinancialRecord> FinancialRecords => 
        _database.GetCollection<FinancialRecord>("FinancialRecords");
    
    public IMongoCollection<DashboardData> DashboardData => 
        _database.GetCollection<DashboardData>("DashboardData");
}