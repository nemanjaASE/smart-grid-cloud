using MediatR;
using SmartGrid.Domain.Models;

namespace SmartGrid.Application.Features.Telemetries.Events
{
    public record TelemetryProcessedEvent(Telemetry Telemetry) : INotification;
}
