using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;

namespace SmartGrid.Application.Features.DeviceStatuses.Queries
{
    // QUERY DTO
    public record DeviceStatusDto(
        string DeviceId,
        DeviceType DeviceType,
        double CurrentPower,
        double LoadPercentage,
        bool IsOnline,
        bool IsUnderperforming,
        bool IsOverloaded,
        string CurrentFirmwareVersion,
        string? TargetFirmwareVersion,
        UpdateStatus UpdateStatus
    );

    public record GetDeviceStatusesQuery() : IRequest<Result<IEnumerable<DeviceStatusDto>>?>;

    // HANDLER
    internal class GetDeviceStatusesHandler(
        IDeviceStatusQueryRepository deviceStatusQueryRepository,
        IMapper<DeviceStatus, DeviceStatusDto> mapper,
        ILogger<GetDeviceStatusesHandler> logger
    ) : IRequestHandler<GetDeviceStatusesQuery, Result<IEnumerable<DeviceStatusDto>>?>
    {
        public async Task<Result<IEnumerable<DeviceStatusDto>>?> Handle(GetDeviceStatusesQuery request, CancellationToken ct)
        {
            try
            {
                var statuses = await deviceStatusQueryRepository.GetAllAsync(ct);

                var deviceStatuseDTO = statuses.Select(mapper.Map).ToList();

                return Result<IEnumerable<DeviceStatusDto>>.Success(deviceStatuseDTO);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while retreiving devices..");
                    return Result<IEnumerable<DeviceStatusDto>>.Failure("Failed to retrieve devices.",
                     ErrorType.Failure);
            }
        }
    }
}
