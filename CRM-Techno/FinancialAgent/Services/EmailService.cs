using System.Net.Mail;
using System.Text;
using FinancialAgent.Models;

namespace FinancialAgent.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendAnomalyAlertsAsync(DashboardData dashboard, string recipient)
    {
        using var client = new SmtpClient(_configuration["Smtp:Host"], int.Parse(_configuration["Smtp:Port"]))
        {
            Credentials = new System.Net.NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_configuration["Smtp:From"]),
            Subject = $"Alertas de Anomalias - {dashboard.Module} ({dashboard.Period})",
            Body = GenerateEmailBody(dashboard),
            IsBodyHtml = true
        };
        mailMessage.To.Add(recipient);

        await client.SendMailAsync(mailMessage);
    }

    private string GenerateEmailBody(DashboardData dashboard)
    {
        var body = new StringBuilder();
        body.AppendLine($"<h1>Relatório de Anomalias - {dashboard.Module}</h1>");
        body.AppendLine($"<p>Período: {dashboard.Period} ({dashboard.StartDate:yyyy-MM-dd} a {dashboard.EndDate:yyyy-MM-dd})</p>");
        body.AppendLine("<h2>Anomalias Detectadas:</h2><ul>");

        foreach (var anomaly in dashboard.Anomalies)
        {
            body.AppendLine($"<li>{anomaly.Type}: {anomaly.Description}</li>");
        }
        body.AppendLine("</ul>");

        body.AppendLine("<h2>KPIs:</h2><ul>");
        foreach (var kpi in dashboard.Kpis)
        {
            body.AppendLine($"<li>{kpi.Key}: {kpi.Value:C}</li>");
        }
        body.AppendLine("</ul>");

        return body.ToString();
    }
}