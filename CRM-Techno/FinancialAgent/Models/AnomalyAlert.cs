using MongoDB.Bson.Serialization.Attributes;

namespace FinancialAgent.Models;


public class AnomalyAlert
{
    public string Type { get; set; } // Ex.: "Queda de Receita", "Aumento de Custos"
    public string Description { get; set; }
    public decimal Threshold { get; set; }
    public decimal ActualValue { get; set; }
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime DetectedAt { get; set; }
}