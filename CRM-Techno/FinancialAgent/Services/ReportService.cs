using FinancialAgent.Data;
using FinancialAgent.Models;
using MongoDB.Driver;
using Quartz;
using Quartz.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialAgent.Services;

public class ReportService
{
    private readonly MongoDbContext _context;
    private readonly AnomalyDetectionService _anomalyService;
    private readonly EmailService _emailService;

    public ReportService(MongoDbContext context, AnomalyDetectionService anomalyService, EmailService emailService)
    {
        _context = context;
        _anomalyService = anomalyService;
        _emailService = emailService;
    }

    public async Task<DashboardData> GenerateDashboardDataAsync(string module, string period)
    {
        var (startDate, endDate) = GetDateRange(period);
        var filter = Builders<FinancialRecord>.Filter
            .Where(r => r.Category == module && r.Date >= startDate && r.Date <= endDate);
        var records = await _context.FinancialRecords
            .Find(filter)
            .ToListAsync();

        var dashboard = new DashboardData
        {
            Module = module,
            Period = period,
            StartDate = startDate,
            EndDate = endDate,
            Kpis = CalculateKpis(records),
            HistoricalData = GenerateHistoricalData(records, period),
            Anomalies = _anomalyService.DetectAnomalies(records)
        };

        // Salvar os dados do dashboard no MongoDB
        await _context.DashboardData.InsertOneAsync(dashboard);

        if (dashboard.Anomalies.Any())
        {
            await _emailService.SendAnomalyAlertsAsync(dashboard, "admin@example.com");
        }

        return dashboard;
    }

    private (DateTime startDate, DateTime endDate) GetDateRange(string period)
    {
        var now = DateTime.UtcNow;
        return period switch
        {
            "Diário" => (now.Date, now.Date.AddDays(1).AddTicks(-1)),
            "Semanal" => (now.Date.AddDays(-(int)now.DayOfWeek), now.Date.AddDays(7).AddTicks(-1)),
            "Mensal" => (new DateTime(now.Year, now.Month, 1), new DateTime(now.Year, now.Month, 1).AddMonths(1).AddTicks(-1)),
            "Anual" => (new DateTime(now.Year, 1, 1), new DateTime(now.Year, 12, 31).AddDays(1).AddTicks(-1)),
            _ => throw new ArgumentException("Período inválido")
        };
    }

    private Dictionary<string, decimal> CalculateKpis(List<FinancialRecord> records)
    {
        return new Dictionary<string, decimal>
        {
            { "Receita Total", records.Sum(r => r.Revenue) },
            { "Custos Totais", records.Sum(r => r.Cost) },
            { "Margem", records.Sum(r => r.Revenue - r.Cost) },
            { "Receita Média por Cliente", records.GroupBy(r => r.Client).Average(g => g.Sum(r => r.Revenue)) },
            { "Custo Médio por Produto", records.GroupBy(r => r.Product).Average(g => g.Sum(r => r.Cost)) }
        };
    }

    private List<HistoricalDataPoint> GenerateHistoricalData(List<FinancialRecord> records, string period)
    {
        var groupBy = records.GroupBy(r => r.Date.Date);
        if (period == "Semanal")
            groupBy = records.GroupBy(r => r.Date.Date.AddDays(-(int)r.Date.DayOfWeek));
        else if (period == "Mensal")
            groupBy = records.GroupBy(r => new DateTime(r.Date.Year, r.Date.Month, 1));
        else if (period == "Anual")
            groupBy = records.GroupBy(r => new DateTime(r.Date.Year, 1, 1));

        return groupBy.Select(g => new HistoricalDataPoint
        {
            Period = g.Key,
            Revenue = g.Sum(r => r.Revenue),
            Cost = g.Sum(r => r.Cost),
            Margin = g.Sum(r => r.Revenue - r.Cost)
        }).OrderBy(h => h.Period).ToList();
    }
}