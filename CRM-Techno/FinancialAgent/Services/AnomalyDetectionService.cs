using FinancialAgent.Models;

namespace FinancialAgent.Services;

public class AnomalyDetectionService
{
    public List<AnomalyAlert> DetectAnomalies(List<FinancialRecord> records)
    {
        var anomalies = new List<AnomalyAlert>();
        var avgRevenue = records.Any() ? records.Average(r => r.Revenue) : 0;
        var avgCost = records.Any() ? records.Average(r => r.Cost) : 0;

        foreach (var record in records)
        {
            if (avgRevenue > 0 && record.Revenue < avgRevenue * 0.7m)
            {
                anomalies.Add(new AnomalyAlert
                {
                    Type = "Queda de Receita",
                    Description = $"Receita de {record.Revenue:C} abaixo de 70% da média ({avgRevenue:C}) em {record.Date:yyyy-MM-dd}",
                    Threshold = avgRevenue * 0.7m,
                    ActualValue = record.Revenue,
                    DetectedAt = DateTime.UtcNow
                });
            }
            if (avgCost > 0 && record.Cost > avgCost * 1.3m)
            {
                anomalies.Add(new AnomalyAlert
                {
                    Type = "Aumento de Custos",
                    Description = $"Custo de {record.Cost:C} acima de 130% da média ({avgCost:C}) em {record.Date:yyyy-MM-dd}",
                    Threshold = avgCost * 1.3m,
                    ActualValue = record.Cost,
                    DetectedAt = DateTime.UtcNow
                });
            }
        }

        return anomalies;
    }
}