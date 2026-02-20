using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;

namespace SmartGrid.Application.Features.Devices.Queries
{
    // QUERY DTO
    public record DeviceDto(
        string DeviceId,
        string Name,
        DeviceType Type,
        string Location,
        double NominalPower,
        DateTime RegisteredAt
    );

    public record GetDevicesQuery() : IRequest<Result<IEnumerable<DeviceDto>>?>;

    // HANDLER
    internal class GetDevicesHandler(
        IDeviceRepository deviceRepository,
        IMapper<Device, DeviceDto> mapper,
        ILogger<GetDevicesHandler> logger) : IRequestHandler<GetDevicesQuery, Result<IEnumerable<DeviceDto>>?>
    {
        public async Task<Result<IEnumerable<DeviceDto>>?> Handle(GetDevicesQuery request, CancellationToken ct)
        {
            try
            {
                var devices = await deviceRepository.GetAllAsync(ct);

                var devicesDTO = devices.Select(mapper.Map).ToList();

                return Result<IEnumerable<DeviceDto>>.Success(devicesDTO);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while retreiving devices..");
                return Result<IEnumerable<DeviceDto>>.Failure("Failed to retrieve devices.",
                    ErrorType.Failure);
            }
        }
    }
}
