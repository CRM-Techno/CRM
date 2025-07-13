using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FinancialAgent.Models;


public class DashboardData
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Module { get; set; } // Vendas, Atendimento, Financeiro, Estoque
    public string Period { get; set; } // Diário, Semanal, Mensal
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Dictionary<string, decimal> Kpis { get; set; }
    public List<HistoricalDataPoint> HistoricalData { get; set; }
    public List<AnomalyAlert> Anomalies { get; set; }
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}