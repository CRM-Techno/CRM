using FinancialAgent.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancialAgent.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly ReportService _reportService;

    public DashboardController(ReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("{module}/{period}")]
    public async Task<IActionResult> GetDashboard(string module, string period, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (!new[] { "Vendas", "Atendimento", "Financeiro", "Estoque" }.Contains(module))
            return BadRequest("Módulo inválido");
        if (!new[] { "Diário", "Semanal", "Mensal" }.Contains(period))
            return BadRequest("Período inválido");

        var dashboard = await _reportService.GenerateDashboardDataAsync(module, period, startDate, endDate);
        return Ok(dashboard);
    }
}