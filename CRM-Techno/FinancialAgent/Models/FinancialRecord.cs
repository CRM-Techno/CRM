using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FinancialAgent.Models;


public class FinancialRecord
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public decimal Cost { get; set; }
    public string Category { get; set; } // Vendas, Atendimento, Financeiro, Estoque
    public string Client { get; set; }
    public string Product { get; set; }
    public string Salesperson { get; set; }
}