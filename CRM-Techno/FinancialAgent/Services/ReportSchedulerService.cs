using FinancialAgent.Jobs;
using Quartz;
using Quartz.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialAgent.Services;

public class ReportSchedulerService
{
    private readonly IScheduler _scheduler;
    private readonly IServiceProvider _serviceProvider;

    public ReportSchedulerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
    }

    public async Task StartAsync()
    {
        await _scheduler.Start();

        var modules = new[] { "Vendas", "Atendimento", "Financeiro", "Estoque" };
        var periods = new[] { "Diário", "Semanal", "Mensal", "Anual" };

        foreach (var module in modules)
        {
            foreach (var period in periods)
            {
                var job = JobBuilder.Create<ReportJob>()
                    .WithIdentity($"{module}-{period}-ReportJob")
                    .UsingJobData("module", module)
                    .UsingJobData("period", period)
                    .Build();

                var trigger = CreateTrigger(module, period);
                await _scheduler.ScheduleJob(job, trigger);
            }
        }
    }

    private ITrigger CreateTrigger(string module, string period)
    {
        var cronExpression = period switch
        {
            "Diário" => "0 0 0 * * ?", // Meia-noite todos os dias
            "Semanal" => "0 0 0 ? * MON", // Meia-noite às segundas-feiras
            "Mensal" => "0 0 0 1 * ?", // Meia-noite no 1º dia do mês
            "Anual" => "0 0 0 1 1 ?", // Meia-noite em 1º de janeiro
            _ => throw new ArgumentException("Período inválido")
        };

        return TriggerBuilder.Create()
            .WithIdentity($"{module}-{period}-Trigger")
            .WithCronSchedule(cronExpression)
            .Build();
    }
}