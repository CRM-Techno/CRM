using FinancialAgent.Models;
using FinancialAgent.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancialAgent.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DashboardController : ControllerBase
{
    private readonly ReportService _reportService;

    public DashboardController(ReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Generates a dashboard report for the specified module and period.
    /// </summary>
    /// <param name="module">The module to generate the report for (e.g., Vendas, Atendimento, Financeiro, Estoque)</param>
    /// <param name="period">The period of the report (e.g., Diário, Semanal, Mensal, Anual)</param>
    /// <returns>A dashboard data object containing KPIs, historical data, and anomalies</returns>
    /// <response code="200">Returns the generated dashboard data</response>
    /// <response code="400">If the module or period is invalid</response>
    [HttpGet("{module}/{period}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDashboard(string module, string period)
    {
        if (!new[] { "Vendas", "Atendimento", "Financeiro", "Estoque" }.Contains(module))
            return BadRequest("Módulo inválido");
        if (!new[] { "Diário", "Semanal", "Mensal", "Anual" }.Contains(period))
            return BadRequest("Período inválido");

        var dashboard = await _reportService.GenerateDashboardDataAsync(module, period);
        return Ok(dashboard);
    }
}