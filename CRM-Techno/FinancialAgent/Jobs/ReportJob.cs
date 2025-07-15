using Quartz;
using FinancialAgent.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialAgent.Jobs;

public class ReportJob : IJob
{
    private readonly IServiceProvider _serviceProvider;

    public ReportJob(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using var scope = _serviceProvider.CreateScope();
        var reportService = scope.ServiceProvider.GetRequiredService<ReportService>();
        var module = context.JobDetail.JobDataMap.GetString("module");
        var period = context.JobDetail.JobDataMap.GetString("period");
        await reportService.GenerateDashboardDataAsync(module, period);
    }
}