using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SmartGrid.Application.Common.Behaviors;
using SmartGrid.Application.Extensions;
using System.Reflection;

namespace SmartGrid.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddValidatorsFromAssembly(assembly);

            services.AddMappers();

            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });

            return services;
        }
    }
}
