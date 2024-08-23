using Domain.interfaces.messagebrokers;
using Domain.interfaces.repositories;
using Domain.interfaces.services;
using HealthChecks.UI.Client;
using Infra.Data.configurations.rabbitmq;
using Infra.Data.contexts;
using Infra.Data.queue;
using Infra.Data.repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Service.mappings;
using Service.tarefas;
using System.Data.Common;
using System.Diagnostics;

namespace InfraInversionControl.extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            var stringConexao = configuration.GetConnectionString("TarefaConnection");
            if (stringConexao == null) {
                throw new NullReferenceException("GetConnectionString retornou a nullo");
            }
            services.AddSingleton<DbConnectionStringBuilder>(new SqlConnectionStringBuilder
            {
                ConnectionString = stringConexao
            });

            services.AddSingleton<ContextoTarefa>(provider =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<ContextoTarefa>();
                optionsBuilder.UseSqlServer(stringConexao, mig => mig.MigrationsAssembly(typeof(ContextoTarefa).Assembly.FullName));
                return new ContextoTarefa(optionsBuilder.Options);
            });

            //services.AddDbContext<ContextoTarefa>(options => {
            //    options.UseSqlServer(stringConexao, mig => mig.MigrationsAssembly(typeof(ContextoTarefa).Assembly.FullName));
            //    }
            //);
            services.Configure<RabbitMqConfiguracao>(a => configuration.GetSection(nameof(RabbitMqConfiguracao)).Bind(a));

            var rabbitMqConfiguration = configuration.GetSection(nameof(RabbitMqConfiguracao)).Get<RabbitMqConfiguracao>();
            if (rabbitMqConfiguration == null)
            {
                throw new NullReferenceException("RabbitMqConfiguracao retornou a nullo");
            }
            var rabbitConnectionString = $"amqp://{rabbitMqConfiguration?.Username}:{rabbitMqConfiguration?.Password}@{rabbitMqConfiguration?.HostName}";
            if (rabbitConnectionString == null)
            {
                throw new NullReferenceException("rabbitConnectionString retornou a nullo");
            }

            services.AddSingleton<IMensagemService, RabbitMqService>();
            services.AddTransient<ITarefaRepository, TarefaRepository>();
            services.AddTransient<ITarefaService, TarefaService>();  
            services.AddAutoMapper(typeof(TarefaMppingProfile).Assembly);
            services 
                .AddHealthChecks()
                .AddRabbitMQ(rabbitConnectionString, name: "rabbitmq-check", tags: new string[] { "rabbitmq" })
                .AddSqlServer(stringConexao, name: "sqlserver-check", tags: new string[] { "sqlserver" });

            return services;
        }
        public static IApplicationBuilder ConfigureHealthCheck(this IApplicationBuilder builder)
        {
            builder.UseHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = p => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            return builder;
        }
    }
}
