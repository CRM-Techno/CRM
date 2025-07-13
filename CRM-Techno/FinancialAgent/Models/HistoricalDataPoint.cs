namespace FinancialAgent.Models;

public class HistoricalDataPoint
{
    public DateTime Period { get; set; }
    public decimal Revenue { get; set; }
    public decimal Cost { get; set; }
    public decimal Margin { get; set; }
}